using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    private AudioSource sfxAudioSource;    

    [Header("Particle Effects")]
    public ParticleSystem cashParticle;
    public ParticleSystem shooParticle;

    [Header("Audio")]
    public AudioClip shooSound;
    public AudioClip cashSound;
    public AudioClip popSound;
    
    // Rabbit currently being touched
    private Collider currentTarget; 
    public GameManager gameManager;

    void Start()
    {
        // Setup audio source for sound effects
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.volume = 0.3f;
    }

    void Update()
    {
        // --- SHOO action ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SHOO!");

            // Play shoo sound ALWAYS
            if (shooSound != null)
                sfxAudioSource.PlayOneShot(shooSound, 0.3f);

            // Destroy rabbit ONLY if colliding
            if (currentTarget != null)
            {
                RabbitMover rabbit = currentTarget.GetComponent<RabbitMover>();
                if (rabbit != null && rabbit.gameManager != null)
                {
                    // Update score and play effects
                    gameManager.UpdateScore(rabbit.scoreValue);
                    sfxAudioSource.PlayOneShot(popSound);
                    shooParticle.Play();
                    Debug.Log("Rabbit Scored: " + rabbit.scoreValue);
                }

                Destroy(currentTarget.gameObject);
                currentTarget = null;
            }
        }

        // // --- CASH action --- (Not used anymore)
        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     Debug.Log("CASH!");

        //     // Play cash sound
        //     if (cashSound != null)
        //         sfxAudioSource.PlayOneShot(cashSound, 1.0f);

        //     // Show cash particle
        //     if (cashParticle != null)
        //         cashParticle.Play();
        // }
        
    }

    // Track rabbit when entering trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rabbit"))
        {
            currentTarget = other;
        }
    }

    // Clear target when exiting trigger
    private void OnTriggerExit(Collider other)
    {
        if (currentTarget == other)
        {
            currentTarget = null;
        }
    }
}
