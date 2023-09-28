using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Image bossHP;
    public Image bossSubHP;
    public Image bossD;
    public TextMeshProUGUI bossName;
    [SerializeField] private Image[] HitPanel;
    [SerializeField] private float hitTime = 0.5f;

    public void HitEffect()
    {
        StopCoroutine("Hit");
        StartCoroutine("Hit");
    }

    private IEnumerator Hit()
    {
        float a = 1;
        float currentTime = 0;
        HitPanel[0].gameObject.SetActive(true);

		for (int i = 1; i < HitPanel.Length; i++)
		{
			HitPanel[i].color = new Color(0, 0, 0, a);
		}

		while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime < hitTime)
            {
                a = Mathf.Lerp(1, 0, currentTime / hitTime);

                HitPanel[0].color = new Color(0, 0, 0, a);
            }
            else
            {
                a = Mathf.Lerp(1, 0, (currentTime - hitTime) / 0.1f);

                for (int i = 1; i < HitPanel.Length; i++)
                {
                    HitPanel[i].color = new Color(0, 0, 0, a);
                }
                if (currentTime >= hitTime + 0.1f)
                {
                    HitPanel[0].gameObject.SetActive(false);
                    yield break;
                }
            }
            yield return null;
        }
    }
}
