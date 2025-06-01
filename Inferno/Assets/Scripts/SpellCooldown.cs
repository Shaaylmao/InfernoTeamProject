using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellCooldown : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image buffCooldownImage;
    [SerializeField] private TMP_Text buffCooldownText;

    [SerializeField] private Image rangedCooldownImage;
    [SerializeField] private TMP_Text rangedCooldownText;

    [Header("Cooldown Durations")]
    [SerializeField] private float buffCooldownDuration = 15f;
    [SerializeField] private float rangedCooldownDuration = 45f;

    [Header("Ability Flags")]
    public bool isBuffReady = true;
    public bool isRangedReady = true;

    private float buffTimer = 0f;
    private float rangedTimer = 0f;

    private TargetingSystem targetingSystem;

    void Start()
    {
        targetingSystem = GetComponent<TargetingSystem>();

        // Initialize UI
        SetCooldownUI(buffCooldownImage, buffCooldownText, 0f);
        SetCooldownUI(rangedCooldownImage, rangedCooldownText, 0f);
    }

    void Update()
    {
        // Only work if an enemy is targeted
        if (targetingSystem == null || targetingSystem.GetCurrentTarget() == null)
            return;

        // Handle Buff Ability (Key 1)
        if (Input.GetKeyDown(KeyCode.Alpha1) && isBuffReady)
        {
            if (TryUseBuff())
            {
                isBuffReady = false;
                buffTimer = buffCooldownDuration;
                SetCooldownUI(buffCooldownImage, buffCooldownText, 1f);
            }
        }

        // Handle Ranged Magic (Key 2)
        if (Input.GetKeyDown(KeyCode.Alpha2) && isRangedReady)
        {
            if (TryUseRanged())
            {
                isRangedReady = false;
                rangedTimer = rangedCooldownDuration;
                SetCooldownUI(rangedCooldownImage, rangedCooldownText, 1f);
            }
        }

        // Update Cooldowns
        if (!isBuffReady)
        {
            buffTimer -= Time.deltaTime;
            UpdateCooldown(buffTimer, buffCooldownDuration, buffCooldownImage, buffCooldownText, () => isBuffReady = true);
        }

        if (!isRangedReady)
        {
            rangedTimer -= Time.deltaTime;
            UpdateCooldown(rangedTimer, rangedCooldownDuration, rangedCooldownImage, rangedCooldownText, () => isRangedReady = true);
        }
    }

    void UpdateCooldown(float timer, float duration, Image img, TMP_Text txt, System.Action onReady)
    {
        if (timer <= 0)
        {
            img.fillAmount = 0f;
            txt.gameObject.SetActive(false);
            onReady?.Invoke();
        }
        else
        {
            img.fillAmount = timer / duration;
            txt.gameObject.SetActive(true);
            txt.text = Mathf.CeilToInt(timer).ToString();
        }
    }

    void SetCooldownUI(Image img, TMP_Text txt, float fillAmount)
    {
        if (img != null)
            img.fillAmount = fillAmount;
        if (txt != null)
            txt.gameObject.SetActive(false);
    }

    // Link to ability logic
    bool TryUseBuff()
    {
        var playerAttack = GetComponent<PlayerAttacl>();
        if (playerAttack != null)
        {
            playerAttack.ActivateBuff();
            return true;
        }
        return false;
    }

    bool TryUseRanged()
    {
        var playerAttack = GetComponent<PlayerAttacl>(); // Assuming the typo is in original script
        if (playerAttack != null)
        {
            playerAttack.CastRangedAttack();
            return true;
        }
        return false;
    }
}
