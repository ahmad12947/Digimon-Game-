using UnityEngine;

public enum ItemType
{
    Food,
    Medicine,
    Equipment,
    KeyItem,
    Misc
}

[CreateAssetMenu(menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int quantity = 1;

    public ItemType itemType;

    // Feed stats (optional based on type)
    public int energy;
    public int weight;
    public int tiredness;
    public int discipline;
    public int HP;
    public int MP;
    public int offense;
    public int defense;
    public int speed;
    public int brains;
    public int lifeSpan;
    public int happiness;

    [Header("World Drop")]
    public GameObject dropPrefab;
}
