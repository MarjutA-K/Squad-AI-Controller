using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage3 : MonoBehaviour
{
    public float damage;

    void Damage()
    {
        HealthController.currentHealthSquad4 -= damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Damage();
            print("collision");
        }
    }
}
