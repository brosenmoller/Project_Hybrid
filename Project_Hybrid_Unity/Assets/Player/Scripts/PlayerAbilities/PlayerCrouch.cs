using System.Collections;
using UnityEngine;

public class PlayerCrouch : PlayerAbility
{
    [Header("Crouch Settings")]
    [SerializeField] private Vector2 colliderOffset = new(0, -0.25f);
    [SerializeField] private Vector2 colliderSize = new(1, 0.5f);
    [SerializeField] private LayerMask groundMask;

    private Vector2 colliderDefaultOffset;
    private Vector2 colliderDefaultSize;

    private BoxCollider2D boxCollider;

    private Coroutine cancelCrouchRoutine;

    private bool crouchCancelled = true;

    protected override void Initialize()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        colliderDefaultOffset = boxCollider.offset;
        colliderDefaultSize = boxCollider.size;

        InputService.CrouchStarted += StartCrouch;
        InputService.CrouchCancelled += CancelCrouch;
    }

    private void OnDisable()
    {
        InputService.CrouchStarted -= StartCrouch;
        InputService.CrouchCancelled -= CancelCrouch;
    }

    private void StartCrouch()
    {
        if (!crouchCancelled) { return; }

        StartCoroutine(Squeeze(new Vector3(1.5f, 0.5f, 1f), new Vector3(1, 1, 1), Vector3.zero, new Vector3(0, -0.25f, 0), .2f));
        boxCollider.size = colliderSize;
        boxCollider.offset = colliderOffset;
        Controller.CanJump = false;
        crouchCancelled = false;

        if (cancelCrouchRoutine != null) { StopCoroutine(cancelCrouchRoutine); }
    }

    private void CancelCrouch()
    {
        cancelCrouchRoutine = StartCoroutine(CancelCrouchRoutine());
    }

    private IEnumerator CancelCrouchRoutine()
    {
        Collider2D collider; 

        do
        {
            collider = Physics2D.OverlapBox(
                (Vector2)transform.position + colliderDefaultOffset + new Vector2(0, 0.05f),
                new Vector2(transform.localScale.x * colliderDefaultSize.x, transform.localScale.y * colliderDefaultSize.y),
                0,
                groundMask
            );

            yield return new WaitForSeconds(0.05f);
        }
        while (collider != null);


        StartCoroutine(Squeeze(new Vector3(1f, 1f, 1f), new Vector3(1.5f, 0.5f, 1), new Vector3(0, -0.25f, 0), Vector3.zero, .2f));
        boxCollider.size = colliderDefaultSize;
        boxCollider.offset = colliderDefaultOffset;
        Controller.CanJump = true;
        crouchCancelled = true;
    }

    private IEnumerator Squeeze(Vector3 newSize, Vector3 oldSize, Vector3 oldPos, Vector3 newPos, float seconds)
    {
        float time = 0f;
        while (time <= 1.0)
        {
            time += Time.deltaTime / seconds;
            SpriteHolder.localScale = Vector3.Lerp(oldSize, newSize, time);
            SpriteHolder.localPosition = Vector3.Lerp(oldPos, newPos, time);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            (Vector2)transform.position + colliderOffset,
            new Vector2(transform.localScale.x * colliderSize.x, transform.localScale.y * colliderSize.y)
        );
    }
}
