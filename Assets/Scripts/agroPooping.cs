using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agroPooping : MonoBehaviour
{

    public Transform player, movPos;
    public float range = 10;
    public DigimonMoodManager manager;
    private bool runOnce = false;
    public GameObject digimon;
    public bool isPooping=false;
   
    private void Update()
    {
        if (manager == null)
        {
            manager = GameObject.FindGameObjectWithTag("Player").GetComponent<DigimonMoodManager>();
            digimon=manager.gameObject;
        }
        else if(manager!=null&& manager.gameObject.activeInHierarchy==false)
         
        {
            manager = GameObject.FindGameObjectWithTag("Player").GetComponent<DigimonMoodManager>();
            digimon = manager.gameObject;
        }

        if (runOnce == false && manager.poop>80)
        {

            float distance = Vector3.Distance(movPos.transform.position, player.position);
            //Debug.Log(distance);
            if (distance <= range)
            {
                isPooping = true;
                digimon.transform.GetComponent<FollowerAI>().enabled = false;
                //manager.plannedPoop();
               
                runOnce = true;
            }
        }

        if(isPooping)
        {
            digimon.transform.position= Vector3.MoveTowards(digimon.transform.position, movPos.transform.position, 5*Time.deltaTime);
            float dist2 = Vector3.Distance(movPos.position, digimon.transform.position);

            if (dist2 <= 1)
            {
                
                manager.plannedPoop();
                Invoke("Reset", 10);
                runOnce = false;
                isPooping =false;
                 
            }
        }
    }

    public void Reset()
    {
        digimon.transform.GetComponent<FollowerAI>().enabled = true;
    }
}
