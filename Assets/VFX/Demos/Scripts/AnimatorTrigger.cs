using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTrigger : MonoBehaviour
{
    private Animator animator;
    public string animationName;
    public float vfxTriggerDelay;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IEnumerator Animation()
            {
                yield return new WaitForSeconds(vfxTriggerDelay);
                animator.Play(animationName);
            }
            StartCoroutine(Animation());
        }
    }
}
