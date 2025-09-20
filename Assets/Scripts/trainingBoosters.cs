using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

public class TrainingBoosters : MonoBehaviour
{
    // List of UnityEvents to allow drag-and-drop functions
    [System.Serializable]
    public class FunctionCall
    {
        public string functionName; // Optional name for clarity
        public UnityEvent functionToCall;
    }

    [SerializeField]
    private List<FunctionCall> functionsToCall = new List<FunctionCall>();

    // Start is called before the first frame update
    void Start()
    {
        //CallAllFunctions();
    }

    // Function to call all functions in the list
    public void CallAllFunctions()
    {
        foreach (FunctionCall functionCall in functionsToCall)
        {
            functionCall.functionToCall.Invoke();
        }
    }
}
