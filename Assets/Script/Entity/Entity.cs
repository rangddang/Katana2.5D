using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum MoveState
{
    Idle,
    Walk,
    Run,
    Dash
}

[System.Serializable]
public class Skill
{
    public string name;
    public float attackDamage;
    public float attackDelay;
    public Transform attackRange;
}

[RequireComponent(typeof(Status))]
public class Entity : MonoBehaviour
{
    protected Status status;

    [SerializeField] public float currentHealth;
    [SerializeField] public float currentSpeed;
    [SerializeField] public float currentToughness;

    public MoveState moveState;
    public bool isDead;

    private void Awake()
    {
        status = GetComponent<Status>();
    }

    public virtual void Move(Vector3 dir)
    {
        transform.position += dir * currentSpeed * Time.deltaTime;
    }

    public virtual void Attack(float damage)
    {

    }

    public virtual void Attack(float damage, float toughnessDamage)
    {

    }

    public virtual void Attack(float damage, float toughnessDamage, Vector2 dir)
    {

    }

    public virtual void Hit(float hitDamage)
    {
        if (isDead) return;

        currentHealth -= hitDamage;
        if(currentHealth <= 0)
        {
            Dead();
        }
    }

    public virtual void Dead()
    {
        isDead = true;
    }
}
