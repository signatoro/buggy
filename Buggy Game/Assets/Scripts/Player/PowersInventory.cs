using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Required to be on the player.
/// </summary>
public class PowersInventory : MonoBehaviour
{
    [Serializable]
    private struct UnlockOnSpecies
    {
        [Tooltip("The Power to Unlock")] public Power PowerUnlock;

        [Tooltip("The Species that, when caught, Unlocks the Power")]
        public Species SpeciesUnlock;
    }

    [Serializable]
    private struct UnlockOnCount
    {
        [Tooltip("The Power to Unlock")] public Power PowerUnlock;

        [Tooltip("The Number of Bugs Caught that Unlocks the Power")]
        public GlobalInt UnlockCount;
    }

    [Tooltip("Keys to use the first Power")] [SerializeField]
    private GlobalKeyCodeList useFirstPowerKeys;

    [Tooltip("Keys to use the second Power")] [SerializeField]
    private GlobalKeyCodeList useSecondPowerKeys;

    [Tooltip("Keys to use the third Power")] [SerializeField]
    private GlobalKeyCodeList useThirdPowerKeys;

    [Tooltip("Keys to use the fourth Power")] [SerializeField]
    private GlobalKeyCodeList useFourthPowerKeys;

    [Tooltip("The Powers you have unlocked")] [SerializeField]
    private List<Power> unlockedPowers = new();

    [Tooltip("Powers to Cycle Between with Unlock Conditions")] [SerializeField]
    private List<UnlockOnSpecies> useableUnlockablePowers = new();

    [Tooltip("Passive Powers with Unlock Conditions")] [SerializeField]
    private List<UnlockOnCount> passiveUnlockablePowers = new();

    private Power _currentPower;

    private bool _wasPressingAKeyLastFrame = true;

    private BugInventoryData _bugInventoryData;

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
        _bugInventoryData = GetComponent<BugInventory>().GetBugInventoryData();
        _bugInventoryData.OnBugCaught.AddListener(CheckForUnlock);

        if (unlockedPowers.Count > 0)
        {
            _currentPower = unlockedPowers[0];

            foreach (Power power in unlockedPowers)
            {
                power.enabled = _currentPower == power;
            }
        }

        CheckForUnlock();
    }

    private void Update()
    {
        if (!_currentPower) return;

        if (useFirstPowerKeys.PressingOneOfTheKeys() ||
            useSecondPowerKeys.PressingOneOfTheKeys() ||
            useThirdPowerKeys.PressingOneOfTheKeys() ||
            useFourthPowerKeys.PressingOneOfTheKeys())
        {
            if (!_wasPressingAKeyLastFrame)
            {
                Power tempPower = null;
                if (useFirstPowerKeys.PressingOneOfTheKeys())
                {
                    tempPower = unlockedPowers.Count >= 1 ? unlockedPowers[0] : null;
                }
                else if (useSecondPowerKeys.PressingOneOfTheKeys())
                {
                    tempPower = unlockedPowers.Count >= 2 ? unlockedPowers[1] : null;
                }
                else if (useThirdPowerKeys.PressingOneOfTheKeys())
                {
                    tempPower = unlockedPowers.Count >= 3 ? unlockedPowers[2] : null;
                }
                else if (useFourthPowerKeys.PressingOneOfTheKeys())
                {
                    tempPower = unlockedPowers.Count >= 4 ? unlockedPowers[3] : null;
                }

                if (tempPower && tempPower != _currentPower)
                {
                    _currentPower.enabled = false;
                    _currentPower = tempPower;
                    _currentPower.enabled = true;
                }

                if (tempPower)
                {
                    _currentPower.AttemptToExecute();
                }
            }

            _wasPressingAKeyLastFrame = true;

            return;
        }

        _wasPressingAKeyLastFrame = false;
    }

    /// <summary>
    /// Checks for if we can Unlock a new Power.
    /// </summary>
    public void CheckForUnlock()
    {
        // Unlock useable Powers
        foreach (UnlockOnSpecies unlockable in useableUnlockablePowers.Where(unlockable =>
                     !unlockedPowers.Contains(unlockable.PowerUnlock) &&
                     _bugInventoryData.HasCaught(unlockable.SpeciesUnlock)))
        {
            UnlockUseable(unlockable.PowerUnlock);
        }

        foreach (UnlockOnCount unlockable in passiveUnlockablePowers.Where(unlockable =>
                     _bugInventoryData.NumberOfBugsCaught() >= unlockable.UnlockCount.CurrentValue))
        {
            UnlockPassive(unlockable.PowerUnlock);
        }
    }

    /// <summary>
    /// Unlocks a useable Power.
    /// </summary>
    /// <param name="power">The Power to Unlock.</param>
    private void UnlockUseable(Power power)
    {
        unlockedPowers.Add(power);
        _currentPower = power;

        foreach (Power p in unlockedPowers)
        {
            p.enabled = _currentPower == p;
        }
    }

    /// <summary>
    /// Unlocks a passive Power.
    /// </summary>
    /// <param name="power">The Power to Unlock.</param>
    private void UnlockPassive(Power power)
    {
        power.enabled = true;
    }
}