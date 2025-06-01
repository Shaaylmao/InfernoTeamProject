using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Targetable : MonoBehaviour
{
    public delegate void OnDeathDelegate(Targetable target);
    public static event OnDeathDelegate OnTargetableDeath;

    public string DisplayName = "Enemy";
    public float MaxHealth = 100f;
    public float CurrentHealth = 100f;

    public bool IsTargeted { get; private set; }

    [Header("Target Type")]
    public bool IsEnemy = true;

    [Header("Death Settings")]
    [SerializeField] private float deathDelay = 2f;

    [Header("Highlight Settings")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.red;

    private bool isDead = false;
    private bool isHighlighting = false;
    private float pulseSpeed = 2f;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();
    }

    public void SetTargeted(bool value)
    {
        IsTargeted = value;
        isHighlighting = value;

        if (!isHighlighting)
        {
            SetBaseColor(normalColor);
        }
    }

    private void Update()
    {
        if (isHighlighting && targetRenderer != null)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            Color pulsingColor = Color.Lerp(normalColor, highlightColor, t);
            SetBaseColor(pulsingColor);
        }
    }

    private void SetBaseColor(Color color)
    {
        foreach (var mat in targetRenderer.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                mat.color = color;
            }
        }
    }

    public float GetHealthPercent()
    {
        return Mathf.Clamp01(CurrentHealth / MaxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);

        if (CurrentHealth <= 0f)
        {
            isDead = true;
            OnTargetableDeath?.Invoke(this);
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        Debug.Log($"{DisplayName} died.");

        DisableMovement();

        yield return new WaitForSeconds(deathDelay);

        if (!IsEnemy)
        {
            GameOverManager gameOver = FindObjectOfType<GameOverManager>();
            if (gameOver != null)
            {
                gameOver.TriggerGameOver();
            }
        }

        Destroy(gameObject);
    }

    private void DisableMovement()
    {
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();
        foreach (var comp in components)
        {
            if (comp != this)
                comp.enabled = false;
        }

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
    }
}
