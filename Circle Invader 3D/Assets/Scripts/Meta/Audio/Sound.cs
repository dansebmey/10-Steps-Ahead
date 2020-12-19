using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)] public float initVolume = 0.75f;
    [HideInInspector] public float targetVolume;
    [Range(0.5f, 1.5f)] public float initPitch = 1;
    [Range(0, 0.1f)] public float pitchVar = 0.05f;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}