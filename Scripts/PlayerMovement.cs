using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 0f; // You can override this from Player.cs if needed

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Player playerStats; // Reference to Player.cs script

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<Player>();
    }

    void Update()
    {
        // Input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Optionally use speed from Player.cs if you want
        moveSpeed = playerStats.speed;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
