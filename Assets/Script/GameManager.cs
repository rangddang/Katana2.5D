using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Enemy boss;
    public PlayerController player;
    [SerializeField] private UIController ui;
    [SerializeField] private Volume volume;

    public bool isWeaknessTime = false;
    public bool isReverse = false;
    public bool isBreakEffect;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
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
