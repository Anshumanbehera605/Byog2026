using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f;

    [Header("Pop Up")]
    public float undergroundDepth = 2f;
    public float popUpSpeed = 4f;

    private Transform player;
    private bool poppingUp = true;
    private float groundY;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

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

        // Then: chase the player
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            transform.position +=
                direction * moveSpeed * Time.deltaTime;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }
    }
}