using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public float movementSpeed;
    public float rotationSpeed;

    CharacterController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 input = new(hor, 0, ver);
        Vector3 camFwd = cameraTransform.forward;
        Vector3 camRgt = cameraTransform.right;
        camFwd.y = 0; camRgt.y = 0;
        camFwd.Normalize(); camRgt.Normalize();
        Vector3 movement = camFwd * input.z + camRgt * input.x;
        controller.Move(movement * movementSpeed * Time.deltaTime);

        if (movement.sqrMagnitude > 0.001f) {
            Quaternion lookRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
