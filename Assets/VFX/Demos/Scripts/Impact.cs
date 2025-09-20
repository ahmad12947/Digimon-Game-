using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Impact : MonoBehaviour
{
    public VisualEffect impact;

    public void Play()
    {
        impact.Play();
    }
}
