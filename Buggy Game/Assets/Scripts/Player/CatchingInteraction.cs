using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(BugInventoryData))]
public class CatchingInteraction : MonoBehaviour
{
    [Tooltip("The distance away that we can catch things.")] [SerializeField]
    private GlobalFloat catchRange;

    [Tooltip("The reticule image.")] [SerializeField]
    private Image reticule;

    [Tooltip("The color of the reticule when you can catch something.")] [SerializeField]
    private GlobalColor canCatchColor;

    [Tooltip("The color of the reticule when you can attempt to catch something that you've already caught.")]
    [SerializeField]
    private GlobalColor canAttemptToCatchColor;

    [Tooltip("The color of the reticule when you can't catch something.")] [SerializeField]
    private GlobalColor cannotCatchColor;

    [Tooltip("The color of the reticule when nothing is interactable.")] [SerializeField]
    private GlobalColor reticuleInactive;

    [Tooltip("Catch KeyCodes.")] [SerializeField]
    private GlobalKeyCodeList catchKeycodes;

    private CatchableLifeForm _currentLifeForm = null;
    private Camera _camera;
    private BugInventory _bugInventory;

    [Header("Debug")] [Tooltip("Should we show the catching range gizmos?")] [SerializeField]
    private GlobalBool showCatchingRangeGizmos;

    private void Start()
    {
        reticule.color = reticuleInactive.CurrentValue;
        _camera = GetComponent<CameraController>().Camera;
        _bugInventory = GetComponent<BugInventory>();
        SetReticuleVisibility(true);
    }

    void Update()
    {
        if (catchKeycodes.PressingOneOfTheKeys())
        {
            // Do Catch Animation
        }

        CatchableLifeForm lifeForm = GetCatchableLifeFormWereFacing();
        if (lifeForm == null) // Not looking at an interactable
        {
            _currentLifeForm = null;
            reticule.color = reticuleInactive.CurrentValue;
            return;
        }

        if (lifeForm != _currentLifeForm) // First frame we're looking at this interactable
        {
            _currentLifeForm = lifeForm;
            return;
        }


        if (lifeForm.CanBeCaught(_bugInventory.GetBugInventoryData()))
        {
            reticule.color = canCatchColor.CurrentValue;
            if (catchKeycodes.PressingOneOfTheKeys())
            {
                _bugInventory.GetBugInventoryData().CatchBug(lifeForm.Species);
                lifeForm.BugCaught();
            }
        }
        else if (lifeForm.CatchButRelease())
        {
            reticule.color = canAttemptToCatchColor.CurrentValue;
            if (catchKeycodes.PressingOneOfTheKeys())
            {
                lifeForm.BugReleased();
            }
        }
        else
        {
            reticule.color = cannotCatchColor.CurrentValue;
        }
    }

    private CatchableLifeForm GetCatchableLifeFormWereFacing()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

        if (Physics.Raycast(ray, out var hit, catchRange.CurrentValue, layerMask))
        {
            if (hit.collider.GetComponent<CatchableLifeForm>())
            {
                CatchableLifeForm lifeForm = hit.collider.GetComponent<CatchableLifeForm>();
                return lifeForm;
            }
        }

        return null;
    }

    /// <summary>
    /// Determines whether to enable or disable the reticule.
    /// </summary>
    /// <param name="visible">Whether or not it is visible.</param>
    public void SetReticuleVisibility(bool visible)
    {
        reticule.enabled = visible;
    }

    private void OnDrawGizmos()
    {
        // Display the current catching range as a sphere
        if (_camera && showCatchingRangeGizmos && showCatchingRangeGizmos.CurrentValue)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(_camera.transform.position, catchRange.CurrentValue);
        }
    }
}