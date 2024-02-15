using System.Collections;
using UnityEngine;

/// <summary>
/// This should be used instead of Playing Clip at Point.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]
public class InUniverseSound : MonoBehaviour
{
    [Tooltip("The Catchable Life Form that made this sound.")] [SerializeField]
    private CatchableLifeForm catchableLifeForm;

    public AudioSource audioSource { get; private set; }

    [Tooltip("The Catchable Life Form that made this sound.")] [SerializeField]
    private SphereCollider _sphereCollider;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource.loop)
        {
            StartCoroutine(DestroySound(audioSource.clip.length));
        }

        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.radius = audioSource.maxDistance;
        _sphereCollider.isTrigger = true;
    }

    public InUniverseSound(CatchableLifeForm catchableLifeForm)
    {
        this.catchableLifeForm = catchableLifeForm;
    }

    /// <summary>
    /// The amount of time before this GameObject is destroyed.
    /// </summary>
    /// <param name="timeToDestroy">The amount of time the audio clip will run for.</param>
    /// <returns>Nothing.</returns>
    private IEnumerator DestroySound(float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(gameObject);
    }

    /// <summary>
    /// Gets the Catchable Life Form.
    /// </summary>
    /// <returns>The Catchable Life Form.</returns>
    public CatchableLifeForm GetCatchableLifeForm() => catchableLifeForm;
}