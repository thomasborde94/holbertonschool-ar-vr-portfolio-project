using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VolumeScript : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private Slider soundSlider;
    private float volume;

    private void Start()
    {
        soundSlider = GetComponent<Slider>();
        volume = PlayerPrefs.GetFloat("MasterVolume", 0);
        soundSlider.value = volume;
    }
    void Update()
    {
        SetMasterVolume();
    }
    public void SetMasterVolume()
    {
        volume = soundSlider.value;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
}
