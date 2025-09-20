using UnityEngine;
using System.Collections;

public class BattleDigimonController : MonoBehaviour
{
    public DigimonCombatStats stats;
    public MoveData[] equippedMoves;
    public Animator anim;

    public Transform projectileSpawnPoint;
    public bool isDead = false;

    public float attackCooldown = 1.5f;
    public float vfxSpawnDelay = 0.3f;

    private bool isInCooldown = false;
    public bool isDefending = false;

    private Vector3 startingPosition;
    public float baseMoveSpeed = 4f;
    public float baseMoveDistance = 2.5f;
    private Transform lastAttackTarget;
    private bool isBeingHit = false;
    public EnemyHealthManager healthManager;

    public GameObject damageNumberPrefab; // assign in Inspector
    public Transform damagePopupAnchor; // optional: to control exact spawn position

    public GameObject babyDigimon;
    private void Start()
    {
        Invoke(nameof(startImporting), 2);
        anim = GetComponent<Animator>();
        projectileSpawnPoint = transform;
        startingPosition = transform.position;
    }

    public void startImporting()
    {
        stats = GetComponent<DigimonCombatStats>();
    }
    public void SetBattleStartPosition()
    {
        startingPosition = transform.position;
    }

    public void PerformAttack(int index, DigimonCombatStats target)
    {
        if (isDead || isInCooldown || index >= equippedMoves.Length) return;

        if (BattleManager.Instance != null && BattleManager.Instance.IsInHitPause())
        {
            Debug.Log("Skipping attack due to hit pause.");
            return;
        }

        MoveData move = equippedMoves[index];

        if (stats.currentMP < move.mpCost)
        {
            Debug.Log($"{stats.digimonName} doesn't have enough MP!");
            return;
        }

      

        stats.UseMP(move.mpCost);
        stats.numAttacks++;
        lastAttackTarget = target.transform;

        StartCoroutine(AttackSequence(index, target, move));
    }

