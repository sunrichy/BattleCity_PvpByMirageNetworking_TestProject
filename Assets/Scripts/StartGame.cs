using Mirage;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    NetworkManager networkManager;
    [SerializeField] private MapDataBase _mapData;
    [SerializeField] private PlayerManager _playerManager;

    Dictionary<string, MapDataBase.TileMapData> dictionary;

    private void Awake()
    {
        dictionary = _mapData.GetDictionaryMap();
        networkManager = GameManager.Instacne.networkManager;

        networkManager.Server.Disconnected.AddListener((c) => SceneManager.LoadScene(0));
        networkManager.Client.Disconnected.AddListener( (c) => SceneManager.LoadScene(0) );

        if (!networkManager.Server.IsHost)
        {
            if (dictionary.TryGetValue(GameManager.Instacne.loadLevelKey, out var data))
            {
                TileMapManager tileMapManager = Instantiate(data.tileMapManager, null);
                tileMapManager.tilemap.gameObject.SetActive(false);

                networkManager.Client.World.onSpawn += (identity) =>
                {
                    if(identity.gameObject.tag == "Wall") 
                    {
                        identity.gameObject.transform.SetParent(tileMapManager.wallGrid.transform);
                        identity.transform.localScale = Vector3.one;
                    }
                };

                tileMapManager.GenMapLocalOnly();
            }

            return;
        }

        StartCoroutine(Wait());
    }

    IEnumerator Wait() 
    {
        yield return new WaitForSeconds(2f);

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

                
                networkManager.ServerObjectManager.AddCharacter(playerData, identity);
            }

        }
    }
}
