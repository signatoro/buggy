using UnityEngine;

[CreateAssetMenu(fileName = "New Global Bool", menuName = "Global Variables/Bool")]
public class GlobalBool : GlobalVar<bool>
{
    /// <summary>
    /// Flips the CurrentValue of the GlobalBool.
    /// </summary>
    public void ToggleValue()
    {
        CurrentValue = !CurrentValue;
    }
}