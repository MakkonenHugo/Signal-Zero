using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public string exposedParameterName = "MasterVolume";

    private const string PrefsKey = "MasterVolume";

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(PrefsKey, 0.75f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        SetVolume(savedVolume);
    }

    public void SetVolume(float value)
    {
        if (audioMixer != null)
        {
            float dB = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
            audioMixer.SetFloat(exposedParameterName, dB);
        }

        PlayerPrefs.SetFloat(PrefsKey, value);
    }
}