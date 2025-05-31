using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider healthSlider;
    public Targetable playerTarget;

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTarget = player.GetComponent<Targetable>();
        }

        if (healthSlider != null)
        {
            healthSlider.interactable = false;
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        float health = playerTarget.CurrentHealth;
        float maxHealth = playerTarget.MaxHealth;

        healthSlider.value = health / maxHealth;
    }
}
