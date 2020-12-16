using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    
    public Sound music;
    public Sound[] sfx;

    private float _musicVolume = -1;
    public float MusicVolume
    {
        get => _musicVolume;
        set 
        {
            _musicVolume = value;
            music.targetVolume = sfx[0].initVolume * _musicVolume;
        }
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        music.source.volume = music.targetVolume;
    }
    
    private float _sfxVolume = -1;
    public float SfxVolume
    {
        get => _sfxVolume;
        set 
        {
            _sfxVolume = value;
            foreach (Sound s in sfx)
            {
                s.targetVolume = s.initVolume * _sfxVolume;
                s.source.volume = s.targetVolume;
            }
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        
        music.source = gameObject.AddComponent<AudioSource>();
        music.targetVolume = MusicVolume >= 0 ? MusicVolume : music.initVolume;
        music.source.clip = music.clip;
        music.source.volume = 0;
        music.source.pitch = music.initPitch;
        music.source.loop = music.loop;
        
        foreach (Sound s in sfx)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.targetVolume = s.initVolume;
            
            s.source.clip = s.clip;
            s.source.volume = s.initVolume;
            s.source.pitch = s.initPitch;
            s.source.loop = s.loop;
        }
    }

    public void PlayMusic()
    {
        music.source.Play();
        FadeMusicVolume(0, music.targetVolume, 2);
    }

    public void Play(string soundName)
    {
        Sound s = FindSound(soundName);
        s.source.pitch = s.initPitch + UnityEngine.Random.Range(-s.pitchVar, s.pitchVar);
        s.source.Play();
    }

    public void Play(string soundName, float pitchVar)
    {
        Sound s = FindSound(soundName);
        s.source.pitch = s.initPitch + UnityEngine.Random.Range(pitchVar, pitchVar);
        s.source.Play();
    }

    private void FadeMusicVolume(float startValue, float targetValue, float fadeDuration)
    {
        StartCoroutine(AdjustVolume(startValue, targetValue, fadeDuration));
    }

    private IEnumerator AdjustVolume(float startValue, float targetValue, float fadeDuration, Action onCompleteAction = null)
    {
        float delta = targetValue - startValue;
        float volume = startValue;

        while(Mathf.Abs(volume - targetValue) > 0.02f)
        {
            volume += (0.01f / fadeDuration) * delta;
            music.source.volume = volume;
            
            yield return new WaitForSeconds(0.01f);
        }

        onCompleteAction?.Invoke();
    }

    public void Stop(string soundName)
    {
        Sound s = FindSound(soundName);
        s.source.Stop();
    }

    public void PlayPitched(string soundName, float pitch)
    {
        Sound s = FindSound(soundName);
        s.source.pitch = pitch;
        s.source.loop = s.loop;
        s.source.Play();
    }

    public void PlayPitched(string soundName, float pitch, float pitchVar)
    {
        Sound s = FindSound(soundName);
        s.source.pitch = pitch + UnityEngine.Random.Range(-pitchVar, pitchVar);
        s.source.loop = s.loop;
        s.source.Play();
    }

    public static AudioManager GetInstance()
    {
        return _instance;
    }

    public Sound FindSound(string soundName)
    {
        Sound foundSound = Array.Find(sfx, sound => sound.name == soundName);
        if (foundSound == null)
        {
            Debug.LogError("Sound ["+soundName+"] was not found");
        }
        return foundSound;
    }

    public void TogglePause(string soundName, bool doPause)
    {
        Sound s = FindSound(soundName);
        s.source.volume = _sfxVolume * (doPause ? 0.1f : s.targetVolume);
        // StartCoroutine(doPause
        //     ? AdjustVolume(s, s.source.volume, 0.1f, 0.5f, s.source.Pause)
        //     : AdjustVolume(s, 0.1f, s.initVolume, 0.5f, s.source.UnPause));
    }
}