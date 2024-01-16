using UnityEngine;

public class EnemyPatrolScript : MonoBehaviour
{
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rigidBody2D;
    private Transform currentPoint;

    private void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
    }

    private void Update()
    {
        if (currentPoint == pointB.transform)
        {
            rigidBody2D.velocity = new Vector2(moveSpeed, 0);
        }
        else
        {
            rigidBody2D.velocity = new Vector2(-moveSpeed, 0);
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            Flip();
            currentPoint = pointA.transform;
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            Flip();
            currentPoint = pointB.transform;
        }
        
    }
    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
