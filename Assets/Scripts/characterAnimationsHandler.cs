using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;
using Invector.vCharacterController;
public class characterAnimationsHandler : MonoBehaviour
{
    public Animator playerAnimations;
    public GameObject meat, digimon;
    private Quaternion rotations;
    public vShooterMeleeInput vShooterMeleeInput_;
    public DigimonMoodManager digimonMoodManager_;

    private void Start()
    {
        digimonMoodManager_ = FindObjectOfType<DigimonMoodManager>();
        digimon = GameObject.FindGameObjectWithTag("Player");
        
    }
    public void changeDigimonHandler(GameObject obj)
    {
        digimon=obj;
    }
    private void Update()
    {
        digimon= GameObject.FindGameObjectWithTag("Player");
    }
    public void disableMovements()
    {
        //vShooterMeleeInput_.enabled= false;
    }
    public void enableMovements()
    {
        //vShooterMeleeInput_.enabled = true;
    }
    public void useCalled()
    {
        playerAnimations.Play("Pass_Card");
        rotations = transform.rotation;
        transform.LookAt(digimon.transform.position);
        meat.SetActive(true);
        Invoke("deActivate", 3);
    }
    public void deActivate()
    {
        transform.rotation = rotations;
        meat.SetActive(false);
    }

    public void scoldCalled()
    {
        rotations = transform.rotation;
        transform.LookAt(digimon.transform.position);
        Invoke("deActivate", 3);
        digimon.GetComponent<digimonaAnimationManager>().scold();
    
        playerAnimations.Play("scold");
    }
    public void praiseCalled()
    {
        rotations = transform.rotation;
        transform.LookAt(digimon.transform.position);
        Invoke("deActivate", 3);
        digimon.GetComponent<digimonaAnimationManager>().praise();
        playerAnimations.Play("Clapping");
    }

    public void sleep()

    {
        
        if(digimon.name=="Botamon")
        {
            digimon.GetComponent<digimonaAnimationManager>().sleepFresh();
        }
        else if(digimon.name== "Koromon")
        {
            digimon.GetComponent<digimonaAnimationManager>().sleepInTraining();
        }

        else
        {
            digimon.GetComponent<digimonaAnimationManager>().sleep();
        }
    }
  
    public void rest()
    {
        digimon.GetComponent<digimonaAnimationManager>().rest();
    }
}
