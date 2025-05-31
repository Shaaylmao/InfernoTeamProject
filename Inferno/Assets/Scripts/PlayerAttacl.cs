using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacl : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.2f;
    public float attackDamage = 25f;

    [Header("References")]
    public Animator animator;
    public Transform attackOrigin;

    private float cooldownTimer = 0f;
    private TargetingSystem targetingSystem;

    void Start()
    {
        targetingSystem = GetComponent<TargetingSystem>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0)) // Left-click to attack
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        var target = targetingSystem.GetCurrentTarget();
        if (target == null || target.CurrentHealth <= 0f)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange)
        {
            return;
        }

        if (cooldownTimer <= 0f)
        {
            ExecuteAttack(target);
            cooldownTimer = attackCooldown;
        }
    }

    void ExecuteAttack(Targetable target)
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        target.CurrentHealth -= attackDamage;
        target.CurrentHealth = Mathf.Max(0, target.CurrentHealth);

        Debug.Log($"Attacked {target.name} for {attackDamage} damage");
    }

    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackOrigin.position, attackRange);
    }
}
