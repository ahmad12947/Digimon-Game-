using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class enemyAi : MonoBehaviour
{
    [SerializeField] private float currentTimer = 0;
    [SerializeField] private float maxAttackTimer = 4;
    [SerializeField] private float battleRange = 10;
    [SerializeField]  private bool attackState=false;
    public Transform targetDigimon;

    private Animator animator;

    public GameObject attackGo;
    public bool isAttacking=false;
    private TextMeshProUGUI messageTxt;
    public GameObject playerHandler;


    private void clearText()
    {
        messageTxt.text = "";
    }

    private void Start()
    {
      playerHandler = GameObject.FindGameObjectWithTag("Player2");
      targetDigimon = GameObject.FindGameObjectWithTag("Player").transform;
      animator = GetComponent<Animator>();
      transform.LookAt(targetDigimon);   
    }

    public void startAttacking()
    {
        targetDigimon.GetComponent<AttackManager>().isAttacking = true;
        playerHandler.GetComponent<characterAnimationsHandler>().disableMovements();
    }
    public void checkRange()
    { 
        if (targetDigimon.GetComponent<AttackManager>().isAttacking == false)
        {
            float distance = Vector3.Distance(transform.position, targetDigimon.transform.position);
            if (distance <= battleRange)
            {
                //messageTxt.text = "Battle Commencing";
                //Invoke("clearText", 3);
                startAttacking();
            }
        }
    }
    private void Update()
    {
        transform.LookAt(targetDigimon);
        checkRange();
        if (attackState && transform.GetComponent<EnemyHealthManager>().isHit==false)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer > maxAttackTimer)
            {
                StartCoroutine("spawnVfx");
                animator.Play("Attack");
                currentTimer= 0;

            }
        }
        if(transform.GetComponent<EnemyHealthManager>().isHit == true)
        {
            currentTimer = 0;
        }
    }

    private IEnumerator spawnVfx()
    {
        
        yield return new WaitForSeconds(1);
        isAttacking = true;
        GameObject obj = Instantiate(attackGo,transform.position + new Vector3(0, 1, 0), attackGo.transform.rotation);
        obj.GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);
        yield return new WaitForSeconds(1);
        isAttacking = false;
        //isAttacking = false;
        Destroy(obj, 3);

    }
}
