using Mirage;
using System;
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

        _singleplayerButton.onClick.AddListener(SingleplayerButton);
        _multiplayerButton.onClick.AddListener(MultiplayerButton);
        _exitButton.onClick.AddListener(ExitButton);

        gameManager.loadLevelKey = "1";
    }

    private void Start()
    {

        //_networkManager.Server.Started.AddListener(() => Debug.Log("Started"));
        //_networkManager.Server.Connected.AddListener((s) => Debug.Log("Connected"));
        //_networkManager.Server.OnStartHost.AddListener(() => Debug.Log("OnStartHost"));
        //_networkManager.Server.OnStopHost.AddListener(() => Debug.Log("OnStopHost"));

        //_networkManager.Server.StartServer(_networkManager.Client);
        //_networkManager.Server.SetAuthenticationFailedCallback((p,r) => 
        //{
        //    Debug.Log(p.Identity + " : " + r.Success);
        //});
        
    }

    private void ExitButton()
    {
        _networkManagerPrefab.Server.Stop();
    }

    private void OnServerAddPlayerInternal(INetworkPlayer player, AddCharacterMessage msg)
    {
        //if (player.HasCharacter)
        //{
        //    // player already has character on server, but client asked for it
        //    // so we respawn it here so that client recieves it again
        //    // this can happen when client loads normally, but server addititively
        //    _networkManagerPrefab.ServerObjectManager.Spawn(player.Identity);
        //}
        //else
        //{
        //    OnServerAddPlayer(player);
        //}

        Debug.Log("INetworkPlayer " + player.Identity);
    }

    public virtual void OnServerAddPlayer(INetworkPlayer player)
    {
        //var startPos = GetStartPosition();
        //var character = startPos != null
        //    ? Instantiate(PlayerPrefab, startPos.position, startPos.rotation)
        //    : Instantiate(PlayerPrefab);

        //if (SetName)
        //    SetCharacterName(player, character);
        //ServerObjectManager.AddCharacter(player, character.gameObject);
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
