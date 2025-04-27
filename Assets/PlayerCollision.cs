using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCollision : MonoBehaviour
{
    public AudioClip collisionSound;
    private AudioSource audioSource;
    private FeatureController featureController;

    public float bounceForce = 5f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        featureController = FindObjectOfType<FeatureController>();
        if (featureController == null)
        {
            Debug.LogError("FeatureController not found in the scene!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (featureController == null || !featureController.IsCollisionEnabled())
            return; // Feature disabled

        // Проверка дали објектот со кој се судира е на слојот "Brush"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Brush"))
        {
            // Логирање на информациите за колизијата
            Debug.Log($"Collision detected with: {collision.gameObject.name} at {collision.contacts[0].point}");

            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
            Rigidbody myRb = GetComponent<Rigidbody>();

            if (otherRb != null && myRb != null)
            {
                Vector3 direction = (collision.transform.position - transform.position).normalized;
                direction.y = 0f; // Само хоризонтално движење

                myRb.AddForce(-direction * bounceForce, ForceMode.Impulse);
                otherRb.AddForce(direction * bounceForce, ForceMode.Impulse);
            }

            if (collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
        }
    }
}