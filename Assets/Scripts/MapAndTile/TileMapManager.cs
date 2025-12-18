using Mirage;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    public TileData[] tile;
    public Tilemap tilemap;
    public Transform[] playerSpawnPoints;
    public Transform wallGrid;

    private Dictionary<TileBase, TileData> keyValuePairs = new();

    public void GenMap() 
    {
        float x = 0f;
        GenMap(ref x);
    }

    public void GenMap(ref float percentLoad) 
    {
        keyValuePairs.Clear();
        for (int i = 0; i < tile.Length; i++) 
        {
            keyValuePairs.Add(tile[i].tile, tile[i]);
        }


        percentLoad = 0f;
        int current = 0, max = 0;

        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int posIndex in bounds.allPositionsWithin)
        {
            max++;
        }

        foreach (Vector3Int posIndex in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(posIndex);

            if (tile != null)
            {
                if (!keyValuePairs.TryGetValue(tile, out var data))
                {
                    continue;
                }

                NetworkManager networkManager = GameManager.Instacne.networkManager;
                GameObject newGame = null;
                newGame = Instantiate(data.gameObject, tilemap.layoutGrid.transform);
                newGame.transform.position = tilemap.GetCellCenterWorld(posIndex);
                newGame.name += "_Tile_" + current;

                if (networkManager && networkManager.Client.IsHost && !data.localOnly) 
                {
                    newGame.name += "_Network";
                    NetworkIdentity identity = newGame.GetOrAddComponent<NetworkIdentity>();

                    networkManager.ServerObjectManager.Spawn(newGame, identity.PrefabHash);
                    networkManager.Server.SendToMany(networkManager.Server.AllPlayers, 
                        new StartGame.SetSpawnObjectName() { netId = identity.NetId, name = newGame.name}, true );
                }
                else 
                {
                    newGame.name += "_Local";
                }
            }
            current++;
            percentLoad = current / max;
        }

        tilemap.gameObject.SetActive(false);
    }

    public void GenMapLocalOnly() 
    {
        keyValuePairs.Clear();

        for (int i = 0; i < tile.Length; i++) 
        {
            keyValuePairs.Add(tile[i].tile, tile[i]);
        }


        BoundsInt bounds = tilemap.cellBounds;
        int index = 0;
        foreach (Vector3Int posIndex in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(posIndex);

            if (tile != null)
            {
                if (!keyValuePairs.TryGetValue(tile, out var data))
                {
                    continue;
                }

                if (data.localOnly)
                {
                    GameObject newGame = null;
                    newGame = Instantiate(data.gameObject, tilemap.layoutGrid.transform);
                    newGame.transform.position = tilemap.GetCellCenterWorld(posIndex);
                    newGame.name += "_Local_" + index;
                }
            }

            index++;
        }

        tilemap.gameObject.SetActive(false);
    }
}
