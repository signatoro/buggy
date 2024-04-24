public class CMod_OnEvent : CatchableModule
{
    private bool _canCatch;

    /// <summary>
    /// Sets can catch to true.
    /// </summary>
    public void CanCatch() => _canCatch = true;
    
    /// <summary>
    /// Sets can catch to false.
    /// </summary>
    public void CannotCatch() => _canCatch = false;
    
    /// <summary>
    /// Can catch if event says we can.
    /// </summary>
    /// <returns></returns>
    public override bool CatchCheck()
    {
        return _canCatch;
    }
}
