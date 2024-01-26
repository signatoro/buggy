using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Global KeyCodeList", menuName = "Global Variables/KeyCodeList")]
public class GlobalKeyCodeList : GlobalVar<List<KeyCode>>
{
    /// <summary>
    /// Are one of the keys pressed?
    /// </summary>
    /// <returns>Returns true if one of the keys is pressed.</returns>
    public bool PressingOneOfTheKeys() => CurrentValue.Any(Input.GetKey);
}