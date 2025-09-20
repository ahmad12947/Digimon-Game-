using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    public Transform targetPoint;   // Point to move towards
    public float speed = 3f;        // Movement speed
    private Animator anim;

    private bool reached = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play("walk"); // start walking animation
        }
    }

    void Update()
    {
        if (reached || targetPoint == null) return;

        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Check if reached
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            reached = true;

            if (anim != null)
            {
                anim.Play("idle"); // play idle animation once reached
            }

            // Rotate only on Y = 180, X and Z = 0
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
}
