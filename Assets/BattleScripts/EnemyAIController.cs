using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public BattleDigimonController controller;
    public DigimonCombatStats target;

    private float actionCooldown = 3f;
    private float timer = 0f;


    private void Start()
    {
        controller=GetComponent<BattleDigimonController>();
        target=GameObject.FindGameObjectWithTag("Player").GetComponent<DigimonCombatStats>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= actionCooldown)
        {
            timer = 0f;
            int index = Random.Range(0, controller.equippedMoves.Length);
            controller.PerformAttack(index, target);
        }
    }
}
