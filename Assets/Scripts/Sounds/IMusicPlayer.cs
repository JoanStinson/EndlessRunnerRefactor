using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public interface IMusicPlayer
{
    AudioMixer Mixer { get; }

    AudioClip GetStem(int index);
    IEnumerator RestartAllStems();
    void SetStem(int index, AudioClip clip);
    void UpdateVolumes(float currentSpeedRatio);
}