using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ESkillsCDDisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject driverSkill;
    [SerializeField] private GameObject shooterSkill;
    [SerializeField] private Image shockwaveImage;
    [SerializeField] public FloatVariable _shockwaveCd;
    [SerializeField] private Image rainImage;
    [SerializeField] private FloatVariable _rainCd;

    private void Start()
    {
        if (Player.Instance.PlayerRoleString() == "Driver")
        {
            driverSkill.SetActive(true);
            shooterSkill.SetActive(false);
        }
        else
        {
            driverSkill.SetActive(false);
            shooterSkill.SetActive(true);
        }
    }

    private void Update()
    {
        if (Player.Instance.PlayerRoleString() == "Driver")
        {
            shockwaveImage.fillAmount = Player.Instance._nextShockwaveTime / _shockwaveCd.value;

        }
        else
        {
            rainImage.fillAmount = Player.Instance._nextRainTime / _rainCd.value;
        }
    }
}
