using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Species<Name>", menuName = "Bug/Species")]
public class Species : ScriptableObject
{
    [Tooltip("Name of the Species.")] [SerializeField]
    private string speciesName;

    [Tooltip("Description of species before it is caught.")] [SerializeField]
    private string preCaughtDescription;

    [Tooltip("Description of species after it is caught.")] [SerializeField]
    private string postCaughtDescription;

    [Tooltip("Silhouette.")] [SerializeField]
    private Sprite silhouette;

    [Tooltip("Full Sprite.")] [SerializeField]
    private Sprite fullSprite;

    [Tooltip("List of Prey.")] [SerializeField]
    private List<Species> prey;

    [Tooltip("List of Predators.")] [SerializeField]
    private List<Species> predators;

    [Tooltip("List of Neutral.")] [SerializeField]
    private List<Species> neutral;
}