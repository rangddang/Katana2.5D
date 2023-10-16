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
        ui.bossSubHP.fillAmount = Mathf.Lerp(ui.bossSubHP.fillAmount, ui.bossHP.fillAmount, Time.deltaTime * 5);
        ui.bossToughness.fillAmount = boss.currentToughness / boss.status.toughness;
        ui.bossName.text = boss.name;
    }
}
