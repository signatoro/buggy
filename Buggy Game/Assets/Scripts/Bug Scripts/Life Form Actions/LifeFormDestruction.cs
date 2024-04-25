using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
public class LifeFormDestruction : MonoBehaviour
{
    [Tooltip("The visuals to be instantiated on destruction.")] [SerializeField]
    private List<GameObject> destructionVisuals = new();

    private CatchableLifeForm _catchableLifeForm;

    private void Awake()
    {
        _catchableLifeForm = GetComponent<CatchableLifeForm>();
    }

    /// <summary>
    /// Destroys the Life Form.
    /// </summary>
    [ContextMenu("Destroy")]
    public void Destroy()
    {
        foreach (GameObject visual in destructionVisuals)
        {
            Instantiate(visual, transform.position, Quaternion.identity);
        }

        if (_catchableLifeForm && _catchableLifeForm.Spawner)
        {
            _catchableLifeForm.Spawner.RemoveLifeForm(_catchableLifeForm);
        }

        Destroy(gameObject);
    }
}