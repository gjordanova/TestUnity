using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private AudioClip bounceSoundClip;
    [SerializeField] private float bounceRecoveryTime = 1f;
    [SerializeField] private float shrinkScale = 0.5f;
    [SerializeField] private float expandScale = 1.5f;
    [SerializeField] private float soundVolume = 1f;

    private Rigidbody rb;
    private Player player;
    private bool isRecovering;
    private Vector3 originalScale;
    private BattleRoyaleManager battleRoyaleManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        originalScale = transform.localScale;
        battleRoyaleManager = BattleRoyaleManager.Instance;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isRecovering) return;

        // Ignore collisions if player is eliminated or dead
        if (player != null && (player.isEliminated || player.isDead))
            return;

        // Check if the collided object is part of the "Brush" layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Brush"))
        {
            // Apply collision force
            if (rb != null)
            {
                Vector3 force = collision.contacts[0].normal * 10f;
                rb.AddForce(-force, ForceMode.Impulse);
            }

            // Play bounce sound during gameplay using AudioClip
            if (battleRoyaleManager.m_IsPlaying)
            {
                if (bounceSoundClip != null)
                {
                    AudioSource.PlayClipAtPoint(bounceSoundClip, transform.position, soundVolume);
                }
            }

            // Start the visual bounce effect
            StartCoroutine(BounceRecoveryRoutine());
        }
    }

    private System.Collections.IEnumerator BounceRecoveryRoutine()
    {
        isRecovering = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.localScale = originalScale * shrinkScale;
        yield return new WaitForSeconds(0.05f);

        transform.localScale = originalScale * expandScale;
        yield return new WaitForSeconds(0.05f);

        float elapsedTime = 0;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, originalScale, elapsedTime / 0.1f);
            yield return null;
        }

        transform.localScale = originalScale;
        isRecovering = false;
    }
}