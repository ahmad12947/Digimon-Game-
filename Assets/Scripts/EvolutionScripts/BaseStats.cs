using UnityEngine;

public class BaseStats : MonoBehaviour
{
    public int HP;
    public int MP;
    public int Off;
    public int Def;
    public int Speed;
    public int Brain;
    public int Weight;
    public int maxHP;
    public int maxMP;

    public DigimonStatusCanvasManager digimonStatusCanvasManager;
    private void Start()
    {
        digimonStatsManager statsManager = FindObjectOfType<digimonStatsManager>();
      
        if (statsManager != null)
        {
            statsManager.SetStats(HP, MP, Off, Def, Speed, Brain, Weight,maxHP,maxMP);
            Debug.Log($"Applied base stats from {name} to Stats Manager");
            Invoke("setdigimonCanvasStats", 2);
        }
        else
        {
            Debug.LogWarning("digimonStatsManager not found in the scene.");
        }


       
    }

    public void setDigimonstats()
    {
        digimonStatsManager statsManager = FindObjectOfType<digimonStatsManager>();

        if (statsManager != null)
        {
            statsManager.SetStats(HP, MP, Off, Def, Speed, Brain, Weight, maxHP, maxMP);
            Debug.Log($"Applied base stats from {name} to Stats Manager");
            Invoke("setdigimonCanvasStats", 2);
        }
        else
        {
            Debug.LogWarning("digimonStatsManager not found in the scene.");
        }
    }
    public void setdigimonCanvasStats()
    {

            digimonStatusCanvasManager.gameObject.GetComponent<Canvas>().enabled = false;
            digimonStatusCanvasManager.gameObject.SetActive(true); 
            digimonStatusCanvasManager.updateStats();
           digimonStatusCanvasManager.gameObject.SetActive(false);
           digimonStatusCanvasManager.gameObject.GetComponent<Canvas>().enabled = true;


    }
}
