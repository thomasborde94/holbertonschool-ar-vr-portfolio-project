using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() => {
            StartCoroutine(SFXManager.Instance.PlaySoundAndLoadLobbyScene());
        });
        quitButton.onClick.AddListener(() => {
            SFXManager.Instance.PlaySFX(0);
            Application.Quit();
        });

        Time.timeScale = 1f;
    }
}
