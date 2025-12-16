using Mirage;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] private MapDataBase _mapDataBase;
    [SerializeField] private Transform _content;
    [SerializeField] private Button _cellPrefab;

    List<Button> allCells = new(); 

    private void Awake()
    {
        for(int i = 0; i < _mapDataBase.tileMapManagers.Length; i++)
        {
            Button newButton = Instantiate( _cellPrefab, _content);
            TextMeshProUGUI text = newButton.GetComponentInChildren<TextMeshProUGUI>();
            allCells.Add(newButton);

            string key = _mapDataBase.tileMapManagers[i].key;
            newButton.onClick.AddListener(() =>
            {
                CallLoadMap(key);
            });

            if (text)
            {
                text.text = key;
            }
        }

        _cellPrefab.gameObject.SetActive(false);
    }

    public void CallLoadMap(string key) 
    {
        NetworkManager networkManager = GameManager.Instacne.networkManager;
        networkManager.Server.SendToMany(networkManager.Server.AllPlayers, 
            new StartGame.LoadSceneName() { keySceneName = key }, false);

        for (int i = 0; i < allCells.Count; i++) 
        {
            allCells[i].interactable = false;
        }
    }
}