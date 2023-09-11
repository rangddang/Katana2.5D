using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	protected Animator animator;

    [SerializeField] protected Transform target;
    

    private void Awake()
    {
        animator = transform.Find("Sprite").GetComponent<Animator>();
    }

    private void Update()
    {
        if(target != null)
        {
            
        }
    }
}
