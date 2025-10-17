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

        playerAnim = GetComponent<Animator>();
        if (playerAnim != null)
            playerAnim.applyRootMotion = false;

        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        walkAudioSource.clip = walkSound;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
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
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void HandleMovementAnimations()
    {
        bool isMoving = movementInput.sqrMagnitude > 0.001f;

        if (playerAnim != null)
            playerAnim.SetBool("isRunning", isMoving);

        if (dirtParticle != null)
        {
            if (isMoving && !dirtParticle.isPlaying)
                dirtParticle.Play();
            else if (!isMoving && dirtParticle.isPlaying)
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
