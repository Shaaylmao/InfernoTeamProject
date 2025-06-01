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

    [Header("Buff Ability Settings (Key 1)")]
    public float buffedAttackDamage = 50f;
    public float buffDuration = 5f;
    public float buffCooldown = 15f;
    public float healAmount = 30f;
    public string buffAnimationTrigger = "Buff";

    private bool isBuffed = false;
    private float buffCooldownTimer = 0f;

    [Header("Ranged Magic Ability (Key 2)")]
    public float magicCooldown = 45f;
    public float magicDamage = 75f;
    public float magicRange = 10f;
    public GameObject magicImpactEffect; // particle effect prefab
    public string magicAnimationTrigger = "CastSpell"; // optional animation trigger

    private float magicCooldownTimer = 0f;

    void Start()
    {
        targetingSystem = GetComponent<TargetingSystem>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        buffCooldownTimer -= Time.deltaTime;
        magicCooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0)) TryAttack();
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryBuffAbility();
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryMagicAbility();
    }

    void TryAttack()
    {
        var target = targetingSystem.GetCurrentTarget();
        if (target == null || target.CurrentHealth <= 0f) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange) return;

        if (cooldownTimer <= 0f)
        {
            ExecuteAttack(target);
            cooldownTimer = attackCooldown;
        }
    }

    void ExecuteAttack(Targetable target)
    {
        if (animator) animator.SetTrigger("Attack");

        float finalDamage = isBuffed ? buffedAttackDamage : attackDamage;
        target.TakeDamage(finalDamage);

        Debug.Log($"Attacked {target.name} for {finalDamage} damage");
    }

    void TryBuffAbility()
    {
        if (buffCooldownTimer > 0f) return;

        isBuffed = true;
        if (animator && !string.IsNullOrEmpty(buffAnimationTrigger))
            animator.SetTrigger(buffAnimationTrigger);

        Targetable playerTarget = GetComponent<Targetable>();
        if (playerTarget != null)
            playerTarget.CurrentHealth = Mathf.Clamp(playerTarget.CurrentHealth + healAmount, 0, playerTarget.MaxHealth);

        buffCooldownTimer = buffCooldown;
        StartCoroutine(EndBuffAfterDuration());
    }

    IEnumerator EndBuffAfterDuration()
    {
        yield return new WaitForSeconds(buffDuration);
        isBuffed = false;
    }

    void TryMagicAbility()
    {
        if (magicCooldownTimer > 0f) return;

        var target = targetingSystem.GetCurrentTarget();
        if (target == null || target.CurrentHealth <= 0f) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > magicRange) return;

        if (animator && !string.IsNullOrEmpty(magicAnimationTrigger))
            animator.SetTrigger(magicAnimationTrigger);

        target.TakeDamage(magicDamage);
        Debug.Log($"Cast magic on {target.name} for {magicDamage} damage.");

        if (magicImpactEffect != null)
        {
            Instantiate(magicImpactEffect, target.transform.position + Vector3.up, Quaternion.identity);
        }

        magicCooldownTimer = magicCooldown;
    }

    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackOrigin.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackOrigin.position, magicRange);
    }
}
