using TMPro;
using UnityEngine;
using System.Collections;

public class DigiClock : MonoBehaviour
{
    public enum TimePhase
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

    [Header("Time Settings")]
    public float secondsPerInGameHour = 60f; // 1 in-game hour = 60 real seconds
    [Range(0f, 24f)] public float inGameTime = 8f;

    [Header("UI Elements")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI rawTimeText;

    public TimePhase CurrentPhase { get; private set; }

    private bool isPaused = false;

    [Header("Digimon Mood")]
    public DigimonMoodManager moodManager;
    public GameObject digimon;

    [Header("Sleepiness Settings")]
    [Range(0f, 100f)] public float sleepiness = 0f;
    public float maxSleepiness = 100f;
    public digimonStatsManager statsManager;

    private float lastSleepinessUpdateTime;
    private int lastHour = -1;

    [Header("Lighting Settings")]
    public Light sunLight;                 // Directional Light (Sun)
    public Material skyboxMaterial;        // Skybox material
    public float dayExposure = 0.58f;      // Max brightness at noon
    public float nightExposure = 0.1f;     // Min brightness at midnight
    public float sunriseAngle = -20f;      // Sun angle at dawn
    public float sunsetAngle = 200f;       // Sun angle at dusk
    [Header("Clock Needle")]
    public RectTransform clockNeedle; // UI needle (Image or RectTransform)
    public float rotationOffset = 0f; // adjust if your needle "12 o'clock" isn't at 0°
    void Start()
    {
        moodManager = FindObjectOfType<DigimonMoodManager>();
        lastSleepinessUpdateTime = Time.time;
        digimon = GameObject.FindGameObjectWithTag("Player");
        lastHour = Mathf.FloorToInt(inGameTime);

        if (sunLight == null)
            sunLight = RenderSettings.sun;

        if (skyboxMaterial == null)
            skyboxMaterial = RenderSettings.skybox;
    }
    private void UpdateNeedle()
    {
        if (clockNeedle == null) return;

        // 1 full circle = 24 in-game hours
        float rotationPerHour = 360f / 24f;
        float angle = (inGameTime * rotationPerHour) + rotationOffset;

        clockNeedle.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }
    public void setUp(GameObject obj)
    {
        digimon = obj;
        moodManager = obj.GetComponent<DigimonMoodManager>();
    }

    void Update()
    {
        if (!isPaused)
        {
            inGameTime += Time.deltaTime / secondsPerInGameHour;
            if (inGameTime >= 24f)
                inGameTime %= 24f;

            UpdateSleepiness();
            CheckForHourChange();
            UpdateLighting(); // sync light
            UpdateNeedle();
        }

        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
            AddTime(1f);
        //if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Underscore))
        //    AddTime(-1f);

        UpdateTimePhase();
        UpdateUI();
    }

    //  Detect hourly tick and call aging
    private void CheckForHourChange()
    {
        int currentHour = Mathf.FloorToInt(inGameTime);
        if (currentHour != lastHour)
        {
            lastHour = currentHour;

            if (statsManager != null)
                statsManager.HandleAging();

            digimon.GetComponent<EvolutionManager>().CheckForEvolution();
            moodManager?.OnHourPassed();
        }
    }

    private void UpdateTimePhase()
    {
        if (inGameTime >= 6f && inGameTime < 11f)
            CurrentPhase = TimePhase.Morning;
        else if (inGameTime >= 11f && inGameTime < 17f)
            CurrentPhase = TimePhase.Afternoon;
        else if (inGameTime >= 17f && inGameTime < 20f)
            CurrentPhase = TimePhase.Evening;
        else
            CurrentPhase = TimePhase.Night;
    }

    private void UpdateUI()
    {
        string formattedTime = GetFormattedTime();
        string fullDisplay = $"Time: {CurrentPhase} {formattedTime}";

        if (phaseText != null)
            phaseText.text = fullDisplay;

        if (rawTimeText != null)
            rawTimeText.text = formattedTime;
    }

    public string GetFormattedTime()
    {
        int hour = Mathf.FloorToInt(inGameTime);
        int minute = Mathf.FloorToInt((inGameTime - hour) * 60);
        return $"{hour:00}:{minute:00}";
    }

    public void AddTime(float hoursToAdd)
    {
        StartCoroutine(AnimateHourPassage(hoursToAdd));
    }

    private IEnumerator AnimateHourPassage(float hoursToAdd)
    {
        int intHours = Mathf.FloorToInt(Mathf.Abs(hoursToAdd));
        float direction = Mathf.Sign(hoursToAdd);
        digimon.GetComponent<EvolutionManager>().CheckForEvolution();

        for (int i = 0; i < intHours; i++)
        {
            float oldTime = inGameTime;

            inGameTime += direction;
            inGameTime %= 24f;
            if (inGameTime < 0f) inGameTime += 24f;

            UpdateSleepinessManual(oldTime, inGameTime, 1f);
            //moodManager?.OnHourPassed();
            moodManager?.forcedHourPass();
            statsManager?.HandleAging();
            lastHour = Mathf.FloorToInt(inGameTime);

            UpdateTimePhase();
            UpdateUI();
            UpdateLighting();

            yield return new WaitForSeconds(0.2f);
        }

        float fractional = Mathf.Abs(hoursToAdd) - intHours;
        if (fractional > 0f)
        {
            float oldTime = inGameTime;

            inGameTime += fractional * direction;
            inGameTime %= 24f;
            if (inGameTime < 0f) inGameTime += 24f;

            UpdateSleepinessManual(oldTime, inGameTime, fractional);
            UpdateTimePhase();
            UpdateUI();
            UpdateLighting();
        }
    }

    private void UpdateSleepiness()
    {
        float currentRealTime = Time.time;
        float elapsedRealSeconds = currentRealTime - lastSleepinessUpdateTime;

        float hoursToMax = 15f; // default for all digimons

        if (digimon != null)
        {
            string name = digimon.name;
            if (name == "Botamon")
                hoursToMax = 3f; // Botamon special case
        }

        // Sleepiness only increases during awake hours
        if (inGameTime >= 7f && inGameTime <= 22f)
        {
            float sleepinessRate = maxSleepiness / (hoursToMax * secondsPerInGameHour);
            sleepiness += elapsedRealSeconds * sleepinessRate;
        }

        if (sleepiness >= maxSleepiness - 0.05f)
            sleepiness = maxSleepiness;

        sleepiness = Mathf.Clamp(sleepiness, 0f, maxSleepiness);
        moodManager.sleep = Mathf.RoundToInt(sleepiness);

        lastSleepinessUpdateTime = currentRealTime;
    }

    private void UpdateSleepinessManual(float oldTime, float newTime, float hoursPassed)
    {
        float totalSleepIncrease = 0f;
        float step = 0.1f;

        float hoursToMax = 15f; // default
        if (digimon != null)
        {
            string name = digimon.name;
            if (name == "Botamon")
                hoursToMax = 3f; // Botamon special case
        }

        for (float t = 0f; t < hoursPassed; t += step)
        {
            float currentHour = (oldTime + t) % 24f;
            if (currentHour >= 7f && currentHour <= 22f)
            {
                float ratePerHour = maxSleepiness / hoursToMax;
                totalSleepIncrease += step * ratePerHour;
            }
        }

        sleepiness += totalSleepIncrease;
        sleepiness = Mathf.Clamp(sleepiness, 0f, maxSleepiness);

        if (Mathf.Abs(sleepiness - maxSleepiness) < 0.01f)
            sleepiness = maxSleepiness;

        moodManager.sleep = (int)sleepiness;
    }
    private void UpdateLighting()
    {
        if (sunLight == null || skyboxMaterial == null)
            return;

        // Clamp sun rotation between sunrise  sunset (instead of full 360)
        float t = inGameTime / 24f;
        float sunRotation = Mathf.Lerp(sunriseAngle, sunsetAngle, t);
        sunLight.transform.rotation = Quaternion.Euler(sunRotation, 170f, 0f);

        // Smooth exposure with sin wave (bright at noon, dark at night)
        float exposure = Mathf.Lerp(nightExposure, dayExposure, Mathf.Sin(t * Mathf.PI));
        skyboxMaterial.SetFloat("_Exposure", exposure);
    }

    public float GetSleepiness() => sleepiness;
    public void PauseClock() => isPaused = true;
    public void UnpauseClock() => isPaused = false;
}
