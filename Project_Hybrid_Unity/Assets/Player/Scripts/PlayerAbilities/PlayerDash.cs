using System.Collections;
using UnityEngine;

public class PlayerDash : PlayerAbility
{
    [Header("Dash Settings")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashActiveTime;
    [SerializeField] private float dashCooldownTime;

    private bool canDash = true;

    protected override void Initialize()
    {
        InputService.DashStarted += StartDash;
    }

    private void OnDisable()
    {
        InputService.DashStarted -= StartDash;
    }

    private void StartDash()
    {
        if (canDash && InputService.HorizontalAxis != 0)
        {
            StartCoroutine(Dashing());
        }
    }

    private IEnumerator Dashing()
    {
        RigidBody.velocity = new Vector2(InputService.HorizontalAxis * dashForce, 0);
        float rigidBodyGravityScale = RigidBody.gravityScale;
        RigidBody.gravityScale = 0;
        Controller.CanMove = false;
        canDash = false;

        yield return new WaitForSeconds(dashActiveTime);

        RigidBody.gravityScale = rigidBodyGravityScale;
        Controller.CanMove = true;

        yield return new WaitForSeconds(dashCooldownTime);

        canDash = true;
    }
}

