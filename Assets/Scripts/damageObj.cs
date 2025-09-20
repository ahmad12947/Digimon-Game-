using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageObj : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
       if(other.GetComponent<EnemyHealthManager>() != null)
        {
            other.GetComponent<EnemyHealthManager>().damage(25);
        }
    }

}
