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
    public float currentToughness;

    public bool isDead;

    public int weaknessCount;
    private int toughnessCount = 3;
    private int maxToughnessCount;

    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected Transform target;
    [SerializeField] protected AnimationClip attackAnim;

    [SerializeField] protected GameObject hitEffect;
    [SerializeField] protected GameObject head;

    [SerializeField] protected float attackDistance = 3f;
    [SerializeField] protected float attackDelay = 1.5f;
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
        currentToughness = status.toughness;
        maxToughnessCount = toughnessCount;
    }

    private void Update()
    {
        if (isDead) return;

        if(target != null && !gameManager.isReverse)
        {
            //print("아빠아직안잔다");
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
            //print("아빠잔다");
            isAttack = false;
            StopCoroutine("AttackTarget");
            animator.Play("Enemy_Idle", -1, 0);
            animator.SetBool("IsWalk", false);
		}
    }

    public virtual void Move()
    {
        currentMoveSpeed = status.speed;

        //transform.position += transform.forward * Time.deltaTime * currentMoveSpeed;
        rigid.MovePosition(transform.position + (transform.forward * currentMoveSpeed * 10));
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

    public virtual void Hit(float damage, float toughnessDamage, Quaternion dir)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            currentToughness -= toughnessDamage;
            if (currentHealth <= 0)
            {
                Dead();
            }
            if (isDead) return;
            if (!isAttack)
            { 
                animator.Play("Enemy_Hit");
            }
            if (currentToughness <= ((status.toughness / maxToughnessCount) * (toughnessCount - 1)))
            {
                toughnessCount--;
                weaknessCount++;
                StopCoroutine("StartHit");
                StartCoroutine("StartHit");
            }

        }

        GameObject effect = Instantiate(hitEffect);
        effect.transform.position = transform.position;
        effect.transform.rotation = dir;
    }

    public virtual void Dead()
    {
        isDead = true;
        gameManager.ReverseColors(true);
        animator.SetBool("IsDead", true);
        Rigidbody headRigid = Instantiate(head).GetComponent<Rigidbody>();
        headRigid.transform.position = transform.position;
        headRigid.AddForce(new Vector3(Random.Range(0f, 3f), 10f, Random.Range(0f, 3f)), ForceMode.Impulse);
        StopCoroutine("DeadEffect");
        StartCoroutine("DeadEffect");
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

    private IEnumerator StartHit()
    {
        while (true)
        {
            GameObject effect = Instantiate(hitEffect);
            effect.transform.position = transform.position;
            effect.transform.rotation = Quaternion.Euler(Random.Range(-45f, -90f), Random.Range(0f, 360f), 0);
            if(weaknessCount < 1)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    private IEnumerator DeadEffect()
    {
        float currentTime = 0;
        float playTime = 1;
        float time = 0.1f;

        while (true)
        {
            currentTime += time;

            GameObject effect = Instantiate(hitEffect);
            effect.transform.position = transform.position;
            effect.transform.rotation = Quaternion.Euler(Random.Range(-75f, -90f), Random.Range(0f, 360f), 0);
            if (currentTime > playTime)
            {
                yield break;
            }
            yield return new WaitForSeconds(time);
            gameManager.ReverseColors(false);
        }
    }
}
