using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Invector.vCamera;

public class AttackManager : MonoBehaviour
{
    public GameObject menuCanvas;  // Assign the Canvas in the inspector
    public Button[] buttons;       // Assign all three buttons in the inspector
    public TextMeshProUGUI buttonText; // Assign the TextMeshProUGUI component
    private Button selectedButton;
    public vThirdPersonCamera cameraThird;
    public Animator animator;
    public battleCommands battleCommands;
    public bool isAttacking = false;
    public float detectionRadius = 10f; // Radius to search for enemies
    public GameObject[] allEnemies;
    public GameObject closestEnemy;
    public GameObject playerCamera, battleCamera;
    public FollowerAI follower;

    private int targetIndex = 0; // Index to track current target

    void Start()
    {
        follower = GetComponent<FollowerAI>();
        menuCanvas.SetActive(false); // Hide canvas initially
        Cursor.visible = false;
        animator = GetComponent<Animator>();

        // Assign click event listeners
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => OnButtonSelected(btn));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Optional: Close menu on Escape
        {
            CloseMenu();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            OpenMenu();
        }
        if (isAttacking)
        {
            if (closestEnemy == null) // Find target only once
            {
                FindClosestTarget();
            }
            else
            {
                follower.enabled = false;
                transform.LookAt(closestEnemy.transform.position); // Ensure player stays focused
            }
            if (menuCanvas.activeInHierarchy == false)
            {
                OpenMenu();
            }

            // Switch target when pressing Tab
            if (Input.GetKeyDown(KeyCode.N))
            {
                SwitchTarget();
            }
        }
    }

    private void FindClosestTarget()
    {
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy"); // Get all enemies once

        if (allEnemies.Length == 0)
        {
            closestEnemy = null;
            return;
        }

        float closestDistance = detectionRadius;
        int closestIndex = -1;

        for (int i = 0; i < allEnemies.Length; i++)
        {
            GameObject enemy = allEnemies[i];

            if (enemy.GetComponent<EnemyHealthManager>() != null) // Check if the enemy is valid
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }
        }

        if (closestIndex != -1)
        {
            targetIndex = closestIndex; // Store the index of the closest enemy
            closestEnemy = allEnemies[targetIndex];
            transform.LookAt(closestEnemy.transform.position);
        }
    }

    public void SwitchTarget()
    {
        if (allEnemies.Length > 1)
        {
            targetIndex = (targetIndex + 1) % allEnemies.Length; // Cycle through the list
            closestEnemy = allEnemies[targetIndex];
            transform.LookAt(closestEnemy.transform.position);
        }
    }

    public void OpenMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        cameraThird.enabled = false;
        Cursor.visible = true;
        menuCanvas.SetActive(true);
        playerCamera.SetActive(false);
        battleCamera.SetActive(true);
        EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        OnButtonSelected(buttons[0]); // Select first button by default
    }

    private void OnButtonSelected(Button btn)
    {
        // Set text to button name
        buttonText.text = btn.name;

        // Highlight the selected button
        foreach (Button b in buttons)
        {
            Transform highlighter = b.transform.GetChild(0);
            highlighter.gameObject.SetActive(b == btn);
        }

        selectedButton = btn;

        // Perform actions based on selected button
        if (btn.name == "ATTACK")
        {
            PerformAttack();
        }
        else if (btn.name == "AUTO")
        {
            PerformAuto();
        }
        else if (btn.name == "RUN")
        {
            PerformRun();
        }
        else if (btn.name == "Defend")
        {
            PerformDefense();
        }
        else if (btn.name == "Distance")
        {
            PerformDistance();
        }
    }

    private void PerformDistance()
    {
        battleCommands.performDistance();
    }

    private void PerformDefense()
    {
        battleCommands.performDefense();
    }

    private void PerformAttack()
    {
        battleCommands.peroformAttack();
    }

    private void PerformRun()
    {
        isAttacking = false;
        cameraThird.gameObject.GetComponent<characterAnimationsHandler>().enableMovements();
        closestEnemy = null; // Reset target when running away
        CloseMenu();
        battleCommands.runOff();
    }

    private void PerformAuto()
    {
        battleCommands.Set_autoAttack();
    }

    public void CloseMenu()
    {
        playerCamera.SetActive(true);
        battleCamera.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        cameraThird.enabled = true;
        menuCanvas.SetActive(false);
        Cursor.visible = false;
    }

    public void CheckBattleStatus()
    {
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            EnemyHealthManager healthManager = enemy.GetComponent<EnemyHealthManager>();
            if (healthManager != null && healthManager.c_health > 0)
            {
                return; // At least one enemy is still alive
            }
        }
        Debug.Log("BattleOver");
        transform.GetComponent<FollowerAI>().enabled = true;
        // If no alive enemies are found, end the battle
        isAttacking = false;
        closestEnemy = null;
        CloseMenu();
    }
}
