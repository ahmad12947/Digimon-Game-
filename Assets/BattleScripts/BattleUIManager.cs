    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections;
    using Invector.vCharacterController;

    public class BattleUIManager : MonoBehaviour
    {
        public DigimonCombatStats playerStats;
        public BattleDigimonController playerController;
        public BattleManager battleManager;

        public Slider hpBar;
        public Slider mpBar;
        public Slider finishBar;

        public TextMeshProUGUI hpText, mpText;

        public GameObject attackPanel;
        public Button[] moveButtons;

        public Button autoButton;
        public Button runButton;
        public TextMeshProUGUI autoButtonText;

        private float finisherProgress = 0f;
        private float finisherTarget;
        private bool isAutoMode = false;

        public Button defendButton;
        private bool isDefending = false;
        public bool IsDefending => isDefending;

        private int currentTargetIndex = 0;
        private DigimonCombatStats[] enemyTargets;

        public Button changeTargetButton;
        public Button moveAwayButton;
        private bool overrideAutoOnce = false;
        public GameObject[] enemies;
        public void ResetDefendState()
        {
            isDefending = false;
        }

        private void Start()
        {
            SetupMoveButtons();

            autoButton.onClick.AddListener(ToggleAutoMode);
            runButton.onClick.AddListener(AttemptRun);
            defendButton.onClick.AddListener(OnDefend);
            changeTargetButton.onClick.AddListener(CycleTarget);
            moveAwayButton.onClick.AddListener(OnMoveAway);

            ToggleAutoMode();
            finisherTarget = (playerStats != null) ? 3000 - playerStats.speed : 3000;
            UpdateBars();
            EnablePlayerTurnUI(true);
        }

        private void Update()
        {
            if (playerStats == null) return;

            finisherProgress += Time.deltaTime;
            UpdateBars();
        }

        private void SetupMoveButtons()
        {
            for (int i = 0; i < moveButtons.Length; i++)
            {
                int index = i;
                moveButtons[i].onClick.RemoveAllListeners();

                if (index < playerController.equippedMoves.Length && playerController.equippedMoves[index] != null)
                {
                    moveButtons[i].interactable = true;
                    moveButtons[i].onClick.AddListener(() => OnMoveButton(index));
                }
                else
                {
                    moveButtons[i].interactable = false;
                }
            }
        }
        private void OnMoveAway()
        {
            if (playerStats.brains < 200) return;

            overrideAutoOnce = true;

            // Force move away immediately
            Debug.Log($"{playerStats.digimonName} is moving away!");
            playerController.ForceImmediateMoveAway();

            StartCoroutine(ResumeAutoAfterCooldown());
        }
        private void OnMoveButton(int index)
        {
            DigimonCombatStats target = GetCurrentTarget();
            if (target == null) return;

            overrideAutoOnce = true;

            // Force attack immediately, even if on cooldown
            playerController.ForceImmediateAttack(index, target);

            float chargeAmount = playerController.equippedMoves[index].isFinisher ? 0f : finisherTarget * 0.04f;
            finisherProgress += chargeAmount;

            StartCoroutine(ResumeAutoAfterCooldown());
        }

        private void OnDefend()
        {
            if (playerController == null) return;
            if (playerStats.brains < 300)
            {
                Debug.Log($"{playerStats.digimonName} doesn't have enough brains to defend.");
                return;
            }

            overrideAutoOnce = true;

            // Force defend immediately
            playerController.ForceImmediateDefend();

            StartCoroutine(ResumeAutoAfterCooldown());
        }
     
        void AttemptRun()
        {
            if ( playerStats.brains < 100 ) return;

            float chance = Random.value;
            if (chance <= 0.6f)
            {
                Debug.Log("Successfully escaped!");
                var input = battleManager.GetComponentInChildren<vShooterMeleeInput>();
                if (input != null) input.enabled = true;

                if (attackPanel != null)
                    attackPanel.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                battleManager.EndBattle(false);
            }
            else
            {
                Debug.Log("Failed to run!");
            }
        }

        private void CycleTarget()
        {
            if (playerStats.brains < 400) return;

            enemyTargets = GetAllEnemies();
            if (enemyTargets.Length <= 1) return;

            currentTargetIndex = (currentTargetIndex + 1) % enemyTargets.Length;
            Debug.Log("Target changed to: " + enemyTargets[currentTargetIndex].digimonName);
        }

        private DigimonCombatStats[] GetAllEnemies()
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            return System.Array.ConvertAll(enemies, e => e.GetComponent<DigimonCombatStats>());
        }

        private DigimonCombatStats GetCurrentTarget()
        {
            if (enemyTargets == null || enemyTargets.Length == 0)
                enemyTargets = GetAllEnemies();

            return enemyTargets[Mathf.Clamp(currentTargetIndex, 0, enemyTargets.Length - 1)];
        }

        private void UpdateBars()
        {
            hpBar.maxValue = playerStats.maxHP;
            hpBar.value = playerStats.currentHP;
            hpText.text = playerStats.currentHP + "/" + playerStats.maxHP;

            mpBar.maxValue = playerStats.maxMP;
            mpBar.value = playerStats.currentMP;
            mpText.text = playerStats.currentMP + "/" + playerStats.maxMP;

            finishBar.maxValue = finisherTarget;
            finishBar.value = finisherProgress;
        }

        public void EnablePlayerTurnUI(bool isEnabled)
        {
            attackPanel.SetActive(true);

            runButton.interactable = playerStats.brains >= 100;
            moveAwayButton.interactable = playerStats.brains >= 200;
            defendButton.interactable = playerStats.brains >= 300;
            changeTargetButton.interactable = playerStats.brains >= 400;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    public void RefreshEnemies()
    {
        enemyTargets = GetAllEnemies();
        currentTargetIndex = 0;
    }
    private void ToggleAutoMode()
        {
            isAutoMode = true;
            autoButtonText.text = "Auto: ON";
        }

        public void AutoAttack()
        {
            StartCoroutine(DelayedAutoAttackCoroutine());
        }

        private IEnumerator DelayedAutoAttackCoroutine()
        {
            yield return new WaitForSeconds(0.25f);

            if (overrideAutoOnce || playerController == null || playerStats.currentHP <= 0)
                yield break;

            DigimonCombatStats target = GetCurrentTarget();
            if (target == null) yield break;

            int brain = playerStats.brains;
            float choice = Random.value;

            if (choice < 0.2f && brain >= 200)
            {
                playerController.StartMoveAway();
            }
            else if (choice < 0.4f && brain >= 300)
            {
                playerController.isDefending = true;
            }
            else
            {
                int tries = 0;
                int index = Random.Range(0, playerController.equippedMoves.Length);
                while ((playerController.equippedMoves[index] == null ||
                       playerController.equippedMoves[index].mpCost > playerStats.currentMP) && tries < 10)
                {
                    index = Random.Range(0, playerController.equippedMoves.Length);
                    tries++;
                }

                if (playerController.equippedMoves[index] == null) yield break;

                playerController.PerformAttack(index, target);
                float chargeAmount = playerController.equippedMoves[index].isFinisher ? 0f : finisherTarget * 0.04f;
                finisherProgress += chargeAmount;
            }
        }

        private IEnumerator ResumeAutoAfterCooldown()
        {
            while (playerController.IsInCooldown())
                yield return null;

            yield return new WaitForSeconds(0.5f);
            overrideAutoOnce = false;
        }

        public bool IsAutoModeEnabled()
        {
            return isAutoMode;
        }
    }
