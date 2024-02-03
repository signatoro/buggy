using UnityEngine;

public class CLF_OnlyNight : CatchableLifeForm
{
    private DayCycleController _dayCycleController;

    private void Start()
    {
        _dayCycleController = GameObject.FindWithTag($"DayNightController").GetComponent<DayCycleController>();
    }

    /// <summary>
    /// Can only be caught if it is nighttime.
    /// </summary>
    /// <returns>True if can be caught, else false</returns>
    public override bool CanBeCaught() => base.CanBeCaught() && _dayCycleController.IsNight();
}