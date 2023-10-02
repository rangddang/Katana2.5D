using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
	None,
	Attack,
	StrongAttack,
	CounterAttack
}

public enum ParryingType
{
	None,
	Parrying,
	PerfectParrying
}

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

    public float attackDelay = 0.2f;
	public float strongAttackDelay = 0.5f;
	public float counterAttackDelay = 0.4f;
	public float perpectParryingTime = 0.3f;
	public bool parryingSuccess;
	[SerializeField] private float attackWait = 0.7f;

    public bool katanaOn = false;
	public AttackType attackType;
	public ParryingType parryingType;

	private float attackTime;
	private float parryingSucTime;
    private LeftRight leftRight = LeftRight.Left;
    private int attackNum;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
		attackTime += Time.deltaTime;

		if (parryingType == ParryingType.None)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				katanaOn = !katanaOn;
				animator.SetBool("IsAttack", false);
				animator.SetBool("KatanaOn", katanaOn);
			}
			if (katanaOn)
			{
				if (attackType == AttackType.Attack)
				{
					Attack();
				}
				else if (attackType == AttackType.StrongAttack)
				{
					StrongAttack();
				}
				else if(attackType == AttackType.CounterAttack)
				{
					CounterAttack();
				}
			}
		}
		if (!(parryingType == ParryingType.None))
		{
			Parrying();
		}

        if(attackTime >= attackWait)
        {
            animator.SetBool("IsAttack", false);
        }

		if (parryingSuccess)
		{
			parryingSucTime += Time.deltaTime;
			if (parryingSucTime > 1f)
			{
				parryingSucTime = 0;
				parryingSuccess = false;
			}
		}

		animator.SetBool("IsParrying", !(parryingType == ParryingType.None));
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
            targets[i].transform.parent.GetComponent<Enemy>().Hit(player.status.damage, new Vector2(x, -y * 0.3f));
        }
        if (i > 0)
        {
            camera.ShakeCamera(0.2f , 0.4f);
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
			targets[i].transform.parent.GetComponent<Enemy>().Hit(player.status.damage * 3f, new Vector2(0,1));
		}
		if (i > 0)
		{
			camera.ShakeCamera(0.3f, 0.6f);
			//player.SlowTime();
		}

		attackTime = 0;
	}

	public void CounterAttack()
	{
		parryingSuccess = false;
		parryingSucTime = 0;
		StopCoroutine("CountAttack");
		StartCoroutine("CountAttack");
	}

	private IEnumerator CountAttack()
	{
		attackNum = Random.Range(1, 3 + 1);
		leftRight = LeftRight.Right;
		Count();
		yield return new WaitForSeconds(0.15f);
		leftRight = LeftRight.Left;
		Count();
	}

	private void Count()
	{
		animator.SetBool("IsAttack", true);

		animator.Play("Katana_Attack_" + leftRight.ToString() + "_" + attackNum.ToString(), -1, 0);

		float x = leftRight == LeftRight.Left ? 1 : -1;
		float y = attackNum - 2;
		y *= leftRight == LeftRight.Left ? 1 : -1;
		camera.CutCamera(new Vector2(x, y));

		Collider[] targets = Physics.OverlapBox(transform.position + (transform.forward * attackRange.localPosition.z), attackRange.lossyScale, transform.rotation, enemyMask);

		int i;

		for (i = 0; i < targets.Length; i++)
		{
			targets[i].transform.parent.GetComponent<Enemy>().Hit(player.status.damage * 1.75f, new Vector2(x, -y * 0.3f));
		}
		if (i > 0)
		{
			camera.ShakeCamera(0.2f, 0.4f);
			//player.SlowTime();
		}

		attackTime = 0;
	}

	public void Parrying()
	{
		animator.SetBool("IsAttack", false);
		animator.Play("Katana_Parrying");
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position + (transform.forward * attackRange.localPosition.z), attackRange.lossyScale);
	}
}
