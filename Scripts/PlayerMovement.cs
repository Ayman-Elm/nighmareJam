using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; 
    // ^ You can expose this in the Inspector or override it with "playerStats.speed" if you have a separate Player script.

    private Rigidbody2D rb;
    private Vector2 moveInput;
    public Animator animator;
    
    // Reference to your "Player" script, if you need it
    private Player playerStats; 
    
    // SpriteRenderer for flipping horizontally
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // If you have a Player script that includes "speed":
        playerStats = GetComponent<Player>();
        
        // If your SpriteRenderer is on the same GameObject:
        sr = GetComponent<SpriteRenderer>();
        // If on a child, use "GetComponentInChildren<SpriteRenderer>()" instead.
    }

    void Update()
    {
        // 1) Gather horizontal/vertical input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        
        // 2) If you want to use speed from Player script:
        // moveSpeed = playerStats.speed;
        
        // 3) Normalize so diagonal movement isn’t faster
        moveInput = moveInput.normalized;
        
        // --- ANIMATION STATE LOGIC ---
        // If moving up
        if (moveInput.y > 0)
        {
            animator.SetBool("UpTrue", true);
            animator.SetBool("DownTrue", false);
            animator.SetBool("RunTrue", false);
        }
        // If moving down
        else if (moveInput.y < 0)
        {
            animator.SetBool("UpTrue", false);
            animator.SetBool("DownTrue", true);
            animator.SetBool("RunTrue", false);
        }
        // If moving horizontally
        else if (moveInput.x != 0)
        {
            animator.SetBool("UpTrue", false);
            animator.SetBool("DownTrue", false);
            animator.SetBool("RunTrue", true);

            // Flip sprite left or right
            if (moveInput.x > 0)
            {
                sr.flipX = false; // face right
            }
            else
            {
                sr.flipX = true; // face left
            }
        }
        // If not pressing anything (idle)
        else
        {
            // All booleans set to false → Goes to idle animation (default state)
            animator.SetBool("UpTrue", false);
            animator.SetBool("DownTrue", false);
            animator.SetBool("RunTrue", false);
        }
    }

    void FixedUpdate()
    {
        // Apply velocity here for physics consistency
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
