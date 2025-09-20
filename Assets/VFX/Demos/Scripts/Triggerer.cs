using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Triggerer : MonoBehaviour
{
    public VisualEffect vfx;

    [Header("Animation")]
    public Animator animator;
    public string animationName;
    public float vfxTriggerDelay;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TriggerAnimation());
        }
    }

    private IEnumerator TriggerAnimation()
    {
        animator.Play(animationName);
        yield return new WaitForSeconds(vfxTriggerDelay);
        vfx.SendEvent("OnPlay");
    }
}
