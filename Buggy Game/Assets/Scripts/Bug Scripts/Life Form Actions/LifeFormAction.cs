using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LifeFormBrain))]
public abstract class LifeFormAction : MonoBehaviour
{
    /// <summary>
    /// Performs the Action.
    /// </summary>
    /// <param name="position">Optional Position for the Action.</param>
    /// <returns>Nothing.</returns>
    public abstract IEnumerator PerformAction(Vector3 position = new());
}