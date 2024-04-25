using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class Power_Silence : Power
{
    [Tooltip("Spawn Position")] [SerializeField]
    private Transform rootPosition;

    [FormerlySerializedAs("silenceTriggerRange")] [Tooltip("Range of the Silence Sphere")] [SerializeField]
    private GlobalFloat silenceSphereRange;

    [FormerlySerializedAs("silenceTriggerGrowLerpTime")]
    [Tooltip("Time for the Silence Sphere to Grow to full size")]
    [SerializeField]
    private GlobalFloat silenceSphereGrowLerpTime;

    [FormerlySerializedAs("timeBeforeNextUse")] [Tooltip("Added Time Before Next Use")] [SerializeField]
    private GlobalFloat timeAdditionalCooldown;

    [Tooltip("Time the Sphere remains at")] [SerializeField]
    private GlobalFloat timeActive;

    [Tooltip("VFX")] [SerializeField] private VisualEffect silenceVFX;

    private bool _canUse;

    private CatchableLifeForm _catchableLifeForm;

    private HashSet<CatchableLifeForm> _lifeFormsSilenced = new();

    private HashSet<InUniverseSound> _soundsSilenced = new();

    /// <summary>
    /// Setup the Sound Generator.
    /// </summary>
    internal override void Awake()
    {
        _catchableLifeForm = GetComponent<CatchableLifeForm>();
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
    [ContextMenu("Reset Power")]
    public override IEnumerator ResetPower()
    {
        // Enable Speaking and Hearing for the Catchable Life Forms no longer hit
        foreach (CatchableLifeForm lifeForm in _lifeFormsSilenced)
        {
            if (lifeForm.GetComponent<Sense_Sound>())
            {
                lifeForm.GetComponent<Sense_Sound>().enabled = true;
            }

            if (lifeForm.GetComponent<SoundGenerator>())
            {
                lifeForm.GetComponent<SoundGenerator>().enabled = true;
            }
        }

        // Unmute all sounds no longer hit
        foreach (InUniverseSound sound in _soundsSilenced)
        {
            sound.audioSource.mute = false;
        }

        // Reset Silenced Life Forms
        _lifeFormsSilenced = new HashSet<CatchableLifeForm>();
        _soundsSilenced = new HashSet<InUniverseSound>();
        _canUse = true;

        if (silenceVFX)
        {
            silenceVFX.Stop();
        }
        
        yield return null;
    }

    /// <summary>
    /// Can only Execute if _canUse is true.
    /// </summary>
    [ContextMenu("Attempt to Execute Power")]
    public override void AttemptToExecute()
    {
        if (!IsActive())
        {
            StartCoroutine(ExecutePower());
            StartCoroutine(StopPower());
        }
    }

    /// <summary>
    /// Handles the logic for Silencing.
    /// </summary>
    /// <returns>Nothing.</returns>
    [ContextMenu("Execute Power")]
    public override IEnumerator ExecutePower()
    {
        _canUse = false;
        yield return StartCoroutine(GrowSphere());
        yield return StartCoroutine(MaintainSphere());
        yield return StartCoroutine(ShrinkSphere());
        yield return null;
    }

    /// <summary>
    /// Un-silences all Life Forms affected.
    /// </summary>
    /// <returns>Nothing.</returns>
    [ContextMenu("Stop Power")]
    public override IEnumerator StopPower()
    {
        yield return new WaitForSeconds(timeAdditionalCooldown.CurrentValue + timeActive.CurrentValue +
                                        2 * silenceSphereGrowLerpTime.CurrentValue);
        yield return StartCoroutine(ResetPower());
        yield return null;
    }

    /// <summary>
    /// Grows the Sphere.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator GrowSphere()
    {
        float timer = 0;

        while (timer < silenceSphereGrowLerpTime.CurrentValue)
        {
            HandleSilencing(Mathf.Lerp(0, silenceSphereRange.CurrentValue,
                timer / silenceSphereGrowLerpTime.CurrentValue));
            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Keeps the Sphere at max size.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator MaintainSphere()
    {
        if (silenceVFX)
        {
            silenceVFX.Play();
        }
        float timer = 0;

        while (timer < timeActive.CurrentValue)
        {
            HandleSilencing(silenceSphereRange.CurrentValue);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Shrinks the Sphere.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShrinkSphere()
    {
        float timer = 0;

        while (timer < silenceSphereGrowLerpTime.CurrentValue)
        {
            HandleSilencing(Mathf.Lerp(0, silenceSphereRange.CurrentValue,
                timer / silenceSphereGrowLerpTime.CurrentValue));
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Silences any Life Form hit that has a valid relationship.
    /// </summary>
    /// <param name="range">Radius of the sphere.</param>
    private void HandleSilencing(float range)
    {
        // Get all hits in range
        Collider[] hits = new Collider[50];
        Physics.OverlapSphereNonAlloc(rootPosition.position, range, hits);

        // All in universe audio in range
        HashSet<InUniverseSound> sounds = new();

        // Keep the Catchable Life Forms that have a prey, neutral, or enemy relationship to this Catchable Life Form
        HashSet<CatchableLifeForm> catchableLifeFormsWithValidRelationship = new();
        foreach (Collider hit in hits)
        {
            if (hit && hit.GetComponent<CatchableLifeForm>() &&
                _catchableLifeForm.Species.NotAPredator(hit.GetComponent<CatchableLifeForm>().Species))
            {
                catchableLifeFormsWithValidRelationship.Add(hit.GetComponent<CatchableLifeForm>());
            }

            if (hit && hit.GetComponent<InUniverseSound>() &&
                _catchableLifeForm.Species.NotAPredator(hit.GetComponent<InUniverseSound>().CatchableLifeForm.Species))
            {
                sounds.Add(hit.GetComponent<InUniverseSound>());
            }
        }

        // Disable Speaking and Hearing for the Catchable Life Forms hit
        foreach (CatchableLifeForm lifeForm in catchableLifeFormsWithValidRelationship)
        {
            if (lifeForm.GetComponent<Sense_Sound>())
            {
                lifeForm.GetComponent<Sense_Sound>().enabled = false;
            }

            if (lifeForm.GetComponent<SoundGenerator>())
            {
                lifeForm.GetComponent<SoundGenerator>().enabled = false;
            }
        }

        // Disable all sounds hit
        foreach (InUniverseSound sound in sounds)
        {
            sound.audioSource.mute = true;
        }

        // Enable Speaking and Hearing for the Catchable Life Forms no longer hit
        foreach (CatchableLifeForm lifeForm in _lifeFormsSilenced.Where(lifeForm =>
                     !catchableLifeFormsWithValidRelationship.Contains(lifeForm)))
        {
            if (lifeForm.GetComponent<Sense_Sound>())
            {
                lifeForm.GetComponent<Sense_Sound>().enabled = true;
            }

            if (lifeForm.GetComponent<SoundGenerator>())
            {
                lifeForm.GetComponent<SoundGenerator>().enabled = true;
            }
        }

        // Unmute all sounds that are no longer hit
        foreach (InUniverseSound sound in _soundsSilenced.Where(sound => !sounds.Contains(sound)))
        {
            if (sound.audioSource != null)
            {
                sound.audioSource.mute = false;
            }
        }

        // Reset Silenced Life Forms
        _lifeFormsSilenced = new HashSet<CatchableLifeForm>(catchableLifeFormsWithValidRelationship);

        // Reset Silenced sounds
        _soundsSilenced = new HashSet<InUniverseSound>(sounds);
    }

    /// <inheritdoc />
    public override bool IsActive()
    {
        return !_canUse;
    }
}