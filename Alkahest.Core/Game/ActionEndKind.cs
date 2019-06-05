namespace Alkahest.Core.Game
{
    public enum ActionEndKind : uint
    {
        Unknown1 = 0,
        CancelLockOn = 1,
        Cancel = 2,
        SpecialInterrupt = 3,
        Chain = 4,
        Retaliate = 5,
        Interrupt = 6,
        Unknown2 = 8,
        Unknown3 = 9,
        ButtonRelease = 10,
        ButtonReleaseChain = 11,
        ResourceDepleted = 13,
        InvalidTarget = 19,
        Unknown4 = 23,
        Unknown5 = 24,
        Unknown6 = 25,
        TerrainInterrupt = 29,
        LockOnCast = 36,
        LoadInterrupt = 37,
        DashEnd = 39,
        MovieInterrupt = 43,
        Unknown7 = 49,
        ButtonReleaseFinished = 51,
        Unknown8 = 52,
    }
}
