using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCamera;

public class CanvasesManager : MonoBehaviour
{
    public GameObject menuCanvas, InventoryCanvas,optionPanel, digimonStatusMnager;
    public vThirdPersonCamera cameraThird;

    private bool isMenuOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isMenuOpen = !isMenuOpen;
        menuCanvas.SetActive(isMenuOpen);

        if (InventoryCanvas.activeInHierarchy)
        {
            InventoryCanvas.SetActive(false);
            optionPanel.SetActive(false);
        }

        if (digimonStatusMnager.activeInHierarchy)
        {
            digimonStatusMnager.SetActive(false);
        }

        if (isMenuOpen)
        {
            // Pause game
            Time.timeScale = 0f;

            cameraThird.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Resume game
            Time.timeScale = 1f;

            cameraThird.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
