using Mirage;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    NetworkManager networkManager;
    [SerializeField] private MapDataBase _mapData;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private Button _gameOverButton;

    Dictionary<string, MapDataBase.TileMapData> dictionary;
    Dictionary<ulong, NetworkIdentity> allNetworkObjects = new ();

    [Header("Load Game")]
    [SerializeField] private GameObejectSetActionSetting _gameObjectWhenStartLoadGame;
    [SerializeField] private GameObejectSetActionSetting _gameObjectWhenFinishLoadGame;

    [Header("Game Over")]
    [SerializeField] private GameObejectSetActionSetting _gameObjectWhenGameOver;

    private float percentLoadGame;
    private bool IsServer;
    private int playerCount;

    [System.Serializable]
    public struct LoadingScenePercent 
    {
        public float percent;
    }

    [System.Serializable]
    public struct LoadSceneName 
    {
        public string keySceneName;
    }

    [System.Serializable]
    public struct GameOverMessage
    {

    }

    [System.Serializable]
    public struct PlayerDead
    {

    }

    [System.Serializable]
    public struct SetSpawnObjectName
    {
        public ulong netId;
        public string name;
    }

    private void Awake()
    {
        _gameObjectWhenStartLoadGame.RunSetActive();

        dictionary = _mapData.GetDictionaryMap();
        networkManager = GameManager.Instacne.networkManager;

        networkManager.Server.Disconnected.AddListener((c) => OnGameOver(new GameOverMessage()));
        networkManager.Client.Disconnected.AddListener( (c) => OnGameOver(new GameOverMessage()));

        IsServer = networkManager.Server.IsHost;

        networkManager.Client.World.onSpawn += (identity) =>
        {
            allNetworkObjects.Add(identity.NetId, identity);
        };

        if (!IsServer)
        {
            networkManager.Client.MessageHandler.RegisterHandler<LoadSceneName>(OnGetLoadSceneName);
            networkManager.Client.MessageHandler.RegisterHandler<LoadingScenePercent>(OnGetLoadScene);
            networkManager.Client.MessageHandler.RegisterHandler<GameOverMessage>(OnGameOver);
            networkManager.Client.MessageHandler.RegisterHandler<SetSpawnObjectName>(OnSetSpawnObjectName);


            //if (dictionary.TryGetValue(GameManager.Instacne.loadLevelKey, out var data))
            //{
            //    TileMapManager tileMapManager = Instantiate(data.tileMapManager, null);
            //    tileMapManager.tilemap.gameObject.SetActive(false);

            //    networkManager.Client.World.onSpawn += (identity) =>
            //    {
            //        if(identity.gameObject.tag == "Wall") 
            //        {
            //            identity.gameObject.transform.SetParent(tileMapManager.wallGrid.transform);
            //            identity.transform.localScale = Vector3.one;
            //        }
            //    };

            //    tileMapManager.GenMapLocalOnly();
            //}
            StartCoroutine(WaitLocal());
            return;
        }
        networkManager.Server.MessageHandler.RegisterHandler<PlayerDead>(OnPlayerDead);
        StartCoroutine(WaitServer());
    }

    private void Start()
    {
        MusicBox.Instacne.PlayBg("Gameplay");
    }

    IEnumerator WaitLocal() 
    {
        while(percentLoadGame < 1f) 
        {
            yield return null;
        }

        _gameObjectWhenFinishLoadGame.RunSetActive();
    }
    IEnumerator WaitServer()
    {
        yield return new WaitForSeconds(2f);

        networkManager.Server.SendToMany(networkManager.Server.AllPlayers,
            new LoadSceneName() { keySceneName = GameManager.Instacne.loadLevelKey }, true);


        if (dictionary.TryGetValue(GameManager.Instacne.loadLevelKey, out var data))
        {
            TileMapManager tileMapManager = Instantiate(data.tileMapManager, null);
            tileMapManager.GenMap();


            for (int i = 0; i < networkManager.Server.AllPlayers.Count; i++)
            {
                var playerData = networkManager.Server.AllPlayers.ElementAt(i);
                var point = tileMapManager.playerSpawnPoints[i];
                PlayerManager b = Instantiate(_playerManager, null);
                b.transform.position = point.position;
                b.transform.rotation = point.rotation;
                NetworkIdentity identity = b.gameObject.GetComponent<NetworkIdentity>();

                identity.name = "Player_" + (i + 1);
                networkManager.ServerObjectManager.AddCharacter(playerData, identity);
                networkManager.Server.SendToMany(networkManager.Server.AllPlayers, new SetSpawnObjectName() { netId = identity.NetId, name = identity.name} , true);
            }
        }

        playerCount = networkManager.Server.AllPlayers.Count;
        percentLoadGame = 1f;

        networkManager.Server.SendToMany<LoadingScenePercent>(networkManager.Server.AllPlayers, new LoadingScenePercent() { percent = percentLoadGame} , true);
        _gameObjectWhenFinishLoadGame.RunSetActive();
        enabled = false;
    }


    private void OnGetLoadScene(LoadingScenePercent loadScene) 
    {
        percentLoadGame = loadScene.percent;
        networkManager.Client.MessageHandler.UnregisterHandler<LoadingScenePercent>();
    }

    private void OnGetLoadSceneName(LoadSceneName loadScene) 
    {
        networkManager.Client.MessageHandler.UnregisterHandler<LoadSceneName>();

        if (dictionary.TryGetValue(loadScene.keySceneName, out var data))
        {
            TileMapManager tileMapManager = Instantiate(data.tileMapManager, null);
            tileMapManager.tilemap.gameObject.SetActive(false);

            networkManager.Client.World.onSpawn += (identity) =>
            {
                if (identity.gameObject.tag == "Wall")
                {
                    identity.gameObject.transform.SetParent(tileMapManager.wallGrid.transform);
                    identity.transform.localScale = Vector3.one;
                }
            };

            tileMapManager.GenMapLocalOnly();
        }
    }

    private void OnGameOver(GameOverMessage gameOverMessage)
    {
        _gameObjectWhenGameOver.RunSetActive();
        MusicBox.Instacne.PlayBg("GameOver");
        Destroy(GameManager.Instacne.networkManager.gameObject);
        _gameOverButton.onClick.AddListener(() => 
        {
            MusicBox.Instacne.StopBg();
            SceneManager.LoadScene(0);
        });
    }

    private void OnPlayerDead(PlayerDead playerDead)
    {
        Debug.Log("OnPlayerDead");

        if (IsServer)
        {
            playerCount--;
            if (playerCount <= 1)
            {
                networkManager.Server.SendToMany(networkManager.Server.AllPlayers, new GameOverMessage(), true);
                OnGameOver(new());
                return;
            }
        }
    }

    private void OnSetSpawnObjectName(SetSpawnObjectName message)
    {
        if (allNetworkObjects.TryGetValue(message.netId, out NetworkIdentity identity))
        {
            identity.name = message.name;
        }
    }
}
