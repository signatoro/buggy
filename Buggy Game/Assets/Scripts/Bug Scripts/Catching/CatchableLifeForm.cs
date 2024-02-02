using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CatchableLifeForm : MonoBehaviour
{
    [Tooltip("The species of this life form.")] [SerializeField]
    private Species species;

    /// <summary>
    /// Can this Life Form be Caught?
    /// </summary>
    /// <returns>True if can be caught, else false.</returns>
    public virtual bool CanBeCaught() => true;
}