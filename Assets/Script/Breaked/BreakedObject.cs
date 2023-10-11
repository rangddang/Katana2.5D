using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakedObject : MonoBehaviour
{
    [SerializeField] protected GameObject breakedEffect;

    public virtual void Breaked(float x, float y)
    {
        GameObject effect = Instantiate(breakedEffect);
        effect.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
