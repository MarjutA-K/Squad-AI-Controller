using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamege2 : MonoBehaviour
{
    public float damage;

    void Damage()
    {
        HealthController.currentHealthSquad3 -= damage;
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
