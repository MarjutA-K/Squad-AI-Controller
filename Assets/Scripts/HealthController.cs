using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public Slider[] healthSlider;
    public float maxHealth;
    public static float currentHealth;
    public static float currentHealthSquad2;
    public static float currentHealthSquad3;
    public static float currentHealthSquad4;

    private void Start()
    {
        currentHealth = maxHealth;
        currentHealthSquad2 = maxHealth;
        currentHealthSquad3 = maxHealth;
        currentHealthSquad4 = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider[0].value = CalculateHealth();
        healthSlider[1].value = CalculateHealth2();
        healthSlider[2].value = CalculateHealth3();
        healthSlider[3].value = CalculateHealth4();

        if (currentHealth == 0)
        {
            print("dead");
        }
    }

    float CalculateHealth()
    {
        return currentHealth / maxHealth;
    }

    float CalculateHealth2()
    {
        return currentHealthSquad2 / maxHealth;
    }

    float CalculateHealth3()
    {
        return currentHealthSquad3 / maxHealth;
    }

    float CalculateHealth4()
    {
        return currentHealthSquad4 / maxHealth;
    }
}
