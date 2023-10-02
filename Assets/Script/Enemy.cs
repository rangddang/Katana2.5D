using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string name = "Enemy";
	protected Animator animator;
    public Status status;

    public float currentHealth;
    public float currentMoveSpeed;

    [SerializeField] protected Transform target;
    [SerializeField] protected GameObject hitEffect;
    [SerializeField] protected AnimationClip attackAnim;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackDelay = 1.5f;
    protected Rigidbody rigid;
    
    private float attackTime;
    private bool isAttack;

    private void Awake()
    {
        animator = transform.Find("Sprite").GetComponent<Animator>();
        status = GetComponent<Status>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentHealth = status.health;
    }

    private void Update()
    {
        if(target != null)
        {
            if (!isAttack)
            {
                attackTime += Time.deltaTime;
            }
			Quaternion dir = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position);
			transform.rotation = dir;
            if(Vector3.Distance(transform.position, target.position) < attackDistance && attackTime >= attackDelay)
            {
                TryAttack();
            }
            else if(!isAttack && Vector3.Distance(transform.position, target.position) > 1.5f)
            {
                Move();
            }
            else
            {
                animator.SetBool("IsWalk", false);
            }
		}
        else
        {
			animator.SetBool("IsWalk", false);
		}
    }

    public virtual void Move()
    {
        currentMoveSpeed = status.walkSpeed;

        transform.position += transform.forward * Time.deltaTime * currentMoveSpeed;
        animator.SetBool("IsWalk", true);
    }

    public virtual void TryAttack()
    {
		animator.SetBool("IsWalk", false);
		StopCoroutine("AttackTarget");
        StartCoroutine("AttackTarget");
    }

    public virtual void Attack()
    {
        if(Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            target.GetComponent<PlayerController>().Hit(status.damage);
        }
    }

    public virtual void Hit(float damage, Vector2 dir)
    {
        currentHealth -= damage;
        if (!isAttack)
        {
            animator.Play("Enemy_Hit");
        }
        GameObject effect = Instantiate(hitEffect);
        effect.transform.position = transform.position;
        effect.transform.rotation = Quaternion.Euler(-(90 * dir.y), transform.eulerAngles.y + (90 * dir.x), 0);
        if(currentHealth <= 0)
        {

        }
    }

    public virtual void Dead()
    {

    }

    private IEnumerator AttackTarget()
    {
        isAttack = true;
		attackTime = 0;
		animator.Play("Enemy_Attack", -1, 0);
		yield return new WaitForSeconds(attackAnim.length);
        Attack();
        isAttack = false;
	}
}
