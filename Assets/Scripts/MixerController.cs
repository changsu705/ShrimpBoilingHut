using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;

    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    private void Awake()
    {
        if (m_MusicMasterSlider != null) m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (m_MusicBGMSlider != null) m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        if (m_MusicSFXSlider != null) m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        float db = (volume <= 0.0002f) ? -80f : Mathf.Log10(volume) * 20;
        m_AudioMixer.SetFloat("Master", db);

        if (m_MusicMasterSlider != null && m_MusicMasterSlider.value != volume)
        {
            m_MusicMasterSlider.value = volume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        float db = (volume <= 0.0002f) ? -80f : Mathf.Log10(volume) * 20;
        m_AudioMixer.SetFloat("BGM", db);

        if (m_MusicBGMSlider != null && m_MusicBGMSlider.value != volume)
        {
            m_MusicBGMSlider.value = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        float db = (volume <= 0.0002f) ? -80f : Mathf.Log10(volume) * 20;
        m_AudioMixer.SetFloat("SFX", db);

        if (m_MusicSFXSlider != null && m_MusicSFXSlider.value != volume)
        {
            m_MusicSFXSlider.value = volume;
        }
    }
}