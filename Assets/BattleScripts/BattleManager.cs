using UnityEngine;
using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public bool battleEnded = false;
    private bool checkStart = false;
    private bool isInHitPause = false;

    private DigimonCombatStats playerStats;
    private DigimonCombatStats[] enemyStatsArray;

    private BattleDigimonController playerController;
    public BattleDigimonController[] enemyControllers;
    public digimonStatsManager statsManager;
    private FollowerAI followAI;
    public BattleUIManager battleUI;
    public vShooterMeleeInput input;

    public GameObject attackCanvas;

    private float enemyAttackCooldown = 2.5f;
    private float[] enemyAttackTimers;

    public Camera battleCamera;
    private Transform originalCameraParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    public GameObject player;
    public DigiClock digiClock;
    public GameObject statusCanvas;
    private int cameraViewIndex = 0;
    private List<Vector3> cameraOffsets = new List<Vector3>();
    private List<Vector3> cameraAngles = new List<Vector3>();
    public bool isInBattle = false;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AutoAssignReferences();
    }

    public void AutoAssignReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player GameObject not found! Check tag.");
            return;
        }

        playerStats = player.GetComponent<DigimonCombatStats>();
        playerController = player.GetComponent<BattleDigimonController>();
        followAI = player.GetComponent<FollowerAI>();
        digiClock = GetComponent<DigiClock>();  

        // Use names instead of GetChild(index)
        battleCamera = player.GetComponentInChildren<Camera>(true);
        attackCanvas = player.transform.Find("AttackCanvas")?.gameObject;

        if (attackCanvas == null)
            Debug.LogWarning("Attack canvas not found using name.");

        battleUI = attackCanvas != null ? attackCanvas.GetComponent<BattleUIManager>() : null;
    }

    public void InitiateBattle(DigimonCombatStats[] enemies)
    {
        if (battleEnded || checkStart) return;

        enemyStatsArray = enemies;
        enemyControllers = new BattleDigimonController[enemies.Length];
        enemyAttackTimers = new float[enemies.Length];

        for (int i = 0; i < enemies.Length; i++)
        {   
            enemyControllers[i] = enemies[i].GetComponent<BattleDigimonController>();
            enemyControllers[i].stats = enemies[i];
        }

        checkStart = true;
        BeginBattle();
    }
    
    private void BeginBattle()
    {if(attackCanvas==null) return;
        attackCanvas.SetActive(true);   

        statusCanvas.SetActive(false);
        digiClock.PauseClock();
        isInBattle = true;
        playerController.stats = playerStats;

        if (player.transform.childCount > 0)
        {
            if (attackCanvas != null)
            {
               Canvas canvas = attackCanvas.GetComponent<Canvas>();
                    if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                    {
                        canvas.worldCamera = battleCamera;
                    }
                
                attackCanvas.SetActive(true);
                battleUI = attackCanvas.GetComponent<BattleUIManager>();
            }
            else
            {
                Debug.LogWarning("Attack canvas is null in BeginBattle.");
            }
            attackCanvas.SetActive(true);
            battleUI = attackCanvas.GetComponent<BattleUIManager>();
            battleUI.RefreshEnemies();
        }
        else
        {
            Debug.LogWarning("Player has no child for Attack Canvas.");
        }

        if (followAI != null) followAI.enabled = false;

            playerStats.ResetBattleStats();
            foreach (var enemyStats in enemyStatsArray)
            enemyStats.ResetBattleStats();

        if (battleCamera != null)
        {
            originalCameraParent = battleCamera.transform.parent;
            originalLocalPosition = battleCamera.transform.localPosition;
            originalLocalRotation = battleCamera.transform.localRotation;
            battleCamera.transform.parent = null;
            battleCamera.enabled = true;
        }
        if (attackCanvas != null)
        {
            attackCanvas.SetActive(true);
            battleUI = attackCanvas.GetComponent<BattleUIManager>();
        }
        else
        {
            Debug.LogWarning("Attack canvas is null in BeginBattle.");
        }
        //  Move this block UP before PositionBattleCamera
        cameraOffsets.Clear();
        cameraAngles.Clear();

        cameraOffsets.Add(new Vector3(0, 6, -10));     // Behind player
        cameraAngles.Add(new Vector3(15, 0, 0));

        cameraOffsets.Add(new Vector3(10, 4, 0));      // Side
        cameraAngles.Add(new Vector3(10, -90, 0));

        cameraOffsets.Add(new Vector3(0, 12, 0));      // Overhead
        cameraAngles.Add(new Vector3(90, 0, 0));

        cameraOffsets.Add(new Vector3(-6, 4, 6));      // Diagonal front
        cameraAngles.Add(new Vector3(20, 135, 0));

        if (battleUI != null)
        {
            PositionBattleCamera(); //  Now safe to call
            battleUI.playerStats = playerStats;
            battleUI.playerController = playerController;
            playerController.SetBattleStartPosition();
            battleUI.battleManager = this;
            battleUI.EnablePlayerTurnUI(true);
            Debug.Log("Auto mode is " + battleUI.IsAutoModeEnabled());
          
        }

        playerController.stats = playerStats;

        foreach (var enemy in enemyControllers)
            enemy.stats = enemy.GetComponent<DigimonCombatStats>();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (enemyControllers.Length > 0)
        {
            playerController.transform.LookAt(enemyControllers[0].transform.position);
            enemyControllers[0].transform.LookAt(playerController.transform.position);
        }

        input.enabled = false;
        input.gameObject.GetComponent<Rigidbody>().useGravity = false;
        input.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        input.gameObject.GetComponent<Animator>().enabled = false;

        var characterController = playerController.GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = false;
    }
    private void UpdateBattleCameraView()
    {
        if (playerController == null || enemyControllers == null || enemyControllers.Length == 0)
            return;

        Transform playerTransform = playerController.transform;
        Transform enemyTransform = enemyControllers[0].transform;

        Vector3 midPoint = (playerTransform.position + enemyTransform.position) / 2f;

        Vector3 offset = cameraOffsets[cameraViewIndex];
        Vector3 angle = cameraAngles[cameraViewIndex];

        battleCamera.transform.position = midPoint + offset;
        battleCamera.transform.rotation = Quaternion.Euler(angle);
    }
    void Update()
    {
        if (!checkStart || battleEnded) return;

        // Switch camera view with C
        if (Input.GetKeyDown(KeyCode.C) && cameraOffsets.Count > 0)
        {
            cameraViewIndex = (cameraViewIndex + 1) % cameraOffsets.Count;
            UpdateBattleCameraView();
        }

        if (playerStats.currentHP <= 0)
        {
            EndBattle(false);
            return;
        }
       
        bool allEnemiesDead = true;
        for (int i = 0; i < enemyStatsArray.Length; i++)
        {
            if (enemyStatsArray[i] != null && enemyStatsArray[i].currentHP > 0)
            {
                allEnemiesDead = false;

                if (!enemyControllers[i].IsInCooldown() && playerStats.currentHP > 0)
                {
                    enemyAttackTimers[i] += Time.deltaTime;
                    if (enemyAttackTimers[i] >= enemyAttackCooldown)
                    {
                        enemyControllers[i].PerformAIAttack(playerStats);
                        enemyAttackTimers[i] = 0f;
                    }
                }
            }
        }

        if (allEnemiesDead)
        {
            EndBattle(true);
            return;
        }

        if (battleUI != null && battleUI.IsAutoModeEnabled())
        {
            if (!playerController.IsInCooldown() && IsPlayerReadyForAuto())
            {
                battleUI.AutoAttack();
            }
        }
    }

    private bool IsPlayerReadyForAuto()
    {
        if (playerController == null || playerController.anim == null)
            return false;

        if (playerController.IsInCooldown())
            return false;

        AnimatorStateInfo animState = playerController.anim.GetCurrentAnimatorStateInfo(0);
        return !(animState.IsName("hit") || animState.IsName("fall") || animState.IsName("knockdown"));
    }

    public void StartPlayerHitPause(float duration)
    {
        if (!isInHitPause)
            StartCoroutine(HitPauseCoroutine(duration));
    }
    private void PositionBattleCamera()
    {
        if (battleCamera == null || enemyControllers == null || enemyControllers.Length == 0)
            return;

        Transform playerTransform = playerController.transform;
        Transform enemyTransform = enemyControllers[0].transform;

        // Calculate midpoint and direction
        Vector3 midPoint = (playerTransform.position + enemyTransform.position) / 2f;
        float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);

        // Offset direction from enemy to player    
        Vector3 offsetDirection = (playerTransform.position - enemyTransform.position).normalized;
        Vector3 cameraOffset = Quaternion.Euler(15f, 30f, 0f) * offsetDirection * (distance * 0.9f);

        Vector3 finalCamPos = midPoint + cameraOffset + Vector3.up * 2f;

        battleCamera.transform.position = finalCamPos;
        battleCamera.transform.LookAt(midPoint + Vector3.up * 1f);
        UpdateBattleCameraView();
    }
    private IEnumerator HitPauseCoroutine(float duration)
    {
        isInHitPause = true;

        if (battleUI != null)
            battleUI.EnablePlayerTurnUI(false);

        yield return new WaitForSeconds(duration);

        isInHitPause = false;

        if (battleUI != null && battleUI.IsAutoModeEnabled())
            battleUI.AutoAttack();
        else if (battleUI != null)
            battleUI.EnablePlayerTurnUI(true);
    }

    public bool IsInHitPause() => isInHitPause;

    public void EndBattle(bool playerWon)
    {
        isInBattle = false;
        statusCanvas.SetActive(true);
        battleEnded = true;
        digiClock.UnpauseClock();
        if (battleCamera != null)
        {
            battleCamera.transform.parent = originalCameraParent;
            battleCamera.transform.localPosition = originalLocalPosition;
            battleCamera.transform.localRotation = originalLocalRotation;
            battleCamera.enabled = false;
        }
        if (followAI != null) followAI.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (battleUI != null)
        {
            battleUI.EnablePlayerTurnUI(false);
            battleUI.gameObject.SetActive(false);
        }

        Debug.Log(playerWon ? "Battle Won!" : "You Lost!");

        DigimonMoodManager mood = GetComponent<DigimonMoodManager>();
        if (mood != null && playerStats != null)
        {
            mood.CheckForInjury(playerStats.currentHP, playerStats.maxHP);
        }

        if (playerWon)
            BattleStatGainSystem.CalculateStatGains(playerStats, enemyStatsArray,statsManager);
            EvolutionManager evoManager = playerStats.GetComponent<EvolutionManager>();
            statsManager.updateStatsCanvas();
        if (evoManager != null)
        {   
            evoManager.IncrementBattleCount();
        }
        if (input != null)  
            input.enabled = true;
        input.gameObject.GetComponent<Rigidbody>().useGravity = true;
        input.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        input.gameObject.GetComponent<Animator>().enabled = true;
        // Reset flags for next battle
        battleEnded = false;
        checkStart = false;
    }
}
