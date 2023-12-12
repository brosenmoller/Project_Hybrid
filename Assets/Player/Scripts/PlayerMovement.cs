using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField, Range(0, 1)] private float basicHorizontalDamping = 0.3f;
    [SerializeField, Range(0, 1)] private float horizontalDampingWhenStopping = 0.5f;
    [SerializeField, Range(0, 1)] private float horizontalDampingWhenTurning = 0.4f;

    [Header("Vertical Movement Settings")]
    [SerializeField] private float maxJumpVelocity = 18f;
    [SerializeField, Range(0, 1)] float jumpCutOff = 0.5f;
    [SerializeField] private float rigidBodyGravityScale = 4f;

    [Header("GroundDetection")]
    [SerializeField] private float jumpDelay = 0.15f;
    [SerializeField] private float groundDelay = 0.15f;
    [SerializeField] private Vector3 colliderWidth = new Vector3(0.55f, 0f, 0f);
    [SerializeField] private Vector3 colliderOffset;
    [SerializeField] private float groundDistance = 0.55f;
    [SerializeField] private LayerMask groundLayer;

    [Header("JumpSqueeze")]
    [SerializeField] private float xSqueeze = 1.2f;
    [SerializeField] private float ySqueeze = 0.8f;
    [SerializeField] private float squeezeDuration = 0.1f;

    [Header("References")]
    [SerializeField] private Transform spriteHolder;

    private bool isGrounded;
    private bool wasGrounded;

    private float groundTimer;
    private float jumpTimer;
    private float currentJumpVelocity;

    private float movementX;

    private Rigidbody2D rb;
    private InputService inputService;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = rigidBodyGravityScale;
        currentJumpVelocity = maxJumpVelocity;

        isGrounded = GroundCheck();
    }

    private void Start()
    {
        inputService = ServiceLocator.Instance.Get<InputService>();

        inputService.playerInputActions.Gameplay.HorizontalMovement.performed += UpdateMovementDirection;
        inputService.playerInputActions.Gameplay.HorizontalMovement.canceled += ResetMovementDirection;
        inputService.playerInputActions.Gameplay.Jump.started += InitiateJump;
        inputService.playerInputActions.Gameplay.Jump.canceled += CutJumpVelocity;
    }

    private void OnDisable()
    {
        inputService.playerInputActions.Gameplay.HorizontalMovement.performed -= UpdateMovementDirection;
        inputService.playerInputActions.Gameplay.HorizontalMovement.canceled -= ResetMovementDirection;
        inputService.playerInputActions.Gameplay.Jump.started -= InitiateJump;
        inputService.playerInputActions.Gameplay.Jump.canceled -= CutJumpVelocity;
    }

    private void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = GroundCheck();

        // take off (coyote time)
        if (wasGrounded && !isGrounded)
        {
            groundTimer = Time.time + groundDelay;
            wasGrounded = false;
        }

        // landing
        if (!wasGrounded && isGrounded)
        {
            StartCoroutine(JumpSqueeze(xSqueeze, ySqueeze, squeezeDuration));

            if (groundTimer < Time.time - groundDelay)
            {
                //Posible Land animation
            }
        }
    }

    private void InitiateJump(InputAction.CallbackContext callbackContext)
    {
        jumpTimer = Time.time + jumpDelay;
    }

    private void CutJumpVelocity(InputAction.CallbackContext callbackContext)
    {
        if (jumpTimer > 0) // Release Jumpbutton before touching the ground
        {
            currentJumpVelocity = maxJumpVelocity * (jumpCutOff + 0.1f);
        }
        else if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutOff);
        }
    }

    public void UpdateMovementDirection(InputAction.CallbackContext callbackContext)
    {
        SetMovementDirection((int)callbackContext.ReadValue<float>());
    }

    public void ResetMovementDirection(InputAction.CallbackContext callbackContext)
    {
        SetMovementDirection(0);
    }

    private void SetMovementDirection(int newMovementX)
    {
        movementX = newMovementX;
        FlipSprite(movementX);
    }

    private void FlipSprite(float movementX)
    {
        if (movementX == 1)
        {
            spriteHolder.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (movementX == -1)
        {
            spriteHolder.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    private void FixedUpdate()
    {
        HorizontalMovement();
        if (jumpTimer > Time.time && (groundTimer > Time.time || isGrounded))
        {
            Jump(currentJumpVelocity);
        }
    }

    private void HorizontalMovement()
    {
        float horizontalVelocity = rb.velocity.x;
        horizontalVelocity += movementX;

        if (Mathf.Abs(movementX) < 0.01f)
        {
            horizontalVelocity *= Mathf.Pow(1f - horizontalDampingWhenStopping, Time.deltaTime * 10f);
        }
        else if (Mathf.Sign(movementX) != Mathf.Sign(horizontalVelocity))
        {
            horizontalVelocity *= Mathf.Pow(1f - horizontalDampingWhenTurning, Time.deltaTime * 10f);
        }
        else
        {
            horizontalVelocity *= Mathf.Pow(1f - basicHorizontalDamping, Time.deltaTime * 10f);
        }

        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
    }
    private void Jump(float jumpVelocity)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);

        jumpTimer = 0;
        groundTimer = 0;
        currentJumpVelocity = maxJumpVelocity;
        StartCoroutine(JumpSqueeze(ySqueeze, xSqueeze, squeezeDuration));
    }

    public bool GroundCheck()
    {
        if (Physics2D.Raycast((transform.position + colliderWidth) + colliderOffset, Vector2.down, groundDistance, groundLayer) ||
            Physics2D.Raycast((transform.position - colliderWidth) + colliderOffset, Vector2.down, groundDistance, groundLayer)) return true;
        else return false;
    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new(xSqueeze, ySqueeze, originalSize.z);

        Vector3 oldPos = Vector3.zero;
        Vector3 newPos = new(0, -0.1f, oldPos.z);

        float time = 0f;
        while (time <= 1.0)
        {
            time += Time.deltaTime / seconds;
            spriteHolder.localScale = Vector3.Lerp(originalSize, newSize, time);
            spriteHolder.localPosition = Vector3.Lerp(oldPos, newPos, time);
            yield return null;
        }
        time = 0f;
        while (time <= 1.0)
        {
            time += Time.deltaTime / seconds;
            spriteHolder.localScale = Vector3.Lerp(newSize, originalSize, time);
            spriteHolder.localPosition = Vector3.Lerp(newPos, oldPos, time);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay((transform.position + colliderWidth) + colliderOffset, Vector2.down * groundDistance);
        Gizmos.DrawRay((transform.position - colliderWidth) + colliderOffset, Vector2.down * groundDistance);
    }
}
