using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LifeFormBrain))]
[RequireComponent(typeof(CatchableLifeForm))]
public abstract class LifeFormAction : MonoBehaviour
{
    protected CatchableLifeForm CatchableLifeForm;

    internal virtual void Awake()
    {
        CatchableLifeForm = GetComponent<CatchableLifeForm>();
    }

    /// <summary>
    /// Performs the Action.
    /// </summary>
    /// <param name="position">Optional Position for the Action.</param>
    /// <returns>Nothing.</returns>
    public abstract IEnumerator PerformAction(Vector3 position = new());
}