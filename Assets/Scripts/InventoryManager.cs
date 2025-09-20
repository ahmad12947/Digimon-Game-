using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    private int meat = 0;
    private int mp_Floppy=0;
    private int bandage = 0;
    private int smRecovery = 0;
    private int medicine = 0;
    private int restore = 0;

    public TextMeshProUGUI meatText, mp_FloppyText, bandageText, smRecoveryText, medicineText, restoreText;

    public static InventoryManager instance;
    public GameObject inventoryItemPrefab;  // prefab with icon/text
    public Transform contentParent;         // parent inside scroll view

    private Dictionary<string, InventoryItemUI> inventoryMap = new Dictionary<string, InventoryItemUI>();

    private void Awake()
    {
        instance = this;
    }
    public void AddItemToInventory(ItemData itemData)
    {
        if (inventoryMap.ContainsKey(itemData.itemName))
        {
            // Item already exists, increase quantity
            inventoryMap[itemData.itemName].IncreaseQuantity(itemData.quantity);
        }
        else
        {
            // Item does not exist, instantiate prefab
            GameObject newItem = Instantiate(inventoryItemPrefab, contentParent);
            InventoryItemUI itemUI = newItem.GetComponent<InventoryItemUI>();

            if (itemUI != null)
            {
                itemUI.Initialize(itemData);
                inventoryMap.Add(itemData.itemName, itemUI);
            }
        }
    }
    public void addMeat()
    {
        meat++;
        meatText.text = meat.ToString();
    }
    public void addmp_Floppy()
    {
        mp_Floppy++;
        mp_FloppyText.text = mp_Floppy.ToString();
    }
    public void addBandage()
    {
        bandage++;
        bandageText.text = bandage.ToString();
    }
    public void addSmRecovery()
    {
        smRecovery++;
        smRecoveryText.text = smRecovery.ToString();
    }
    public void addMedicine()
    {
        medicine++;
        medicineText.text = medicine.ToString();
    }
    public void addRestore()
    {
        restore++;
        restoreText.text = restore.ToString();
    }

    public void decMeat()
    {
        meat--;
        meatText.text = meat.ToString();
    }
    public void decMpFloppy()
    {
        mp_Floppy--;
        mp_FloppyText.text = mp_Floppy.ToString();
    }
    public void decBandage()
    {
        bandage--;
        bandageText.text = bandage.ToString();
    }
    public void decSmRecovery()
    {
        smRecovery--;
        smRecoveryText.text= smRecovery.ToString();
    }
    public void decMedicine()
    {
        medicine--;
        medicineText.text= medicine.ToString();
    }
    public void decRestore()
    {
        restore--;
        restoreText.text= restore.ToString();
    }
    public void RemoveItem(ItemData itemData)
    {
        if (inventoryMap.ContainsKey(itemData.itemName))
        {
            inventoryMap[itemData.itemName].DecreaseQuantity();
            if (inventoryMap[itemData.itemName].GetQuantity() <= 0)
            {
                Destroy(inventoryMap[itemData.itemName].gameObject);
                inventoryMap.Remove(itemData.itemName);
            }
        }
    }
    private void Start()
    {
       
        mp_Floppy=3;    
        bandage=2;
        smRecovery=3;
        medicine=1;
        restore=1;

    }

    





}
