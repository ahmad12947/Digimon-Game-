using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public string itemName;  // Name of the item
    private ItemPanelManager panelManager;

    void Start()
    {
        panelManager = FindObjectOfType<ItemPanelManager>();
        GetComponent<Button>().onClick.AddListener(OpenItemPanel);
    }

    void OpenItemPanel()
    {
        panelManager.OpenItemPanel(this, transform.position);
    }
}
