using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame
{
    // Возможные типы обработки пустых баз.
    public enum enmSurroundCondition
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
