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

    [Header("Game Mechanics")]
    private Collider currentTarget;
    public GameManager gameManager;

    void Start()
    {
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.volume = 0.3f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SHOO!");

            if (shooSound != null)
                sfxAudioSource.PlayOneShot(shooSound, 0.3f);

            if (currentTarget != null)
            {
                RabbitMover rabbit = currentTarget.GetComponent<RabbitMover>();
                if (rabbit != null && rabbit.gameManager != null)
                {
                    gameManager.UpdateScore(rabbit.scoreValue);
                    sfxAudioSource.PlayOneShot(popSound);
                    shooParticle.Play();
                    Debug.Log("Rabbit Scored: " + rabbit.scoreValue);
                }

                Destroy(currentTarget.gameObject);
                currentTarget = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rabbit"))
            currentTarget = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentTarget == other)
            currentTarget = null;
    }
}
