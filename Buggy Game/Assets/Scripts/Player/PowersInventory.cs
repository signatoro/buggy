using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Required to be on the player.
/// </summary>
[RequireComponent(typeof(BugInventory))]
public class PowersInventory : MonoBehaviour
{
    [Tooltip("Keys to use the current Power")] [SerializeField]
    private GlobalKeyCodeList usePowerKeys;

    [Tooltip("Keys to go to the next Power")] [SerializeField]
    private GlobalKeyCodeList swapToNextPowerKeys;

    [Tooltip("The Powers you have unlocked")] [SerializeField]
    private List<Power> unlockedPowers;

    private Power _currentPower;

    private bool _wasPressingAKeyLastFrame = true;

    private void OnValidate()
    {
        if (unlockedPowers.Count > 0)
        {
            _currentPower = unlockedPowers[^1];

            foreach (Power power in unlockedPowers)
            {
                power.enabled = _currentPower == power;
            }
        }
    }

    private void Start()
    {
        if (unlockedPowers.Count > 0)
        {
            _currentPower = unlockedPowers[0];

            foreach (Power power in unlockedPowers)
            {
                power.enabled = _currentPower == power;
            }
        }
    }

    private void Update()
    {
        if (!_currentPower) return;

        if (usePowerKeys.PressingOneOfTheKeys())
        {
            if (!_wasPressingAKeyLastFrame)
            {
                _currentPower.AttemptToExecute();
                _wasPressingAKeyLastFrame = true;
            }

            return;
        }

        if (swapToNextPowerKeys.PressingOneOfTheKeys())
        {
            if (!_wasPressingAKeyLastFrame)
            {
                _currentPower.enabled = false;
                int currentItem = unlockedPowers.IndexOf(_currentPower);
                currentItem = currentItem + 1 == unlockedPowers.Count ? 0 : currentItem + 1;
                _currentPower = unlockedPowers[currentItem];
                _currentPower.enabled = true;
                _wasPressingAKeyLastFrame = true;
            }

            return;
        }

        _wasPressingAKeyLastFrame = false;
    }
}