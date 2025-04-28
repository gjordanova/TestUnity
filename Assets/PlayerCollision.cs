using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCollision : MonoBehaviour
{
    public AudioClip collisionSound;
    private AudioSource audioSource;
    private FeatureController featureController;
    private Rigidbody rb;
    private Player player;

    public float bounceForce = 5f;
    private bool isCollisionEnabled = true;

    private void Start()
    {
        // Cache components
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        featureController = FindObjectOfType<FeatureController>();
        if (featureController == null)
        {
            Debug.LogError("FeatureController not found in the scene!");
        }

        // Setup rigidbody for better collision handling
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isCollisionEnabled || featureController == null || !featureController.IsCollisionEnabled())
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Brush"))
        {
            HandleBrushCollision(collision);
        }
    }

    private void HandleBrushCollision(Collision collision)
    {
        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
        
        if (otherRb != null && rb != null)
        {
            // Calculate collision direction
            Vector3 direction = (collision.transform.position - transform.position).normalized;
            direction.y = 0f; // Keep movement horizontal

            // Apply bounce forces
            rb.AddForce(-direction * bounceForce, ForceMode.Impulse);
            otherRb.AddForce(direction * bounceForce, ForceMode.Impulse);

            // Temporarily stop drawing while bouncing
            if (player != null)
            {
                StartCoroutine(BounceRecoveryRoutine());
            }

            // Play sound effect
            if (collisionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
        }
    }

    private System.Collections.IEnumerator BounceRecoveryRoutine()
    {
        // Stop drawing
        foreach (DrawLine drawLine in player.GetComponents<DrawLine>())
        {
            drawLine.StopDraw();
        }

        // Wait a short moment for the bounce to settle
        yield return new WaitForSeconds(0.1f);

        // Resume drawing
        foreach (DrawLine drawLine in player.GetComponents<DrawLine>())
        {
            drawLine.StartDraw();
        }
    }

    public void SetCollisionEnabled(bool enabled)
    {
        isCollisionEnabled = enabled;
    }
}