using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LifeFormSpawner : MonoBehaviour
{
    [Tooltip("Life Form to Spawn.")] [SerializeField]
    private CatchableLifeForm lifeForm;

    [Tooltip("Spawn Radius.")] [SerializeField]
    private GlobalFloat spawnRadius;

    [Tooltip("Spawn Amount.")] [SerializeField]
    private GlobalInt spawnAmount;

    [Tooltip("Spawn Cadence.")] [SerializeField]
    private GlobalFloat spawnCadence;

    [Tooltip("Spawn Visuals. Should Destroy themselves.")] [SerializeField]
    private List<GameObject> spawnVisuals = new();

    private List<CatchableLifeForm> _currentlyActiveLifeForms = new();

    private void Start()
    {
        StartCoroutine(ShouldSpawn());
    }

    /// <summary>
    /// Should we spawn a Life Form?
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator ShouldSpawn()
    {
        if (_currentlyActiveLifeForms.Count < spawnAmount.CurrentValue)
        {
            Spawn();
        }

        yield return new WaitForSeconds(spawnCadence.CurrentValue);
        StartCoroutine(ShouldSpawn());
        yield return null;
    }

    /// <summary>
    /// Spawns a Life Form in the spawn Area.
    /// </summary>
    private void Spawn()
    {
        float randomX = Random.Range(-1.0f * spawnRadius.CurrentValue, spawnRadius.CurrentValue);
        float randomZ = Random.Range(-1.0f * spawnRadius.CurrentValue, spawnRadius.CurrentValue);
        Vector2 twoDVector = new Vector2(randomX, randomZ).normalized;
        Vector3 spawnPos = new Vector3(twoDVector.x * spawnRadius.CurrentValue, 0f,
            twoDVector.y * spawnRadius.CurrentValue) + transform.position;

        CatchableLifeForm life = Instantiate(lifeForm, spawnPos, Quaternion.identity);
        life.Spawner = this;
        _currentlyActiveLifeForms.Add(life);
        foreach (GameObject visual in spawnVisuals)
        {
            Instantiate(visual, spawnPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// Removes a Life Form from the spawner's list if it gets destroyed.
    /// </summary>
    /// <param name="catchableLifeForm">The Life Form to remove.</param>
    public void RemoveLifeForm(CatchableLifeForm catchableLifeForm)
    {
        _currentlyActiveLifeForms.Remove(catchableLifeForm);
    }

    /// <summary>
    /// Gets the Spawn Radius.
    /// </summary>
    /// <returns>The spawn radius.</returns>
    public float GetSpawnRadius() => spawnRadius.CurrentValue;
}