using System.Collections;
using UnityEngine;

/// <summary>
/// Lets out a loud yell!
/// </summary>
[RequireComponent(typeof(SoundGenerator))]
public class Power_Shriek : Power
{
    [Tooltip("Shriek Sound Effect")] [SerializeField]
    private InUniverseSound shriekSFX;

    [Tooltip("Shriek Cooldown")] [SerializeField]
    private GlobalFloat cooldownTime;

    private float _currentCooldown;

    private SoundGenerator _soundGenerator;

    /// <summary>
    /// Setup the Sound Generator.
    /// </summary>
    internal override void Awake()
    {
        _soundGenerator = GetComponent<SoundGenerator>();
        base.Awake();
    }

    /// <summary>
    /// Don't reset the cooldown.
    /// </summary>
    internal override void OnEnable()
    {
    }

    /// <summary>
    /// Don't reset the cooldown.
    /// </summary>
    internal override void OnDisable()
    {
    }


    /// <inheritdoc />
    public override IEnumerator ResetPower()
    {
        StopAllCoroutines();
        _currentCooldown = 0;
        yield return null;
    }

    /// <summary>
    /// Can execute if the cooldown is 0.
    /// </summary>
    public override void AttemptToExecute()
    {
        if (!IsActive())
        {
            StartCoroutine(ExecutePower());
        }
    }

    /// <summary>
    /// Plays the Shriek SFX then starts the cooldown.
    /// </summary>
    /// <returns>Nothing.</returns>
    public override IEnumerator ExecutePower()
    {
        _soundGenerator.PlaySound(shriekSFX);

        while (_currentCooldown < cooldownTime.CurrentValue)
        {
            _currentCooldown += Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine(StopPower());
        yield return null;
    }

    /// <summary>
    /// Sets the cooldown to 0.
    /// </summary>
    /// <returns>Nothing.</returns>
    public override IEnumerator StopPower()
    {
        _currentCooldown = 0;
        yield return null;
    }

    /// <inheritdoc />
    public override bool IsActive() => _currentCooldown > 0;
}