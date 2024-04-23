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

    private bool assignedRole = false;
    private bool isShooter = false;
    private bool isDriver = false;

    private void Update()
    {
        if (Player.Instance != null)
        {
            if (!assignedRole)
            {
                if (Player.Instance.PlayerRoleString() == "Driver")
                {
                    isDriver = true;
                    driverSkill.SetActive(true);
                    shooterSkill.SetActive(false);
                    assignedRole = true;
                }

                else if (Player.Instance.PlayerRoleString() == "Shooter")
                {
                    isShooter = true;
                    driverSkill.SetActive(false);
                    shooterSkill.SetActive(true);
                    assignedRole = true;
                }
            }
            
        }

        if (isDriver)
            shockwaveImage.fillAmount = Player.Instance._nextShockwaveTime / _shockwaveCd.value;
        else if (isShooter)
            rainImage.fillAmount = Player.Instance._nextRainTime / _rainCd.value;
    }
}
