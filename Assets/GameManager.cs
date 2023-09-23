using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIController ui;
    [SerializeField] private Enemy boss;

    private void Update()
    {
        ui.bossHP.fillAmount = boss.currentHealth / boss.status.health;
        ui.bossName.text = boss.name;
    }
}
