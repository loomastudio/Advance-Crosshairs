using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [Header("Settings")]
    public float sensitivity = 150f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private float pitch;
    private float yaw;

    [Header("Debug")]
    public float debugRayLength = 100f;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        // Camera Look
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * sensitivity * Time.deltaTime;
        pitch -= mouseDelta.y * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Shoot Ray
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, debugRayLength))
        {
            // Enemy detected
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);

                // Left Mouse Shoot
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();

                    if (enemy != null)
                    {
                        enemy.TakeDamage(25);
                    }
                }
            }
            else
            {
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * debugRayLength, Color.green);
        }
    }
}