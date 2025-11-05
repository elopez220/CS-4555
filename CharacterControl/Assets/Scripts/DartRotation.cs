// DartRotation.cs - Makes darts point in direction of travel
using UnityEngine;

public class DartRotation : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            // Make dart point in direction of movement
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }
}