using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }

    [SerializeField] private AudioClip mainLoop;
    [SerializeField] private AudioMixer audioMixer;
    

    public int musicState = 1;
    [HideInInspector] public bool canPlay = true;
    [HideInInspector] public string volumeId = "volumeId";

    private AudioSource audioPlayer;
    private Slider soundSlider;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        soundSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
        audioPlayer = GetComponent<AudioSource>();
        float volume = PlayerPrefs.GetFloat(volumeId, 0);
        soundSlider.value = volume;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    private void Update()
    {
        if (canPlay)
        {
            canPlay = false;
            if (musicState == 1)
            {
                PlayMainMusic();
            }
        }
    }
    #endregion

    private void PlayMainMusic()
    {
        audioPlayer.clip = mainLoop;
        audioPlayer.Play();
        audioPlayer.loop = true;
    }
}
