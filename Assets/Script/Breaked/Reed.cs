using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reed : BreakedObject
{
    public override void Breaked(float x, float y)
    {
        GameObject effect = Instantiate(breakedEffect);
        effect.transform.position = transform.position;
        effect.transform.rotation = Quaternion.Euler((90 * y), transform.eulerAngles.y + -(90 * x), 0);
    }
}
