using UnityEngine;
using UnityEngine.EventSystems;

public class MenuAutoSelect : MonoBehaviour
{
    [Header("The first button to select")]
    public GameObject firstButton;
    [Header("The previous panel to go back to")]
    public GameObject previousPanel;

    private void Update()
    {


        // Check for Cancel input (Escape / Back button / Controller B)
        if (Input.GetButtonDown("Cancel"))
        {
            if (previousPanel != null)
            {
                // Close this panel
                gameObject.SetActive(false);

                // Open previous panel
                if (previousPanel.activeInHierarchy == true)
                {
                    previousPanel.GetComponent<MenuAutoSelect>().enabled = true;
                }
                previousPanel.SetActive(true);
                
            }
        }
    }
    void OnEnable()
    {
        // Clear previous selection
        EventSystem.current.SetSelectedGameObject(null);

        // Set new selection
        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton);
    }
}
