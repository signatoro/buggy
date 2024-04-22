using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 - Resets time
 - Resets Spawn Point
 - Resets Jar amount
 - Gives new hints in the book
 - Fades out then in again
 - Saves your progress (Not for demo)
 */

[RequireComponent(typeof(Collider))]
public class Bed : MonoBehaviour
{
    #region Variables

    [Tooltip("The amount to change time by.")] [SerializeField]
    private GlobalFloat timeToChangeBy;

    [Tooltip("Spawn Point.")] [SerializeField]
    private Transform spawnPoint;

    [Tooltip("Bed Interaction Radius.")] [SerializeField]
    private GlobalFloat interactionRadius;

    [Tooltip("Bed Interaction Visuals.")] [SerializeField]
    private List<GameObject> visuals = new();
    
    [Tooltip("Sleep Keys")] [SerializeField]
    private GlobalKeyCodeList sleepKeys;

    [Tooltip("Time of Day Global Variable")] [SerializeField]
    private GlobalFloat timeOfDay;

    private GameObject _player;

    private CameraController _cameraController;

    private PlayerMovement _playerMovement;

    private BugInventoryData _bugInventoryData;

    private bool _couldInteractLastFrame;

    private bool _isSleeping;

    #endregion

    #region Methods

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _cameraController = _player.GetComponent<CameraController>();
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _bugInventoryData = _player.GetComponent<BugInventory>().GetBugInventoryData();
        SetVisuals(false);
    }

    private void Update()
    {
        if (_isSleeping) return;

        bool canInteract = CanInteract();
        if (canInteract)
        {
            SetVisuals(true);

            // Go to Sleep
            if (sleepKeys.PressingOneOfTheKeys())
            {
                StartCoroutine(GoToSleep());
            }
        }
        else if (_couldInteractLastFrame)
        {
            SetVisuals(false);
        }

        _couldInteractLastFrame = canInteract;
    }

    /// <summary>
    /// Is the player able to interact with the bed?
    /// </summary>
    /// <returns>True if the player can interact with the bed, else false.</returns>
    private bool CanInteract()
    {
        if (_player && _cameraController)
        {
            Ray ray = new Ray(_cameraController.Camera.transform.position, _cameraController.Camera.transform.forward);
            int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

            if (Physics.Raycast(ray, out var hit, interactionRadius.CurrentValue, layerMask))
            {
                if (hit.collider.GetComponent<Bed>())
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Coroutine for all actions that happen while you sleep.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator GoToSleep()
    {
        _isSleeping = true;

        SetSpawnPoint();
        ResetJars();
        UnlockNewBookHints();
        AdjustTimeOfDay();
        SetVisuals(false);

        yield return StartCoroutine(_playerMovement.Respawn());
        
        SaveGame();

        _isSleeping = false;
        yield return null;
    }

    /// <summary>
    /// Sets the spawn point of the player to the bed's spawn point.
    /// </summary>
    private void SetSpawnPoint()
    {
        if (spawnPoint)
        {
            _playerMovement.SetRespawnPoint(spawnPoint);
        }
    }

    /// <summary>
    /// Resets the glass jar count.
    /// </summary>
    private void ResetJars()
    {
        if (_bugInventoryData)
        {
            _bugInventoryData.ResetGlassJarCount();
        }
    }

    /// <summary>
    /// Unlocks new book hints. 
    /// </summary>
    private void UnlockNewBookHints()
    {
        // TODO: Implement this once the book is created.
    }

    /// <summary>
    /// Adjusts the time of day.
    /// </summary>
    private void AdjustTimeOfDay()
    {
        if (timeOfDay)
        {
            float newTime = timeOfDay.CurrentValue + timeToChangeBy.CurrentValue;
            while (newTime >= 24)
            {
                newTime -= 24;
            }

            timeOfDay.CurrentValue = newTime;
        }
    }

    /// <summary>
    /// Saves the game.
    /// </summary>
    private void SaveGame()
    {
        // TODO: Implement this once there is a save system.
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

    #endregion
}