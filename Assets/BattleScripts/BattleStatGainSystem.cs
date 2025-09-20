using UnityEngine;

public static class BattleStatGainSystem
{
    public static void CalculateStatGains(DigimonCombatStats player, DigimonCombatStats[] enemies, digimonStatsManager statsManager)
    {
        if (enemies == null || enemies.Length == 0 || statsManager == null) return;

        DigimonCombatStats strongestEnemy = enemies[0];
        foreach (var enemy in enemies)
        {
            if (enemy.offense > strongestEnemy.offense) strongestEnemy = enemy;
        }

        float factor = BattleUtils.GetEnemyFactor(enemies.Length);

        // Apply stat gains directly to digimonStatsManager
        GainStat(player.offense, strongestEnemy.offense, factor, statsManager.addOff);
        GainStat(player.defense, strongestEnemy.defense, factor, statsManager.addDef);
        GainStat(player.speed, strongestEnemy.speed, factor, statsManager.addSpeed);
        GainStat(player.brains, strongestEnemy.brains, factor, statsManager.addBrain);

        // Secondary chance-based gains (random up to 10 instead of always 1)
        TryChance(100f * (player.maxHP - player.currentHP) / Mathf.Max(1, player.maxHP), statsManager.addHp);
        TryChance(player.numAttacks * 10f, statsManager.addMp);
        TryChance(player.heavyHits * 10f, statsManager.addDef);
        TryChance(50f * (player.maxHP - player.currentHP) / Mathf.Max(1, player.maxHP) + player.numBlocked * 10f, statsManager.addSpeed);
        TryChance(player.numAttacks * 5f + player.heavyHits * 5f, statsManager.addBrain);

        // Refresh UI
        statsManager.updateStatsCanvas();
    }

    private static void GainStat(int playerStat, int enemyStat, float factor, System.Action<int> applyGain)
    {
        if (playerStat <= 0) playerStat = 1; // Prevent divide by zero

        if (playerStat < enemyStat)
        {
            // Scale gain to a max of 10
            int gain = Mathf.Clamp(Mathf.FloorToInt(1 + (enemyStat * factor - 1f) / playerStat), 1, 10);
            applyGain?.Invoke(gain);
            Debug.Log($"Stat gain: +{gain}");
        }
        else
        {
            float chance = enemyStat * factor * 100f / playerStat;
            if (Random.Range(0f, 100f) < chance)
            {
                int gain = Random.Range(5, 11); // Random 1–10
                applyGain?.Invoke(gain);
                Debug.Log($"Random stat gain success: +{gain}");
            }
        }
    }

    private static void TryChance(float chance, System.Action<int> applyGain)
    {
        if (Random.Range(0f, 100f) < chance)
        {
            int gain = Random.Range(5, 11); // Random 1–10
            applyGain?.Invoke(gain);
            Debug.Log($"Secondary gain success: +{gain}");
        }
    }
}
