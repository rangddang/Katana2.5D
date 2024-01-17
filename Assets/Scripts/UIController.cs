using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static bool isOnMenu;

    public GameObject bossUI;

    public Image bossHP;
    public Image bossSubHP;
    public Image bossToughness;
    public TextMeshProUGUI bossName;

    public RectTransform weakness;
    public Image weaknessPanel;
    public GameObject breakedEffect;

    [SerializeField] private Transform hitEffect;
    private List<Image> hitEffects = new List<Image>();
    [SerializeField] private float hitTime = 0.5f;

    [SerializeField] private Image parryingEffect;
    [SerializeField] private float parryingTime = 0.5f;
    [SerializeField] private Animator menuAnim;

    private float alpha = 1;

    private void Start()
    {
        OffCursor();
        //print(hitEffect.childCount);
        for (int i = 0; i < hitEffect.childCount; i++)
        {
            hitEffects.Add(hitEffect.GetChild(i).GetComponent<Image>());
        }
    }

    private void Update()
    {
        Vector3 playerPosition = GameManager.instance.player.transform.position;
        Vector3 enemyPosition = GameManager.instance.boss.transform.position;

        Vector3 directionToPlayer = (enemyPosition - playerPosition).normalized;

        Vector3 playerForward = GameManager.instance.player.transform.forward;

        float dotProduct = Vector3.Dot(directionToPlayer, playerForward);

        // ���� �ڽſ� �տ� ������ ������ ���󰡰� �����
        // (�ڿ� �������� ������ �տ� ǥ�õǴ� ���� ����)
        if(dotProduct > 0)
        {
            weakness.position = Camera.main.WorldToScreenPoint(GameManager.instance.boss.transform.position);
            breakedEffect.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(GameManager.instance.boss.transform.position);

        }

        bossHP.fillAmount = GameManager.instance.boss.currentHealth / GameManager.instance.boss.status.health;
        bossSubHP.fillAmount = Mathf.Lerp(bossSubHP.fillAmount, bossHP.fillAmount, Time.deltaTime * 5);
        bossToughness.fillAmount = GameManager.instance.boss.currentToughness / GameManager.instance.boss.status.toughness;
        bossName.text = GameManager.instance.boss.name;
    }

    public void OnCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OffCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void WeaknessAttackEffect()
    {
        StopCoroutine("WeaknessEffect");
        StartCoroutine("WeaknessEffect");
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

    public void OnMenu()
    {
        isOnMenu = true;
        OnCursor();
        menuAnim.gameObject.SetActive(true);
        menuAnim.Play("Menu Opening", -1, 0);
        StopCoroutine("DisActive");
    }

    public void OffMenu()
    {
        isOnMenu = false;
        OffCursor();
        menuAnim.Play("Menu Ending", -1, 0);
        StopCoroutine("DisActive");
        StartCoroutine("DisActive");
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

    private IEnumerator WeaknessEffect()
    {
        float currentTime = 0;
        float playTime = 0.7f;

        float startRotate = -720;
        float rotate = startRotate;

        GameManager.instance.boss.weaknessCount--;

        GameManager.instance.isBreakEffect = true;

        weaknessPanel.color = Color.black;
        weaknessPanel.gameObject.SetActive(true);
        weakness.rotation = Quaternion.Euler(0,0, startRotate);

        while (true)
        {
            currentTime += Time.unscaledDeltaTime;

            float t = currentTime / playTime;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            rotate = Mathf.Lerp(startRotate, 0, t);
            weakness.rotation = Quaternion.Euler(0, 0, rotate);

            if(currentTime >= playTime)
            {
                StopCoroutine("BreakedEffect");
                StartCoroutine("BreakedEffect");
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator BreakedEffect()
    {
        float playTime = 0.55f;
        GameManager.instance.OnWeakness(false);
        breakedEffect.SetActive(true);

        weaknessPanel.color = Color.white;
        Camera.main.GetComponent<CameraController>().ShakeCamera(playTime * Time.timeScale, 1.5f);
        yield return new WaitForSecondsRealtime(playTime);

        GameManager.instance.isBreakEffect = false;

        breakedEffect.SetActive(false);
        weaknessPanel.gameObject.SetActive(false);
        GameManager.instance.isWeaknessTime = false;
        GameManager.instance.ReverseColors(false);
        GameManager.instance.boss.Hit(GameManager.instance.boss.status.health / 7, 0, Quaternion.Euler(-90,0,0));
        //while (true)
        //{
        //    if ()
        //    {
        //        yield break;
        //    }
        //    yield return null;
        //}
    }

    private IEnumerator DisActive()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        menuAnim.gameObject.SetActive(false);
    }
}