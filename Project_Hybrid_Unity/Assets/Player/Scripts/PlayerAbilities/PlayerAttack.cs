using UnityEngine;

public class PlayerAttack : PlayerAbility
{
    [Header("Attack Settings")]
    [SerializeField] private int damgeOutput;
    [SerializeField] private float startAttackCooldown;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackOffset;
    [SerializeField] private GameObject attackAnimation;

    private float attackCooldown;
    public Animator animator;

    protected override void Initialize()
    {
        InputService.AttackStarted += Attack;

        PointAttackPoint();
    }

    private void OnDisable()
    {
        InputService.AttackStarted -= Attack;
    }

    private void Attack()
    {
        if (attackCooldown <= Time.time)
        {
            //Transform rotationPlayer = transform.GetChild(0);
            // GameObject attackGbj = Instantiate(attackAnimation, attackPoint.position, rotationPlayer.transform.rotation);
            //attackGbj.transform.parent = attackPoint.transform;
            
            attackCooldown = startAttackCooldown + Time.time;
            animator.SetBool("Attack", true);

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

    public void EndAttack()
    {
        animator.SetBool("Attack", false);
    }

    private void FixedUpdate()
    {
        PointAttackPoint();
    }

    private void PointAttackPoint()
    {
        attackPoint.localPosition = new Vector2(InputService.Direction * attackOffset, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
