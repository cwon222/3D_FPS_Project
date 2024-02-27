using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBarrel : InteractionObject
{
    [SerializeField]
    GameObject destructibleBarrelPiecess;

    bool isDestroyed = false;

    public override void TakeDamage(int damage)
    {
        currentHP -= damage;

        if(currentHP <= 0 && isDestroyed == false)
        {
            isDestroyed = true;

            Instantiate(destructibleBarrelPiecess, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
