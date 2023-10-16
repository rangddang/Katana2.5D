using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakedObject : MonoBehaviour
{
    [SerializeField] protected GameObject breakedEffect;
    protected bool isBreaked = false;

    public virtual void Breaked(float x, float y)
    {
        if (!isBreaked)
        {
            GameObject effect = Instantiate(breakedEffect);
            effect.transform.position = transform.position;
            gameObject.SetActive(false);
            isBreaked = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
