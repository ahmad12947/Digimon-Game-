using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currentTrainingInteractionManager : MonoBehaviour
{
    public GameObject currentActive;


    public void currentObjActive(GameObject obj)
    {
        currentActive=obj;
    }
    public void callFunction()
    {
        currentActive.transform.GetComponent<TrainingBoosters>().CallAllFunctions();
        makeNull();
    }
    public void makeNull()
    {
        currentActive=null;
    }
}
