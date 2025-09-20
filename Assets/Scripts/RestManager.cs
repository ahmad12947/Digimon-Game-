using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestManager : MonoBehaviour
{
    public static RestManager Instance;

    [Header("UI References")]
    public GameObject restBlackScreen;
    public GameObject restPromptUI;
    public TextMeshProUGUI timeText;

    [Header("Rest Settings")]
    public float restTickDuration = 2.5f; // Time per in-game hour
    private float currentRestTime = 0f;
    private bool isResting = false;
    public int restHours = 0;

    [Header("References")]
    public DigimonMoodManager digimonMoodManager;
    public digimonStatsManager digimonStatsManager;
    public characterAnimationsHandler characterAnimationsHandler;

    private RestTrigger currentRestTrigger;

    private Coroutine restCoroutine;
    public DigiClock clock;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (digimonMoodManager == null||digimonMoodManager.gameObject.activeInHierarchy==false)
        {
            digimonMoodManager=GameObject.FindGameObjectWithTag("Player").GetComponent<DigimonMoodManager>();
        }
        // Global Escape key listener to stop resting
        if (isResting && Input.GetKeyDown(KeyCode.Escape))
        {
            EndRest();
        }
    }

    public void ShowRestPrompt(RestTrigger trigger)
    {
        restPromptUI.SetActive(true);
        currentRestTrigger = trigger;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        restPromptUI.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        restPromptUI.GetComponentInChildren<Button>().onClick.AddListener(() => StartRest(trigger));
    }

    public void HideRestPrompt()
    {
        restPromptUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void StartRest(RestTrigger trigger)
    {
        if (isResting) return;
        clock.PauseClock();
        restPromptUI.SetActive(false);
        restBlackScreen.SetActive(true);

        currentRestTime = 0f;
        restHours = 0;
        currentRestTrigger = trigger;
        isResting = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        characterAnimationsHandler.rest();

        restCoroutine = StartCoroutine(RestRoutine());
    }

    private IEnumerator RestRoutine()
    {
        while (isResting)
        {
            yield return new WaitForSeconds(restTickDuration);
            restHours++;
            clock.AddTime(1);
            digimonMoodManager.Tiredness -= 10 * restHours;
           UpdateRestTimerUI(restHours);
            ApplyRestEffects(currentRestTrigger);
        }
    }

    private void ApplyRestEffects(RestTrigger trigger)
    {
        if (digimonMoodManager != null)
            digimonMoodManager.changeTiredness(-trigger.tirednessRestore);

        if (digimonStatsManager != null)
        {
            digimonStatsManager.addHp(trigger.hpRestore);
            digimonStatsManager.addMp(trigger.mpRestore);
           
        }
    }

    private void UpdateRestTimerUI(int hours)
    {
        if (timeText != null)
        {
            timeText.text = $"Rested: {hours} hr{(hours > 1 ? "s" : "")}";
        }
    }

    private void EndRest()
    {
        isResting = false;
       
        if (restCoroutine != null)
            StopCoroutine(restCoroutine);
       
        restBlackScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        clock.UnpauseClock();
        StartCoroutine(ClearTextAfterDelay());
    }

    private IEnumerator ClearTextAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        timeText.text = "";
    }
}
