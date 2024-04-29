using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioSource[] _sfx;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(int sfxToplay)
    {
        _sfx[sfxToplay].Play();
    }

    public void StopSFX(int sfxToStop)
    {
        _sfx[sfxToStop].Stop();
    }

    public IEnumerator PlaySoundAndLoadLobbyScene()
    {
        PlaySFX(0);
        yield return new WaitForSeconds(0.2f);
        Loader.Load(Loader.Scene.LobbyScene);
    }
    public IEnumerator PlaySoundAndLoadMainMenuScene()
    {
        PlaySFX(0);
        yield return new WaitForSeconds(0.2f);
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}
