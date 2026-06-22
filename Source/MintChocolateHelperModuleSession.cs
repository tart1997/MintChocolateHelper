namespace Celeste.Mod.MintChocolateHelper;

public class MintChocolateHelperModuleSession : EverestModuleSession 
{
    //Heart Breaker Refill
    public bool HasHeartBreakerDash { get; set; }
    public bool HeartBreakerDashActive { get; set; }
    
    
    //SpeedFlipRefill
    public bool HasSpeedFlipRefill { get; set; }
    
    
    //CancelDeathTrigger
    
    public bool PlayerIsPsuedoDead { get; set; }
    public int CDT_Depth { get; set; }
    public bool CDT_Active { get; set; }
    public bool CDT_Collidable { get; set; }
    public bool CDT_Visible { get; set; }
}