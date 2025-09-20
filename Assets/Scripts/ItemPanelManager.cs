using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Invector.vCamera;

public class ItemPanelManager : MonoBehaviour
{
    public GameObject itemPanel, inventoryClose;  // Reference to the UI panel
    public Button useButton, moveButton, sortButton, dropButton;  // Action buttons
    public InventoryItem selectedItem;  // The item currently selected
    public characterAnimationsHandler characterAnimationsHandler;
    public digimonaAnimationManager digimonaAnimationManager;
    public vThirdPersonCamera vCamera;
    public DigimonMoodManager moodManager;

    void Start()
    {
        // Ensure the panel is hidden at the start
        itemPanel.SetActive(false);

        // Attach button events
        useButton.onClick.AddListener(() => UseItem());
        moveButton.onClick.AddListener(() => MoveItem());
        sortButton.onClick.AddListener(() => SortItem());
        dropButton.onClick.AddListener(() => DropItem());

        moodManager = GameObject.FindGameObjectWithTag("Player").GetComponent<DigimonMoodManager>();
        digimonaAnimationManager = GameObject.FindGameObjectWithTag("Player").GetComponent<digimonaAnimationManager>();
    }

    public void digimonEvolved(GameObject obj)
    {
        moodManager = obj.GetComponent<DigimonMoodManager>();
        digimonaAnimationManager = obj.GetComponent<digimonaAnimationManager>();
    }

    void Update()
    {
        // Detect if the player clicks anywhere outside the panel to close it
        if (itemPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject(itemPanel))
            {
                ClosePanel();
            }
        }
    }

    // Helper method to check if pointer is over the item panel or its children
    private bool IsPointerOverUIObject(GameObject panel)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == panel || result.gameObject.transform.IsChildOf(panel.transform))
            {
                return true; // Clicked inside the panel
            }
        }

        return false; // Clicked outside
    }

    // Function to open the panel for a specific item
    public void OpenItemPanel(InventoryItem item, Vector3 position)
    {
        selectedItem = item;
        itemPanel.SetActive(true);
        inventoryClose.GetComponent<MenuAutoSelect>().enabled = false;
        itemPanel.transform.position = position;
    }

    // Function to close the panel
    public void ClosePanel()
    {
        inventoryClose.GetComponent<MenuAutoSelect>().enabled = true;
        itemPanel.SetActive(false);
        Time.timeScale = 1;
        selectedItem = null;
    }

    void UseItem()
    {
        if (selectedItem != null)
        {
            var ui = selectedItem.transform.parent.GetComponent<InventoryItemUI>();
            if (ui == null)
            {
                Debug.LogError("InventoryItemUI script not found on parent!");
                return;
            }

            var itemData = ui.itemData;
            Debug.Log($"Using {itemData.itemName}");
           
            switch (itemData.itemType)
            {
                case ItemType.Food:
                    InventoryManager.instance.RemoveItem(itemData);
                    characterAnimationsHandler.useCalled();
                    digimonaAnimationManager.eat();
                    moodManager.onFeed(
                        itemData.energy,
                        itemData.weight,
                        itemData.tiredness,
                        itemData.discipline,
                        itemData.HP,
                        itemData.MP,
                        itemData.offense,
                        itemData.defense,
                        itemData.speed,
                        itemData.brains,
                        itemData.lifeSpan,
                        itemData.happiness
                    );
                    break;

                case ItemType.Medicine:
                    InventoryManager.instance.RemoveItem(itemData);
                    // TODO: Add healing logic here
                    break;

                case ItemType.KeyItem:
                    // Handle special key item logic here
                    break;
            }

            vCamera.enabled = true;
            inventoryClose.SetActive(false);
            ClosePanel();
        }
    }

    void MoveItem()
    {
        if (selectedItem != null)
        {
            Debug.Log($"Moving {selectedItem.itemName}");
            ClosePanel();
        }
    }

    void SortItem()
    {
        if (selectedItem != null)
        {
            Debug.Log($"Sorting {selectedItem.itemName}");
            ClosePanel();
        }
    }

    void DropItem()
    {
        if (selectedItem != null)
        {
            var ui = selectedItem.transform.parent.GetComponent<InventoryItemUI>();
            if (ui == null)
            {
                Debug.LogError("InventoryItemUI script not found on parent!");
                return;
            }

            var itemData = ui.itemData;

            //  Instantiate drop prefab if available
            if (itemData.dropPrefab != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Vector3 dropPosition = player.transform.position + player.transform.forward * 1.5f;
                    Quaternion dropRotation = Quaternion.identity;

                    Instantiate(itemData.dropPrefab, dropPosition, dropRotation);
                }
                else
                {
                    Debug.LogWarning("Player object not found when trying to drop item.");
                }
            }
            else
            {
                Debug.LogWarning($"Drop prefab not assigned for item: {itemData.itemName}");
            }

            //  Remove item from inventory
            InventoryManager.instance.RemoveItem(itemData);

            //  UI handling
            vCamera.enabled = true;
            inventoryClose.SetActive(false);
            ClosePanel();
        }
    }
}
