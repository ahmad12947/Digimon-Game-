using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class digimonaAnimationManager : MonoBehaviour
{
    public Animator animator;
    public FollowerAI FollowerAI;
    public GameObject sleepCanvas;
    public DigimonMoodManager moodManager;
    public DigiClock clock;
  
    private void Start()
    {
        FollowerAI=GetComponent<FollowerAI>();
    }
    public void eat()
    {  
        FollowerAI.enabled = false;
        moodManager.ChangeHunger(-70);
        //moodManager.ChangePoop(30);
        Invoke("ReEnable", 4);
        animator.Play("Eat");
    }
    public void praise()
    {
        moodManager.onPraise();
        Invoke("ReEnable", 4);
        FollowerAI.enabled = false;
        animator.Play("happy");
    }
    public void scold()
    {
        Invoke("ReEnable", 4);
        FollowerAI.enabled = false;
        moodManager.ChangeHappiness(-10);
        moodManager.ChangeDiscipline(4);
        animator.Play("Scold");

    }
    public void sleep()
    {
        moodManager.plannedSleep();
        //moodManager.poop = 0;
        //moodManager.Happiness = 80;
        //moodManager.hunger = 20;
        FollowerAI.enabled = false;
        sleepCanvas.SetActive(true);
        sleepCanvas.transform.GetChild(0).GetComponent<Animator>().Play("blackScreen");
        Invoke("ReEnable", 3);
        animator.Play("sleep");
        clock.AddTime(8);
    }

    public void sleepFresh()
    {
        moodManager.plannedSleep();
        //moodManager.poop = 0;
        //moodManager.Happiness = 80;
        //moodManager.hunger = 20;
        FollowerAI.enabled = false;
        sleepCanvas.SetActive(true);
        sleepCanvas.transform.GetChild(0).GetComponent<Animator>().Play("blackScreen");
        Invoke("ReEnable", 3);
        animator.Play("sleep");
        clock.AddTime(1);
    }
    public void sleepInTraining()
    {
        moodManager.plannedSleep();
        //moodManager.poop = 0;
        //moodManager.Happiness = 80;
        //moodManager.hunger = 20;
        FollowerAI.enabled = false;
        sleepCanvas.SetActive(true);
        sleepCanvas.transform.GetChild(0).GetComponent<Animator>().Play("blackScreen");
        Invoke("ReEnable", 3);
        animator.Play("sleep");
        clock.AddTime(3);
    }

    public void autoSleep()
    {
        //moodManager.poop = 0;
        //moodManager.Happiness = 80;
        //moodManager.hunger = 20;
        FollowerAI.enabled = false;
        sleepCanvas.SetActive(true);
        sleepCanvas.transform.GetChild(0).GetComponent<Animator>().Play("blackScreen");
        Invoke("ReEnable", 3);
        animator.Play("sleep");
        clock.AddTime(8);

    }
    public void rest()
    {
        FollowerAI.enabled = false;
        sleepCanvas.SetActive(true);
        sleepCanvas.transform.GetChild(0).GetComponent<Animator>().Play("blackScreen");
        Invoke("ReEnable", 3);
        animator.Play("sleep");
    }
    public void poop()
    {
        FollowerAI.enabled = false;
        Invoke("ReEnable", 3);
        animator.Play("StandToSit");
        moodManager.ChangeHunger(30);
        moodManager.ChangePoop(-100);
    }


    public void ReEnable()
    {
        sleepCanvas.SetActive(false);
        animator.Play("Idle");
        FollowerAI.enabled = true;
    }
}
