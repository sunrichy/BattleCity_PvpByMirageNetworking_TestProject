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

    [SerializeField] private TMPro.TextMeshProUGUI _hostText;
    [SerializeField] private float _timerToChooseLevel = 10f;
    
    List<Button> allCells = new();
    bool chooseLevel;
    float n_timerToChooseLevel;
    string formatText;

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

        n_timerToChooseLevel = _timerToChooseLevel;
        formatText = _hostText.text;
        _cellPrefab.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (chooseLevel) 
        {
            return;
        }


        if (n_timerToChooseLevel <= 0f) 
        {
            allCells[Random.Range(0, allCells.Count)].onClick.Invoke();
            return;
        }

        _hostText.text = string.Format(formatText, n_timerToChooseLevel.ToString("0"));
        n_timerToChooseLevel -= Time.deltaTime;
    }

    public void CallLoadMap(string key) 
    {
        chooseLevel = true;
        NetworkManager networkManager = GameManager.Instacne.networkManager;
        networkManager.Server.SendToMany(networkManager.Server.AllPlayers, 
            new StartGame.LoadSceneName() { keySceneName = key }, false);

        for (int i = 0; i < allCells.Count; i++) 
        {
            allCells[i].interactable = false;
        }
    }
}