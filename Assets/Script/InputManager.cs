using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    None,
    Attack,
    StrongAttack
}

public enum ParryingType
{
    None,
	Parrying,
	PerfectParrying
}

public class InputManager : MonoBehaviour
{
    [SerializeField] private KatanaController katana;
    [SerializeField] private float strongAttackTime = 0.2f;

    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode parryingKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode katataKey = KeyCode.E;
    [SerializeField] private KeyCode dashKey = KeyCode.Space;

    private float attackHoldTime;
    private bool isAttack = false;

    public AttackType attackType;
    public ParryingType parryingType;

    private void Update()
    {
        Attack();
        Parrying();

	}

    private void Attack()
    {
        if (!isAttack)
        {
			attackType = AttackType.None;
		}
        if (Input.GetKeyDown(attackKey))
        {
            attackHoldTime = 0;
            isAttack = true;
		}
        else if (Input.GetKey(attackKey) && isAttack)
        {
            attackHoldTime += Time.deltaTime;
			if (attackHoldTime >= strongAttackTime)
            {
                attackType = AttackType.StrongAttack;
                print(attackType);
				isAttack = false;
			}
		}
        else if (Input.GetKeyUp(attackKey))
        {
			if (attackHoldTime < strongAttackTime)
			{
				attackType = AttackType.Attack;
				print(attackType);
				isAttack = false;
			}
		}
    }

    private void Parrying()
    {

    }
}
