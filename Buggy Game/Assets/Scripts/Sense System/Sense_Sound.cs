using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Sense_Sound : SenseSystem
{
    public class SoundData : SenseData
    {
        [Tooltip("The Volume Level at the Point Seen")]
        public float VolumeLevel;

        public SoundData(CatchableLifeForm catchableLifeForm, Vector3 sensePosition, float volumeLevel) : base(
            catchableLifeForm, sensePosition)
        {
            VolumeLevel = volumeLevel;
        }
    }

    [Tooltip("Minimum Volume Threshold for this Life Form to Hear a sound")] [SerializeField]
    private GlobalFloat volumeThresholdData;

    [Tooltip("What to do when disabled.")]
    public UnityEvent OnDisabled = new();

    // The current volume threshold that can be modified.
    private float _currentVolumeThreshold;

    internal override void Awake()
    {
        base.Awake();
        _currentVolumeThreshold = volumeThresholdData.CurrentValue;
        volumeThresholdData.OnChanged.AddListener(SetCurrentVolumeThreshold);
    }

    private void OnDisable()
    {
        OnDisabled?.Invoke();
    }

    /// <summary>
    /// Updates the current Volume Threshold Value when the data changes.
    /// </summary>
    /// <param name="value">The new value of the data.</param>
    private void SetCurrentVolumeThreshold(float value) => _currentVolumeThreshold = value;

    internal override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Checks the Life Form's Radius.
    /// </summary>
    /// <returns>Returns the sound data for everything heard.</returns>
    [ContextMenu("Check Sound Radius")]
    private List<SoundData> CheckSoundRadius()
    {
        List<SoundData> soundDatas = new();

        int layerMask = LayerMask.GetMask("Audio");
        List<Collider> hitColliders = Physics.OverlapSphere(root.position, radius.CurrentValue, layerMask).ToList();

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<InUniverseSound>())
            {
                InUniverseSound inUniverseSound = collider.GetComponent<InUniverseSound>();
                CatchableLifeForm lifeForm = inUniverseSound.CatchableLifeForm;
                AudioSource audioSource = inUniverseSound.audioSource;
                Vector3 directionToLifeForm = root.position - inUniverseSound.transform.position;

                if (_catchableLifeForm.Species.HasConnectionToSpecies(lifeForm.Species) &&
                    Physics.Raycast(inUniverseSound.transform.position, directionToLifeForm, out RaycastHit hit,
                        radius.CurrentValue, ~layerMask) &&
                    hit.transform.GetComponent<CatchableLifeForm>() &&
                    hit.transform.GetComponent<CatchableLifeForm>() == _catchableLifeForm)
                {
                    Vector3 position = inUniverseSound.transform.position;
                    float volumeLevel = GetVolumeLevelOfSoundAtPoint(audioSource);
                    if (volumeLevel >= _currentVolumeThreshold)
                    {
                        soundDatas.Add(new SoundData(lifeForm, position, volumeLevel));
                        Debug.Log($"{name} heard: {inUniverseSound.name} with a volume value of {volumeLevel}", this);
                    }
                }
            }
        }

        return soundDatas;
    }

    /// <summary>
    /// Gets how loud the sound from the Audio Source is at the root.
    /// </summary>
    /// <param name="audioSource">The Audio Source of the sound we are checking.</param>
    /// <returns>The volume level of the sound at the root.</returns>
    private float GetVolumeLevelOfSoundAtPoint(AudioSource audioSource)
    {
        // Get Audio Rolloff
        AnimationCurve volumeRolloff = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
        float distance = Vector3.Distance(audioSource.transform.position, root.position);
        float volumeRolloffVal = volumeRolloff.Evaluate(distance / audioSource.maxDistance);

        return audioSource.volume * volumeRolloffVal * 100f;
    }
}