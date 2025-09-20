using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class itemInfo : MonoBehaviour
{
    public bool isCollected = false;
    public ItemData itemData; //  Reference to ScriptableObject for this item's info

    [System.Serializable]
    public class FunctionCall
    {
        public UnityEvent functionToCall;
    }

    [SerializeField]
    private List<FunctionCall> functionToCall = new List<FunctionCall>();

    public void callAllFunctions()
    {
        foreach (FunctionCall functionCall in functionToCall)
        {
            functionCall.functionToCall.Invoke();
        }
    }
}