    private IEnumerator AttackSequence(int index, DigimonCombatStats target, MoveData move)
    {
        isInCooldown = true;

        bool willMove = Random.value > 0.3f;
        float moveDistance = baseMoveDistance + Random.Range(-0.5f, 0.5f);
        float moveSpeed = baseMoveSpeed + Random.Range(-0.5f, 0.5f);

        Vector3 attackPosition = transform.position;

        if (willMove)
        {
            Vector3 targetDirection = (target.transform.position - transform.position).normalized;
            attackPosition += targetDirection * moveDistance;
            yield return MoveToPosition(attackPosition, moveSpeed);
        }

        anim?.Play("Attack");
        yield return new WaitForSeconds(vfxSpawnDelay);

        if (move.projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(move.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            BattleProjectile proj = projectile.GetComponent<BattleProjectile>();
            proj.Launch(move, target, this);
        }
        else
        {
            yield return ApplyDamageAfterDelay(target, move, 0.5f);
        }

        yield return new WaitForSeconds(attackCooldown);

        if (willMove)
            yield return MoveToPosition(startingPosition, moveSpeed);

        anim?.Play("Idle");

        yield return new WaitForSeconds(1f);
        isInCooldown = false;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        anim?.Play("Walking");

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        if (directionToTarget.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(directionToTarget);

        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;

        if (targetPosition == startingPosition && lastAttackTarget != null)
        {
            Vector3 faceDirection = (lastAttackTarget.position - transform.position).normalized;
            if (faceDirection.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(new Vector3(faceDirection.x, 0, faceDirection.z));
        }

        anim?.Play("Idle");
    }

    public IEnumerator ApplyDamageAfterDelay(DigimonCombatStats target, MoveData move, float delay)
    {
        yield return new WaitForSeconds(delay);
        ApplyDamageLogic(target, move);
    }

    public void ApplyDamageLogic(DigimonCombatStats target, MoveData move)
    {
        if (isDead) return;

       
        float hitChance = BattleUtils.CalculateHitChance(move.accuracy, stats.speed, target.speed);
        float roll = Random.Range(0f, 100f);

        if (roll <= hitChance)
        {
            int damage = BattleUtils.CalculateDamage(move.power, stats.offense, target.defense);

            var targetUI = target.GetComponentInChildren<BattleUIManager>();
            if (targetUI != null && targetUI.IsDefending)
            {
                damage = Mathf.RoundToInt(damage * 0.25f);
                targetUI.ResetDefendState();

                target.GetComponent<Animator>()?.Play("Defend");
                Debug.Log($"{target.digimonName} defended! Reduced damage to {damage}.");
            }
          
            bool isHeavy = (float)damage / target.maxHP >= 0.2f;
            if (isHeavy) target.heavyHits++;

            target.TakeDamage(damage);
            if (healthManager != null)
            {
                healthManager.damage(damage);
            }
           
            if (damageNumberPrefab != null)
            {
                Vector3 spawnPos = target.transform.position + Vector3.up * 2f; // or use target.damagePopupAnchor.position
                GameObject dmgObj = Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity);
                Destroy(dmgObj, 3);
                DamageNumber dmgNumber = dmgObj.GetComponent<DamageNumber>();
                if (dmgNumber != null)
                    dmgNumber.Initialize(damage);
            }
            
            target.GetComponent<BattleDigimonController>()?.OnHit();

            //Debug.Log($"{stats.digimonName} hit {target.digimonName} for {damage} damage!");
        }
        else
        {
            //Debug.Log($"{stats.digimonName}'s attack missed!");
        }
    }
    public void StartDefend()
    {
        if (isDead || isInCooldown)
        {
            Debug.Log($"{stats.digimonName} cannot defend now (dead or cooling down).");
            return;
        }

        isDefending = true;
        anim?.Play("Defend");
        Debug.Log($"{stats.digimonName} is now defending!");
    }
    public void OnHit()
    {
        if (isDead || isBeingHit) return;

        if (isDefending)
        {
            anim?.Play("Defend");
        }
        else
        {
            StartCoroutine(DelayedHitAnimation());
        }

        isDefending = false;

        BattleManager.Instance?.StartPlayerHitPause(1.2f);

        if (stats.currentHP <= 0)
            Die();
    }


    private IEnumerator DelayedHitAnimation()
    {
        isBeingHit = true;
        anim?.Play("hit");

        // Optional: you could stop movement or other actions here

        yield return new WaitForSeconds(0.8f); // match hit animation duration

        // Only return to Idle if not performing another animation
        if (!isDead && anim.GetCurrentAnimatorStateInfo(0).IsName("hit"))
        {
            anim?.Play("Idle");
        }

        isBeingHit = false;
    }


    public bool IsInCooldown() => isInCooldown;

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        anim?.Play("Death");
        if (transform.GetComponent<EvolutionManager>() != null)
        {
            transform.GetComponent<EvolutionManager>().onDeathEvolved(babyDigimon);
        }
        this.enabled = false;
    }

    public void PerformAIAttack(DigimonCombatStats target)
    {
        if (isDead || isInCooldown || target.currentHP <= 0) return;

        int tries = 0;
        int index = Random.Range(0, equippedMoves.Length);
        while ((equippedMoves[index] == null || equippedMoves[index].mpCost > stats.currentMP) && tries < 10)
        {
            index = Random.Range(0, equippedMoves.Length);
            tries++;
        }

        if (equippedMoves[index] != null)
            PerformAttack(index, target);
    }

    public IEnumerator PerformMoveAway(DigimonCombatStats target)
    {
        if (isDead || isInCooldown) yield break;

        isInCooldown = true;

        anim?.Play("Walking");

        Vector3 directionFromEnemy = (transform.position - target.transform.position).normalized;

        Vector3 lateralDir = Vector3.Cross(Vector3.up, directionFromEnemy).normalized;
        float side = Random.Range(-1f, 1f);
        Vector3 moveDir = (side < -0.33f) ? directionFromEnemy :
                          (side < 0.33f) ? lateralDir : -lateralDir;

        float retreatDistance = 2.5f;
        Vector3 retreatPosition = transform.position + moveDir * retreatDistance;

        float maxDistanceFromStart = 5f;
        if (Vector3.Distance(retreatPosition, startingPosition) > maxDistanceFromStart)
        {
            Vector3 dirToStart = (startingPosition - transform.position).normalized;
            retreatPosition = transform.position + dirToStart * retreatDistance;
        }

        yield return MoveToPosition(retreatPosition, baseMoveSpeed);

        if (target != null)
        {
            Vector3 faceDir = (target.transform.position - transform.position).normalized;
            if (faceDir.sqrMagnitude > 0.001f)
            {
                Quaternion lookAtEnemy = Quaternion.LookRotation(new Vector3(faceDir.x, 0, faceDir.z));
                transform.rotation = lookAtEnemy;
            }
        }

        anim?.Play("Idle");
        yield return new WaitForSeconds(1f);
        isInCooldown = false;
    }

    public void StartMoveAway()
    {
        DigimonCombatStats closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        if (BattleManager.Instance != null && BattleManager.Instance.enemyControllers != null)
        {
            foreach (var controller in BattleManager.Instance.enemyControllers)
            {
                if (controller == null || controller.stats == null || controller.stats.currentHP <= 0)
                    continue;

                float dist = Vector3.Distance(transform.position, controller.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestEnemy = controller.stats;
                }
            }
        }

        if (closestEnemy != null)
            StartCoroutine(PerformMoveAway(closestEnemy));
    }

    public void ForceImmediateAttack(int index, DigimonCombatStats target)
    {
        if (isDead || index >= equippedMoves.Length || equippedMoves[index] == null)
            return;

        StopAllCoroutines();       // Interrupt cooldown/waiting
        isInCooldown = false;      // Reset cooldown
        PerformAttack(index, target); // Execute the move, which will restart cooldown naturally
    }

    public void ForceImmediateDefend()
    {
        if (isDead) return;

        StopAllCoroutines();
        isInCooldown = false;
        StartDefend(); // Handles all the animation/logic
    }

    public void ForceImmediateMoveAway()
    {
        if (isDead) return;

        StopAllCoroutines();
        isInCooldown = false;
        StartMoveAway(); // Move back or reposition
    }



}
