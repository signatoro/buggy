using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Controls the Day Night Cycle and the fog.
/// </summary>
public class DayCycleController : MonoBehaviour
{
    [Tooltip("What is the current time of day?")] [SerializeField]
    private GlobalFloat timeOfDay;

    [Tooltip("Sun Light.")] [SerializeField]
    private Light sun;

    [Tooltip("Moon Light.")] [SerializeField]
    private Light moon;

    [Tooltip("What does time.DeltaTime get multiplied by?")] [SerializeField]
    private GlobalFloat orbitSpeed;

    [Tooltip("Color of the fog during day.")] [SerializeField]
    private GlobalColor dayColor;

    [Tooltip("Color of the fog during night.")] [SerializeField]
    private GlobalColor nightColor;

    [Tooltip("Sky Fog Volume.")] [SerializeField]
    private Volume skyFog;

    [Tooltip("What does the fog color get multiplied by?")] [SerializeField]
    private GlobalFloat fogColorSpeed;

    [Tooltip("KeyCode for freezing time.")] [SerializeField]
    private GlobalKeyCodeList freezeKey;

    [Tooltip("Is it night?")] [SerializeField]
    private GlobalBool isNight;

    [Tooltip("Is it time frozen?")] [SerializeField]
    private GlobalBool freeze;

    // Update is called once per frame
    void Update()
    {
        // Freezes Time
        if (freezeKey.PressingOneOfTheKeys())
        {
            freeze.ToggleValue();
        }

        if (freeze.CurrentValue) return;

        // Update Time
        timeOfDay.CurrentValue += Time.deltaTime * orbitSpeed.CurrentValue;
        if (timeOfDay.CurrentValue >= 24)
            timeOfDay.CurrentValue = 0;
        UpdateTime();
    }

    private void OnValidate()
    {
        UpdateTime();
    }

    /// <summary>
    /// Update Everything related to the time of day
    /// </summary>
    private void UpdateTime()
    {
        float alpha = timeOfDay.CurrentValue / 24.0f;
        float sunRotation = Mathf.Lerp(-90, 270, alpha);
        float moonRotation = sunRotation - 180;
        sun.transform.rotation = Quaternion.Euler(sunRotation, 150f, 0);
        moon.transform.rotation = Quaternion.Euler(moonRotation, 150f, 0);


        UnityEngine.Rendering.HighDefinition.Fog fog;
        skyFog.profile.TryGet(out fog);

        Color targColor = isNight.CurrentValue ? nightColor.CurrentValue : dayColor.CurrentValue;

        Color currColor = fog.albedo.value;

        fog.albedo.Override(Color.Lerp(currColor, targColor,
            orbitSpeed.CurrentValue * fogColorSpeed.CurrentValue * Time.deltaTime));

        CheckNightDayTransition();
    }

    /// <summary>
    /// Check whether the state of time should shift from day to night or night to day
    /// </summary>
    private void CheckNightDayTransition()
    {
        if (isNight.CurrentValue)
        {
            if (moon.transform.rotation.eulerAngles.x > 180)
            {
                StartDay();
            }
        }
        else
        {
            if (sun.transform.rotation.eulerAngles.x > 180)
            {
                StartNight();
            }
        }
    }

    /// <summary>
    /// Setup daytime
    /// </summary>
    private void StartDay()
    {
        isNight.CurrentValue = false;
        sun.shadows = LightShadows.Soft;
        moon.shadows = LightShadows.None;
    }

    /// <summary>
    /// Setup nighttime
    /// </summary>
    private void StartNight()
    {
        isNight.CurrentValue = true;
        moon.shadows = LightShadows.Soft;
        sun.shadows = LightShadows.None;
    }

    /// <summary>
    /// Is it nighttime?
    /// </summary>
    /// <returns>Yes if night, else false.</returns>
    public bool IsNight() => isNight.CurrentValue;
}