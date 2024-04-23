using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BookPhysical : MonoBehaviour
{
    [Tooltip("Book Interaction Radius.")] [SerializeField]
    private GlobalFloat interactionRadius;
    
    [Tooltip("Book Interaction Visuals.")] [SerializeField]
    private List<GameObject> visuals = new();
    
    [Tooltip("Read Keys")] [SerializeField]
    private GlobalKeyCodeList readKeys;

    [Tooltip("Book UI")] [SerializeField] private GameObject bookUI;
    
    private GameObject _player;

    private CameraController _cameraController;
    
    private bool _couldInteractLastFrame;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _cameraController = _player.GetComponent<CameraController>();
    }

    // Update is called once per frame
    private void Update()
    {
        bool canInteract = CanInteract();
        if (canInteract)
        {
            SetVisuals(true);

            // Start Reading
            if (readKeys.PressingOneOfTheKeys())
            {
                bookUI.SetActive(true);
            }
        }
        else if (_couldInteractLastFrame)
        {
            SetVisuals(false);
        }

        _couldInteractLastFrame = canInteract;
    }
    
    /// <summary>
    /// Is the player able to interact with the book?
    /// </summary>
    /// <returns>True if the player can interact with the book, else false.</returns>
    private bool CanInteract()
    {
        if (_player && _cameraController)
        {
            Ray ray = new Ray(_cameraController.Camera.transform.position, _cameraController.Camera.transform.forward);
            int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

            if (Physics.Raycast(ray, out var hit, interactionRadius.CurrentValue, layerMask))
            {
                if (hit.collider.GetComponent<BookPhysical>())
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    /// <summary>
    /// Set the visibility of the visuals to the incoming value.
    /// </summary>
    /// <param name="value">Visuals are active if true and inactive if false.</param>
    private void SetVisuals(bool value)
    {
        if (visuals != null)
        {
            foreach (GameObject visual in visuals)
            {
                visual.SetActive(value);
            }
        }
    }
}
