namespace Alkahest.Core.Game
{
    public enum MovementKind : uint
    {
        Run = 0,
        Walk = 1,
        Fall = 2,
        Jump = 5,
        JumpCollide = 6,
        Stop = 7,
        StartSwim = 8,
        StopSwim = 9,
        JumpFall = 10,
    }
}
