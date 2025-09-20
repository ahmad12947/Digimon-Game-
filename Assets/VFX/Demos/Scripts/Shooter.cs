using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Shooter : MonoBehaviour
{
    public Transform target;
    public Projectile projectilePrefab;
    public float speed;
    public bool isProminenceBeam = false;

    [Header("Animation")]
    public Animator animator;
    public string animationName;
    public float vfxTriggerDelay;

    [Header("Shoot to direction")]
    public bool shootToDirection = false;
    public Vector3 shootVelocity;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TriggerAnimation());
        }
    }

    private void Shoot()
    {
        //Projectile projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        //if (isProminenceBeam)
        //{
        //    projectile.GetComponent<VisualEffect>().SetVector3("Position", transform.position);
        //    projectile.GetComponent<VisualEffect>().SetVector3("RingVelocity", speed * 0.5f * (target.position - transform.position).normalized);
        //}
        //projectile.GetComponent<VisualEffect>().SendEvent("OnPlay");
        //projectile.Shoot(speed * (target.position - transform.position).normalized);

        StartCoroutine(TriggerAnimation());
    }

    private void ShootWithVelocity(Vector3 velocity)
    {
        //Projectile projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        ////projectile.GetComponent<VisualEffect>().SendEvent("OnPlay");
        //projectile.Shoot(velocity);
        StartCoroutine(TriggerAnimation());
    }

    private IEnumerator TriggerAnimation()
    {
        animator.Play(animationName);
        yield return new WaitForSeconds(vfxTriggerDelay);

        if (!shootToDirection)
        {
            Projectile projectile = Instantiate(projectilePrefab, transform.position, new Quaternion());
            if (isProminenceBeam)
            {
                projectile.GetComponent<VisualEffect>().SetVector3("Position", transform.position);
                projectile.GetComponent<VisualEffect>().SetVector3("RingVelocity", speed * 0.2f * (target.position - transform.position).normalized);
            }
            projectile.GetComponent<VisualEffect>().SendEvent("OnPlay");
            projectile.Shoot(speed * (target.position - transform.position).normalized);
        }
        else
        {
            Projectile projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            //projectile.GetComponent<VisualEffect>().SendEvent("OnPlay");
            projectile.Shoot(shootVelocity);
        }
    }
}
