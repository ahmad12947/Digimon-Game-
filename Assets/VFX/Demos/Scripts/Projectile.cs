using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public VisualEffect vfx;
    public Impact impactPrefab;
    private Rigidbody rb;
    public bool killImmedatelyOnHit = true;
    public float killTime = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 velocity)
    {
        vfx.SendEvent("OnPlay");
        rb.linearVelocity = velocity;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (killImmedatelyOnHit)
        {
            Destroy(gameObject);
        }
        else
        {
            vfx.SetBool("IsAlive", false);
            Destroy(gameObject, killTime);
        }

        Impact impact = Instantiate(impactPrefab, transform.position, Quaternion.identity);
        impact.Play();
    }
}
