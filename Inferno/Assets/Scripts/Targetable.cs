using UnityEngine;

public class Targetable : MonoBehaviour
{
    public string DisplayName = "Enemy";
    public float MaxHealth = 100f;
    public float CurrentHealth = 100f;

    public bool IsTargeted { get; private set; }

    [Header("Target Type")]
    public bool IsEnemy = true; // Set this in Inspector for enemies (true) or player (false)

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.red;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();
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
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
    }

}
