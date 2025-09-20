using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject PressToStartPanel,Mainmenu;
    [Header("The previous panel to go back to")]
    public GameObject previousPanel, greenBackground;
    
    private void Update()
    {
        if(Input.GetKey(KeyCode.E))
        {
            if (PressToStartPanel.activeInHierarchy == true)
            {
                Mainmenu.SetActive(true);
                greenBackground.SetActive(true);
                PressToStartPanel.SetActive(false);
            }
        }

       

    }
}
