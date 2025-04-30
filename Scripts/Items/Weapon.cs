using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePointTrans;

    protected float shootIntervalWaitTime, shootFrequencyWaitTime;

    public virtual IEnumerator Shoot()
    {
        yield return null;
    }
}
