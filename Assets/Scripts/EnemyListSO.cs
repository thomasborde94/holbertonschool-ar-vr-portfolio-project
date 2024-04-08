using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu]
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

    public int Count()
    {
        return enemyList.Count;
    }

    public void Clear()
    {
        enemyList.Clear();
    }

    public GameObject GetEnemy(int index)
    {
        if (index >= 0 && index < enemyList.Count)
        {
            return enemyList[index];
        }
        else
        {
            Debug.LogError("Index out of range in EnemyListSO");
            return null;
        }
    }

}
