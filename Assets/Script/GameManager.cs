using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIController ui;
    public Enemy boss;
    public PlayerController player;
    [SerializeField] private Volume volume;

    public bool isWeaknessTime = false;
    public bool isReverse = false;
    public bool isBreakEffect;

    private void Update()
    {
        ui.bossHP.fillAmount = boss.currentHealth / boss.status.health;
        ui.bossSubHP.fillAmount = Mathf.Lerp(ui.bossSubHP.fillAmount, ui.bossHP.fillAmount, Time.deltaTime * 5);
        ui.bossToughness.fillAmount = boss.currentToughness / boss.status.toughness;
        ui.bossName.text = boss.name;
    }

    public void ReverseColors(bool reverse)
    {
        isReverse = reverse;

        if (isReverse)
        {
            Time.timeScale = 0.1f;
            //player.sensitivity = 0.5f;
        }
        else
        {
            Time.timeScale = 1f;
            //player.sensitivity = 5f;
        }

        volume.weight = isReverse ? 1 : 0;
        ui.bossUI.gameObject.SetActive(!isReverse);
    }

    public void OnWeakness(bool isOn)
    {
        ui.weakness.gameObject.SetActive(isOn);
        ui.weakness.GetComponent<Animator>().speed = 1 / Time.timeScale;
        ui.weakness.GetComponent<Animator>().SetBool("Play", isOn);

    }
}
