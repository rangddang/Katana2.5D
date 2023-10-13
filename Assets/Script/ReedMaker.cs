using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReedMaker : MonoBehaviour
{
    [SerializeField] private GameObject reed;
    [SerializeField] private float range = 10;
    [SerializeField] private float number = 100;

    private void Start()
    {
        for(int i = 0; i < number; i++)
        {
            //print(i);
            GameObject r = Instantiate(reed);
            r.transform.position = new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f, 1f)).normalized * range * Random.Range(0f, 1f);
            r.transform.position += new Vector3(0, 0.68f, 0);
        }
    }
}
