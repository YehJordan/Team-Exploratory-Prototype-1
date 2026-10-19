using UnityEngine;

public class KillingFloor : MonoBehaviour
{
    [SerializeField] private Vector3 respawnPosition = new Vector3(0f, 1f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        other.transform.position = respawnPosition;

        if (cc != null) cc.enabled = true;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}