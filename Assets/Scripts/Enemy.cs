using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float separationWeight = 1.5f;
    public LayerMask enemyLayer;

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

    public float attackRate = 0f;
    float attackTimer = 0;

    // Death state
    public bool isDead = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        groundY = transform.position.y;

        transform.position = new Vector3(
            transform.position.x,
            groundY - undergroundDepth,
            transform.position.z
        );
    }

    void Update()
    {
        if (isDead) return;

        if (poppingUp)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(pos.y, groundY, popUpSpeed * Time.deltaTime);
            transform.position = pos;

            if (pos.y == groundY)
            {
                poppingUp = false;
            }
            return;
        }

        attackTimer += Time.deltaTime;

        if (isBeingKnockedBack)
        {
            transform.position += knockbackDirection * knockbackForce * Time.deltaTime;
            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isBeingKnockedBack = false;
            }
            return;
        }

        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;

        if (directionToPlayer.sqrMagnitude < 20 && !isDead && attackTimer > attackRate) {
            animator.SetTrigger("Attack");
            attackTimer = 0f; // Reset the attack timer
        }

        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = directionToPlayer.normalized;

            // Anti-clumping separation logic
            Vector3 separation = Vector3.zero;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, separationRadius, enemyLayer);
            
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject != gameObject)
                {
                    Vector3 pushAway = transform.position - hitCollider.transform.position;
                    pushAway.y = 0f;
                    separation += pushAway.normalized / Mathf.Max(pushAway.magnitude, 0.1f);
                }
            }

            moveDirection += separation * separationWeight;
            moveDirection.Normalize();

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    public void ApplyKnockback(Vector3 direction)
    {
        if (isDead) return;

        knockbackDirection = direction.normalized;
        knockbackTimer = knockbackDuration;
        isBeingKnockedBack = true;

        Quaternion targetRotation = Quaternion.LookRotation(knockbackDirection);
        transform.rotation = targetRotation;
    }
}