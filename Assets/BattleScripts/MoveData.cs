using UnityEngine;

[CreateAssetMenu(menuName = "Digimon/MoveData")]
public class MoveData : ScriptableObject
{
    public string moveName;
    public int power;
    public int mpCost;
    public float accuracy;
    public bool isFinisher;

    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    [Tooltip("Maximum distance allowed to use this move")]
    public float attackRange = 5f; // Default value, adjust per move in Inspector
}
