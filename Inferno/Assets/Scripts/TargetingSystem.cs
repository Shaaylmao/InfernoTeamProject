using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TargetingSystem : MonoBehaviour
{
    [Header("Targeting")]
    public float maxTargetDistance = 30f;
    public LayerMask targetableLayer;

    [Header("UI")]
    public GameObject targetUIPanel;
    public Slider healthSlider;

    private List<Targetable> availableTargets = new();
    private int currentTargetIndex = -1;
    private Targetable currentTarget;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        UpdateTargetUI(null);

        if (healthSlider != null)
        {
            healthSlider.interactable = false;
            var nav = healthSlider.navigation;
            nav.mode = Navigation.Mode.None;
            healthSlider.navigation = nav;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            FindAvailableTargets();
            CycleTarget();
        }

        if (Input.GetMouseButtonDown(0))
        {
            ClickSelectTarget();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearTarget();
        }

        if (currentTarget != null)
        {
            UpdateTargetUI(currentTarget);

            if (Vector3.Distance(transform.position, currentTarget.transform.position) > maxTargetDistance)
            {
                ClearTarget();
            }
        }
    }

    void FindAvailableTargets()
    {
        var allTargets = FindObjectsOfType<Targetable>();
        availableTargets = allTargets
            .Where(t => t != null && t.IsEnemy && Vector3.Distance(transform.position, t.transform.position) <= maxTargetDistance)
            .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .ToList();
    }

    void CycleTarget()
    {
        if (availableTargets.Count == 0) return;

        ClearTarget();

        currentTargetIndex = (currentTargetIndex + 1) % availableTargets.Count;
        currentTarget = availableTargets[currentTargetIndex];
        currentTarget.SetTargeted(true);
        UpdateTargetUI(currentTarget);
    }

    void ClickSelectTarget()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxTargetDistance, targetableLayer))
        {
            Targetable clicked = hit.collider.GetComponentInParent<Targetable>();
            if (clicked != null && clicked.IsEnemy)
            {
                ClearTarget();
                currentTarget = clicked;
                currentTarget.SetTargeted(true);
                UpdateTargetUI(currentTarget);
            }
        }
    }

    public void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.SetTargeted(false);
            currentTarget = null;
        }
        currentTargetIndex = -1;
        UpdateTargetUI(null);
    }

    void UpdateTargetUI(Targetable target)
    {
        if (target == null || !target.IsEnemy)
        {
            targetUIPanel.SetActive(false);
            return;
        }

        healthSlider.value = target.GetHealthPercent();
        targetUIPanel.SetActive(true);
    }

    public Targetable GetCurrentTarget() => currentTarget;
}
