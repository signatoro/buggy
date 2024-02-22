using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
public abstract class CatchableModule : MonoBehaviour
{
    private CatchableLifeForm _catchableLifeForm;

    // Start is called before the first frame update
    internal virtual void Start()
    {
        _catchableLifeForm = GetComponent<CatchableLifeForm>();
        _catchableLifeForm.AddModule(this);
    }

    /// <summary>
    /// Is this module passed?
    /// </summary>
    /// <returns>True if it is passed, else false.</returns>
    public abstract bool CatchCheck();
}