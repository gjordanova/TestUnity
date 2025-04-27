using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCollision : MonoBehaviour
{
    public AudioClip collisionSound; // Drag your bounce sound here
    private AudioSource audioSource;
    private FeatureController featureController;

    private void Start()
    {
        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Find FeatureController
        featureController = FindObjectOfType<FeatureController>();
        if (featureController == null)
        {
            Debug.LogError("FeatureController not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (featureController == null || !featureController.IsCollisionEnabled())
            return; // Collision feature disabled

        if (other.CompareTag("Player"))
        {
            Vector3 bounceDirection = (transform.position - other.transform.position).normalized;
            bounceDirection.y = 0f; // Only horizontal movement

            transform.position += bounceDirection * 0.5f; // Small push back

            // Play collision sound
            if (collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
        }
    }
}