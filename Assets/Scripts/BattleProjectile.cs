using UnityEngine;

public class BattleProjectile : MonoBehaviour
{
    private MoveData moveData;
    private DigimonCombatStats targetStats;
    private BattleDigimonController sourceController;
    private bool isOutOfRange = false;

    public void Launch(MoveData move, DigimonCombatStats target, BattleDigimonController source)
    {
        moveData = move;
        targetStats = target;
        sourceController = source;

        float dist = Vector3.Distance(transform.position, target.transform.position);
        isOutOfRange = dist > move.attackRange;

        Vector3 dir = (target.transform.position + Vector3.up * 1.5f - transform.position).normalized;
        GetComponent<Rigidbody>().linearVelocity = dir * move.projectileSpeed;

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DigimonCombatStats>() == targetStats)
        {
            if (isOutOfRange)
            {
                Debug.Log($"Projectile from {sourceController.name} MISSED due to being out of range.");
                // Optional: Play a "miss" effect here
            }
            else
            {
                sourceController.StartCoroutine(sourceController.ApplyDamageAfterDelay(targetStats, moveData, 0.3f));
            }

            Destroy(gameObject);
        }
    }
}
