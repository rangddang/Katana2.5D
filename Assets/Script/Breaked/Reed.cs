using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reed : BreakedObject
{
    private Transform player;

    private Animator anim;
    

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public override void Breaked(Quaternion dir)
    {
        if (!isBreaked)
        {
            GameObject effect = Instantiate(breakedEffect);
            effect.transform.position = transform.position;
            effect.transform.rotation = dir;
            isBreaked = true;
            anim.SetBool("IsBreaked", isBreaked);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        player = other.transform;
    //        Move();
    //    }
    //}

    private void Move()
    {
        StartCoroutine("MoveReed");
    }

    private IEnumerator MoveReed()
    {
        float currentTime = 0;
        float moveSize = 45f;
        float moveInTime = 0.1f;
        float moveOutTime = 0.4f;

        float value = 0;

        Quaternion q = Quaternion.LookRotation(new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position);
        transform.rotation = q;
        
        while (true)
        {
            currentTime += Time.deltaTime;
            if(currentTime < moveInTime)
            {
                value = Mathf.Lerp(0, moveSize, currentTime / moveInTime);
                transform.rotation = Quaternion.Euler(-value, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else
            {
                value = Mathf.Lerp(moveSize, 0, (currentTime - moveInTime) / moveOutTime);
                transform.rotation = Quaternion.Euler(-value, transform.eulerAngles.y, transform.eulerAngles.z);
            }

            if(currentTime > (moveInTime + moveOutTime))
            {
                yield break;
            }
            yield return null;
        }
    }
}
