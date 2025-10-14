using UnityEngine;

public class Destroy : MonoBehaviour
{
    private AudioSource sfxAudioSource;    

    [Header("Particle Effects")]
    public ParticleSystem cashParticle;
    public ParticleSystem shooParticle;

    [Header("Audio")]
    public AudioClip shooSound;
    public AudioClip cashSound;

    private Collider currentTarget; // rabbit currently being touched
    public GameManager gameManager;

    void Start()
    {
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.volume = 0.3f;
    }

    void Update()
    {
        // --- SHOO action ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SHOO!");

            // Play sound and particles ALWAYS
            if (shooSound != null)
                sfxAudioSource.PlayOneShot(shooSound, 0.3f);

            if (shooParticle != null)
                shooParticle.Play();

            // Destroy rabbit ONLY if colliding
            if (currentTarget != null)
            {
                RabbitMover rabbit = currentTarget.GetComponent<RabbitMover>();
                if (rabbit != null && rabbit.gameManager != null)
                {
                    gameManager.UpdateScore(rabbit.scoreValue);
                    Debug.Log("Rabbit Scored." + rabbit.scoreValue);
                }   

                Destroy(currentTarget.gameObject);
                currentTarget = null;
            }
        }

        // --- CASH action ---
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("CASH!");

            if (cashSound != null)
                sfxAudioSource.PlayOneShot(cashSound, 1.0f);

            if (cashParticle != null)
                cashParticle.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rabbit"))
        {
            currentTarget = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentTarget == other)
        {
            currentTarget = null;
        }
    }
}
