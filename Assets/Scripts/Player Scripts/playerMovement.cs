using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    public float player_move_speed = 5.0f;
    public float player_rotation_speed = 360.0f;

    [Header("Jump Settings")]
    public float min_jump_force = 5.0f;
    public float max_jump_force = 8.0f;
    public float max_charge_time = 1.0f; // Maximum time in seconds to reach full charge

    [Header("UI Settings")]
    public Slider jumpChargeSlider; // Drag your UI Slider here in the Inspector

    private Rigidbody rb;
    private Vector3 movement;
    private bool jumpRequested;
    private bool isCharging;
    private float currentChargeTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Automatically hide the slider at the start of the game
        if (jumpChargeSlider != null)
        {
            jumpChargeSlider.gameObject.SetActive(false);
            jumpChargeSlider.minValue = 0f;
            jumpChargeSlider.maxValue = max_charge_time;
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontalInput, 0, verticalInput);
        input = Vector3.ClampMagnitude(input, 1.0f);

        Transform cameraTransform = UnityEngine.Camera.main != null
            ? UnityEngine.Camera.main.transform
            : null;
        Vector3 forward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 right = cameraTransform != null ? cameraTransform.right : Vector3.right;
        forward.y = 0.0f;
        right.y = 0.0f;
        forward.Normalize();
        right.Normalize();

        movement = (right * input.x) + (forward * input.z);

        bool isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        // Start charging when space is pressed down on the ground
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            currentChargeTime = 0f;

            if (jumpChargeSlider != null)
            {
                jumpChargeSlider.gameObject.SetActive(true);
                jumpChargeSlider.value = 0f;
            }
        }

        // While holding space, accumulate charge time and update the UI slider
        if (isCharging && Input.GetKey(KeyCode.Space))
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Min(currentChargeTime, max_charge_time);

            if (jumpChargeSlider != null)
            {
                jumpChargeSlider.value = currentChargeTime;
            }
        }

        // When space is released, trigger the jump and hide the UI
        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            jumpRequested = true;
            isCharging = false;

            if (jumpChargeSlider != null)
            {
                jumpChargeSlider.gameObject.SetActive(false);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            player_move_speed = 10.0f;
        }
        else
        {
            player_move_speed = 5.0f;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * player_move_speed * Time.fixedDeltaTime);

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            Quaternion rotation = Quaternion.RotateTowards(
                rb.rotation,
                targetRotation,
                player_rotation_speed * Time.fixedDeltaTime
            );
            rb.MoveRotation(rotation);
        }

        if (jumpRequested)
        {
            // Calculate charge ratio (0 to 1) and interpolate between min and max force
            float chargeRatio = currentChargeTime / max_charge_time;
            float finalJumpForce = Mathf.Lerp(min_jump_force, max_jump_force, chargeRatio);

            rb.AddForce(Vector3.up * finalJumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
    }
}