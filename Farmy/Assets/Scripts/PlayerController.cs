using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 5.0f;

    [Header("Audio")]
    public AudioClip walkSound;
    private AudioSource walkAudioSource;   

    [Header("Player Animator")]
    public Animator playerAnim; 

    [Header("Particle Effects")]
    public ParticleSystem dirtParticle;

    private Vector3 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Get animator 
        playerAnim = GetComponent<Animator>();
        if (playerAnim != null)
        
            // don't let animation move player
            playerAnim.applyRootMotion = false; 

        // Setup audio source for walking
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        walkAudioSource.clip = walkSound;
    }

    void Update()
    {
        // Get player input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(horizontalInput, 0, verticalInput);

        // Play animations and sounds
        HandleMovementAnimations();
        HandleWalkSound();
    }

    void FixedUpdate()
    {
        // Move player based on input
        if (movementInput.sqrMagnitude > 0.001f)
        {
            Vector3 move = movementInput.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
        // Freeze player to prevent drifting when not moving
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    // Handle player running animations and dirt particle
    private void HandleMovementAnimations()
    {
        bool isMoving = movementInput.sqrMagnitude > 0.001f;

        // Play running animation if animator exists
        if (playerAnim != null)
            playerAnim.SetBool("isRunning", isMoving);

        // Play/stop dirt particles while moving
        if (dirtParticle != null)
        {
            if (isMoving && !dirtParticle.isPlaying)
                dirtParticle.Play();
            else if (!isMoving && dirtParticle.isPlaying)
                dirtParticle.Stop();
        }
    }

    // Play or stop walking sound based on movement
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
