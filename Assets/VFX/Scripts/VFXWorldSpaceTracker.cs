using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[DefaultExecutionOrder(-1)]
public class VFXWorldSpaceTracker : MonoBehaviour
{
    public VisualEffect vfx;
    
    // Update is called once per frame
    void Update()
    {
        vfx.SetVector3("Position", transform.position);
    }
}
