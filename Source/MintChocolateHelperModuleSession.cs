namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModuleSession : EverestModuleSession 
{
    public bool HasHeartBreakerDash { get; set; }
    public bool HeartBreakerDashActive { get; set; }
    public bool HasSpeedFlipRefill { get; set; }
    
    public bool AlternateDebrisFadeHookLoaded { get; set; }
}