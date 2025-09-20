using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class battleCommands : MonoBehaviour
{
    [Header("Attack Components")]

    public GameObject attackGo;
    public Transform attackSpawnPoint;
    public bool isAttacking=false;
    [Header("Auto Components")]
    public bool autoAttack=false;
    private float currentTime = 0;
    [SerializeField]private float autoAttackTimer = 3;
    public Animator animator;

    [Header("Run Components")]
    public bool isRunning=false;

    [Header("Defense Components")]
    private float currentDefenseTimer = 0;
    [SerializeField] private float maxDefenseTimer = 10;
    private bool isDefending = false;
    private bool defenseTimeOUT=false;


    [Header("Distance Component")]
    private bool isDistanceCalled=false;

    public agumonHealth health;
    public AttackManager attack_Manager;
    private void Start()
    {
        animator = GetComponent<Animator>();
        attack_Manager = GetComponent<AttackManager>();
    }
    public void peroformAttack()
    {
        if (isAttacking == false)
        {
            animator.Play("Attack");
            autoAttack = false;
            StartCoroutine("spawnVfx");
            isDefending = false;
            isAttacking = true;
        }
    }
    public void performAutoAttack()
    {
        StartCoroutine("spawnVfx");
    }
    public void Set_autoAttack()
    {
       if(autoAttack==false)
        {
            isDefending = false;
            autoAttack = true;
        }
    }

    public void performDefense()
    {
        if (isDefending == false&&defenseTimeOUT==false)
        {
            autoAttack = false;
            isAttacking = false;
            //animator.Play("Defense");
            isDefending = true;
        }
    }

    public void runOff()
    {
        isRunning= true;
        autoAttack = false;
        animator.Play("Running");
    }

    public void performDistance()
    {
        StartCoroutine(defense());
        isDistanceCalled = true;
        animator.Play("Running");
        autoAttack = false;
    }
    private IEnumerator defense()
    {
        yield return new WaitForSeconds(0);
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(4);
        transform.GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<Rigidbody>().isKinematic =false;
    }
    private void Update()
    {
        if (health.isHit == false)
        {
            if (autoAttack)
            {
                currentTime += Time.deltaTime;
                if (currentTime > autoAttackTimer)
                {
                    animator.Play("Attack");
                    performAutoAttack();
                    currentTime = 0;
                }

            }

            if (isRunning == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, 0, 5), 10 * Time.deltaTime);
                Vector3 direction = transform.position - transform.position + new Vector3(0, 0, 5);
                Invoke("stopRunning", 4);
                transform.rotation = Quaternion.LookRotation(direction);

            }

            if (isDistanceCalled == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, 0, 5), 10 * Time.deltaTime);
                Vector3 direction = transform.position - transform.position + new Vector3(0, 0, 5);
                transform.rotation = Quaternion.LookRotation(direction);      
                Invoke("stopRunning", 2);
                
            }
        }
        else
        {
            currentTime = 0;
        }

        if (isDefending == true)
        {
            if (attack_Manager.closestEnemy.GetComponent<enemyAi>().isAttacking&&defenseTimeOUT==false)
            {
                StartCoroutine("defense");
                animator.Play("Defend");              
                defenseTimeOUT = true;
               
            }
        }
        if (defenseTimeOUT == true)
        {
            currentDefenseTimer += Time.deltaTime;
            if (currentDefenseTimer >= maxDefenseTimer)
            {

                defenseTimeOUT = false;
                currentDefenseTimer = 0;
            }
        }
    }
    public void stopRunning()
    {
        isDistanceCalled = false;
        isRunning = false;
    }
    public IEnumerator spawnVfx()
    {
        yield return new WaitForSeconds(1);
        GameObject obj = Instantiate(attackGo, attackSpawnPoint.transform.position+new Vector3(0,1,0), attackGo.transform.rotation);
        obj.GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);
        yield return new WaitForSeconds(2);
        isAttacking = false;
        Destroy(obj, 3);

    }
}
