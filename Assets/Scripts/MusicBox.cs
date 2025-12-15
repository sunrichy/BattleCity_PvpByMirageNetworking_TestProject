using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBox : MonoBehaviour
{
    private static MusicBox _instacne { get; set; }
    public static MusicBox Instacne
    {
        get
        {
            return _instacne;
        }
    }

    [SerializeField] private MusicDataBase _musicDataBase;
    [SerializeField] private AudioSource _audioBg;
    [SerializeField] private AudioSource _audioSfx;

    public AudioSource audioBg => _audioBg;

    private Dictionary<string, AudioClip> audioIndex = new();

    private void Awake()
    {
        if (_instacne) 
        {
            Destroy(gameObject);
            return;
        }

        _instacne = this;

        for (int i = 0; i < _musicDataBase.clipSoundDatas.Length; i++) 
        {
            audioIndex.Add(_musicDataBase.clipSoundDatas[i].key, _musicDataBase.clipSoundDatas[i].audioClip);
        }

        DontDestroyOnLoad(_instacne);
    }

    public void PlayBg(string key)
    {
        AudioClip clip = GetAudioClip(key);

        if (clip)
        {
            _audioBg.Stop();
            _audioBg.clip = clip;
            _audioBg.PlayDelayed(1f);
        }
    }

    public void StopBg()
    {
        _audioBg.Stop();
    }

    public void PlaySfx(string key)
    {
        AudioClip clip = GetAudioClip(key);

        if (clip) 
        {
            StartCoroutine(Wait());
            IEnumerator Wait()
            {
                AudioSource audioSource = new GameObject("Sfx").AddComponent<AudioSource>();
                audioSource.volume = _audioSfx.volume;
                audioSource.gameObject.transform.SetParent(transform);
                audioSource.PlayOneShot(clip);
                yield return new WaitForSecondsRealtime(clip.length);
                Destroy(audioSource.gameObject);
            }
        }
    }

    public AudioClip GetAudioClip(string key) 
    {
        if(audioIndex.TryGetValue(key, out AudioClip audioClip)) 
        {
            return audioClip;    
        }

        return null;
    }
}
