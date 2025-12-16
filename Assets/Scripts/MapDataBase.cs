using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataBase", menuName = "MapDataBase")]
public class MapDataBase : ScriptableObject
{
    [SerializeField] private TileMapData[] _tileMapManagers;
    public TileMapData[] tileMapManagers => _tileMapManagers;

    [System.Serializable]
    public struct TileMapData
    {
        public string key;
        public TileMapManager tileMapManager;
    }

    public Dictionary<string,TileMapData> GetDictionaryMap() 
    {
        Dictionary<string,TileMapData> keyValuePairs = new Dictionary<string,TileMapData>();

        for (int i = 0; i < _tileMapManagers.Length; i++) 
        {
            keyValuePairs.Add(_tileMapManagers[i].key, _tileMapManagers[i]);
        }

        return keyValuePairs;
    }
}
