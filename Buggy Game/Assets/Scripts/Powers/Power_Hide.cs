using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Hide : Power
{
    [Tooltip("Renderers that will become invisible.")] [SerializeField]
    private List<Renderer> renderers = new();

    [Tooltip("Invisibility Alpha Value.")] [SerializeField]
    private GlobalFloat invisibilityAlpha;

    [Tooltip("Time for Invisibility to activate or deactivate")] [SerializeField]
    private GlobalFloat invisibilityLerpTime;

    private bool _isActive;

    private List<Material> _materials = new();
    private List<Color> _originalColors = new();
    private List<Color> _invisibilityColors = new();

    private CatchableLifeForm _catchableLifeForm;

    internal override void Awake()
    {
        _catchableLifeForm = GetComponent<CatchableLifeForm>();
        _catchableLifeForm.SetVisibility(true);

        foreach (Renderer renderer in renderers)
        {
            _materials.AddRange(renderer.materials);
        }

        foreach (Material material in _materials)
        {
            _originalColors.Add(material.color);
            _invisibilityColors.Add(new Color(material.color.r, material.color.g, material.color.b,
                invisibilityAlpha.CurrentValue));
        }

        base.Awake();
    }

    /// <inheritdoc />
    [ContextMenu("Reset Power")]
    public override IEnumerator ResetPower()
    {
        if (gameObject)
        {
            StopAllCoroutines();
            yield return StartCoroutine(StopPower());
        }

        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].color = _originalColors[i];
        }

        _catchableLifeForm.SetVisibility(true);

        yield return null;
    }

    /// <summary>
    /// Toggles between on and off every time we Attempt To Execute.
    /// </summary>
    [ContextMenu("Attempt to Execute Power")]
    public override void AttemptToExecute() => StartCoroutine(!IsActive() ? ExecutePower() : StopPower());

    /// <summary>
    /// Starts Hiding the Life Form.
    /// </summary>
    /// <returns>Nothing.</returns>
    [ContextMenu("Execute Power")]
    public override IEnumerator ExecutePower()
    {
        // Power is active
        _isActive = true;

        // Start Invisibility
        yield return StartCoroutine(StartInvisibility());

        yield return null;
    }

    /// <summary>
    /// Sets up the Invisibility coloring and Hides the Life Form.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator StartInvisibility()
    {
        // Hide the Life Form
        _catchableLifeForm.SetVisibility(false);

        // Set the alpha of the renderers
        float timer = 0;

        while (timer < invisibilityLerpTime.CurrentValue)
        {
            for (int i = 0; i < _materials.Count; i++)
            {
                _materials[i].color = Color.Lerp(_materials[i].color, _invisibilityColors[i],
                    timer / invisibilityLerpTime.CurrentValue);
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Ends the Invisibility coloring and Reveals the Life Form.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator EndInvisibility()
    {
        // Set the alpha of the renderers
        float timer = 0;

        while (timer < invisibilityLerpTime.CurrentValue)
        {
            for (int i = 0; i < _materials.Count; i++)
            {
                _materials[i].color = Color.Lerp(_materials[i].color, _originalColors[i],
                    timer / invisibilityLerpTime.CurrentValue);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Show the Life Form
        _catchableLifeForm.SetVisibility(true);
        yield return null;
    }

    /// <summary>
    /// Stops Hiding the Life Form.
    /// </summary>
    /// <returns>Nothing.</returns>
    [ContextMenu("Stop Power")]
    public override IEnumerator StopPower()
    {
        // Stop Invisibility
        yield return StartCoroutine(EndInvisibility());

        // Power is no longer active
        _isActive = false;

        yield return null;
    }

    /// <inheritdoc />
    public override bool IsActive() => _isActive;
}