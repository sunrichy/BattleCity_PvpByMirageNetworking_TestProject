using UnityEngine;

[CreateAssetMenu(fileName = "MusicDataBase", menuName = "Scriptable Objects/MusicDataBase")]
public class MusicDataBase : ScriptableObject
{
    [System.Serializable]
    public struct ClipSoundDataSlot 
    {
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private string _key;

        public AudioClip audioClip => _audioClip;
        public string key => _key;
    }

    public ClipSoundDataSlot[] clipSoundDatas;
}