using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;

    [Header("Movement")]
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Gravity")]
    public float gravity = -20f;

    [Header("Ground Detection")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.15f;
    public Animator animator;

    private CharacterController controller;

    private float verticalVelocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckGround();

        // ------------------------------------------------
        // Horizontal movement
        // ------------------------------------------------

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        Vector3 input = new(hor, 0f, ver);

        Vector3 camFwd = cameraTransform.forward;
        Vector3 camRgt = cameraTransform.right;

        camFwd.y = 0f;
        camRgt.y = 0f;

        camFwd.Normalize();
        camRgt.Normalize();

        Vector3 movement =
            camFwd * input.z +
            camRgt * input.x;

        // ------------------------------------------------
        // Rotation
        // ------------------------------------------------

        if (movement.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation =
                Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        animator.SetFloat("speed", movement.sqrMagnitude);

        // ------------------------------------------------
        // Gravity
        // ------------------------------------------------

        if (isGrounded && verticalVelocity < 0f)
        {
            // Small downward force keeps the player
            // connected to the ground.
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        // Add gravity to movement
        Vector3 finalMovement = movement * movementSpeed;
        finalMovement.y = verticalVelocity;

        controller.Move(finalMovement * Time.deltaTime);
    }

    void CheckGround()
    {
        // Bottom-center of the CharacterController
        Vector3 origin =
            transform.position +
            controller.center +
            Vector3.down *
            (controller.height / 2f - controller.radius);

        float radius = controller.radius * 0.9f;

        isGrounded = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (controller == null)
            return;

        Vector3 origin =
            transform.position +
            controller.center +
            Vector3.down *
            (controller.height / 2f - controller.radius);

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            origin,
            controller.radius * 0.9f
        );

        Gizmos.DrawWireSphere(
            origin + Vector3.down * groundCheckDistance,
            controller.radius * 0.9f
        );
    }
}
