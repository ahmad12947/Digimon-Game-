using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public AttackManager attackManager;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            attackManager.OpenMenu();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // Optional: Close menu on Escape
        {
            attackManager.CloseMenu();
        }
    }
}
