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

    [SerializeField] private InputManager inputManager;
    [SerializeField] private CameraController camera;
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform attackRange;
    [SerializeField] private LayerMask enemyMask;

    [SerializeField] private float attackDelay = 0.20f;
	[SerializeField] private float strongAttackDelay = 0.40f;
	[SerializeField] private float perpectParryingTime = 0.5f;
    [SerializeField] private float attackWait = 0.7f;

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

        if (attackTime > attackDelay)
        {
            if (!isParrying)
            {
				if (Input.GetKeyDown(KeyCode.E))
				{
					katanaOn = !katanaOn;
					animator.SetBool("IsAttack", false);
					animator.SetBool("KatanaOn", katanaOn);
				}
                if (katanaOn)
                {
					if (inputManager.attackType == AttackType.Attack)
					{
						Attack();
					}
					else if (inputManager.attackType == AttackType.StrongAttack)
					{
						StrongAttack();
					}
				}
			}
			if (Input.GetKey(KeyCode.Mouse1) && !isParrying && katanaOn)
			{
				isParrying = true;
				animator.SetBool("IsAttack", false);
                animator.Play("Katana_Parrying");
			}
            else if(Input.GetKeyUp(KeyCode.Mouse1))
            {
				isParrying = false;
			}
		}

        if(attackTime >= attackWait)
        {
            animator.SetBool("IsAttack", false);
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
    }

    public void Attack()
    {
		animator.SetBool("IsAttack", true);
		if (attackTime < attackWait)
		{
			leftRight = leftRight == LeftRight.Left ? LeftRight.Right : LeftRight.Left;
		}
		else
		{
			leftRight = LeftRight.Right;
		}

		attackNum = Random.Range(1, 3 + 1);

		animator.Play("Katana_Attack_" + leftRight.ToString() + "_" + attackNum.ToString(), -1, 0);

		float x = leftRight == LeftRight.Left ? 1 : -1;
		float y = attackNum - 2;
		y *= leftRight == LeftRight.Left ? 1 : -1;
		camera.CutCamera(new Vector2(x, y));

		Collider[] targets = Physics.OverlapBox(transform.position + (transform.forward * attackRange.localPosition.z), attackRange.lossyScale, transform.rotation, enemyMask);

        int i;

        for (i = 0; i < targets.Length; i++)
        {
            targets[i].transform.parent.GetComponent<Enemy>().Hit(player.status.damage * Random.Range(0.7f, 1.3f), new Vector2(x, 0));
        }
        if (i > 0)
        {
            camera.ShakeCamera();
            //player.SlowTime();
        }

		attackTime = 0;
	}

    public void StrongAttack()
    {
		animator.SetBool("IsAttack", true);

		animator.Play("Katana_StrongAttack", -1, 0);

		camera.CutCamera(new Vector2(0, -1));

		Collider[] targets = Physics.OverlapBox(transform.position + (transform.forward * attackRange.localPosition.z), attackRange.lossyScale, transform.rotation, enemyMask);

		int i;

		for (i = 0; i < targets.Length; i++)
		{
			targets[i].transform.parent.GetComponent<Enemy>().Hit(player.status.damage * 3f * Random.Range(0.7f, 1.3f), new Vector2(0,1));
		}
		if (i > 0)
		{
			camera.ShakeCamera();
			//player.SlowTime();
		}

		attackTime = 0;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position + (transform.forward * attackRange.localPosition.z), attackRange.lossyScale);
	}
}
