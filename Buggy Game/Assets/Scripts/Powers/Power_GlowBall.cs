using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Creates a source of light around you.
/// </summary>
public class Power_GlowBall : Power
{
    [Tooltip("VFX Graph for the Glow Orb")] [SerializeField]
    private VisualEffect glowOrb;

    [Tooltip("Light for the Glow Orb.")] [SerializeField]
    private Light glowLight;

    [Tooltip("Intensity of the Glow Light")] [SerializeField]
    private GlobalFloat glowLightIntensity;

    [Tooltip("Range of the Glow Light")] [SerializeField]
    private GlobalFloat glowLightRange;

    [Tooltip("Time for Glow Light to activate or deactivate")] [SerializeField]
    private GlobalFloat glowLightLerpTime;

    private bool _isActive;

    /// <inheritdoc />
    public override IEnumerator ResetPower()
    {
        if (gameObject)
        {
            StopAllCoroutines();
            yield return StartCoroutine(StopPower());
        }

        // Reset Glow Light
        if (glowLight)
        {
            glowLight.range = glowLightRange.CurrentValue;
            glowLight.intensity = 0;
            glowLight.enabled = false;
        }

        // Reset Glow Orb
        if (glowOrb)
        {
            glowOrb.Stop();
        }

        // Reset Active State
        _isActive = false;
        yield return null;
    }

    /// <summary>
    /// Toggles between on and off every time we Attempt To Execute.
    /// </summary>
    public override void AttemptToExecute() => StartCoroutine(!IsActive() ? ExecutePower() : StopPower());

    /// <summary>
    /// Turns on the Glow Orb and Glow Light.
    /// </summary>
    /// <returns>Nothing.</returns>
    public override IEnumerator ExecutePower()
    {
        // Power is active
        _isActive = true;

        // Show the Glow Orb
        if (glowOrb)
        {
            yield return StartCoroutine(TurnOnGlowOrb());
        }

        // Show the Light
        if (glowLight)
        {
            yield return StartCoroutine(TurnOnLight());
        }

        yield return null;
    }

    /// <summary>
    /// Starts the VFX for the Glow Orb.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator TurnOnGlowOrb()
    {
        glowOrb.Play();
        yield return null;
    }

    /// <summary>
    /// Stops the VFX for the Glow Orb.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnOffGlowOrb()
    {
        glowOrb.Stop();
        yield return null;
    }

    /// <summary>
    /// Starts the light for the Glow Orb.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator TurnOnLight()
    {
        glowLight.enabled = true;
        glowLight.range = glowLightRange.CurrentValue;

        float timer = 0;

        while (timer < glowLightLerpTime.CurrentValue)
        {
            glowLight.intensity = Mathf.Lerp(glowLight.intensity,
                glowLightIntensity.CurrentValue,
                timer / glowLightLerpTime.CurrentValue);
            timer += Time.deltaTime;
            yield return null;
        }

        glowLight.intensity = glowLightIntensity.CurrentValue;
        yield return null;
    }

    /// <summary>
    /// Stops the light for the Glow Orb.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnOffLight()
    {
        glowLight.range = glowLightRange.CurrentValue;

        float timer = 0;

        while (timer < glowLightLerpTime.CurrentValue)
        {
            glowLight.intensity = Mathf.Lerp(glowLight.intensity,
                0,
                timer / glowLightLerpTime.CurrentValue);
            timer += Time.deltaTime;
            yield return null;
        }

        glowLight.intensity = 0;
        glowLight.enabled = false;
        yield return null;
    }

    /// <summary>
    /// Turns of the Glow Light and Glow Orb.
    /// </summary>
    /// <returns>Nothing.</returns>
    public override IEnumerator StopPower()
    {
        // Power is no longer active
        _isActive = false;

        // Turn Off the Light
        if (glowLight)
        {
            yield return StartCoroutine(TurnOffLight());
        }

        // Hide the Glow Orb
        if (glowOrb)
        {
            yield return StartCoroutine(TurnOffGlowOrb());
        }

        yield return null;
    }


    /// <inheritdoc />
    public override bool IsActive() => _isActive;
}