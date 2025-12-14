using Mirage;
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

                if (networkManager && networkManager.Client.IsHost && !data.localOnly) 
                {
                    networkManager.ServerObjectManager.Spawn(newGame, newGame.GetOrAddComponent<NetworkIdentity>().PrefabHash);
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
                }
            }
        }

        tilemap.gameObject.SetActive(false);
    }
}
