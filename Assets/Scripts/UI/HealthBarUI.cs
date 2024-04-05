using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Material currentMat;
    [SerializeField] private Color colorStart, colorEnd;

    [SerializeField, Range(0f, 1f)] private float startThreshold = 0.7f;
    [SerializeField, Range(0f, 1f)] private float endThreshold = 0.3f;

    private GameObject targetLookAt;


    private Color _color;

    private void Start()
    {
        targetLookAt = GameObject.Find("targetLookatHealthBar");
        if (targetLookAt == null)
            Debug.Log("couldnot find target for healthBar");
    }

    private void Update()
    {
        float healthPart = Player.Instance.GetCurrentHealthPart();

        if (healthPart >= startThreshold)
            _color = colorEnd;

        else if (healthPart <= endThreshold)
            _color = colorStart;

        else
            _color = Color.Lerp(colorStart, colorEnd, Player.Instance.GetCurrentHealthPart());

        currentMat.SetFloat("_Health", Player.Instance.GetCurrentHealthPart());
        currentMat.SetColor("_Color", _color);

        transform.LookAt(targetLookAt.transform.position, Vector3.up);
    }
}
