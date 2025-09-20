using UnityEngine;

public class DigimonCombatStats : MonoBehaviour
{
    public string digimonName;

    public int maxHP;
    public int maxMP;
    public int offense;
    public int defense;
    public int speed;
    public int brains;

    public int currentHP;
    public int currentMP;

    // In-battle tracking
    [HideInInspector] public int numAttacks = 0;
    [HideInInspector] public int numBlocked = 0;
    [HideInInspector] public int heavyHits = 0;

    public digimonStatsManager statsManager;
   

    void Start()
    {
        //statsManager = GetComponent<digimonStatsManager>();
        if (statsManager != null)
        {
            Invoke("LoadStatsFromManager", 2);
            //LoadStatsFromManager();
        }
        else
        {
            Debug.LogWarning("digimonStatsManager not found on this GameObject!");
        }
    }

    public void LoadStatsFromManager()
    {
        digimonName = gameObject.name;

        maxHP = Mathf.FloorToInt(statsManager.Hp);
        maxMP = Mathf.FloorToInt(statsManager.Mp);
        offense = Mathf.FloorToInt(statsManager.Off);
        defense = Mathf.FloorToInt(statsManager.Def);
        speed = Mathf.FloorToInt(statsManager.Speed);
        brains = Mathf.FloorToInt(statsManager.Brain);

        currentHP = maxHP;
        currentMP = maxMP;

        ResetBattleStats();

        Debug.Log($"Loaded combat stats for {digimonName}: HP {maxHP}, MP {maxMP}, Off {offense}, Def {defense}, Spd {speed}, Brn {brains}");   
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(currentHP - amount, 0);
        if(currentHP <= 0)
        {
            Invoke("Deactivate", 3);
            Invoke("Reset", 6);
            //currentMP=maxMP;
            //currentHP = maxHP;
            //Destroy(gameObject, 3);
        }    
    }
    public void Reset()
    {
        currentMP = maxMP;
        currentHP = maxHP;
    }

    public void Deactivate()
    {
         gameObject.SetActive(false);
    }
    public void UseMP(int amount)
    {
        currentMP = Mathf.Max(currentMP - amount, 0);
    }

    public void ResetBattleStats()
    {
        numAttacks = 0;
        numBlocked = 0;
        heavyHits = 0;
    }

    public void Heal()
    {
        currentHP = maxHP;
        currentMP = maxMP;
        ResetBattleStats();
    }
}
