namespace SoleHeir
{
    public enum PlayerStatus
    {
        FREE,
        INTERACTING ,
        KIT_INTERACTING,
        PLAYER_LIST
    }

    public enum InteractionStatus
    {
        FREE,
        INTERACTING
    }

    public enum ParentInts
    {
        ConfigIndex,
        PlayerIdentity,
        KillerIdentity
    }

    public enum ParentBools
    {
        Used,
        Sabotaged,
        IsBeingUsedWithKit
    }

    public enum ParentStrings
    {
        OnScreenText
    }
    public enum ParentFloats
    {
        Timer
    }
}