using UnityEngine;
using System.Collections.Generic;

public class BattleTriggerDetector : MonoBehaviour
{
    public float battleRange = 10f;
    public KeyCode triggerKey = KeyCode.E;
    public List<DigimonCombatStats> enemiesInRange;
    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            enemiesInRange = new List<DigimonCombatStats>();
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist <= battleRange)
                {
                    var stats = enemy.GetComponent<DigimonCombatStats>();
                    if (stats != null && stats.currentHP > 0)
                        enemiesInRange.Add(stats);
                }
            }

            if (enemiesInRange.Count > 0)
                BattleManager.Instance?.InitiateBattle(enemiesInRange.ToArray());
        }
    }
}
