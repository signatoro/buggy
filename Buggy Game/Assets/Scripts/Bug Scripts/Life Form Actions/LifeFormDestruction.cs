using System.Collections.Generic;
using UnityEngine;

public class LifeFormDestruction : MonoBehaviour
{
    [Tooltip("The visuals to be instantiated on destruction.")] [SerializeField]
    private List<GameObject> destructionVisuals = new();

    /// <summary>
    /// Destroys the Life Form.
    /// </summary>
    public void Destroy()
    {
        foreach (GameObject visual in destructionVisuals)
        {
            Instantiate(visual, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}
