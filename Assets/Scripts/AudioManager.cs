using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }

    [SerializeField] private AudioClip mainLoop;
    [SerializeField] private AudioMixer audioMixer;

    public int musicState = 1;
    [HideInInspector] public bool canPlay = true;

    private AudioSource audioPlayer;

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
        audioPlayer = GetComponent<AudioSource>();
        float volume = 0.5f;
        audioMixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20);
    }

    private void Update()
    {
        if (canPlay)
        {
            canPlay = false;
            if (musicState == 1)
            {
                audioPlayer.clip = mainLoop;
                audioPlayer.Play();
                audioPlayer.loop = true;
            }
        }
    }

}
