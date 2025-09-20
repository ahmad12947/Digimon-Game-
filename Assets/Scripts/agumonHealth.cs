using SlimUI.ModernMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agumonHealth : MonoBehaviour
{
   public int health;
   public int maxHealth;
   private Animator animator;
    public bool isHit = false;


    private void Start()
    {   
        animator = GetComponent<Animator>();
        health = maxHealth;
    }
    public void damage(int damage)
    {
        health-=damage;
        isHit = true;
        animator.Play("hit");
        Invoke("Reset", 2);
        if (health< 0)
        {
         
            //Dead
        }
    }
    public void Reset()
    {
        isHit = false;
    }

}
