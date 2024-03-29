using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIController uI;
    [SerializeField] private PlayerController player;
    [SerializeField] private KatanaController katana;
    [SerializeField] private float strongAttackTime = 0.2f;

    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode parryingKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode katataOnOffKey = KeyCode.E;
    [SerializeField] private KeyCode dashKey = KeyCode.Space;

    private float attackDelay;
    private float parringDelay;
    private float attackTime;
    private bool isAttack = false;
    private float parryingTime;
    private float attackHoldTime;
    private bool isParrying = false;
    private float dashDelay;
    private float dashTime;

    private void Start()
    {
        dashDelay = player.dashDelay;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && gameManager.boss.weaknessCount > 0)
        {
            gameManager.isWeaknessTime = !gameManager.isWeaknessTime;
            gameManager.ReverseColors(gameManager.isWeaknessTime);
            gameManager.OnWeakness(gameManager.isWeaknessTime);
        }
        if (Input.GetKeyDown(attackKey) && gameManager.isWeaknessTime && !gameManager.isBreakEffect)
        {
            uI.WeaknessAttackEffect();
        }

        if (gameManager.isReverse) return;

        if (Input.GetKeyDown(katataOnOffKey))
        {
            katana.katanaOn = !katana.katanaOn;
        }
        if (katana.katanaOn)
        {
            Attack();
            Parrying();
        }
        if (Input.GetKeyDown(dashKey) && !player.isDash && dashTime > dashDelay)
        {
            Dash();
        }
        dashTime += Time.deltaTime;
	}

    private void Attack()
    {
        if (!isAttack)
        {
            attackTime += Time.deltaTime;
			katana.attackState = AttackState.None;
		}
        if (Input.GetKeyDown(attackKey) && attackTime > attackDelay)
        {
            attackHoldTime = 0;
            isAttack = true;
            if (katana.parryingSuccess)
            {
				katana.attackState = AttackState.CounterAttack;
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
				katana.attackState = AttackState.StrongAttack;
				isAttack = false;
                attackDelay = katana.strongAttackDelay;
                attackTime = 0;
			}
		}
        else if (Input.GetKeyUp(attackKey) && isAttack)
        {
			if (attackHoldTime < strongAttackTime)
			{
				katana.attackState = AttackState.Attack;
				isAttack = false;
				attackDelay = katana.attackDelay;
				attackTime = 0;
			}
		}
    }

    private void Parrying()
    {
        if (isParrying)
        {
            parryingTime += Time.deltaTime;
        }
        else
        {
            parringDelay -= Time.deltaTime;
        }

		if (Input.GetKey(parryingKey) && attackTime > attackDelay && !isParrying && parringDelay <= 0)
		{
            isParrying = true;
			katana.parryingState = ParryingState.PerfectParrying;
		}
        else if(Input.GetKey(parryingKey) && isParrying)
        {
            if(parryingTime > katana.perpectParryingTime)
            {
				katana.parryingState = ParryingState.Parrying;
			}
        }
        else if (!Input.GetKey(parryingKey) && isParrying && parryingTime > 0.25f)
        {
            isParrying = false;
            parryingTime = 0;
            parringDelay = 0.25f;
			katana.parryingState = ParryingState.None;
		}
	}

    private void Dash()
    {
        dashTime = 0;
        StopCoroutine("IsDash");
        StartCoroutine("IsDash");
    }

    private IEnumerator IsDash()
    {
        float dashTime = 0.40f;

        player.isDash = true;
        yield return new WaitForSeconds(dashTime);
        player.isDash = false;
    }
}
