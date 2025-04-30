using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetHurt
{
    public void GetHurt(Vector2 attackerPos, float attackForce, int damage);
}
