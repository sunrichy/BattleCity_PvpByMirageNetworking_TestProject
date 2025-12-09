using Mirage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    NetworkManager networkManager;
    public TileData[] tile;
    public Tilemap tilemap;

    private Dictionary<TileBase, TileData> keyValuePairs = new();

    private void Start()
    {
        GenMap();
    }

    public void GenMap() 
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
                GameObject newGame = null;
                if (networkManager && networkManager.Client.IsHost && !data.localOnly) 
                {
                    newGame = networkManager.ServerObjectManager.SpawnInstantiate(data.gameObject);
                }
                else 
                {
                    newGame = Instantiate(data.gameObject, tilemap.layoutGrid.transform);
                }
                if(newGame) newGame.transform.position = tilemap.GetCellCenterWorld(posIndex);                    
            }
        }

        tilemap.gameObject.SetActive(false);
    }
}
