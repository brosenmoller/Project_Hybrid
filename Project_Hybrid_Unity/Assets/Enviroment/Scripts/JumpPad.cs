using UnityEngine;
using System.Collections;

public class JumpPad : MonoBehaviour
{
    [Header("Jump Pad Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float bounceForce;
    [SerializeField] private float startPostBounceTimer;

    [Header("JumpSqueeze")]
    [SerializeField] private float xSqueeze = 1.6f;
    [SerializeField] private float ySqueeze = 0.4f;
    [SerializeField] private float squeezeDuration = 0.2f;

    [Header("References")]
    [SerializeField] private Transform spriteHolder;

    private float postBounceTimer;

    private PlayerMovement player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        if (postBounceTimer > Time.time && player.GetJumpTimer > Time.time)
        {
            player.GetRigidBody.velocity = new Vector2(player.GetRigidBody.velocity.x, jumpForce - (bounceForce *  startPostBounceTimer));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerMovement playerMovement))
        {
            StartCoroutine(JumpSqueeze(xSqueeze, ySqueeze, squeezeDuration));

            if (player.GetJumpTimer > Time.time)
            {
                player.GetRigidBody.velocity = new Vector2(player.GetRigidBody.velocity.x, jumpForce);
            } else
            {
                postBounceTimer = Time.time + startPostBounceTimer;
                player.GetRigidBody.velocity = new Vector2(player.GetRigidBody.velocity.x, bounceForce);
            }
        }
    }

    private IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
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
}
