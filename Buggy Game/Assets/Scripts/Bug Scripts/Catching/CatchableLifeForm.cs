using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public sealed class CatchableLifeForm : MonoBehaviour
{
    [Tooltip("The species of this life form.")] [SerializeField]
    private Species species;

    [Tooltip("Called when a bug is caught.")]
    public UnityEvent<CatchableLifeForm> OnCaught;

    private readonly List<CatchableModule> _catchableModules = new();

    /// <summary>
    /// Can this life form be caught?
    /// </summary>
    /// <param name="data">The current Bug Inventory Data.</param>
    /// <returns>True if can be caught, else false.</returns>
    public bool CanBeCaught(BugInventoryData data) =>
        _catchableModules.All(cMod => cMod.CatchCheck()) && data.CanCatchBug(species);

    /// <summary>
    /// Gets the Species.
    /// </summary>
    public Species Species => species;

    /// <summary>
    /// What to do when a Life Form is caught.
    /// </summary>
    public void BugCaught() => OnCaught?.Invoke(this);

    /// <summary>
    /// Adds a CatchableModule to the list of Catchable Modules.
    /// </summary>
    /// <param name="catchableModule">A Catchable Module that you are adding for consideration.</param>
    public void AddModule(CatchableModule catchableModule) => _catchableModules.Add(catchableModule);
}