using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectInteraction : MonoBehaviour
{

    public GameObject player;
    [SerializeField] private float rangeOfInteraction = 5f;
    private bool isInRange=false;
    public trainingManager trainingManager;
    public currentTrainingInteractionManager currentManager;
    public DigiClock DigiClock_;
    
    private void Update()
    {
        float distance=Vector3.Distance(transform.position, player.transform.position);
        if(distance<=rangeOfInteraction)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            isInRange = true;
        }
        else
        {   transform.GetChild(0).gameObject.SetActive(false);
            isInRange = false;
        }


        if (isInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Debug.Log("We Interacted With training");
                player.GetComponent<CanvasesManager>().enabled = false;
                trainingManager.performTraining(transform.name);
                currentManager.currentObjActive(gameObject);
             
            }

        }
    }
}
