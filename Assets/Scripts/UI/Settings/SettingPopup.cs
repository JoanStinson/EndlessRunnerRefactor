using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider masterSFXSlider;
    public LoadoutState loadoutState;
    public DataDeleteConfirmation confirmationPopup;

    protected const float k_MinVolume = -80f;
    protected const string k_MasterVolumeFloatName = "MasterVolume";
    protected const string k_MusicVolumeFloatName = "MusicVolume";
    protected const string k_MasterSFXVolumeFloatName = "MasterSFXVolume";

    protected float m_MasterVolume;
    protected float m_MusicVolume;
    protected float m_MasterSFXVolume;

    public void Open()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Close()
    {
        ServiceLocator.Instance.GetService<IPlayerData>().Save();
        gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        mixer.GetFloat(k_MasterVolumeFloatName, out m_MasterVolume);
        mixer.GetFloat(k_MusicVolumeFloatName, out m_MusicVolume);
        mixer.GetFloat(k_MasterSFXVolumeFloatName, out m_MasterSFXVolume);

        masterSlider.value = 1.0f - (m_MasterVolume / k_MinVolume);
        musicSlider.value = 1.0f - (m_MusicVolume / k_MinVolume);
        masterSFXSlider.value = 1.0f - (m_MasterSFXVolume / k_MinVolume);
    }

    public void DeleteData()
    {
        confirmationPopup.Open(loadoutState);
    }

    public void MasterVolumeChangeValue(float value)
    {
        m_MasterVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MasterVolumeFloatName, m_MasterVolume);
        ServiceLocator.Instance.GetService<IPlayerData>().MasterVolume = m_MasterVolume;
    }

    public void MusicVolumeChangeValue(float value)
    {
        m_MusicVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MusicVolumeFloatName, m_MusicVolume);
        ServiceLocator.Instance.GetService<IPlayerData>().MusicVolume = m_MusicVolume;
    }

    public void MasterSFXVolumeChangeValue(float value)
    {
        m_MasterSFXVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MasterSFXVolumeFloatName, m_MasterSFXVolume);
        ServiceLocator.Instance.GetService<IPlayerData>().MasterSFXVolume = m_MasterSFXVolume;
    }
}
