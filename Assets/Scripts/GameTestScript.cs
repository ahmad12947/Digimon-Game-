using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameTestScript : MonoBehaviour
{
    public TextMeshProUGUI nameTxt, weightTxt, AgeTxt, TirednessTxt, Discipline, ageUntilEvolve, currentDigivolve, happinessTxt;
    public TextMeshProUGUI poopTxt, hungerTxt, sleepTxt, CareMistake;
    public GameObject digimon;
    public DigimonMoodManager digimonMoodManager;
    public EvolutionManager evolutionManager;
    public GameObject enemy, enemySpawnArea;
    public digimonStatsManager digimonStatsManager;
    private void Start()
    {
       
        Invoke("setUp", 3);
     
    }
    public void setUp()
    {
        
        digimon = GameObject.FindGameObjectWithTag("Player");
        digimonMoodManager = digimon.GetComponent<DigimonMoodManager>();
        evolutionManager = digimon.GetComponent<EvolutionManager>();

    }
    public void onEvolve(GameObject obj)
    {
       
      
    }
    private void Update()
    {
        if (digimon != null)
        {
            if (digimonMoodManager != null)
            {
                poopTxt.text = "Poop : " + digimonMoodManager.poop.ToString();
                hungerTxt.text = "Hunger : " + digimonMoodManager.hunger.ToString();
                sleepTxt.text = "Sleep : " + digimonMoodManager.sleep.ToString();
                CareMistake.text = "Care Mistake : " + evolutionManager.careMistakeCount.ToString();
                nameTxt.text = "Name : " + evolutionManager.currentDigimonName;
                weightTxt.text = "Weight :" + digimonStatsManager.Weight;
                AgeTxt.text = "Age : " + digimonStatsManager.Age;
                TirednessTxt.text = "Tiredness : " + digimonMoodManager.Tiredness;
                Discipline.text = "Discipline : " + digimonMoodManager.Discipline;
                ageUntilEvolve.text = "Age Unitl Evolve : " + evolutionManager.evolutionAgeThreshold;
                currentDigivolve.text = "Current Digivolv : " + evolutionManager.PredictMostLikelyEvolution();
                happinessTxt.text = "Happiness : " + digimonMoodManager.Happiness;

                    
            }
            


        }
        else
        {
            digimon = GameObject.FindGameObjectWithTag("Player");
            digimonMoodManager = digimon.GetComponent<DigimonMoodManager>();
            evolutionManager = digimon.GetComponent<EvolutionManager>();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            Instantiate(enemy, enemySpawnArea.transform.position+new Vector3(2,0,1), Quaternion.identity);
        }
    }
}
