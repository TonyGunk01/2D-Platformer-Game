using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    public SoundType[] Sounds;
    public bool IsMute = false;
    public float Volume = 1f;

    public AudioSource soundEffect;
    public AudioSource soundMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetVolume(0.5f);
        PlayMusic(global::Sounds.Music);
    }

    public void Mute(bool status)
    {
        IsMute = status;
    }

    public void SetVolume(float volume)
    {
        Volume = volume;
        soundEffect.volume = Volume;
        soundMusic.volume = Volume;
    }

    public void PlayMusic(Sounds sound)
    {
        if (IsMute)
            return;

        AudioClip clip = getSoundClip(sound);

        if (clip != null)
        {
            soundMusic.clip = clip;
            soundMusic.Play();
        }

        else
            Debug.LogError("Clip not found for sound type: " + sound.ToString());
    }

    public void Play(Sounds sound)
    {
        AudioClip clip = getSoundClip(sound);

        if (clip != null)
            soundEffect.PlayOneShot(clip);

        else
            Debug.LogError("Clip not found for sound type: " + sound.ToString());
    }

    private AudioClip getSoundClip(Sounds sound)
    {
        SoundType item = Array.Find(Sounds, i => i.soundType == sound);

        if(item != null)
            return item.soundClip;
        return null;
    }
}

[Serializable]
public class SoundType
{
    public Sounds soundType;
    public AudioClip soundClip;
}

public enum Sounds
{
    ButtonClick,
    Music,
    PlayerDeath,
    ChomperAttack,
    ChomperDeath,
    GunnerDeath,
    KeyCollect,
    BulletFire,
    LevelComplete
}
