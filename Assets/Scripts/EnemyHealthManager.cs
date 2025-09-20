using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    public int c_health;
    public int maxHealth;
    public Slider healthSlider;
    public Animator animator;
    public bool isHit = false;
    public DigimonCombatStats combatController;

    private void Start()
    {
    
        combatController = GetComponent<DigimonCombatStats>();
        c_health = combatController.currentHP;
        maxHealth = combatController.maxHP;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = c_health;
        healthSlider.maxValue=c_health;
    }  

    public void Update()
    {
        c_health = combatController.currentHP;
        healthSlider.value = c_health;
    }

    public void damage(int damage)
    {
        Debug.Log("isRunning");
        isHit = true;
        c_health = combatController.currentHP;
        healthSlider.value = c_health;

        if (c_health <= 0)
        {
            animator.Play("Death");
            Destroy(gameObject);
           
           

         
        }

        Invoke("Reset", 2);
    }

    public void Reset()
    {
        //transform.GetComponent<EnemyHealthManager>().enabled = false;
        isHit = false;
    }
}
