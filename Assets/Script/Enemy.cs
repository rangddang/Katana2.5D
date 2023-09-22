using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	protected Animator animator;
    protected Status status;

    [SerializeField] protected Transform target;
    [SerializeField] protected GameObject hitEffect;
    

    private void Awake()
    {
        animator = transform.Find("Sprite").GetComponent<Animator>();
        status = GetComponent<Status>();
    }

    private void Update()
    {
        if(target != null)
        {
            
        }
    }

    public void Attack()
    {

    }

    public void Hit(float damage, float dir)
    {
        status.health -= damage;
        animator.Play("Enemy_Hit");
        GameObject effect = Instantiate(hitEffect);
        effect.transform.position = transform.position;
        effect.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + (90 * dir), 0);
    }
}
