using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float runSpeed = 5f;
    public float jumpForce = 7f;
    public Transform cameraTransform;

    public LayerMask groundLayer; 
    public float groundCheckDistance = 1.1f; 
    public Vector3 groundCheckOffset = new Vector3(0, -0.5f, 0);

    private Rigidbody rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Vector3.forward;
        Vector3 camRight = Vector3.right;
        if (cameraTransform != null)
        {
            camForward = cameraTransform.forward;
            camRight = cameraTransform.right;
        }
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camForward * z + camRight * x).normalized;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }

        transform.Translate(move * currentSpeed * Time.deltaTime, Space.World);

       
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position + groundCheckOffset, Vector3.down, out hit, groundCheckDistance, groundLayer);

        // Для отладки: рисует луч в редакторе
        Debug.DrawRay(transform.position + groundCheckOffset, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);




        //
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;
        }
    }
    

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }
}

