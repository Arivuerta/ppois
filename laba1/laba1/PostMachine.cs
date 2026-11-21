using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Laba1
{
    /// <summary>
    /// Машина Поста: бесконечная в обе стороны лента с метками,
    /// каретка и программа из пронумерованных команд.
    /// </summary>
    public class PostMachine : IEquatable<PostMachine>
    {
        private HashSet<int> tape = new HashSet<int>();
        private List<PostCommand> program = new List<PostCommand>();

        private int head;
        private int current = 1;
        private bool stopped;

        public PostMachine() { }

        public PostMachine(PostMachine other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            tape = new HashSet<int>(other.tape);

            foreach (PostCommand command in other.program)
                program.Add(new PostCommand(command));

            head = other.head;
            current = other.current;
            stopped = other.stopped;
        }

        public int CurrentLine => current;
        public bool IsStopped => stopped;
        public int CommandCount => program.Count;

        public int GetHead() => head;
        public void SetHead(int position) => head = position;

        public bool IsMarked(int position) => tape.Contains(position);

        public void SetMark(int position, bool marked)
        {
            if (marked) tape.Add(position);
            else tape.Remove(position);
        }

        /// <summary>
        /// Номера помеченных ячеек по возрастанию.
        /// </summary>
        public int[] GetMarks()
        {
            int[] marks = new int[tape.Count];
            tape.CopyTo(marks);
            Array.Sort(marks);
            return marks;
        }

        public void AddCommand(PostCommand command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            program.Add(new PostCommand(command));
        }

        /// <summary>
        /// Возвращает команду по её номеру (1-based). Только для чтения.
        /// </summary>
        public PostCommand GetCommand(int number)
        {
            if (number < 1 || number > program.Count)
                throw new ArgumentOutOfRangeException(
                    nameof(number),
                    $"Нет команды с номером {number}.");

            return program[number - 1];
        }

        /// <summary>
        /// Возвращает машину к первой команде (лента не меняется).
        /// </summary>
        public void Reset()
        {
            current = 1;
            stopped = false;
        }

        /// <summary>
        /// Выполняет одну команду.
        /// </summary>
        public void Step()
        {
            if (stopped)
                throw new InvalidOperationException("Машина уже остановлена.");

            if (program.Count == 0)
                throw new InvalidOperationException("Программа пуста.");

            if (current < 1 || current > program.Count)
                throw new InvalidOperationException(
                    $"Нет команды с номером {current}.");

            PostCommand command = program[current - 1];

            switch (command.Type)
            {
                case CommandType.Mark:
                    tape.Add(head);
                    current = command.Next1;
                    break;

                case CommandType.Unmark:
                    tape.Remove(head);
                    current = command.Next1;
                    break;

                case CommandType.Left:
                    head--;
                    current = command.Next1;
                    break;

                case CommandType.Right:
                    head++;
                    current = command.Next1;
                    break;

                case CommandType.Check:
                    current = tape.Contains(head)
                        ? command.Next1
                        : command.Next2;
                    break;

                case CommandType.Stop:
                    stopped = true;
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Неизвестный тип команды: {command.Type}");
            }
        }

        /// <summary>
        /// Выполняет программу до остановки.
        /// Возвращает число выполненных шагов.
        /// Бросает исключение при превышении лимита (зацикливание).
        /// </summary>
        public int Run(int maxSteps = 100000)
        {
            int steps = 0;

            while (!stopped)
            {
                if (steps >= maxSteps)
                    throw new InvalidOperationException(
                        $"Превышен лимит шагов ({maxSteps}). " +
                        "Возможно, программа зациклилась.");

                Step();
                steps++;
            }

            return steps;
        }

        public bool Equals(PostMachine? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (head != other.head ||
                current != other.current ||
                stopped != other.stopped ||
                !tape.SetEquals(other.tape) ||
                program.Count != other.program.Count)
                return false;

            for (int i = 0; i < program.Count; i++)
            {
                if (program[i] != other.program[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as PostMachine);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(head);
            hash.Add(current);
            hash.Add(stopped);
            foreach (int m in GetMarks()) hash.Add(m);
            foreach (var c in program) hash.Add(c);
            return hash.ToHashCode();
        }

        public static bool operator ==(PostMachine? a, PostMachine? b)
            => Equals(a, b);

        public static bool operator !=(PostMachine? a, PostMachine? b)
            => !Equals(a, b);

        /// <summary>
        /// Записывает машину в текст:
        /// tape: ...
        /// head: ...
        /// current: ...
        /// stopped: ...
        /// и по команде в строке.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("tape: " + string.Join(" ", GetMarks()));
            sb.AppendLine("head: " + head);
            sb.AppendLine("current: " + current);
            sb.AppendLine("stopped: " + (stopped ? "true" : "false"));

            foreach (PostCommand command in program)
                sb.AppendLine(command.ToString());

            return sb.ToString();
        }

        /// <summary>
        /// Читает машину из текста (формат как в ToString).
        /// Префиксы tape:, head:, current:, stopped: необязательны —
        /// если их нет, машина создаётся со значениями по умолчанию.
        /// </summary>
        public static PostMachine Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Строка не должна быть пустой.");

            PostMachine machine = new PostMachine();

            string[] lines = text.Split('\n');

            foreach (string raw in lines)
            {
                string line = raw.Trim();

                if (line.Length == 0)
                    continue;

                if (line.StartsWith("tape:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] numbers = line.Substring(5).Split(
                        new[] { ' ', '\t' },
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (string number in numbers)
                        machine.tape.Add(ParseInt(number));
                }
                else if (line.StartsWith("head:", StringComparison.OrdinalIgnoreCase))
                {
                    machine.head = ParseInt(line.Substring(5).Trim());
                }
                else if (line.StartsWith("current:", StringComparison.OrdinalIgnoreCase))
                {
                    machine.current = ParseInt(line.Substring(8).Trim());
                }
                else if (line.StartsWith("stopped:", StringComparison.OrdinalIgnoreCase))
                {
                    string val = line.Substring(8).Trim();
                    machine.stopped =
                        val.Equals("true", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    machine.program.Add(PostCommand.Parse(line));
                }
            }

            // Проверяем, что все переходы ведут на существующие команды.
            for (int i = 0; i < machine.program.Count; i++)
            {
                PostCommand command = machine.program[i];

                if (command.Type == CommandType.Stop)
                    continue;

                CheckTarget(command.Next1, machine.program.Count, i + 1);

                if (command.Type == CommandType.Check)
                    CheckTarget(command.Next2, machine.program.Count, i + 1);
            }

            // Проверяем корректность current
            if (machine.program.Count > 0 &&
                (machine.current < 1 || machine.current > machine.program.Count))
            {
                throw new FormatException(
                    $"Текущая команда {machine.current} выходит за пределы программы.");
            }

            return machine;
        }

        private static int ParseInt(string text)
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

        private static void CheckTarget(int target, int count, int source)
        {
            if (target < 1 || target > count)
                throw new FormatException(
                    $"Команда {source} ссылается на несуществующую команду {target}.");
        }
    }
}