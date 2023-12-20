using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[IncludeInSettings(true)]
public static class GlobalVarUtils
{
    public static void ResetMultiInt(List<GlobalInt> vars)
    {
        foreach (GlobalInt var in vars) { var.ResetValue(); }
    }
}
