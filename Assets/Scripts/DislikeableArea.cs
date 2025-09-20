using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DislikeableArea : MonoBehaviour
{
    [Tooltip("List of Digimon GameObject names that dislike this area.")]
    public List<string> dislikedDigimonNames;

    [Tooltip("Tiredness increase interval in seconds (3600 = 1 in-game hour)")]
    public float tirednessInterval = 3600f;

    [Tooltip("Tiredness gained per interval")]
    public int tirednessGain = 1;

    // Track timers for Digimon inside this area
    private Dictionary<DigimonMoodManager, float> timers = new Dictionary<DigimonMoodManager, float>();

    private void OnTriggerStay(Collider other)
    {
        DigimonMoodManager mood = other.GetComponent<DigimonMoodManager>();
        if (mood == null) return;

        string digimonName = other.gameObject.name;

        if (!dislikedDigimonNames.Contains(digimonName)) return;

        if (!timers.ContainsKey(mood))
            timers[mood] = 0f;

        timers[mood] += Time.deltaTime;

        if (timers[mood] >= tirednessInterval)
        {
            mood.changeTiredness(tirednessGain);
            timers[mood] = 0f;

            Debug.Log($"{digimonName} dislikes this area. Gained {tirednessGain} tiredness.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DigimonMoodManager mood = other.GetComponent<DigimonMoodManager>();
        if (mood != null && timers.ContainsKey(mood))
        {
            timers.Remove(mood);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Semi-transparent red

        Collider col = GetComponent<Collider>();
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
#endif
}
