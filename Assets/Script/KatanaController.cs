using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaController : MonoBehaviour
{
    private enum LeftRight
    {
        Left,
        Right
    }

    private Animator animator;

    [SerializeField] private float attackDelay = 0.1f;

    private float attackTime;
    private LeftRight leftRight = LeftRight.Right;
    private int attackNum;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        attackTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0) && animator.GetBool("KatanaOn"))
        {
            if(attackTime < 1f)
            {
                leftRight = leftRight == LeftRight.Left ? LeftRight.Right : LeftRight.Left;
            }
            else
            {
                leftRight = LeftRight.Right;
            }

            attackNum = Random.Range(1, 3 + 1);
            animator.Play("Katana_Attack_" + leftRight.ToString() + "_" + attackNum.ToString());

            attackTime = 0;
        }

        animator.SetBool("IsParrying", Input.GetKey(KeyCode.Mouse1));

        if (Input.GetKeyDown(KeyCode.E))
        {
			animator.SetBool("KatanaOn", !animator.GetBool("KatanaOn"));
		}
    }
}
