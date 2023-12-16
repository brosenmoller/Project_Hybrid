using System.Collections;
using UnityEngine;

public class PlayerCrouch : PlayerAbility
{
    [Header("Crouch Settings")]
    [SerializeField] private Vector2 colliderOffset = new(0, -0.25f);
    [SerializeField] private Vector2 colliderSize = new(1, 0.5f);

    private Vector2 colliderDefaultOffset;
    private Vector2 colliderDefaultSize;

    private BoxCollider2D boxCollider;

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
        StartCoroutine(Squeeze(new Vector3(1.5f, 0.5f, SpriteHolder.transform.localScale.z), .2f));
        boxCollider.size = colliderSize;
        boxCollider.offset = colliderOffset;
        Controller.CanJump = false;
    }

    private void CancelCrouch()
    {
        StartCoroutine(Squeeze(new Vector3(1f, 1f, SpriteHolder.transform.localScale.z), .2f));
        boxCollider.size = colliderDefaultSize;
        boxCollider.offset = colliderDefaultOffset;
        Controller.CanJump = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube((Vector2)SpriteHolder.transform.position + colliderOffset, colliderSize);
    }

    private IEnumerator Squeeze(Vector3 newSize, float seconds)
    {
        Vector3 originalSize = SpriteHolder.transform.localScale;

        Vector3 oldPos = SpriteHolder.transform.localPosition;
        Vector3 newPos = new(0, -0.25f, oldPos.z);

        float time = 0f;
        while (time <= 1.0)
        {
            time += Time.deltaTime / seconds;
            SpriteHolder.localScale = Vector3.Lerp(originalSize, newSize, time);
            SpriteHolder.localPosition = Vector3.Lerp(oldPos, newPos, time);
            yield return null;
        }
    }
}
