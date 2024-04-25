using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public sealed class CatchableLifeForm : MonoBehaviour
{
    [Tooltip("The species of this life form.")] [SerializeField]
    private Species species;

    [Tooltip("Called when a bug is going to be caught.")]
    public UnityEvent<CatchableLifeForm> OnCaught;

    [Tooltip("Called when a bug is going to be released.")]
    public UnityEvent<CatchableLifeForm> OnReleased;

    public LifeFormSpawner Spawner { get; set; }

    private readonly List<CatchableModule> _catchableModules = new();

    private bool _canBeSeen = true;

    /// <summary>
    /// Can this life form be caught?
    /// </summary>
    /// <param name="data">The current Bug Inventory Data.</param>
    /// <returns>True if can be caught, else false.</returns>
    public bool CanBeCaught(BugInventoryData data) => CatchButRelease(data) && data.CanCatchBug(species);

    /// <summary>
    /// If this bug is able to be caught does it need to be released?
    /// </summary>
    /// <returns>True if it can at least be released, else false.</returns>
    public bool CatchButRelease(BugInventoryData data) =>
        _catchableModules.All(cMod => cMod.CatchCheck()) && data.AnyJarsRemaining();

    /// <summary>
    /// Gets the Species.
    /// </summary>
    public Species Species => species;

    /// <summary>
    /// What to do when a Life Form is about to be caught.
    /// </summary>
    public void BugCaught() => OnCaught?.Invoke(this);

    /// <summary>
    /// What to do when a Life Form is about to be released.
    /// </summary>
    public void BugReleased() => OnReleased?.Invoke(this);

    /// <summary>
    /// Adds a CatchableModule to the list of Catchable Modules.
    /// </summary>
    /// <param name="catchableModule">A Catchable Module that you are adding for consideration.</param>
    public void AddModule(CatchableModule catchableModule) => _catchableModules.Add(catchableModule);

    /// <summary>
    /// Can this Life Form be seen?
    /// </summary>
    /// <returns>True if can be seen, else false.</returns>
    public bool CanBeSeen() => _canBeSeen;

    /// <summary>
    /// Sets whether or not this Life Form is visible.
    /// </summary>
    /// <param name="visible">Should it be visible or invisible?</param>
    public void SetVisibility(bool visible) => _canBeSeen = visible;
}