using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigid;

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        animator.speed = rigid.velocity.magnitude * 0.2f;
    }
}
