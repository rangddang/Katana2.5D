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
    

    private void Awake()
    {
        animator = transform.Find("Sprite").GetComponent<Animator>();
        status = GetComponent<Status>();
    }

    private void Start()
    {
        currentHealth = status.health;
    }

    private void Update()
    {
        if(target != null)
        {
			Quaternion dir = Quaternion.LookRotation(transform.position - new Vector3(target.position.x, transform.position.y, target.position.z));
			transform.rotation = dir;
		}
    }

    public virtual void Attack()
    {

    }

    public void Hit(float damage, Vector2 dir)
    {
        currentHealth -= damage;
        animator.Play("Enemy_Hit");
        GameObject effect = Instantiate(hitEffect);
        effect.transform.position = transform.position;
        effect.transform.rotation = Quaternion.Euler(-(90 * dir.y), transform.eulerAngles.y + (90 * dir.x), 0);
    }
}
