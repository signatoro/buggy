using UnityEngine;

public abstract class Sense : MonoBehaviour
{
    [Tooltip("Root Transform.")] [SerializeField]
    protected Transform root;

    [Tooltip("Sense Range.")] [SerializeField]
    protected float range;
}
