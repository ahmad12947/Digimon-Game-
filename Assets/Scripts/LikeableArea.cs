using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikeableArea : MonoBehaviour
{
    [Tooltip("List of Digimon GameObject names that like this area.")]
    public List<string> likedDigimonNames;

    [Tooltip("Tiredness reduction interval in seconds (3600 = 1 in-game hour)")]
    public float tirednessInterval = 3600f;

    [Tooltip("Tiredness reduced per interval")]
    public int tirednessReduction = 2;

    private Dictionary<DigimonMoodManager, float> timers = new Dictionary<DigimonMoodManager, float>();

    private void OnTriggerStay(Collider other)
    {
        DigimonMoodManager mood = other.GetComponent<DigimonMoodManager>();
        if (mood == null) return;

        string digimonName = other.gameObject.name;

        if (!likedDigimonNames.Contains(digimonName)) return;

        if (!timers.ContainsKey(mood))
            timers[mood] = 0f;

        timers[mood] += Time.deltaTime;

        if (timers[mood] >= tirednessInterval)
        {
            mood.changeTiredness(-tirednessReduction);
            timers[mood] = 0f;

            Debug.Log($"{digimonName} likes this area. Lost {tirednessReduction} tiredness.");
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
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Semi-transparent green

        Collider col = GetComponent<Collider>();
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
#endif
}
