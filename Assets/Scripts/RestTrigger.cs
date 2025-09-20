using UnityEngine;

public class RestTrigger : MonoBehaviour
{
    public enum RestType { Centarumon, Punimon, Kuwagamon }
    public RestType restType;

    [Header("Rest Effects Per Hour")]
    public int tirednessRestore = 20;
    public int hpRestore = 0;
    public int mpRestore = 0;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            RestManager.Instance.ShowRestPrompt(this);
        }
    }

    private void OnTriggerStay(Collider other)
    { if (other.CompareTag("Player2"))
        {
            //Debug.Log("rest");
            isPlayerInRange = true;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Trigger Entered by: {other.name}");
        if (other.CompareTag("Player2"))
        {
            Debug.Log("Player entered rest zone.");
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player2"))
        {
            isPlayerInRange = false;
            RestManager.Instance.HideRestPrompt();
        }
    }
}
