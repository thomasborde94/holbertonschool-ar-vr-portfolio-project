using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyListSO : ScriptableObject
{
    public List<GameObject> enemyList = new List<GameObject>();

    public void AddEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
    }
}
