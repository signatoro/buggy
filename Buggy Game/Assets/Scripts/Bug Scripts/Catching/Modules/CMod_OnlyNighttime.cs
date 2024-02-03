using UnityEngine;

public class CMod_OnlyNighttime : CatchableModule
{
    private DayCycleController _dayCycleController;

    // Start is called before the first frame update
    internal override void Start()
    {
        base.Start();
        _dayCycleController = GameObject.FindWithTag($"DayNightController").GetComponent<DayCycleController>();
    }

    /// <summary>
    /// Is it nighttime?
    /// </summary>
    /// <returns>Only returns true if it is nighttime.</returns>
    public override bool CatchCheck() => _dayCycleController.IsNight();
}