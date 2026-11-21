using System;
using System.Collections.Generic;
using System.Text;

namespace Laba1
{
    /// <summary>
    /// Тип команды машины Поста.
    /// </summary>
    public enum CommandType
    {
        Mark,   // V j  - поставить метку, перейти к команде j
        Unmark, // X j  - стереть метку, перейти к команде j
        Left,   // L j  - сдвиг влево, перейти к команде j
        Right,  // R j  - сдвиг вправо, перейти к команде j
        Check,  // ? j1 j2 - если метка есть, перейти к j1, иначе к j2
        Stop    // !    - остановка
    }
}
