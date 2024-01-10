using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyPatrolScript : MonoBehaviour
{
    [SerializeField] private GameObject _pointA;
    [SerializeField] private GameObject _pointB;
    [SerializeField] private float _speed;

    private Rigidbody2D _rb2D;
    private Transform _currentPoint;
    void Start()
    {
        _rb2D= GetComponent<Rigidbody2D>();
        _currentPoint = _pointB.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 point = _currentPoint.position - transform.position;    
        if(_currentPoint == _pointB.transform)
        {
            _rb2D.velocity = new Vector2(_speed, 0);
        }
        else
        {
            _rb2D.velocity = new Vector2(-_speed, 0);
        }
        if (Vector2.Distance(transform.position, _currentPoint.position) < 0.5f && _currentPoint == _pointB.transform)
        {
            
            flip();
            _currentPoint = _pointA.transform;
        }
        if (Vector2.Distance(transform.position, _currentPoint.position) < 0.5f && _currentPoint == _pointA.transform)
        {
           
            flip();
            _currentPoint = _pointB.transform;
        }
        
    }
    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
