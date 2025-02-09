using UnityEngine;

public class PhysicsPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float jumpForce = 5f;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.3f;
    
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Configure Rigidbody for character control
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tipping over
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smoother movement
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Create movement direction
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Check if grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        
        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    void FixedUpdate()
    {
        // Apply movement in FixedUpdate for physics consistency
        if (moveDirection != Vector3.zero)
        {
            // Calculate target velocity
            Vector3 targetVelocity = moveDirection * moveSpeed;
            targetVelocity.y = rb.linearVelocity.y; // Preserve current vertical velocity
            
            // Apply movement force
            Vector3 velocityChange = targetVelocity - rb.linearVelocity;
            velocityChange.y = 0f; // Don't affect vertical velocity
            
            // Apply the force
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        
        // Limit horizontal velocity
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxVelocity)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize ground check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
