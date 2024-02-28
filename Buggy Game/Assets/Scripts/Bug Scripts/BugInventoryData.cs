using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
[CreateAssetMenu(fileName = "BugInventoryData<Name>", menuName = "Bug/Inventory Data")]
public class BugInventoryData : ScriptableObject
{
    [FormerlySerializedAs("_bugsCaught")] [Tooltip("The list of unique Species Caught.")] [SerializeField]
    private List<Species> bugsCaught = new();

    [Tooltip("Number of Glass Jars.")] [SerializeField]
    private GlobalInt glassJars;

    [Tooltip("Invokes when a Bug was Caught.")] [SerializeField]
    public UnityEvent OnBugCaught = new();

    /// <summary>
    /// Gets the number of bugs caught.
    /// </summary>
    /// <returns>Number of bugs caught.</returns>
    public int NumberOfBugsCaught() => bugsCaught.Count;

    /// <summary>
    /// Adds a bug to the list of bugs caught.
    /// </summary>
    /// <param name="caughtBug">The bug you caught.</param>
    private void AddBug(Species caughtBug) => bugsCaught.Add(caughtBug);

    /// <summary>
    /// Clears the _bugsCaught list.
    /// </summary>
    [ContextMenu("Clear Bug Inventory")]
    public void ClearBugsCaught() => bugsCaught.Clear();

    /// <summary>
    /// Have we caught the given Species?
    /// </summary>
    /// <param name="species">The Species we are checking.</param>
    /// <returns>True if we have caught it, else false.</returns>
    public bool HasCaught(Species species) => bugsCaught.Contains(species);

    /// <summary>
    /// Can we catch this Bug.
    /// </summary>
    /// <param name="species">The Species we are checking.</param>
    /// <returns>True if we can catch this Bug, else false.</returns>
    public bool CanCatchBug(Species species)
    {
        return !HasCaught(species) && AnyJarsRemaining();
    }

    /// <summary>
    /// Are there any Glass Jars remaining?
    /// </summary>
    /// <returns>True if there are Glass Jars remaining, else false.</returns>
    public bool AnyJarsRemaining() => glassJars.CurrentValue > 0;

    /// <summary>
    /// Sets the current value of the Glass Jars to the initial value.
    /// </summary>
    [ContextMenu("Reset Glass Jar Count")]
    public void ResetGlassJarCount() => glassJars.ResetValue();

    /// <summary>
    /// Uses 1 Glass Jar if there are any remaining.
    /// </summary>
    private void UseGlassJar() => glassJars.CurrentValue = glassJars.CurrentValue <= 0 ? 0 : glassJars.CurrentValue - 1;

    /// <summary>
    /// Catch the given Bug.
    /// </summary>
    /// <param name="species">The Species we are trying to catch.</param>
    public void CatchBug(Species species)
    {
        UseGlassJar();
        AddBug(species);
        OnBugCaught.Invoke();
    }
}