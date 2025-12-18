using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Toggle _muteBgToggle;
    [SerializeField] private Toggle _muteSfxToggle;

    private void Awake()
    {
        _muteBgToggle.onValueChanged.AddListener(OnBgToggle);
        _muteSfxToggle.onValueChanged.AddListener(OnSfxToggle);
    }

    private void OnEnable()
    {
        if (MusicBox.Instacne) 
        {
            _muteBgToggle.isOn = MusicBox.Instacne.MuteBg;
            _muteSfxToggle.isOn = MusicBox.Instacne.MuteSfx;
        }
    }

    private void OnBgToggle(bool value)
    {
        if (MusicBox.Instacne) 
        {
            MusicBox.Instacne.MuteBgVolume(value);
        }
    }

    private void OnSfxToggle(bool value)
    {
        if (MusicBox.Instacne) 
        {
            MusicBox.Instacne.MuteSfxVolume(value);
        }
    }
}
