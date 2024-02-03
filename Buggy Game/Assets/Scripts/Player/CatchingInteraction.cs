using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CameraController))]
public class CatchingInteraction : MonoBehaviour
{
    [Tooltip("The distance away that we can catch things.")] [SerializeField]
    private GlobalFloat catchRange;

    [Tooltip("The reticule image.")] [SerializeField]
    private Image reticule;

    [Tooltip("The color of the reticule when you can catch something.")] [SerializeField]
    private GlobalColor canCatchColor;

    [Tooltip("The color of the reticule when you can't catch something.")] [SerializeField]
    private GlobalColor cannotCatchColor;

    [Tooltip("The color of the reticule when nothing is interactable.")] [SerializeField]
    private GlobalColor reticuleInactive;

    [Tooltip("Catch KeyCodes.")] [SerializeField]
    private GlobalKeyCodeList catchKeycodes;

    private CatchableLifeForm _currentLifeForm = null;
    private Camera _camera;

    private void Start()
    {
        reticule.color = reticuleInactive.CurrentValue;
        _camera = GetComponent<CameraController>().Camera;
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
        }

        // Can catch a life form
        reticule.color = lifeForm.CanBeCaught() ? canCatchColor.CurrentValue : cannotCatchColor.CurrentValue;

        if (lifeForm.CanBeCaught() && catchKeycodes.PressingOneOfTheKeys())
        {
            BugInventory.AddBug(lifeForm.Species);
            StartCoroutine(lifeForm.BugCaught());
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
}