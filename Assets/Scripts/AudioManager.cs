using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }

    [SerializeField] private AudioClip mainLoop;

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
