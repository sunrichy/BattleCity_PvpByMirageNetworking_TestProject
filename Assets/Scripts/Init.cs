using Mirage;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Init : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManagerPrefab;

    [SerializeField] private Button _singleplayerButton;
    [SerializeField] private Button _multiplayerButton;
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        GameManager gameManager = GameManager.Instacne;

        if (GameManager.Instacne.networkManager)
        {
            Destroy(GameManager.Instacne.networkManager.gameObject);
        }

        _singleplayerButton.onClick.AddListener(SingleplayerButton);
        _multiplayerButton.onClick.AddListener(MultiplayerButton);
        _exitButton.onClick.AddListener(ExitButton);

        gameManager.loadLevelKey = "1";
    }

    private void Start()
    {

    }

    private void ExitButton()
    {
        if (GameManager.Instacne.networkManager)
        {
            Destroy(GameManager.Instacne.networkManager.gameObject);
        }


#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SingleplayerButton()
    {
        SceneManager.LoadScene(1);
    }

    private void MultiplayerButton()
    {
        if (!GameManager.Instacne.networkManager) 
        {
            NetworkManager networkManager = Instantiate(_networkManagerPrefab);
            GameManager.Instacne.SetNetworkManager(networkManager);
        }

        StartCoroutine(Connecting());

        
    }

    IEnumerator Connecting() 
    {
        NetworkManager networkManager = GameManager.Instacne.networkManager;
        networkManager.Client.Connect();

        {
            bool fail = false;

            UnityAction<ClientStoppedReason> unityAction = (s) => { fail = true; };
            networkManager.Client.Disconnected.AddListener(unityAction);

            float timer = 5f;

            while (timer > 0) 
            {
                if (networkManager.Client.IsConnected) 
                {
                    GameManager.Instacne.networkManager.ClientObjectManager.PrepareToSpawnSceneObjects();
                    yield break;
                }

                if (fail) 
                {
                    break;
                }

                yield return null;
                timer -= Time.deltaTime;
            }
            networkManager.Client.Disconnected.RemoveListener(unityAction);
            if (!networkManager.Client.IsConnected) 
            {
                networkManager.Server.StartServer(networkManager.Client);
            }
        }

        while (networkManager.Server.AllPlayers.Count != 2) 
        {
            yield return null;
        }

        //UnityAction<ClientStoppedReason> unityEvent2 = (s) =>
        //{
        //    if (SceneManager.GetActiveScene().buildIndex != 0)
        //        SceneManager.LoadScene(0);

        //    Debug.LogError(s);
        //};
        //networkManager.Client.Disconnected.AddListener(unityEvent2);

        if (networkManager.Server.IsHost) 
        {
            yield return networkManager.NetworkSceneManager.ServerLoadSceneNormalAsync("StartGame");
            // networkManager.NetworkSceneManager.SetSceneIsReady();
        }
    }

    public void Startgame() 
    {
        
    }
}
