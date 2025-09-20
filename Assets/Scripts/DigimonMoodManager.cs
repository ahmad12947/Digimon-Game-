using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DigimonMoodManager : MonoBehaviour
{
    public int hunger = 50;
    public int sleep = 10;
    public int poop = 10;
    public int virus = 10;
    public int Happiness = 50; // Can be negative too
    public int Discipline = 0;
    public int lifeSpan = 0;
    public int Energy = 0;
    public int Tiredness = 0;
    public int Weight = 0;
    private float timeSinceLastHappinessUpdate = 0f;
    [SerializeField] private float hungerUpdateInterval = 1f;   
    [SerializeField] private float poopUpdateInterval = 10f;
    [SerializeField] private float sleepUpdateInterval = 1f;

    [SerializeField] private GameObject hungerWarningUI;
    [SerializeField] private GameObject sleepWarningUI;
    [SerializeField] private GameObject poopWarningUI;
    [SerializeField] private GameObject sicknessWarningUI;
    public bool isInjured = false;

    [SerializeField] private int hungerWarningThreshold = 80;
    [SerializeField] private int sleepWarningThreshold = 80;
    [SerializeField] private int poopWarningThreshold = 80;

    [SerializeField] private float warningDuration = 5f;
    [SerializeField] private float warningCooldown = 5f;

    [SerializeField] private float energyUpdateInterval = 180f;

    private float timeSinceZeroEnergy = 0f;
    [SerializeField] private float weightLossIntervalAtZeroEnergy = 600f; // 10 minutes in seconds

    private float timeSinceLastEnergyUpdate = 0f;
    //Hunger
    private float timeSinceLastHungerUpdate = 0f;
    //Poop
    private float timeSinceLastPoopUpdate = 0f;
    //Sleep
    private float timeSinceLastSleepUpdate = 0f;

    //LifeSpan
    public float lifeSpanUpdateInterval = 180f; // 3 minutes
    private float timeSinceLastLifeSpanUpdate = 0f;
    private float timeSinceLastPenaltyCheck = 0f;
    public int totalLifeSpan = 360;

    //Tiredeness
    private float timeSinceTiredPenalty = 0f;
    public GameObject tiredEffect; // assign a visual like a sweat bubble prefab

    private float hungerWarningTimer = 0f;
    private float sleepWarningTimer = 0f;
    private float poopWarningTimer = 0f;

    private bool isHungerWarningActive = false;
    private bool isSleepWarningActive = false;
    private bool isPoopWarningActive = false;

    private bool isAutoPoopCountdownActive = false;
    private bool isAutoHungerCountdownActive = false;
    private bool isAutoSleepCountdownActive = false;

    private float autoPoopTimer = 0f;
    private float autoHungerTimer = 0f;
    private float autoSleepTimer = 0f;
    public GameObject poopTurd;

    [SerializeField] private float autoPoopTimerDuration = 15f;
    [SerializeField] private float autoHungerTimerDuration = 15f;
    [SerializeField] private float autoSleepTimerDuration = 15f;

    public digimonaAnimationManager digimonaAnimationManager;
    public Slider disciplineSlider, happinessSlider, virusSlider;

    public TextMeshProUGUI messageTxt;

    private bool isCareMistake = false;

    public digimonStatsManager digimonStatsManager;
 


    private float fatigueCounter = 0f;
    private float timeSinceLastFatigueCheck = 0f;
    private const float fatigueCheckInterval = 600f; // 10 minutes
    private const float fatiguePerMinute = 0.3f;


    public bool hasRestPillow = false; // Add this at class level if not already 

    [SerializeField] private GameObject happyIcon;
    [SerializeField] private GameObject angryIcon;


    private float butterflyCheckTimer = 0f;
    private float butterflyHappinessTimer = 0f;
    private float butterflyPenaltyTimer = 0f;
    private bool isButterflyActive = false;
    public GameObject butterflyEffectIcon;

    private List<GameObject> countdownWarnings = new List<GameObject>();
    private int rotatingWarningIndex = 0;
    private float rotatingWarningTimer = 0f;
    [SerializeField] private float rotatingWarningInterval = 2f;

    public bool isSleeping=false;

    public void timeAddedOrPassed(int hours)
    {
        
    }

    public void clearText()
    {
        messageTxt.text = "";
    }
    private float warningSwitchCooldown = 3f; // Duration for each warning
    private float warningTimer = 0f;
    private List<GameObject> currentActiveWarnings = new List<GameObject>();
    private int currentWarningIndex = 0;

    public Button sleepButton;
    public BattleManager battleManager;
    private void Start()
    {
        sleepButton.interactable = false;
        hunger = 0;
        sleep = 10;
        poop = 10;
        Happiness = 80;
        Discipline = 10;
        lifeSpan = 360;
        Tiredness = 10;
        happinessSlider.value = Happiness;
        disciplineSlider.value = Discipline;
        autoPoopTimerDuration = 60 + 0.6f * Discipline;
        if (hungerWarningUI != null) hungerWarningUI.SetActive(false);
        if (sleepWarningUI != null) sleepWarningUI.SetActive(false);
        if (poopWarningUI != null) poopWarningUI.SetActive(false);
        
    }

    public void forcedHourPass()
    {
        ChangeHunger(30);
        ChangePoop(24);
        if (isAutoPoopCountdownActive == true)
        {
            autoPoopTimer = autoPoopTimerDuration;

        }
        if (isAutoHungerCountdownActive == true)
        {
            autoHungerTimer = autoHungerTimerDuration;
        }
        if (isAutoSleepCountdownActive == true)
        {
            autoSleepTimer = autoSleepTimerDuration;
        }
        timeSinceLastHappinessUpdate += 60f;
        timeSinceLastLifeSpanUpdate += 60f;
        timeSinceLastPenaltyCheck += 60f;
    }
    public void OnHourPassed()
{       
        ChangeHunger(30);
        ChangePoop(24);
        transform.GetComponent<EvolutionManager>().CheckForEvolution();
        timeSinceLastHappinessUpdate += 60f;
        timeSinceLastLifeSpanUpdate += 60f;
        timeSinceLastPenaltyCheck += 60f;

        // Optionally: handle any thresholds or penalties here
    }
    private void Update()
    {
        if (battleManager.isInBattle == false) { 
        timeSinceLastHungerUpdate += Time.deltaTime;
        timeSinceLastPoopUpdate += Time.deltaTime;
        timeSinceLastSleepUpdate += Time.deltaTime;
        timeSinceLastHappinessUpdate += Time.deltaTime;

        timeSinceLastLifeSpanUpdate += Time.deltaTime;
        timeSinceLastPenaltyCheck += Time.deltaTime;

        UpdateMoodIcons();
        lifeSpanUpdate();
        hungerUpdate();
        poopUpdates();
        sleepUpdate();
        tiredUpdate();
        energyUpdate();
        CheckStatDependencies();
        //HandleWarnings();
        ClampStats();
        HandleAutoCountdowns();
        handleZeroEnergyWeightLoss();
        FatigueUpdate();
        HandleButterflyCondition();
    }
    }
    //UPDATES

    public void CheckForInjury(int currentHP, int maxHP)
    {
        float hpRatio = (float)currentHP / maxHP;
        float injuryChance = Tiredness - hpRatio;

        Debug.Log($"Checking injury: Tiredness = {Tiredness}, HP Ratio = {hpRatio}, Chance = {injuryChance}");

        if (UnityEngine.Random.value * 100f < injuryChance)
        {
            SetInjuredState(true);
        }
    }

    public void SetInjuredState(bool state)
    {
        isInjured = state;

        if (sicknessWarningUI != null)
            sicknessWarningUI.SetActive(state);

        if (state)
        {
            Debug.Log(" Digimon is injured!");
            ChangeHappiness(-10);
            changeVirus(2); // Optional penalty
        }
        else
        {
            Debug.Log(" Digimon has recovered from injury.");
        }
    }

    private void HandleButterflyCondition()
    {
        butterflyCheckTimer += Time.deltaTime;

        // Check every 10 minutes (600 seconds)
        if (butterflyCheckTimer >= 600f)
        {
            butterflyCheckTimer = 0f;

            // Step 1: No condition bubble (tiredEffect, hunger/sleep/poop warnings all off)
            bool noConditionBubble =
                (tiredEffect == null || !tiredEffect.activeSelf) &&
                (hungerWarningUI == null || !hungerWarningUI.activeSelf) &&
                (sleepWarningUI == null || !sleepWarningUI.activeSelf) &&
                (poopWarningUI == null || !poopWarningUI.activeSelf);

            // Step 2: Happiness is negative
            bool isSad = (Happiness < 0);

            // Step 3: Random chance logic
            int randomValue = UnityEngine.Random.Range(0, 101);
            int invertedHappiness = Mathf.Abs(Happiness);
            int chanceValue = invertedHappiness - Discipline;

            if (noConditionBubble && isSad && randomValue < chanceValue)
            {
                isButterflyActive = true;
                butterflyHappinessTimer = 0f;
                butterflyPenaltyTimer = 0f;
                Debug.Log("Butterfly condition activated!");
            }
        }

        // Apply butterfly condition effects if active
        if (isButterflyActive)
        {
            butterflyHappinessTimer += Time.deltaTime;
            butterflyPenaltyTimer += Time.deltaTime;

            // Every 0.75 minutes (45 seconds), increase happiness by 1
            if (butterflyHappinessTimer >= 45f)
            {
                butterflyHappinessTimer -= 45f;
                ChangeHappiness(1);
                Debug.Log("Butterfly effect: +1 Happiness");
            }

            // Every 15 minutes (900 seconds), reduce all Digimon stats by 2%
            if (butterflyPenaltyTimer >= 900f)
            {
                butterflyPenaltyTimer -= 900f;

                // Reduce all stats by 2%
                digimonStatsManager.ReduceAllStatsByPercent(2);
                Debug.Log("Butterfly effect: -2% to all stats");
            }

            // End condition if happiness has recovered
            if (Happiness >= 20)
            {
                isButterflyActive = false;
                Debug.Log("Butterfly condition ended.");
            }
        }

        // Update the icon based on current state
        if (butterflyEffectIcon != null)
            butterflyEffectIcon.SetActive(isButterflyActive);
    }


    private void FatigueUpdate()
    {
        fatigueCounter += Time.deltaTime * (Tiredness * fatiguePerMinute / 60f); // fatigue grows per second

        timeSinceLastFatigueCheck += Time.deltaTime;
        if (timeSinceLastFatigueCheck >= fatigueCheckInterval)
        {
            timeSinceLastFatigueCheck = 0f;

            if (fatigueCounter >= 180f)
            {
                ChangeSleep(10); // Digimon gets sleepier earlier
                fatigueCounter = 0f;
                Debug.Log("Fatigue high: Digimon feels sleepier.");
            }
        }
    }

    private void lifeSpanUpdate()
    {//Base 360
        if (timeSinceLastLifeSpanUpdate >= lifeSpanUpdateInterval)
        {
            timeSinceLastLifeSpanUpdate -= lifeSpanUpdateInterval;
            changeLifeSpan(-1);
        }

        // Every 4 in-game hours (12 minutes) apply happiness-based penalty
        if (timeSinceLastPenaltyCheck >= lifeSpanUpdateInterval * 4f)
        {
            timeSinceLastPenaltyCheck -= lifeSpanUpdateInterval * 4f;
            LifeSpanApplyHappinessPenalty();
        }
    }

    private void energyUpdate()
    {
        timeSinceLastEnergyUpdate += Time.deltaTime;
        if (timeSinceLastEnergyUpdate >= energyUpdateInterval)
        {
            ChangeEnergy(-1);  // or another rate depending on the Digimon
            timeSinceLastEnergyUpdate = 0f;
        }
    }
    void LifeSpanApplyHappinessPenalty()
    {
        int penalty = 0;

        if (Happiness >= 80 && Happiness <= 100)
            penalty = -1;
        else if (Happiness >= 31 && Happiness <= 79)
            penalty = -2;
        else if (Happiness >= 19 && Happiness <= 30)
            penalty = -3;
        else if (Happiness >= -69 && Happiness <= -20)
            penalty = -4;
        else if (Happiness >= -100 && Happiness <= -70)
            penalty = -5;

        changeLifeSpan(penalty);
        Debug.Log("Happiness penalty applied: " + penalty + " (Happiness: " + Happiness + ")");
    }


    private void UpdateMoodIcons()
    {
        bool showHappy = (Happiness >= 31);

        bool showAngry = (Happiness < 31);

        if (happyIcon != null)
            happyIcon.SetActive(showHappy);

        if (angryIcon != null)
            angryIcon.SetActive(showAngry);

        // Hide both if out of range
        if (!showHappy && !showAngry)
        {
            if (happyIcon != null) happyIcon.SetActive(false);
            if (angryIcon != null) angryIcon.SetActive(false);
        }
    }
    private void hungerUpdate()
    {
        if (timeSinceLastHungerUpdate >= hungerUpdateInterval)
        {
            ChangeHunger(1);
            timeSinceLastHungerUpdate = 0f;
        }
    }
    private void poopUpdates()
    {
        if (timeSinceLastPoopUpdate >= poopUpdateInterval) //Interval set to 540 Seconds ~ 1 in game hour = 3 minutes -> 3*180(For baby digimon)
        {
            ChangePoop(1);
            timeSinceLastPoopUpdate = 0f;
        }
    }
    private void sleepUpdate()
    {
    //    if (timeSinceLastSleepUpdate >= sleepUpdateInterval)
    //    {
    //        ChangeSleep(1);
    //        timeSinceLastSleepUpdate = 0f;
    //    }
    }

    private void handleZeroEnergyWeightLoss()
    {
        if (Energy <= 0)
        {
            timeSinceZeroEnergy += Time.deltaTime;

            if (timeSinceZeroEnergy >= weightLossIntervalAtZeroEnergy)
            {
                digimonStatsManager.changeWeight(-1);  // Or your custom method
                Weight -= 1;
                Debug.Log("Energy is 0: Lost 1 weight.");
                timeSinceZeroEnergy = 0f;
            }
        }
        else
        {
            timeSinceZeroEnergy = 0f; // Reset if energy is regained
        }
    }

    private void tiredUpdate()
    {
        if (Tiredness >= 80)
        {
            // Show tired visual
            if (tiredEffect != null && !tiredEffect.activeSelf)
                tiredEffect.SetActive(true);

            timeSinceTiredPenalty += Time.deltaTime;

            // Apply penalty every 300 seconds (5 mins real-time)
            if (timeSinceTiredPenalty >= 300f)
            {
                timeSinceTiredPenalty -= 300f;
                Happiness -= 2;
                Happiness = Mathf.Clamp(Happiness, -100, 100);
                Debug.Log("Tiredness penalty: -2 Happiness (Now: " + Happiness + ")");
            }
        }
        else
        {
            // Hide tired visual if under 80
            if (tiredEffect != null && tiredEffect.activeSelf)
                tiredEffect.SetActive(false);

            // Reset the penalty timer if not tired
            timeSinceTiredPenalty = 0f;
        }
    }



    //------------------

    private void HandleAutoCountdowns()
    {
        if (isSleeping == false)
        {
            countdownWarnings.Clear();

            if (poop >= 100 && !isAutoPoopCountdownActive)
            {
                isAutoPoopCountdownActive = true;
                digimonStatsManager.poopyIcon.SetActive(true);
                autoPoopTimer = 0f;
            }
            if (hunger >= 100 && !isAutoHungerCountdownActive)
            {
                isAutoHungerCountdownActive = true;
                digimonStatsManager.hungerIcon.SetActive(true);
                autoHungerTimer = 0f;
            }
            if (sleep >= 100 && !isAutoSleepCountdownActive)
            {
                isAutoSleepCountdownActive = true;
                digimonStatsManager.sleepIcon.SetActive(true);
                autoSleepTimer = 0f;
            }
            if (isInjured && sicknessWarningUI != null)
            {
                countdownWarnings.Add(sicknessWarningUI);
                digimonStatsManager.injuredIcon.SetActive(true);
            }
        }

       
            // Run the countdown timers
            if (isAutoPoopCountdownActive)
            {
                AutoCountdown(ref autoPoopTimer, autoPoopTimerDuration, null, AutomaticPoop);
                countdownWarnings.Add(poopWarningUI);

            }
            if (isAutoHungerCountdownActive)
            {
                AutoCountdown(ref autoHungerTimer, autoHungerTimerDuration, null, AutomaticHunger);
                countdownWarnings.Add(hungerWarningUI);

            }
            if (isAutoSleepCountdownActive)
            {
                AutoCountdown(ref autoSleepTimer, autoSleepTimerDuration, null, AutomaticSleep);
                countdownWarnings.Add(sleepWarningUI);

            }
        

        // Rotate display
        rotatingWarningTimer += Time.deltaTime;

        if (countdownWarnings.Count > 0)
        {
            // Hide all first
            hungerWarningUI?.SetActive(false);
            sleepWarningUI?.SetActive(false);
            poopWarningUI?.SetActive(false);
            sicknessWarningUI?.SetActive(false);
            if (rotatingWarningTimer >= rotatingWarningInterval)
            {
                rotatingWarningIndex = (rotatingWarningIndex + 1) % countdownWarnings.Count;
                rotatingWarningTimer = 0f;
            }

            // Show only one at a time
            if (rotatingWarningIndex < countdownWarnings.Count)
            {
                countdownWarnings[rotatingWarningIndex]?.SetActive(true);
            }
        }
        else
        {
            // No warnings? Reset everything
            rotatingWarningIndex = 0;
            rotatingWarningTimer = 0f;
            hungerWarningUI?.SetActive(false);
            sleepWarningUI?.SetActive(false);
            poopWarningUI?.SetActive(false);
        }
    }
    private void AutoCountdown(ref float timer, float duration, GameObject warningUI, System.Action action)
    {
        timer += Time.deltaTime;
        //if (warningUI != null) warningUI.SetActive(Mathf.FloorToInt(timer % 4) == 0);

        if (timer >= duration)
        {
            action.Invoke();
        }
    }

    public void ChangeHunger(int amount) => hunger += amount;
    public void changeVirus(int amount)
    {
        virus += amount;
        virusSlider.value = virus;
    }
    public void changeLifeSpan(int amount)
    {
        lifeSpan += amount;
    }
    public void ChangeSleep(int amount) => sleep += amount;
    public void ChangePoop(int amount) => poop += amount;
    public void ChangeHappiness(int amount)
    {
        Happiness += amount;
        happinessSlider.value = Happiness;
    }
    public void ChangeEnergy(int amount)
    {
        Energy += amount;
        Energy = Mathf.Clamp(Energy, 0, 100); // Or your custom range
    }
    public void ChangeDiscipline(int amount)
    {
        Discipline += amount;
        disciplineSlider.value =Discipline;
    }

    public void changeTiredness(int amount)
    {
        Debug.Log("Called");
        Tiredness += amount;
    }
    private void CheckStatDependencies()
    {
        if (timeSinceLastHappinessUpdate >= 15f)
        {
            if (hunger > 80) ChangeHappiness(-3);
            if (sleep > 80) ChangeHappiness(-1);
            if (poop > 80) ChangeHappiness(-1);
            timeSinceLastHappinessUpdate = 0f;

            if(sleep>80)
            {
                sleepButton.interactable = true;
            }
        }
    }

    private void HandleWarnings()
    {
        // Step 1: Determine active warnings
        currentActiveWarnings.Clear();

        if (hunger > hungerWarningThreshold && hungerWarningUI != null)
            currentActiveWarnings.Add(hungerWarningUI);
        if (sleep > sleepWarningThreshold && sleepWarningUI != null)
            currentActiveWarnings.Add(sleepWarningUI);
        if (poop > poopWarningThreshold && poopWarningUI != null)
            currentActiveWarnings.Add(poopWarningUI);

        // Step 2: Update timer
        warningTimer += Time.deltaTime;

        // Step 3: Show one warning at a time
        if (currentActiveWarnings.Count > 0)
        {
            // Hide all warnings first
            hungerWarningUI.SetActive(false);
            sleepWarningUI.SetActive(false);
            poopWarningUI.SetActive(false);

            // Display current warning in the list
            if (currentWarningIndex >= currentActiveWarnings.Count)
                currentWarningIndex = 0;

            currentActiveWarnings[currentWarningIndex].SetActive(true);

            // Switch to next warning after cooldown
            if (warningTimer >= warningSwitchCooldown)
            {
                currentWarningIndex = (currentWarningIndex + 1) % currentActiveWarnings.Count;
                warningTimer = 0f;
            }
        }
        else
        {
            // If no active warnings, reset all and timer
            hungerWarningUI?.SetActive(false);
            sleepWarningUI?.SetActive(false);
            poopWarningUI?.SetActive(false);
            warningTimer = 0f;
            currentWarningIndex = 0;
        }
    }
   
    private void AutomaticPoop()
    {
        messageTxt.text = "Care Mistake!";
        transform.GetComponent<EvolutionManager>().IncrementCareMistake();
        Invoke("clearText", 3);
        poop = 0;
        isCareMistake = true;

        ChangeDiscipline(-5);
        ChangeHappiness(-10);
        changeVirus(1);
        GameObject obj= Instantiate(poopTurd, new Vector3(Random.Range(transform.position.x-0.5f,transform.position.x+0.5f),transform.position.y,
                                                         Random.Range(transform.position.z-0.5f,transform.position.z+0.5f)),poopTurd.transform.rotation);
        //Destroy(obj, 10);
        //int reduction = Mathf.FloorToInt(Weight * 0.25f) + UnityEngine.Random.Range(0, 4);
        //digimonStatsManager.changeWeight(-reduction);
        //Weight-=reduction;
        //Debug.Log($"Automatic poop weight reduction: -{reduction}");
       
        digimonaAnimationManager.poop();
        isAutoPoopCountdownActive = false;
        digimonStatsManager.poopyIcon.SetActive(false);
        pausePlayerMovement.instance.pausePlayer();
        pausePlayerMovement.instance.UnPauseWithTime(3);
        if (poopWarningUI != null) poopWarningUI.SetActive(false);
    }

    public void plannedPoop()
    {
        poop = 0;
        ChangeDiscipline(5);
        disciplineSlider.value = Discipline;
        digimonStatsManager.poopyIcon.SetActive(false);
        //int reduction = Mathf.FloorToInt(Weight * 0.25f) + UnityEngine.Random.Range(0, 4); // inclusive of 0, exclusive of 4
        //digimonStatsManager.changeWeight(-reduction);
        //Weight-= reduction;
        //Debug.Log($"Poop weight reduction: -{reduction}");  
        isAutoPoopCountdownActive = false;
        digimonaAnimationManager.poop();
        pausePlayerMovement.instance.pausePlayer();
        pausePlayerMovement.instance.UnPauseWithTime(3);
    }

    private void AutomaticHunger()
    {
        messageTxt.text = "Care Mistake!";
        transform.GetComponent<EvolutionManager>().IncrementCareMistake();
        isCareMistake=true;
        Invoke("clearText", 3);
        hunger = 0;

        ChangeHappiness(-10);
        isAutoHungerCountdownActive = false;
        digimonStatsManager.hungerIcon.SetActive(false);
        if (hungerWarningUI != null) hungerWarningUI.SetActive(false);
    }
    public void ResetSleeping()
    {
        isSleeping = false;
    }
    public void plannedSleep()
    {
        float sleptHours = 8f; // assume fixed for now
        float normalSleepHours = 8f;
        float pillowFactor = hasRestPillow ? 1.2f : 1f;
        float areaFactor = GetAreaFactor(); // now uses area preference
        changeTiredness(-80);
        float restFactor = (sleptHours / normalSleepHours) * pillowFactor * areaFactor;
        float regenMultiplier = UnityEngine.Random.Range(0.7f, 0.8f);
        isSleeping = true;
        //isAutoPoopCountdownActive = false;
        //isAutoHungerCountdownActive=false;
        //isAutoSleepCountdownActive=false;
        //autoPoopTimer = 0;
        //autoSleepTimer = 0;
        //autoHungerTimer= 0;
        Invoke("ResetSleeping", 5);
        int hpRegen = Mathf.FloorToInt(regenMultiplier * restFactor * digimonStatsManager.Hp);
        int mpRegen = Mathf.FloorToInt(regenMultiplier * restFactor * digimonStatsManager.Mp);
        int tirednessReduction = Mathf.FloorToInt((80 + UnityEngine.Random.Range(0, 20)) * restFactor);
        digimonStatsManager.sleepIcon.SetActive(false);
        digimonStatsManager.addHp(hpRegen);
        digimonStatsManager.addMp(mpRegen);
        digimonStatsManager.gameObject.GetComponent<DigiClock>().sleepiness = 0;
        digimonStatsManager.sleepIcon.SetActive(false) ;
        sleepWarningUI.SetActive(false);
        isAutoSleepCountdownActive = false;
        changeTiredness(-tirednessReduction);
        sleep = 0; // resets sleep bar
       
        ChangeHappiness(2);
        sleepButton.interactable = false;
        int reduction = Mathf.FloorToInt(Weight * 0.1f);
        digimonStatsManager.changeWeight(-Mathf.FloorToInt(-reduction));
         Weight-=reduction;
        Debug.Log($"Sleep Regen: +{hpRegen} HP, +{mpRegen} MP, -{tirednessReduction} tiredness");
    }
    public void AutomaticSleep()
    {
        messageTxt.text = "Care Mistake!";
        Invoke("clearText", 3);
        ChangeHappiness(-5);        
        //digimonaAnimationManager.autoSleep();
        isAutoSleepCountdownActive = false;
        if (sleepWarningUI != null) sleepWarningUI.SetActive(false);
        sleepButton.interactable = false;
        int reduction = Mathf.FloorToInt(Weight * 0.1f);
        changeTiredness(25);
        digimonStatsManager.changeWeight(-reduction);
        Weight-=reduction;
        sleep = 0;
        digimonStatsManager.gameObject.GetComponent<DigiClock>().sleepiness = 0;
        digimonStatsManager.sleepIcon.SetActive(false);
        Debug.Log($"Automatic sleep weight reduction: -{reduction}");
    }
    private float GetAreaFactor()
    {
        // Simulated area preference logic. You can replace this with real zone logic later.
        // Example: Sleep area 'Punimon' is liked by 'Botamon', disliked by 'Patamon'

        string digimonName = name; // this script is on the Digimon
        RestTrigger.RestType sleepArea = RestTrigger.RestType.Punimon; // Or however you determine the area

        if (sleepArea == RestTrigger.RestType.Punimon && digimonName.Contains("Botamon"))
            return 1.2f;
        else if (sleepArea == RestTrigger.RestType.Kuwagamon && digimonName.Contains("Patamon"))
            return 0.8f;

        return 1f;
    }
   

    private void ClampStats()
    {
        hunger = Mathf.Clamp(hunger, 0, 100);
        sleep = Mathf.Clamp(sleep, 0, 100);
        poop = Mathf.Clamp(poop, 0, 100);
        Happiness = Mathf.Clamp(Happiness, 0, 100);
        Discipline = Mathf.Clamp(Discipline, 0, 100);
        Tiredness=Mathf.Clamp(Tiredness, 0, 100);
        Weight=Mathf.Clamp(Weight, 1 ,100);
    }
    public void setDigimonCarMistake(bool isAcitve)
    {
        isCareMistake = isAcitve;
    }


    //PRAISE AND SCOLD

    public void onPraise()
    {
        Happiness += Mathf.FloorToInt(Discipline * 0.1f) + 2;
        happinessSlider.value = Happiness;
    }

    public void onScold()
    {
        if (isCareMistake)
        {
            Happiness += 3;
            happinessSlider.value += Happiness;
            Discipline += 8;
            disciplineSlider.value += Discipline;
            isCareMistake = false;
        }
        else
        {
            Happiness -= 10;
            happinessSlider.value -= Happiness;
            Discipline += 2;
            disciplineSlider.value += Discipline;
        }
    }

    public void onFeed(int energy, int weight, int tiredness,int discipline,int HP,int MP, int offense, int defense, int speed,int brains,int life_Span,int happy)
    {
        int tiredNessHolder = 0;
        hunger = 0;
        Energy += energy;
        tiredNessHolder = Tiredness;
        isAutoHungerCountdownActive = false;
        autoHungerTimer = 0;
        Discipline += discipline;
        disciplineSlider.value += Discipline;
        digimonStatsManager.hungerIcon.SetActive(false);
        lifeSpan += life_Span;
        Happiness += happy;
        digimonStatsManager.changeWeight(weight);
        Weight-=weight;
        digimonStatsManager.addHp(HP);
        digimonStatsManager.addMp(MP); 
        digimonStatsManager.addOff(offense);
        digimonStatsManager.addDef(defense);
        digimonStatsManager.addSpeed(speed);
        digimonStatsManager.addBrain(brains);
       
        Tiredness = tiredNessHolder + tiredness;
        if (Tiredness < 0)
        {
            Tiredness = 0;
        }

    }

   
}
