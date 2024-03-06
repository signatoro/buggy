using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Makes all the sounds for a Life Form.
/// </summary>
[RequireComponent(typeof(CatchableLifeForm))]
public class SoundGenerator : MonoBehaviour
{
    [Tooltip("Root Transform of Sounds")] [SerializeField]
    private Transform root;

    [Tooltip("What to do when disabled.")] public UnityEvent OnDisabled = new();

    private CatchableLifeForm _catchableLifeForm;

    private void Awake()
    {
        _catchableLifeForm = GetComponent<CatchableLifeForm>();
    }

    private void OnDisable()
    {
        OnDisabled?.Invoke();
    }

    /// <summary>
    /// Plays a sound in universe.
    /// </summary>
    /// <param name="sound">The Sound to play.</param>
    public void PlaySound(InUniverseSound sound)
    {
        sound.CatchableLifeForm = _catchableLifeForm;
        Instantiate(sound, root.position, Quaternion.identity);
    }
}