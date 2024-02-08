using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
public class CatchableLifeFormBehavior : MonoBehaviour
{
    [Tooltip("Default Action")] [SerializeField]
    private LifeFormAction defaultAction;
    
    
}