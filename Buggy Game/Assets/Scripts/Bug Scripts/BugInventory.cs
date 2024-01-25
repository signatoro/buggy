using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BugInventory
{
    /// <summary>
    /// A list of all the unique species you have caught.
    /// </summary>
    private static readonly HashSet<Species> _bugsCaught = new();

    /// <summary>
    /// Gets the number of bugs caught.
    /// </summary>
    /// <returns>Number of bugs caught.</returns>
    public static int NumberOfBugsCaught() => _bugsCaught.Count;

    /// <summary>
    /// Adds a bug to the list of bugs caught.
    /// </summary>
    /// <param name="caughtBug">The bug you caught.</param>
    public static void AddBug(Species caughtBug) => _bugsCaught.Add(caughtBug);

    /// <summary>
    /// Clears the _bugsCaught list
    /// </summary>
    public static void ClearBugsCaught() => _bugsCaught.Clear();
}