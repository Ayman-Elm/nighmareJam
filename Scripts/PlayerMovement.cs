using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 0f; // Could use your Player.cs speed if desired
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public Animator animator;
    private Player playerStats; // Reference to Player.cs script
    
    // Add this to flip only the sprite, not the child's transform
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<Player>();

        // Get the SpriteRenderer from the same GameObject or from a child
        sr = GetComponent<SpriteRenderer>();
        // If your sprite renderer is on a child, do:
        // sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // Gather input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Optionally use speed from Player.cs
        moveSpeed = playerStats.speed;

        // Normalize so diagonal speed is not faster
        moveInput = moveInput.normalized;

        // --- ANIMATION STATE LOGIC ---
        // If moving vertically, check up or down
        if (moveInput.y > 0)
        {
            // Going up
            animator.SetBool("UpTrue", true);
            animator.SetBool("DownTrue", false);
            animator.SetBool("RunTrue", false);
        }
        else if (moveInput.y < 0)
        {
            // Going down
            animator.SetBool("UpTrue", false);
            animator.SetBool("DownTrue", true);
            animator.SetBool("RunTrue", false);
        }
        // If moving horizontally, set “RunTrue” and flip based on direction
        else if (moveInput.x != 0)
        {
            animator.SetBool("UpTrue", false);
            animator.SetBool("DownTrue", false);
            animator.SetBool("RunTrue", true);

            // Use flipX to face left or right
            if (moveInput.x > 0)
            {
                // Face right
                sr.flipX = false;
            }
            else
            {
                // Face left
                sr.flipX = true;
            }
        }
        else
        {
            // No input, so idle. Turn all directional bools off.
            animator.SetBool("UpTrue", false);
            animator.SetBool("DownTrue", false);
            animator.SetBool("RunTrue", false);
        }
    }

    void FixedUpdate()
    {
        // Apply velocity in FixedUpdate for physics
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
