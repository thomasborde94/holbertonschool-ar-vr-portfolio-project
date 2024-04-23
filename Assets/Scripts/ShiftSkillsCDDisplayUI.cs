using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiftSkillsCDDisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject driverSkill;
    [SerializeField] private GameObject shooterSkill;
    [SerializeField] private Image mineImage;
    [SerializeField] public FloatVariable _mineCd;
    [SerializeField] private Image missileImage;
    [SerializeField] private FloatVariable _missileCd;

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
        else
            Debug.Log("player pas initialisé");

        if (isDriver)
        {
            mineImage.fillAmount = Player.Instance._nextMineTime / _mineCd.value;
        }
        if (isShooter)
            missileImage.fillAmount = Player.Instance._nextAOEMissileTime / _missileCd.value;


    }
}
