using UnityEngine;

public class PlayerAttack : PlayerAbility
{
    [Header("Attack Settings")]
    [SerializeField] private int damgeOutput;
    [SerializeField] private float startAttackCooldown;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackOffset;

    private float attackCooldown;

    protected override void Initialize()
    {
        InputService.AttackStarted += Attack;
    }

    private void OnDisable()
    {
        InputService.AttackStarted -= Attack;
    }

    private void Attack()
    {
        if (attackCooldown <= Time.time)
        {
            attackCooldown = startAttackCooldown + Time.time;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out IMeleeInteractable interactable))
                {
                    interactable.MeleeInteract(damgeOutput);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        PointAttackPoint();
    }

    private void PointAttackPoint()
    {
        attackPoint.localPosition = new Vector2(InputService.Direction *attackOffset, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
