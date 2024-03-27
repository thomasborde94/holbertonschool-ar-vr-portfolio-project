using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRole : MonoBehaviour
{
    [SerializeField] private int playerRole;
    // Start is called before the first frame update
   
    public void SetPlayerRole(int newPlayerRole)
    {
        playerRole = newPlayerRole;
    }
}
