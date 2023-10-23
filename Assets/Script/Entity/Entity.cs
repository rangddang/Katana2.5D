using System.Collections;
using System.Collections.Generic;
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

public class Entity : MonoBehaviour
{
    [SerializeField] public string name;

    protected Status status;

    [SerializeField] public float currentHealth;
    [SerializeField] public float currentSpeed;
    [SerializeField] public float currentToughness;

    public bool isDead;
    public MoveState moveState;

    public List<Skill> skills = new List<Skill>();


}
