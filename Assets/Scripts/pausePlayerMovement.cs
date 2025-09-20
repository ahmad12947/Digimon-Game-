using Invector.vCharacterController;
using System.Collections;
using UnityEngine;

public class pausePlayerMovement : MonoBehaviour
{

    public vShooterMeleeInput input;


    public static pausePlayerMovement instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void UnPauseWithTime(float time)
    {
        StartCoroutine("unPauseWithTime",time);
    }

    private IEnumerator unPauseWithTime(float timer)
    {

        yield return new WaitForSeconds(timer);
        unPausePlayer();
    }
    public void pausePlayer()
    {
        
        input.GetComponent<Rigidbody>().useGravity = false;
        input.GetComponent<Rigidbody>().isKinematic = true;
        input.GetComponent<Animator>().enabled = false;
        input.enabled = false;
    }
    public void unPausePlayer()
    {
        input.enabled = true;
        input.gameObject.GetComponent<Rigidbody>().useGravity = true;
        input.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        input.gameObject.GetComponent<Animator>().enabled = true;
    }
}
