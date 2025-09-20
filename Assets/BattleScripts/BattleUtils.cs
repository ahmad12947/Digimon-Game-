using UnityEngine;

public static class BattleUtils
{
    public static float CalculateHitChance(float accuracy, int attackerSpeed, int victimSpeed)
    {
        float reduction = accuracy * (victimSpeed - attackerSpeed / 10f) / 1998f;
        return Mathf.Clamp(accuracy - reduction, 5f, 100f);
    }

    public static int CalculateDamage(int power, int offense, int defense)
    {
        return Mathf.Max(1, Mathf.FloorToInt((power + offense) * (100f / (100f + defense))));
    }

    public static float GetEnemyFactor(int enemyCount)
    {
        return enemyCount switch
        {
            1 => 1f,
            2 => 1.2f,
            3 => 1.6f,
            _ => 1f
        };
    }
}
