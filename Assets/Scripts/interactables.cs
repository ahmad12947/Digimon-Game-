using UnityEngine;

public class interactables : MonoBehaviour
{
    public string Collectable = "Collectable";
    public InventoryManager inventoryManager; // assign in inspector

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Collectable))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                itemInfo info = other.GetComponent<itemInfo>();
                if (info != null && !info.isCollected)
                {
                    inventoryManager.AddItemToInventory(info.itemData);
                    info.callAllFunctions();
                    info.isCollected = true;
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
