using Mirage;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instacne;
    public static GameManager Instacne 
    {
        get 
        {
            if (_instacne == null) 
            {
                GameObject game = new GameObject(typeof(GameManager).Name);
                _instacne = game.AddComponent<GameManager>();
                DontDestroyOnLoad(game);
            }

            return _instacne;
        }
    }

    public string loadLevelKey { get; set; }

    public NetworkManager networkManager { get; private set; }

    public void SetNetworkManager(NetworkManager networkManager) 
    {
        this.networkManager = networkManager;
    }
}
