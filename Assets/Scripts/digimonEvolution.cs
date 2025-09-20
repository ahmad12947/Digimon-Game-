using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class digimonEvolution : MonoBehaviour
{
    public GameObject fresh, trainee, rookie, champion, ultimate,mega;
    public TextMeshProUGUI leveledUpText;

    public digimonStatsManager statsManager;
    public DigimonStatusCanvasManager digitalStatusCanvasManager_;
    public int currentLevel = 1;

    public characterAnimationsHandler characterAnimationsHandler_;
    public ItemPanelManager itemPanelManager_;

    private void Start()
    {
        digitalStatusCanvasManager_.updateName(fresh.name);
        currentLevel = 1;
        deactivateAllDigimons();
        fresh.SetActive(true);
    }

    public void deactivateAllDigimons()
    {
        fresh.SetActive(false);
        trainee.SetActive(false); 
        rookie.SetActive(false);
        champion.SetActive(false);
        ultimate.SetActive(false);
        mega.SetActive(false);
    }
    private void Update()
    {
        checkForLevelUp();
        testHpBoost();
    }

    private void clearText()
    {
        leveledUpText.text = "";
      
    }
    private void testHpBoost()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            statsManager.addHp(100);
            Debug.Log("AddedHP");
            
        }
    }
    private void checkForLevelUp()
    {
        if (currentLevel == 1)
        {
            if (statsManager.Hp >= 850)
            {
                activateTrainee();
                currentLevel++;
            }
        }
        else if (currentLevel == 2)
        {
            if (statsManager.Hp >= 1000)
            {
                activateRookie();
                currentLevel++;
            }

        }
        else if (currentLevel == 3)

        {
            if (statsManager.Hp >= 1500)
            {
                activateChampion();
                currentLevel++;
            }
        }
        else if (currentLevel == 4)
        {
            if (statsManager.Hp >= 2000)
            {
                activateUltimate();
                currentLevel++;
            }
        }
        else if (currentLevel == 5)
        {
            if (statsManager.Hp >= 2400)
            {
                activateMega();
                currentLevel++;
            }
        }
        


    }
    public void activateFresh()
    {
        deactivateAllDigimons();
        fresh.SetActive(true);
        leveledUpText.text = "Digimon Evolved";
        Invoke("clearText", 3);
    }
    public void activateTrainee()
    {
        deactivateAllDigimons();
        digitalStatusCanvasManager_.updateName(trainee.name);
        Invoke("clearText", 3);
        leveledUpText.text = "Digimon Evolved";
        trainee.SetActive(true);
        characterAnimationsHandler_.changeDigimonHandler(trainee);
        itemPanelManager_.digimonEvolved(trainee);
    }
    public void activateRookie()
    {
        deactivateAllDigimons();
        Invoke("clearText", 3);      
        leveledUpText.text = "Digimon Evolved";
        rookie.SetActive(true);
        digitalStatusCanvasManager_.updateName(rookie.name);
        characterAnimationsHandler_.changeDigimonHandler(rookie);
        itemPanelManager_.digimonEvolved(rookie);
    }
    public void activateChampion()
    {
        deactivateAllDigimons();
        Invoke("clearText", 3);
        leveledUpText.text = "Digimon Evolved";
       
        champion.SetActive(true);
        digitalStatusCanvasManager_.updateName(champion.name);
        characterAnimationsHandler_.changeDigimonHandler(champion);
        itemPanelManager_.digimonEvolved(champion);
    }
    public void activateUltimate()
    {
        deactivateAllDigimons();
        Invoke("clearText", 3);
        leveledUpText.text = "Digimon Evolved!!";
        ultimate.SetActive(true );
        digitalStatusCanvasManager_.updateName(ultimate.name);
        characterAnimationsHandler_.changeDigimonHandler(ultimate);
        itemPanelManager_.digimonEvolved(ultimate);
    }
    public void activateMega()
    {
        deactivateAllDigimons();
        Invoke("clearText", 3);
        leveledUpText.text = "Digimon Evolved";
        mega.SetActive(true);
        digitalStatusCanvasManager_.updateName(mega.name);
        characterAnimationsHandler_.changeDigimonHandler(mega);
        itemPanelManager_.digimonEvolved(mega);
    }

   


}
