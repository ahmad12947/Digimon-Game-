using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;
    public Image iconImage;

    private int quantity = 0;

    public ItemData itemData; //  Store itemData reference

    public void Initialize(ItemData newItemData)
    {
        itemData = newItemData; //  Save the reference
        itemNameText.text = itemData.itemName;
        iconImage.sprite = itemData.itemIcon;
        quantity = itemData.quantity;
        UpdateUI();
    }

    public void IncreaseQuantity(int amount = 1)
    {
        quantity += amount;
        UpdateUI();
    }

    public void DecreaseQuantity(int amount = 1)
    {
        quantity -= amount;
        quantity = Mathf.Max(0, quantity);
        UpdateUI();
    }

    private void UpdateUI()
    {
        quantityText.text = "x" + quantity.ToString();
    }

    public int GetQuantity() => quantity;
}
