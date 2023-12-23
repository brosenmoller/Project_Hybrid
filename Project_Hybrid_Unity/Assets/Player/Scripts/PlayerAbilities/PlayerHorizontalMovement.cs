using UnityEngine;

public class PlayerHorizontalMovement : PlayerAbility
{
    [Header("Horizontal Movement Settings")]
    [SerializeField, Range(0, 1)] private float basicHorizontalDamping = 0.3f;
    [SerializeField, Range(0, 1)] private float horizontalDampingWhenStopping = 0.5f;
    [SerializeField, Range(0, 1)] private float horizontalDampingWhenTurning = 0.4f;

    private float movementX;

    private void Update()
    {
        SetMovementDirection(InputService.HorizontalAxis);
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
            SpriteHolder.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (movementX == -1)
        {
            SpriteHolder.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    private void FixedUpdate()
    {
        if (!Controller.CanMove) { return; }

        HorizontalMovement();
    }

    private void HorizontalMovement()
    {
        float horizontalVelocity = RigidBody.velocity.x;
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

        RigidBody.velocity = new Vector2(horizontalVelocity, RigidBody.velocity.y);
    }
}

