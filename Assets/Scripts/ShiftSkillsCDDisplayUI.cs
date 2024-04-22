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
            mineImage.fillAmount = Player.Instance._nextMineTime / _mineCd.value;

        }
        else
        {
            missileImage.fillAmount = Player.Instance._nextAOEMissileTime / _missileCd.value;
        }
    }
}
