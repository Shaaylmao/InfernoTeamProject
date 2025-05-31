using UnityEngine;

public class Targetable : MonoBehaviour
{
    public string DisplayName = "Enemy";
    public float MaxHealth = 100f;
    public float CurrentHealth = 100f;

    public bool IsTargeted { get; private set; }

    [Header("Target Type")]
    public bool IsEnemy = true; // Set in Inspector for player (false) or enemy (true)

    [Header("Death Handling")]
    [SerializeField] private float destroyDelay = 2f; // Delay before destroy

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.red;

    private bool isDead = false;
    private GameOverManager gameOverManager;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        gameOverManager = FindObjectOfType<GameOverManager>();
    }

    public void SetTargeted(bool value)
    {
        IsTargeted = value;
        if (targetRenderer != null)
        {
            targetRenderer.material.color = value ? highlightColor : normalColor;
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

        if (CurrentHealth <= 0)
        {
            isDead = true;

            DisableMovement();

            if (!IsEnemy && gameOverManager != null)
            {
                gameOverManager.TriggerGameOver();
            }

            Destroy(gameObject, destroyDelay);
        }
    }

    private void DisableMovement()
    {
        MonoBehaviour[] components = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var comp in components)
        {
            if (comp != this && comp.enabled)
            {
                comp.enabled = false;
            }
        }
    }


}
