using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float floatSpeed = 2f;
    public float lifetime = 1.5f;

    private Vector3 floatDirection = Vector3.up;

    public void Initialize(int damage)
    {
        damageText.text = damage.ToString();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); // Face the camera
    }
}
