using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillBoard : MonoBehaviour
{
    [SerializeField] private bool freezeXZAxis = true;

    private void LateUpdate()
    {
        if (freezeXZAxis)
        {
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.y, 0f);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
