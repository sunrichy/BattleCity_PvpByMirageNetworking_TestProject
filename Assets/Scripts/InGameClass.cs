using UnityEngine;

public enum MoveDirection
{
    Idle,
    Up,
    Right,
    Down,
    Left
}

[System.Serializable]
public struct GameObejectSetActionSetting
{
    [System.Serializable]
    public struct GameObjectSlotData 
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private bool _active;

        public GameObject gameObject => _gameObject;
        public bool active => _active;
    }

    public GameObjectSlotData[] gameObjectDatas;

    public void RunSetActive() 
    {
        for (int i = 0; i < gameObjectDatas.Length; i++) 
        {
            gameObjectDatas[i].gameObject.SetActive(gameObjectDatas[i].active);
        }
    }
}