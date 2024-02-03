using System.Collections;
using UnityEngine;

public class CatchableLifeForm : MonoBehaviour
{
    [Tooltip("The species of this life form.")] [SerializeField]
    private Species species;

    /// <summary>
    /// Can this Life Form be Caught?
    /// </summary>
    /// <returns>True if can be caught, else false.</returns>
    public virtual bool CanBeCaught() => !BugInventory.HasCaught(species);
    
    /// <summary>
    /// Gets the Species.
    /// </summary>
    public Species Species => species;

    /// <summary>
    /// What to do when a Life Form is caught.
    /// </summary>
    public virtual IEnumerator BugCaught()
    {
        Destroy(gameObject);
        yield return null;
    }
}