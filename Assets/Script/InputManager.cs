using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private KatanaController katana;
    [SerializeField] private float strongAttackTime = 0.2f;

    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode parryingKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode katataOnOffKey = KeyCode.E;
    [SerializeField] private KeyCode dashKey = KeyCode.Space;

    private float attackDelay;
    private float attackTime;
    private bool isAttack = false;
    private float parryingTime;
    private float attackHoldTime;
    private bool isParrying = false;

    private void Update()
    {
        if (katana.katanaOn)
        {
            Attack();
            Parrying();
        }

	}

    private void Attack()
    {
        if (!isAttack)
        {
            attackTime += Time.deltaTime;
			katana.attackType = AttackType.None;
		}
        if (Input.GetKeyDown(attackKey) && attackTime > attackDelay)
        {
            attackHoldTime = 0;
            isAttack = true;
            if (katana.parryingSuccess)
            {
				katana.attackType = AttackType.CounterAttack;
				isAttack = false;
				attackDelay = katana.counterAttackDelay;
				attackTime = 0;
			}
		}
        else if (Input.GetKey(attackKey) && isAttack)
        {
            attackHoldTime += Time.deltaTime;
			if (attackHoldTime >= strongAttackTime)
            {
				katana.attackType = AttackType.StrongAttack;
				isAttack = false;
                attackDelay = katana.strongAttackDelay;
                attackTime = 0;
			}
		}
        else if (Input.GetKeyUp(attackKey) && isAttack)
        {
			if (attackHoldTime < strongAttackTime)
			{
				katana.attackType = AttackType.Attack;
				isAttack = false;
				attackDelay = katana.attackDelay;
				attackTime = 0;
			}
		}
    }

    private void Parrying()
    {
		if (Input.GetKeyDown(parryingKey) && attackTime > attackDelay)
		{
            isParrying = true;
			katana.parryingType = ParryingType.PerfectParrying;
		}
        else if(Input.GetKey(parryingKey) && isParrying)
        {
            parryingTime += Time.deltaTime;
            if(parryingTime > katana.perpectParryingTime)
            {
				katana.parryingType = ParryingType.Parrying;
			}
        }
        else if (Input.GetKeyUp(parryingKey) && isParrying)
        {
            isParrying = false;
            parryingTime = 0;
			katana.parryingType = ParryingType.None;
		}
	}
}
