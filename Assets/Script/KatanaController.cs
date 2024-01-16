using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState
{
	None,
	Attack,
	StrongAttack,
	CounterAttack
}

public enum ParryingState
{
	None,
	Parrying,
	PerfectParrying
}

public class KatanaController : MonoBehaviour
{
    private enum LeftRight
    {
        Left = 1,
        Right = -1
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
	private bool saveKatanaOn = false;
	public AttackState attackState;
	public ParryingState parryingState;

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

		if (katanaOn != saveKatanaOn)
		{
			saveKatanaOn = katanaOn;
			parryingState = ParryingState.None;
			animator.SetBool("IsAttack", false);
			animator.SetBool("KatanaOn", katanaOn);
		}
		if (parryingState == ParryingState.None)
		{
			if (katanaOn)
			{
				if(Input.GetKeyDown(KeyCode.Y))
				{
					animator.Play("Katana_자세히보기", -1, 0);
				}
				if (attackState == AttackState.Attack)
				{
					Attack();
				}
				else if (attackState == AttackState.StrongAttack)
				{
					StrongAttack();
				}
				else if(attackState == AttackState.CounterAttack)
				{
					CounterAttack();
				}
			}
		}
		if (parryingState != ParryingState.None)
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

		animator.SetBool("IsParrying", !(parryingState == ParryingState.None));
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

		float x = (int)leftRight;
		float y = attackNum - 2;
		y *= (int)leftRight;
		camera.CutCamera(new Vector2(x, y), 3);

		baseAttack(1f, 1f, -x, 0.3f * y);

		attackTime = 0;
	}

    public void StrongAttack()
    {
		animator.SetBool("IsAttack", true);

		animator.Play("Katana_StrongAttack", -1, 0);

		float x = 0;
		float y = -1;

		camera.CutCamera(new Vector2(x, y), 6);

		baseAttack(3.5f, 2f, x, y);

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
		yield return new WaitForSeconds(0.12f);
		leftRight = LeftRight.Left;
		Count();
	}

	public void Count()
	{
		animator.SetBool("IsAttack", true);

		animator.Play("Katana_Attack_" + leftRight.ToString() + "_" + attackNum.ToString(), -1, 0);

		float x = leftRight == LeftRight.Left ? 1 : -1;
		float y = attackNum - 2;
		y *= leftRight == LeftRight.Left ? 1 : -1;
		camera.CutCamera(new Vector2(x, y), 3);

		baseAttack(1.5f, 1.7f, -x, 0.3f * y);

		attackTime = 0;
	}

	private void baseAttack(float damage, float toughnessDamage, float x, float y)
    {
		StartCoroutine(WaitForBaseAttack(damage, toughnessDamage, x, y));
	}

	private IEnumerator WaitForBaseAttack(float damage, float toughnessDamage, float x, float y)
    {
		yield return new WaitForSeconds(0.08f);
		Collider[] targets = Physics.OverlapBox(transform.position, attackRange.lossyScale, transform.rotation);

		int i;
		bool hitEnemy = false;

		Quaternion q;
		Vector3 v;

		foreach (Collider c in targets)
		{
			if (c.CompareTag("Enemy"))
			{
				q = Quaternion.Euler((90 * y), player.transform.eulerAngles.y + (-x * 90), 0);
				hitEnemy = true;
				c.transform.parent.GetComponent<Enemy>().Hit(player.status.damage * damage, toughnessDamage, q);
			}
			else if (c.CompareTag("Reed"))
			{
				q = Quaternion.Euler(-60, player.transform.eulerAngles.y + (-x * 90), 0);
				c.GetComponent<Reed>().Breaked(q);
			}
			else if (c.CompareTag("EnemyHead"))
            {
				v = (-x * transform.right * 0.3f) + (y * Vector3.up) + transform.forward;
				c.GetComponent<Rigidbody>().AddForce(v * 10f, ForceMode.Impulse);
            }
		}
		
		if (hitEnemy)
		{
			camera.ShakeCamera(0.18f, 0.6f);
			//player.SlowTime();
		}
	}

	public void Parrying()
	{
		animator.SetBool("IsAttack", false);
		animator.Play("Katana_Parrying");
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.yellow;
	//	Gizmos.DrawWireCube(attackRange.position, attackRange.lossyScale);
	//}
}
