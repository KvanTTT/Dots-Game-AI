namespace DotsGame
{
    public enum MoveState
    {
        /// <summary>
        /// Dot has been putted (MakeMove).
        /// </summary>
        Add,

        /// <summary>
        /// Dot has been removed (UnmakeMove).
        /// </summary>
        Remove,

        /// <summary>
        /// Start has not been changed.
        /// </summary>
        None,
    }
}
