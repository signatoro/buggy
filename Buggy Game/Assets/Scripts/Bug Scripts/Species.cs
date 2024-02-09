using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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

    [FormerlySerializedAs("Prey")] [Tooltip("List of Prey.")] [SerializeField]
    private List<Species> prey = new();

    [FormerlySerializedAs("Predators")] [Tooltip("List of Predators.")] [SerializeField]
    private List<Species> predators = new();

    [FormerlySerializedAs("Neutral")] [Tooltip("List of Neutral.")] [SerializeField]
    private List<Species> neutral = new();

    /// <summary>
    /// Used to determine how to update the ecosystem.
    /// </summary>
    private List<Species> _preyOnLastValidate = new();

    private List<Species> _predatorsOnLastValidate = new();
    private List<Species> _neutralOnLastValidate = new();

    private void OnValidate()
    {
        UpdateConnectedEcosystem();
    }

    /// <summary>
    /// Updates all connections to this species.
    /// </summary>
    [ContextMenu("Update Connected Ecosystem")]
    private void UpdateConnectedEcosystem()
    {
        Debug.Log($"Updating Ecosystem for {speciesName}");

        // Figure Out what changed for each list
        List<Species> attemptingToAddToPrey = new();
        foreach (Species s in prey)
        {
            if (!_preyOnLastValidate.Contains(s))
            {
                attemptingToAddToPrey.Add(s);
            }
        }

        List<Species> attemptingToRemoveFromPrey = new();
        foreach (Species s in _preyOnLastValidate)
        {
            if (!prey.Contains(s))
            {
                attemptingToRemoveFromPrey.Add(s);
            }
        }

        List<Species> attemptingToAddToPredators = new();
        foreach (Species s in predators)
        {
            if (!_predatorsOnLastValidate.Contains(s))
            {
                attemptingToAddToPredators.Add(s);
            }
        }

        List<Species> attemptingToRemoveFromPredators = new();
        foreach (Species s in _predatorsOnLastValidate)
        {
            if (!predators.Contains(s))
            {
                attemptingToRemoveFromPredators.Add(s);
            }
        }

        List<Species> attemptingToAddToNeutral = new();
        foreach (Species s in neutral)
        {
            if (!_neutralOnLastValidate.Contains(s))
            {
                attemptingToAddToNeutral.Add(s);
            }
        }

        List<Species> attemptingToRemoveFromNeutral = new();
        foreach (Species s in _neutralOnLastValidate)
        {
            if (!neutral.Contains(s))
            {
                attemptingToRemoveFromNeutral.Add(s);
            }
        }

        // Don't add something that already exists in a list
        for (int i = attemptingToAddToPrey.Count - 1; i >= 0; i--)
        {
            if (predators.Contains(attemptingToAddToPrey[i]) ||
                neutral.Contains(attemptingToAddToPrey[i]))
            {
                Debug.LogWarning(
                    $"Attempting to add {attemptingToAddToPrey[i].speciesName} even though it already exists in the ecosystem");
                prey.Remove(attemptingToAddToPrey[i]);
                attemptingToAddToPrey.Remove(attemptingToAddToPrey[i]);
            }
        }

        for (int i = attemptingToAddToPredators.Count - 1; i >= 0; i--)
        {
            if (prey.Contains(attemptingToAddToPredators[i]) ||
                neutral.Contains(attemptingToAddToPredators[i]))
            {
                Debug.LogWarning(
                    $"Attempting to add {attemptingToAddToPredators[i].speciesName} even though it already exists in the ecosystem");
                predators.Remove(attemptingToAddToPredators[i]);
                attemptingToAddToPredators.Remove(attemptingToAddToPredators[i]);
            }
        }

        for (int i = attemptingToAddToNeutral.Count - 1; i >= 0; i--)
        {
            if (prey.Contains(attemptingToAddToNeutral[i]) ||
                predators.Contains(attemptingToAddToNeutral[i]))
            {
                Debug.LogWarning(
                    $"Attempting to add {attemptingToAddToNeutral[i].speciesName} even though it already exists in the ecosystem");
                neutral.Remove(attemptingToAddToNeutral[i]);
                attemptingToAddToNeutral.Remove(attemptingToAddToNeutral[i]);
            }
        }

        // Now you shouldn't have any duplicate species

        // If the given species is being added as a Prey, then make this species a Predator for the given Species
        foreach (Species s in attemptingToAddToPrey.Where(s => !s.predators.Contains(this)))
        {
            Debug.Log($"Adding {speciesName} to {s.speciesName} as a Predator");
            s.AddPredator(this);
        }

        // If the given species is being added as a Predator, then make this species a Prey for the given Species
        foreach (Species s in attemptingToAddToPredators.Where(s => !s.prey.Contains(this)))
        {
            Debug.Log($"Adding {speciesName} to {s.speciesName} as a Prey");
            s.AddPrey(this);
        }

        // If the given species is being added as Neutral, then make this species Neutral for the given Species
        foreach (Species s in attemptingToAddToNeutral.Where(s => !s.neutral.Contains(this)))
        {
            Debug.Log($"Adding {speciesName} to {s.speciesName} as Neutral");
            s.AddNeutral(this);
        }

        // If the given species is being removed as a Prey, then remove this species from the Predator list for the given Species
        foreach (Species s in attemptingToRemoveFromPrey)
        {
            Debug.Log($"Removing {speciesName} from {s.speciesName} as a Predator");
            s.RemovePredator(this);
        }

        // If the given species is being removed as a Predator, then remove this species from the Prey list for the given Species
        foreach (Species s in attemptingToRemoveFromPredators)
        {
            Debug.Log($"Removing {speciesName} from {s.speciesName} as a Prey");
            s.RemovePrey(this);
        }

        // If the given species is being removed as Neutral, then remove this species from the Neutral list for the given Species
        foreach (Species s in attemptingToRemoveFromNeutral)
        {
            Debug.Log($"Removing {speciesName} from {s.speciesName} as Neutral");
            s.RemoveNeutral(this);
        }

        // Now Every Connected Species should be updated

        // Update the last validated lists
        _preyOnLastValidate = new List<Species>(prey);
        _predatorsOnLastValidate = new List<Species>(predators);
        _neutralOnLastValidate = new List<Species>(neutral);
    }

    /// <summary>
    /// Adds the Species to the Prey List and updates the _preyOnLastValidate.
    /// </summary>
    /// <param name="s">The Species to Add.</param>
    public void AddPrey(Species s)
    {
        prey.Add(s);
        _preyOnLastValidate = new List<Species>(prey);
    }

    /// <summary>
    /// Adds the Species to the Predators List and updates the _predatorsOnLastValidate.
    /// </summary>
    /// <param name="s">The Species to Add.</param>
    public void AddPredator(Species s)
    {
        predators.Add(s);
        _predatorsOnLastValidate = new List<Species>(predators);
    }

    /// <summary>
    /// Adds the Species to the Neutral List and updates the _neutralOnLastValidate.
    /// </summary>
    /// <param name="s">The Species to Add.</param>
    public void AddNeutral(Species s)
    {
        neutral.Add(s);
        _neutralOnLastValidate = new List<Species>(neutral);
    }

    /// <summary>
    /// Removes the Species from the Prey List and updates the _preyOnLastValidate.
    /// </summary>
    /// <param name="s">The Species to Add.</param>
    public void RemovePrey(Species s)
    {
        prey.Remove(s);
        _preyOnLastValidate = new List<Species>(prey);
    }

    /// <summary>
    /// Removes the Species from the Predators List and updates the _predatorsOnLastValidate.
    /// </summary>
    /// <param name="s">The Species to Add.</param>
    public void RemovePredator(Species s)
    {
        predators.Remove(s);
        _predatorsOnLastValidate = new List<Species>(predators);
    }

    /// <summary>
    /// Removes the Species from the Neutral List and updates the _neutralOnLastValidate.
    /// </summary>
    /// <param name="s">The Species to Add.</param>
    public void RemoveNeutral(Species s)
    {
        neutral.Remove(s);
        _neutralOnLastValidate = new List<Species>(neutral);
    }

    /// <summary>
    /// Clears all Species Lists
    /// </summary>
    [ContextMenu("Clear Ecosystem")]
    public void ClearEcosystem()
    {
        prey.Clear();
        predators.Clear();
        neutral.Clear();
        _preyOnLastValidate.Clear();
        _predatorsOnLastValidate.Clear();
        _neutralOnLastValidate.Clear();
    }

    public bool HasConnectionToSpecies(Species species) =>
        prey.Contains(species) || predators.Contains(species) || neutral.Contains(species);
}