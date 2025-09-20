using UnityEngine;

public class FollowerAI : MonoBehaviour
{
    public Transform player;
    public float followRadius = 3f;     // Start running if player farther than this
    public float stopRadius = 1.5f;     // Stop moving if closer than this
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    // Extra margins to prevent constant switching
    public float runExitRadius = 2.8f;  // Must get closer than this to stop running
    public float walkExitRadius = 1.8f; // Must get closer than this to stop walking

    private Animator animator;
    private float currentSpeed;
    private string currentState = "Idle";

    private Vector3 lastPosition;
    private float movementThreshold = 0.01f;

    // Animator parameter (bool) to indicate movement intent (true for Walking or Running)
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        string newState = currentState;

        // State machine with hysteresis
        switch (currentState)
        {
            case "Idle":
                if (distanceToPlayer > followRadius)
                    newState = "Running";
                else if (distanceToPlayer > stopRadius)
                    newState = "Walking";
                break;

            case "Walking":
                if (distanceToPlayer > followRadius)
                    newState = "Running";
                else if (distanceToPlayer < walkExitRadius)
                    newState = "Idle";
                break;

            case "Running":
                if (distanceToPlayer < runExitRadius)
                    newState = "Walking";
                break;
        }

        // Set movement speed based on state
        if (newState == "Running") currentSpeed = runSpeed;
        else if (newState == "Walking") currentSpeed = walkSpeed;
        else currentSpeed = 0f;

        // ---- NEW: set animator bool "isWalking" to reflect intent to move
        // True when we intend to Walk or Run; false only when Idle.
        bool shouldWalk = (newState == "Walking" || newState == "Running");
        animator.SetBool(IsWalkingHash, shouldWalk);
        // ----

        // Move & rotate before checking actual movement
        if (currentSpeed > 0)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * currentSpeed * Time.deltaTime;

            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Check if actually moving this frame
        float movementDelta = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
        bool isActuallyMoving = movementDelta > movementThreshold;

        // Play animation if state changes
        if (newState != currentState && (newState == "Idle" || isActuallyMoving))
        {
            animator.Play(newState);
            currentState = newState;
        }
    }
}
