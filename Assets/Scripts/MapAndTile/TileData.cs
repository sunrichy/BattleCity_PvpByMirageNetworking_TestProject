using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "TileData")]
public class TileData : ScriptableObject
{
    public TileBase tile;
    public GameObject gameObject;
    public bool localOnly;
}
