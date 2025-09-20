using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Invector.vCamera;
using Invector.vCharacterController;
using UnityEngine.UI;
public class trainingManager : MonoBehaviour
{
    public GameObject trainingCanvas;
    public Text trainingName;
    public vThirdPersonCamera cam;
    public vShooterMeleeInput input;
    public void performTraining(string training_Name)
    {

        trainingName.text = training_Name;
        trainingCanvas.SetActive(true);
        lockMouseMovement();
        pausePlayerMovement.instance.pausePlayer();
    }
    public void lockMouseMovement()
    {
        cam.enabled=false;
        Cursor.visible = true; // Hide the cursor
        Cursor.lockState = CursorLockMode.None; // Lock the cursor

    }
    public void unlockCamera()
    {
        cam.enabled = true;
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        
    }
}
