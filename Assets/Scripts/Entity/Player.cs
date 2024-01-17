using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public override void Move(Vector3 dir)
    {
        base.Move(dir);
    }

    public override void Attack(float damage)
    {
        base.Attack(damage);
    }

    public override void Hit(float hitDamage)
    {
        base.Hit(hitDamage);
    }

    public override void Dead()
    {
        base.Dead();
    }
}
