using Mirage;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class Init : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManagerPrefab;

    [SerializeField] private Button _singleplayerButton;
    [SerializeField] private Button _multiplayerButton;
    [SerializeField] private Button _exitButton;

    [SerializeField] private Button _closeConnectButton;
    [SerializeField] private TMPro.TextMeshProUGUI _connectingText;

    [SerializeField] private GameObejectSetActionSetting _waitPlayerGameObjects;


    private void Awake()
    {
        GameManager gameManager = GameManager.Instacne;

        if (GameManager.Instacne.networkManager)
        {
            Destroy(GameManager.Instacne.networkManager.gameObject);
        }

        _waitPlayerGameObjects.ReverseSetActive();

        _singleplayerButton.onClick.AddListener(SingleplayerButton);
        _multiplayerButton.onClick.AddListener(MultiplayerButton);
        _exitButton.onClick.AddListener(ExitButton);

        _closeConnectButton.onClick.AddListener(CloseConnectButton);
    }


    private void Start()
    {
        MusicBox.Instacne.StopBg();
        MusicBox.Instacne.PlayBg("MainMenu");
    }

    private void CloseConnectButton()
    {
        StopCoroutine(ConnectingIEnumerator);
        _waitPlayerGameObjects.ReverseSetActive();

        if (GameManager.Instacne.networkManager)
        {
            Destroy(GameManager.Instacne.networkManager.gameObject);
        }
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

        _waitPlayerGameObjects.RunSetActive();
        ConnectingIEnumerator = Connecting();
        StartCoroutine(ConnectingIEnumerator);
    }

    IEnumerator ConnectingIEnumerator;
    IEnumerator Connecting() 
    {
        _connectingText.text = "Connecting";
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
                _connectingText.text = "Connecting";
                for (int i = 0; i < Mathf.RoundToInt(Mathf.Abs(timer - 5f) / 0.5f); i++) 
                {
                    _connectingText.text += ".";
                }
            }
            networkManager.Client.Disconnected.RemoveListener(unityAction);
            if (!networkManager.Client.IsConnected) 
            {
                networkManager.Server.StartServer(networkManager.Client);
                _connectingText.text = "Start Host, Wait for player";
            }
        }

        {
            float timer = 0f;

            while (networkManager.Server.AllPlayers.Count != 2) 
            {
                yield return null;
                timer += Time.deltaTime;
                _connectingText.text = "Start Host, Wait for player";
                for (int i = 0; i < Mathf.RoundToInt(timer % 5f); i++)
                {
                    _connectingText.text += ".";
                }
            }
        }
        _connectingText.text = "Starting game";
        if (networkManager.Server.IsHost)
        {
            yield return networkManager.NetworkSceneManager.ServerLoadSceneNormalAsync("StartGame");
        }
    }
}
