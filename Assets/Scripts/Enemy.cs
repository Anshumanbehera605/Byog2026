using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Pop Up")]
    public float undergroundDepth = 2f;
    public float popUpSpeed = 4f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private Transform player;
    private Animator animator;

    private bool poppingUp = true;
    private float groundY;

    // Knockback variables
    private bool isBeingKnockedBack = false;
    private Vector3 knockbackDirection;
    private float knockbackTimer;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        animator = GetComponent<Animator>();

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Remember the normal ground position
        groundY = transform.position.y;

        // Start underground
        transform.position = new Vector3(
            transform.position.x,
            groundY - undergroundDepth,
            transform.position.z
        );
    }

    void Update()
    {
        // First: pop out of the ground
        if (poppingUp)
        {
            Vector3 pos = transform.position;

            pos.y = Mathf.MoveTowards(
                pos.y,
                groundY,
                popUpSpeed * Time.deltaTime
            );

            transform.position = pos;

            if (pos.y == groundY)
            {
                poppingUp = false;
            }

            return;
        }

        // Handle knockback
        if (isBeingKnockedBack)
        {
            transform.position +=
                knockbackDirection *
                knockbackForce *
                Time.deltaTime;

            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isBeingKnockedBack = false;
            }

            return;
        }

        // Then: chase the player
        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 1) animator.SetTrigger("attack");

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            transform.position +=
                direction *
                moveSpeed *
                Time.deltaTime;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }
    }

    public void ApplyKnockback(Vector3 direction)
    {
        knockbackDirection = direction.normalized;

        knockbackTimer = knockbackDuration;

        isBeingKnockedBack = true;

        // Rotate enemy to face the direction it's being pushed
        Quaternion targetRotation =
            Quaternion.LookRotation(knockbackDirection);

        transform.rotation = targetRotation;
    }
}
