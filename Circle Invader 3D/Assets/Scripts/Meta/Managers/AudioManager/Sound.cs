using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)] public float initVolume = 0.75f;
    [Range(0.5f, 1.5f)] public float initPitch = 1f;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}