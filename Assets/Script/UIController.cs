using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject bossUI;

    public Image bossHP;
    public Image bossSubHP;
    public Image bossToughness;
    public TextMeshProUGUI bossName;

    [SerializeField] private Transform hitEffect;
    private List<Image> hitEffects = new List<Image>();
    [SerializeField] private float hitTime = 0.5f;

    [SerializeField] private Image parryingEffect;
    [SerializeField] private float parryingTime = 0.5f;

    private float alpha = 1;

    private void Start()
    {
        //print(hitEffect.childCount);
        for (int i = 0; i < hitEffect.childCount; i++)
        {
            hitEffects.Add(hitEffect.GetChild(i).GetComponent<Image>());
        }
    }

    public void ParryingEffect(float a)
    {
        alpha = a;
        StopCoroutine("Parrying");
        StartCoroutine("Parrying");
    }

    public void HitEffect(float a)
    {
        alpha = a;
        StopCoroutine("Hit");
        StartCoroutine("Hit");
    }

    private IEnumerator Hit()
    {
        float a = alpha;
        float saveA = alpha;
        float currentTime = 0;
        hitEffect.gameObject.SetActive(true);

		for (int i = 0; i < hitEffects.Count; i++)
		{
			hitEffects[i].color = new Color(0, 0, 0, a);
		}

		while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime < hitTime)
            {
                a = Mathf.Lerp(saveA, 0, currentTime / hitTime);

                hitEffects[0].color = new Color(1, 1, 1, a);
            }
            else
            {
                a = Mathf.Lerp(saveA, 0, (currentTime - hitTime) / 0.1f);

                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].color = new Color(1, 1, 1, a);
                }
                if (currentTime >= hitTime + 0.1f)
                {
                    hitEffect.gameObject.SetActive(false);
                    yield break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator Parrying()
    {
        float a = alpha;
        float saveA = alpha;
        float currentTime = 0;
        parryingEffect.gameObject.SetActive(true);
        parryingEffect.color = new Color(1, 1, 1, a);
        while (true)
        {
            currentTime += Time.deltaTime;
            a = Mathf.Lerp(saveA, 0, currentTime / parryingTime);
            parryingEffect.color = new Color(1, 1, 1, a);
            if (currentTime >= parryingTime)
            {
                parryingEffect.gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }
}
