using UnityEngine;

[CreateAssetMenu(fileName = "EvolutionRequirement", menuName = "Digimon/Evolution Requirement")]
public class EvolutionRequirement : ScriptableObject
{
   
    public string toDigimonName;
    public bool evolveOnThreshold = false;
    public int requiredHP;
    public int requiredMP;
    public int requiredOff;
    public int requiredDef;
    public int requiredSpeed;
    public int requiredBrains;
    public int minWeight;
    public int maxWeight;
    public int maxCareMistakes;
    public int minCareMistakes ;
    public int minBattles;

    
}
