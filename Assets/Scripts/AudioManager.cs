using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    public GameObject audioSourcePrefab; // 预制的 AudioSource
    public int poolSize = 10; // 池的大小
    public float interval = 0.3f; // 播放间隔
    public WaitForSeconds waitInterval; // 播放间隔的等待
    private List<AudioSource> audioSourcePool;
    float timer = 0;
    private void Awake()
    {
        InitializePool();
        DontDestroyOnLoad(gameObject);
        waitInterval = new WaitForSeconds(interval);
    }
    private void InitializePool()
    {
        audioSourcePool = new List<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            AddAudioSourceToPool();
        }
    }
    AudioSource AddAudioSourceToPool()
    {
        AudioSource newAudioSource = GameObjectPool.Instance.GetObject(audioSourcePrefab).GetComponent<AudioSource>();
        audioSourcePool.Add(newAudioSource);
        return newAudioSource;
    }
    public void PlayOneShot(AudioClip clip, float volume = 1.0f)
    {
        AudioSource availableSource = GetAvailableSource();

        if (availableSource != null)
        {
            availableSource.volume = volume;
            if (Time.time - timer > interval)
            {
                availableSource.PlayOneShot(clip);
                //让声音逐渐变小

                timer = Time.time;
            }
        }
    }
    private AudioSource GetAvailableSource()
    {
        foreach (var audioSource in audioSourcePool)
        {
            if (!audioSource.isPlaying)
            {
                return audioSource;
            }
        }
        return AddAudioSourceToPool();
    }
}
