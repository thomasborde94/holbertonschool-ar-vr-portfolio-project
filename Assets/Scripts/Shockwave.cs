using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] int _pointsCount;
    [SerializeField] float _maxRadius;
    [SerializeField] float _speed;
    [SerializeField] float _startWidth;

    [SerializeField] private int _shockwaveDamage = 2;
    public LineRenderer _lineRenderer;
    public SphereCollider _sphereCollider;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _pointsCount + 1;
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        _lineRenderer.enabled = false;
        _sphereCollider.enabled = false;
    }
    public IEnumerator Blast()
    {
        _sphereCollider.enabled = true;
        float currentRadius = 0f;
        while (currentRadius < _maxRadius)
        {
            currentRadius += Time.deltaTime * _speed;
            Draw(currentRadius);
            yield return null;
        }
        _sphereCollider.enabled = false;
    }

    private void Draw(float currentRadius)
    {
        float angleBetweenPoints = 360f / _pointsCount;
        for (int i = 0; i <= _pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction * currentRadius;

            _lineRenderer.SetPosition(i, position);
        }

        _lineRenderer.widthMultiplier = Mathf.Lerp(0f, _startWidth, 1f - currentRadius / _maxRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetCurrentHealthLoss(_shockwaveDamage);
                enemy.GotHit();
            }
        }
    }
}
