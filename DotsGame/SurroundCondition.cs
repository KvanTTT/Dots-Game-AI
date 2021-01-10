namespace DotsGame
{
    // Возможные типы обработки пустых баз.
    public enum SurroundCondition
    {
        /// <summary>
        /// Классическая.
        /// </summary>
        Standart,

        /// <summary>
        /// Всегда захватывает тот игрок, в чье пустое окружение поставлена точка.
        /// </summary>
        Always,

        /// <summary>
        /// Захватывать даже пустые области.
        /// </summary>
        AlwaysEnemy
    }
}
