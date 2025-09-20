using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXSourceScaler : MonoBehaviour
{
    [SerializeField]
    private VisualEffect vfx;

    private void Update()
    {
        SetScale(transform.lossyScale);
    }

    public void SetScale(Vector3 scale)
    {
        vfx.SetVector3("SourceScale", scale);
    }
}
