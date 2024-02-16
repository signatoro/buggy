using System;
using UnityEngine;

/// <summary>
/// Makes all the sounds for a Life Form.
/// </summary>
[RequireComponent(typeof(CatchableLifeForm))]
public class SoundGenerator : MonoBehaviour
{
    [Tooltip("Root Transform of Sounds")] [SerializeField]
    private Transform root;

    private CatchableLifeForm _catchableLifeForm;

    private void Awake()
    {
        _catchableLifeForm = GetComponent<CatchableLifeForm>();
    }

    public void PlaySound(InUniverseSound sound)
    {
        sound.CatchableLifeForm = _catchableLifeForm;
        Instantiate(sound, root.position, Quaternion.identity);
    }
}