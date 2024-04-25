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

    [Tooltip("The color of the reticule when nothing is intractable.")] [SerializeField]
    private GlobalColor reticuleInactive;

    [Tooltip("Catch KeyCodes.")] [SerializeField]
    private GlobalKeyCodeList catchKeycodes;

    private Animator _animator;

    private CatchableLifeForm _currentLifeForm = null;
    private Camera _camera;
    private BugInventory _bugInventory;

    [Header("Debug")] [Tooltip("Should we show the catching range gizmos?")] [SerializeField]
    private GlobalBool showCatchingRangeGizmos;

    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

    private void Start()
    {
        ResetReticuleColor();
        _camera = GetComponent<CameraController>().Camera;
        _bugInventory = GetComponent<BugInventory>();
        _animator = GetComponent<Animator>();
        SetReticuleVisibility(true);
    }

    void Update()
    {
        CatchableLifeForm lifeForm = GetCatchableLifeFormWereFacing();
        if (lifeForm == null) // Not looking at an interactable
        {
            _currentLifeForm = null;
            ResetReticuleColor();
            return;
        }

        if (lifeForm != _currentLifeForm) // First frame we're looking at this interactable
        {
            _currentLifeForm = lifeForm;
            return;
        }


        if (lifeForm.CanBeCaught(_bugInventory.GetBugInventoryData()))
        {
            SetReticuleColor(canCatchColor.CurrentValue);
            if (catchKeycodes.PressingOneOfTheKeys())
            {
                if (_animator)
                {
                    _animator.SetBool(IsAttacking, true);
                }

                _bugInventory.GetBugInventoryData().CatchBug(lifeForm.Species);
                lifeForm.BugCaught();
            }
        }
        else if (lifeForm.CatchButRelease(_bugInventory.GetBugInventoryData()))
        {
            SetReticuleColor(canAttemptToCatchColor.CurrentValue);
            if (catchKeycodes.PressingOneOfTheKeys())
            {
                if (_animator)
                {
                    _animator.SetBool(IsAttacking, true);
                }

                lifeForm.BugReleased();
            }
        }
        else
        {
            SetReticuleColor(cannotCatchColor.CurrentValue);
        }
    }

    /// <summary>
    /// Gets the Catchable Life Form that we are looking at.
    /// </summary>
    /// <returns>The Catchable Life Form that we are facing.</returns>
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
    private void SetReticuleVisibility(bool visible)
    {
        reticule.enabled = visible;
    }

    /// <summary>
    /// Sets the reticule's color to the given color.
    /// </summary>
    /// <param name="color">The color to set the reticule to.</param>
    private void SetReticuleColor(Color color)
    {
        reticule.color = color;
    }

    /// <summary>
    /// Resets the reticule's color to it's inactive color.
    /// </summary>
    private void ResetReticuleColor()
    {
        SetReticuleColor(reticuleInactive.CurrentValue);
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

    /// <summary>
    /// Ends the animation.
    /// </summary>
    public void EndAnimation()
    {
        if (_animator)
        {
            _animator.SetBool(IsAttacking, false);
        }
    }
}