using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A33_Carbon : A22_Swords
{
    public override bool OnProjectileHit(Collider2D other, GameObject p)
    {
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && (object)damageable != status)
        {
            //Heal instead!
            damageable.HealDamage(damage * status.DamageDealMult);
            ParticleManager.SpawnParticles(0, p.transform.position, p.transform.localScale, p.transform.GetChild(0).transform.rotation);
            return true;
        }
        return false;
    }
}

