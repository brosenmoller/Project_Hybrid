using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    protected InputService InputService { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        InputService = ServiceLocator.Instance.Get<InputService>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("Speed", InputService.HorizontalAxis);
    }
}
