using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    [Header("Evolution Data")]
    public List<EvolutionRequirement> possibleEvolutions;

    [Header("Stats and Mood")]
    public DigimonMoodManager mood;
    public digimonStatsManager statManager;

    [Header("Current Digimon Info")]
    public string currentDigimonName;

    [Header("Evolution Timing")]
    public float evolutionCheckInterval = 60f;
    private float timeSinceLastCheck = 0f;

    [Tooltip("Default: 3 = 3 real minutes = 1 in-game hour")]
    public float realMinutesPerGameHour = 3f;

    [Header("Evolution Threshold")]
    public float evolutionAgeThreshold = 72f; // fallback time (72 game hours = 3 days)
    public bool hasEvolved = false;

    [Header("Progress Tracking")]
    public int battleCount = 0;
    public int careMistakeCount = 0;

    [Header("Scene Digimon Objects")]
    public List<string> evolutionNames;
    public List<GameObject> evolutionSceneObjects;

    private Dictionary<string, GameObject> evolutionMap = new();
    public GameObject blackScreen;
    public GameTestScript debugScript;
    public DigiClock clock;
    public int maxLifeSpan = 0;
    public GameObject player, trainingCanvas;
    public ItemPanelManager itemPanelManager;
    public GameObject egg;
    private GameObject evolvedDigi;
    private void Start()
    {
        clock = GameObject.Find("GameManager").GetComponent<DigiClock>();
        debugScript = FindObjectOfType<GameTestScript>();

        if (mood == null) mood = GetComponent<DigimonMoodManager>();
        if (statManager == null) statManager = FindObjectOfType<digimonStatsManager>();

        for (int i = 0; i < evolutionNames.Count; i++)
        {
            if (i < evolutionSceneObjects.Count && !string.IsNullOrEmpty(evolutionNames[i]))
            {
                evolutionMap[evolutionNames[i]] = evolutionSceneObjects[i];
            }
        }
    }

    private void Update()
    {
        if (hasEvolved) return;

        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= evolutionCheckInterval)
        {
            timeSinceLastCheck = 0f;
            CheckForEvolution();
        }
    }

    public void IncrementCareMistake() => careMistakeCount++;
    public void IncrementBattleCount() => battleCount++;

    public void CheckForEvolution()
    {
        if (statManager.Age < evolutionAgeThreshold)
        {
            // Not old enough to evolve yet
            return;
        }

        bool evolved = false;
        List<string> unmet = new List<string>();

        foreach (var evo in possibleEvolutions)
        {
           

            if (evo.evolveOnThreshold)
            {
                Debug.Log($"{currentDigimonName} is evolving into {evo.toDigimonName} (Threshold-based)!");
                EvolveInto(evo.toDigimonName);
                evolved = true;
                break;
            }

            List<string> unmetReqs;
            bool meetsRequirements = CheckRequirements(evo, out unmetReqs);

            if (meetsRequirements)
            {
                Debug.Log($"{currentDigimonName} is evolving into {evo.toDigimonName}!");
                EvolveInto(evo.toDigimonName);
                evolved = true;
                break;
            }
            else
            {
                unmet.AddRange(unmetReqs);
            }
        }

        if (!evolved)
        {
            Debug.LogWarning($"{currentDigimonName} failed to meet any champion requirements after {evolutionAgeThreshold} hours  evolving to Numemon.");
            foreach (var req in unmet)
            {
                Debug.Log($"[Unmet Requirement] {req}");
            }
            EvolveInto("Numemon");
        }
    }

    private bool CheckRequirements(EvolutionRequirement evo, out List<string> unmet)
    {
        unmet = new List<string>();

        if (statManager.Hp < evo.requiredHP) unmet.Add($"HP {evo.requiredHP}+");
        if (statManager.Mp < evo.requiredMP) unmet.Add($"MP {evo.requiredMP}+");
        if (statManager.Off < evo.requiredOff) unmet.Add($"Offense {evo.requiredOff}+");
        if (statManager.Def < evo.requiredDef) unmet.Add($"Defense {evo.requiredDef}+");
        if (statManager.Speed < evo.requiredSpeed) unmet.Add($"Speed {evo.requiredSpeed}+");
        if (statManager.Brain < evo.requiredBrains) unmet.Add($"Brains {evo.requiredBrains}+");

        if (statManager.Weight < evo.minWeight || statManager.Weight > evo.maxWeight)
            unmet.Add($"Weight between {evo.minWeight}-{evo.maxWeight}");

        if (careMistakeCount < evo.minCareMistakes || careMistakeCount > evo.maxCareMistakes)
            unmet.Add($"Care Mistakes {evo.minCareMistakes}-{evo.maxCareMistakes}");

        if (battleCount < evo.minBattles) unmet.Add($"Battles {evo.minBattles}+");

        return unmet.Count == 0;
    }

    public string PredictMostLikelyEvolution()
    {
        string bestMatch = "Unknown";
        float bestScore = -1f;

        foreach (var evo in possibleEvolutions)
        {
           

            float score = 0f;

            score += Mathf.Clamp01(statManager.Hp / (float)evo.requiredHP);
            score += Mathf.Clamp01(statManager.Mp / (float)evo.requiredMP);
            score += Mathf.Clamp01(statManager.Off / (float)evo.requiredOff);
            score += Mathf.Clamp01(statManager.Def / (float)evo.requiredDef);
            score += Mathf.Clamp01(statManager.Speed / (float)evo.requiredSpeed);
            score += Mathf.Clamp01(statManager.Brain / (float)evo.requiredBrains);

            float weightScore = (statManager.Weight >= evo.minWeight && statManager.Weight <= evo.maxWeight) ? 1f : 0f;
            score += weightScore;

            float careScore = (careMistakeCount >= evo.minCareMistakes && careMistakeCount <= evo.maxCareMistakes) ? 1f : 0f;
            score += careScore;

            float battleScore = Mathf.Clamp01(battleCount / (float)evo.minBattles);
            score += battleScore;

            if (score > bestScore)
            {
                bestScore = score;
                bestMatch = evo.toDigimonName;
            }
        }

        return bestMatch;
    }

    private void EvolveInto(string toDigimonName)
    {
        if (evolutionMap.TryGetValue(toDigimonName, out GameObject evolvedForm))
        {
            blackScreen.SetActive(true);
            Invoke("setBlackScreenFalse", 3);

            evolvedForm.SetActive(true);
            debugScript.digimon = evolvedForm;
            debugScript.evolutionManager = evolvedForm.GetComponent<EvolutionManager>();
            debugScript.digimonMoodManager = evolvedForm.GetComponent<DigimonMoodManager>();
            statManager.moodManager = evolvedForm.GetComponent<DigimonMoodManager>();
            clock.setUp(evolvedForm);


            itemPanelManager.digimonEvolved(evolvedForm);
            evolvedForm.GetComponent<BaseStats>().setDigimonstats();
            evolvedForm.GetComponent<BaseStats>().setdigimonCanvasStats();

            careMistakeCount = 0;
            battleCount = 0;
            evolvedForm.transform.position = gameObject.transform.position;
            gameObject.SetActive(false);
            statManager.gameObject.GetComponent<BattleManager>().AutoAssignReferences();
            evolvedForm.SetActive(true);
            player.GetComponent<pausePlayerMovement>().unPausePlayer();
            trainingCanvas.SetActive(false );
            hasEvolved = true;
        }
        else
        {
            Debug.LogWarning($"Evolution GameObject for {toDigimonName} not found.");
        }
    }

    public void setBlackScreenFalse()
    {
        
       
        blackScreen.SetActive(false);
        gameObject.SetActive(false);
    }

    private void resetValues()
    {
        
        gameObject.GetComponent<BattleDigimonController>().isDead = false;
        gameObject.GetComponent<BattleDigimonController>().enabled = true;

    }

    public void reEnable()
    {
        evolvedDigi.SetActive(true);
    }
    public void onDeathEvolved(GameObject evolvedForm)
    {
        evolvedDigi = evolvedForm;
        statManager.Age = 0;
        statManager.moodManager = evolvedForm.GetComponent<DigimonMoodManager>();
        evolvedForm.gameObject.GetComponent<BaseStats>().setDigimonstats();
        blackScreen.SetActive(true);
        careMistakeCount = 0;
        battleCount = 0;
        evolvedForm.GetComponent<FollowerAI>().enabled = false;
        Invoke("setBlackScreenFalse", 3);
        Invoke("resetValues", 3);
        Invoke("reEnable", 6);
        GameObject spawnedEgg = Instantiate(egg, gameObject.transform.position, Quaternion.identity);
        Destroy(spawnedEgg, 6);
        debugScript.digimon = evolvedForm;
        debugScript.evolutionManager = evolvedForm.GetComponent<EvolutionManager>();
        debugScript.digimonMoodManager = evolvedForm.GetComponent<DigimonMoodManager>();
        statManager.moodManager = evolvedForm.GetComponent<DigimonMoodManager>();
        clock.setUp(evolvedForm);
        player.GetComponent<pausePlayerMovement>().unPausePlayer();
        trainingCanvas.SetActive(false);
        ItemPanelManager panel = FindObjectOfType<ItemPanelManager>();
        evolvedForm.GetComponent<BaseStats>().setDigimonstats();
        evolvedForm.GetComponent<BaseStats>().setdigimonCanvasStats();
        if (panel != null)
        {
            panel.digimonEvolved(evolvedForm);
        }
       
       
        evolvedForm.transform.position = gameObject.transform.position;
      
        //evolvedForm.SetActive(true);
        
        evolvedForm.GetComponent<FollowerAI>().enabled = true;
        hasEvolved = true;
      
    }
}
