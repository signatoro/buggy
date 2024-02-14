using UnityEngine;

public class Sense_Sound : SenseSystem
{
    [Tooltip("Minimum Volume Threshold for this Life Form to Hear a sound")] [SerializeField]
    private GlobalFloat volumeThresholdData;

    // The current volume threshold that can be modified.
    private float _currentVolumeThreshold;
}
