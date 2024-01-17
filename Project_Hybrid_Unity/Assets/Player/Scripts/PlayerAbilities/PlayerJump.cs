﻿using UnityEngine;
using System.Collections;

public class PlayerJump : PlayerAbility
{
    [Header("Jump Settings")]
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

    [Header("Physics Material")]
    [SerializeField] private PhysicsMaterial2D inAirPhysicsMaterial;
    [SerializeField] private CapsuleCollider2D capsuleCollider;
    [SerializeField] private BoxCollider2D boxCollider;

    public float GetJumpTimer { get => jumpTimer; }

    private bool isGrounded;
    private bool wasGrounded;

    private float groundTimer;
    private float jumpTimer;
    private float currentJumpVelocity;

    public Animator animator;

    protected override void Initialize()
    {
        RigidBody.gravityScale = rigidBodyGravityScale;
        currentJumpVelocity = maxJumpVelocity;

        isGrounded = GroundCheck();

        InputService.JumpStarted += InitiateJump;
        //InputService.JumpCancelled += CutJumpVelocity;
    }

    private void OnDisable()
    {
        InputService.JumpStarted -= InitiateJump;
        //InputService.JumpCancelled -= CutJumpVelocity;
    }

    private void Update()
    {
        if (!Controller.CanJump) { return; }

        wasGrounded = isGrounded;
        isGrounded = GroundCheck();

        // take off (coyote time)
        if (wasGrounded && !isGrounded)
        {
            groundTimer = Time.time + groundDelay;
            wasGrounded = false;

            RigidBody.sharedMaterial = inAirPhysicsMaterial;
            boxCollider.sharedMaterial = inAirPhysicsMaterial;
            capsuleCollider.sharedMaterial = inAirPhysicsMaterial;
        }

        // landing
        if (!wasGrounded && isGrounded)
        {
            RigidBody.sharedMaterial = null;
            boxCollider.sharedMaterial = null;
            capsuleCollider.sharedMaterial = null;
            animator.SetBool("isJumping", false);


            StartCoroutine(JumpSqueeze(xSqueeze, ySqueeze, squeezeDuration));
        }
    }

    private void InitiateJump()
    {
        jumpTimer = Time.time + jumpDelay;
    }

    private void CutJumpVelocity()
    {
        if (jumpTimer > 0) // Release Jumpbutton before touching the ground
        {
            currentJumpVelocity = maxJumpVelocity * (jumpCutOff + 0.1f);
        }
        else if (RigidBody.velocity.y > 0)
        {
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, RigidBody.velocity.y * jumpCutOff);
        }
    }

    private void FixedUpdate()
    {
        if (!Controller.CanMove || !Controller.CanJump) { return; }

        if (jumpTimer > Time.time && (groundTimer > Time.time || isGrounded))
        {
            Jump(currentJumpVelocity);
            animator.SetBool("isJumping", true);
        }
    }

    private void Jump(float jumpVelocity)
    {
        RigidBody.velocity = new Vector2(RigidBody.velocity.x, jumpVelocity);

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
            SpriteHolder.localScale = Vector3.Lerp(originalSize, newSize, time);
            SpriteHolder.localPosition = Vector3.Lerp(oldPos, newPos, time);
            yield return null;
        }
        time = 0f;
        while (time <= 1.0)
        {
            time += Time.deltaTime / seconds;
            SpriteHolder.localScale = Vector3.Lerp(newSize, originalSize, time);
            SpriteHolder.localPosition = Vector3.Lerp(newPos, oldPos, time);
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

