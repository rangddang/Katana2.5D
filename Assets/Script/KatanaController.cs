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

    [SerializeField] private CameraController camera;

    [SerializeField] private float attackDelay = 0.1f;
    [SerializeField] private float perpectParryingTime = 0.5f;

    public bool katanaOn = false;
    public bool isParrying = false;
    public bool parryingPerpect = false;

    private float attackTime;
    private LeftRight leftRight = LeftRight.Left;
    private int attackNum;
    private float parryingTIme;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        attackTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0) && katanaOn && attackTime > attackDelay)
        {
			isParrying = false;
			if (attackTime < 0.7f)
            {
                leftRight = leftRight == LeftRight.Left ? LeftRight.Right : LeftRight.Left;
            }
            else
            {
                leftRight = LeftRight.Right;
            }

            attackNum = Random.Range(1, 3 + 1);

            animator.Play("Katana_Attack_" + leftRight.ToString() + "_" + attackNum.ToString());

            float x = leftRight == LeftRight.Left ? 1 : -1;
            float y = attackNum - 2;
            y *= leftRight == LeftRight.Left ? -1 : 1;
            camera.CutCamera(new Vector2(x, y));

            attackTime = 0;
        }

        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
			isParrying = true;
		}
        else if(Input.GetKeyUp(KeyCode.Mouse1))
        {
			isParrying = false;
		}
        if (isParrying)
        {
            parryingPerpect = parryingTIme < perpectParryingTime;

			parryingTIme += Time.deltaTime;
        }
        else
        {
            parryingTIme = 0;
        }
		animator.SetBool("IsParrying", isParrying);

        if (Input.GetKeyDown(KeyCode.E))
        {
			isParrying = false;
			katanaOn = !katanaOn;
			animator.SetBool("KatanaOn", katanaOn);
		}
    }
}
