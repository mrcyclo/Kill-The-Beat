using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingObject : MonoBehaviour
{
    protected float health = 100;

    public bool IsDeath
    {
        get
        {
            return health <= 0;
        }
        protected set
        {
            health = 0;
        }
    }

    public abstract void TakeDamage(float damage);

    public abstract void Kill();
}
