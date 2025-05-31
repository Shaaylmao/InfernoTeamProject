using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Targetable))]
public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackDamage = 10f;

    public LayerMask playerLayer;
    public Animator animator;

    private NavMeshAgent agent;
    private Transform player;
    private float cooldownTimer = 0f;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            if (distance > attackRange)
            {
                // Chase
                agent.isStopped = false;
                agent.SetDestination(player.position);
                if (animator) animator.SetBool("IsMoving", true);
            }
            else
            {
                // Attack
                agent.isStopped = true;
                if (animator) animator.SetBool("IsMoving", false);
                TryAttack();
            }
        }

        cooldownTimer -= Time.deltaTime;
    }

    void TryAttack()
    {
        if (cooldownTimer > 0f) return;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Targetable playerTarget = player.GetComponent<Targetable>();
        if (playerTarget != null)
        {
            playerTarget.CurrentHealth -= attackDamage;
            playerTarget.CurrentHealth = Mathf.Max(0, playerTarget.CurrentHealth);
        }

        cooldownTimer = attackCooldown;
        Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
