using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider bgmSlider;
    public Slider effectSlider;

    public void SetMusicVolume(float volume)
    {
        Debug.Log(bgmSlider.value);
        audioMixer.SetFloat("BGM Volumn", bgmSlider.value);
    }

    public void SetSoundEffectVolume(float volume)
    {
        audioMixer.SetFloat("Effect Volumn", effectSlider.value);
    }
}
