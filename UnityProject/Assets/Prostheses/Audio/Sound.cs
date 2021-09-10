using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; 
    public AudioClip clip;
    [Range(0.0f, 5.0f)]
    public float volume;

    [HideInInspector]
    public AudioSource source; 
}
