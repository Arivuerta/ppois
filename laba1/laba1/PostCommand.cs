using System;
using System.Globalization;

namespace Laba1
{
    /// <summary>
    /// Одна команда программы машины Поста.
    /// </summary>
    public class PostCommand : IEquatable<PostCommand>
    {
        private readonly CommandType type;
        private readonly int next1;
        private readonly int next2;

        public CommandType Type => type;
        public int Next1 => next1;
        public int Next2 => next2;

        /// <summary>
        /// Создает команду. next1 и next2 - номера следующих команд (с 1).
        /// Для Stop номера игнорируются. Для Check обязательны оба.
        /// </summary>
        public PostCommand(CommandType type, int next1 = 0, int next2 = 0)
        {
            this.type = type;

            if (type == CommandType.Stop)
            {
                this.next1 = 0;
                this.next2 = 0;
                return;
            }

            if (next1 < 1)
                throw new ArgumentException(
                    "Номер следующей команды должен быть не меньше 1.");

            this.next1 = next1;

            if (type == CommandType.Check)
            {
                if (next2 < 1)
                    throw new ArgumentException(
                        "Номер следующей команды должен быть не меньше 1.");
                this.next2 = next2;
            }
            else
            {
                this.next2 = 0;
            }
        }

        /// <summary>
        /// Конструктор копирования.
        /// </summary>
        public PostCommand(PostCommand other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            type = other.type;
            next1 = other.next1;
            next2 = other.next2;
        }

        public bool Equals(PostCommand? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return type == other.type &&
                   next1 == other.next1 &&
                   next2 == other.next2;
        }

        public override bool Equals(object? obj) => Equals(obj as PostCommand);

        public override int GetHashCode()
            => HashCode.Combine(type, next1, next2);

        public static bool operator ==(PostCommand? a, PostCommand? b)
            => Equals(a, b);

        public static bool operator !=(PostCommand? a, PostCommand? b)
            => !Equals(a, b);

        public override string ToString()
        {
            switch (type)
            {
                case CommandType.Mark: return $"V {next1}";
                case CommandType.Unmark: return $"X {next1}";
                case CommandType.Left: return $"L {next1}";
                case CommandType.Right: return $"R {next1}";
                case CommandType.Check: return $"? {next1} {next2}";
                default: return "!";
            }
        }

        /// <summary>
        /// Создает команду из строки: "V 2", "? 3 5", "!".
        /// Регистр не важен.
        /// </summary>
        public static PostCommand Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Строка не должна быть пустой.");

            string[] parts = text.Split(
                new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            switch (parts[0].ToUpperInvariant())
            {
                case "V":
                    CheckCount(parts, 2);
                    return new PostCommand(CommandType.Mark, Number(parts[1]));

                case "X":
                    CheckCount(parts, 2);
                    return new PostCommand(CommandType.Unmark, Number(parts[1]));

                case "L":
                    CheckCount(parts, 2);
                    return new PostCommand(CommandType.Left, Number(parts[1]));

                case "R":
                    CheckCount(parts, 2);
                    return new PostCommand(CommandType.Right, Number(parts[1]));

                case "?":
                    CheckCount(parts, 3);
                    return new PostCommand(
                        CommandType.Check,
                        Number(parts[1]),
                        Number(parts[2]));

                case "!":
                    CheckCount(parts, 1);
                    return new PostCommand(CommandType.Stop);

                default:
                    throw new FormatException(
                        $"Неизвестная команда: {parts[0]}");
            }
        }

        private static void CheckCount(string[] parts, int count)
        {
            if (parts.Length != count)
                throw new FormatException(
                    $"Неверное число параметров у команды {parts[0]}.");
        }

        private static int Number(string text)
        {
            if (!int.TryParse(
                text,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int value))
            {
                throw new FormatException($"Некорректное число: {text}");
            }

            return value;
        }
    }
}