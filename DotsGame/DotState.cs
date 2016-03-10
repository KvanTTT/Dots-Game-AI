using System;

namespace DotsGame
{
    public static class DotConstants
    {
        public const byte RealPlayerShift = 6;
        public const byte RealPuttedShift = RealPlayerShift + 1;
    }

    /// <summary>
    /// Representation of Dot state on Field.
    /// Player and Putted is general flags.
    /// Additional flags are required for internal using.
    /// All possibles external states:
    /// 00 - empty item.
    /// 01 - invalid (field border).
    /// 10 - red dot is putted.
    /// 11 - blue dot is putted.
    /// </summary>
    [Flags]
    public enum DotState : int
    {
        Player = 1 << 0,
        Putted = 1 << 1,

        Bound = 1 << 2,
        EmptyBound = 1 << 3,
        EmptyBase = 1 << 4,
        Tagged = 1 << 5,
        RealPlayer = 1 << DotConstants.RealPlayerShift,
        RealPutted = 1 << DotConstants.RealPuttedShift,

        Player0 = 0,
        Player1 = 1,
        RealPlayer0 = 0 << DotConstants.RealPlayerShift,
        RealPlayer1 = 1 << DotConstants.RealPlayerShift,
        Empty = 0,
        Invalid = 1,

        EnableMask = DotState.Putted | DotState.Player,
        BoundMask = DotState.Bound | DotState.Putted | DotState.Player,
        AllowingMask = DotState.Putted,
        SurroundCountMask = (1 << 8) | (1 << 9), // 2 bits are enough for base depth saving.
        FirstSurroundLevel = 1 << 8,

        DiagonalGroupMaskStep = 1 << 23,
        DiagonalGroupMaskShift = 22,
        DiagonalGroupMask = -4194304,
    }
}
