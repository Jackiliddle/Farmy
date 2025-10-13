using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 5.0f;

    [Header("Audio")]
    public AudioClip walkSound;
    //public AudioClip shooSound;
    //public AudioClip cashSound;

    private AudioSource walkAudioSource;   
    //private AudioSource sfxAudioSource;    

    [Header("Player Animator")]
    public Animator playerAnim;

    [Header("Particle Effects")]
    public ParticleSystem dirtParticle;
    //public ParticleSystem cashParticle;
    //public ParticleSystem shooParticle;

    private Vector3 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();

        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        walkAudioSource.clip = walkSound;

    }

    void Update()
    {
        // Movement input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        movementInput = new Vector3(horizontalInput, 0, verticalInput);

        HandleMovementAnimations();
        HandleWalkSound();
    }

    void FixedUpdate()
    {
        if (movementInput.sqrMagnitude > 0.001f)
        {
            Vector3 move = movementInput.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
    }

    private void HandleMovementAnimations()
    {
        bool isMoving = movementInput.sqrMagnitude > 0.001f;
        playerAnim.SetBool("isRunning", isMoving);

        if (isMoving)
        {
            if (dirtParticle != null && !dirtParticle.isPlaying)
                dirtParticle.Play();
        }
        else
        {
            if (dirtParticle != null && dirtParticle.isPlaying)
                dirtParticle.Stop();
        }
    }

    private void HandleWalkSound()
    {
        bool isMoving = movementInput.sqrMagnitude > 0.001f;

        if (isMoving)
        {
            if (!walkAudioSource.isPlaying)
                walkAudioSource.Play();
        }
        else
        {
            if (walkAudioSource.isPlaying)
                walkAudioSource.Stop();
        }
    }
}
