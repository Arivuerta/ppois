using System;
using System.Globalization;
using System.Text;

namespace Laba1
{
    static class MenuPost
    {
        static PostMachine machine = new PostMachine();

        const int TAPE_FROM = -10;
        const int TAPE_TO = 10;

        public static void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("========================================");
            Console.WriteLine("           МАШИНА ПОСТА");
            Console.WriteLine("========================================");
            Console.WriteLine("Лента бесконечна. Метки хранятся в множестве,");
            Console.WriteLine("поэтому видны только те ячейки, где есть метка,");
            Console.WriteLine("плюс диапазон вокруг нуля для наглядности.");
            Console.WriteLine();

            while (true)
            {
                PrintMenu();

                Console.Write("Ваш выбор: ");
                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1": CreateMachine(); break;
                        case "2": ParseMachine(); break;
                        case "3": ShowState(); break;
                        case "4": SetMark(); break;
                        case "5": SetHead(); break;
                        case "6": DoStep(); break;
                        case "7": DoRun(); break;
                        case "8": DoReset(); break;
                        case "9": CopyAndCompare(); break;
                        case "10": ShowText(); break;
                        case "11": LoadExampleAddOne(); break;
                        case "12": LoadExampleSubOne(); break;
                        case "13": ShowProgram(); break;
                        case "0":
                            Console.WriteLine("Выход.");
                            return;
                        default:
                            Console.WriteLine(">> Такого пункта нет. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($">> ОШИБКА: {ex.Message}");
                }

                Console.WriteLine();
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(" 1. Создать пустую машину");
            Console.WriteLine(" 2. Прочитать машину из текста");
            Console.WriteLine(" 3. Показать состояние");
            Console.WriteLine(" 4. Поставить / стереть метку");
            Console.WriteLine(" 5. Установить каретку");
            Console.WriteLine(" 6. Выполнить один шаг");
            Console.WriteLine(" 7. Выполнить программу до конца");
            Console.WriteLine(" 8. Вернуться к первой команде");
            Console.WriteLine(" 9. Копия и сравнение");
            Console.WriteLine("10. Записать машину в текст");
            Console.WriteLine("11. Пример: прибавить 1");
            Console.WriteLine("12. Пример: вычесть 1");
            Console.WriteLine("13. Показать программу");
            Console.WriteLine(" 0. Выход");
            Console.WriteLine("----------------------------------------");
        }

        // ---------- Ввод ----------

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine();

                if (int.TryParse(s, NumberStyles.Integer,
                                CultureInfo.InvariantCulture, out int value))
                    return value;

                Console.WriteLine(">> Нужно целое число. Повторите.");
            }
        }

        // ---------- Вывод состояния ----------

        static void PrintState()
        {
            int head = machine.GetHead();

            var positions = new System.Collections.Generic.SortedSet<int>();
            for (int i = TAPE_FROM; i <= TAPE_TO; i++)
                positions.Add(i);

            foreach (int m in machine.GetMarks())
                positions.Add(m);

            StringBuilder numbers = new StringBuilder("   ");
            StringBuilder cells = new StringBuilder("   ");
            StringBuilder pointer = new StringBuilder("   ");

            int prev = int.MinValue;
            foreach (int p in positions)
            {
                if (prev != int.MinValue && p - prev > 1)
                {
                    numbers.Append("... ");
                    cells.Append("... ");
                    pointer.Append("    ");
                }

                numbers.Append(p.ToString().PadLeft(3)).Append(' ');

                cells.Append(machine.IsMarked(p) ? " 1  " : " _  ");

                pointer.Append(p == head ? " ^  " : "    ");

                prev = p;
            }

            Console.WriteLine();
            Console.WriteLine("ЛЕНТА:");
            Console.WriteLine(numbers);
            Console.WriteLine(cells);
            Console.WriteLine(pointer);

            Console.WriteLine();
            Console.WriteLine($"Каретка:         {head}");
            Console.WriteLine($"Текущая команда: {machine.CurrentLine} " +
                              $"из {machine.CommandCount}");
            Console.WriteLine($"Состояние:       " +
                              (machine.IsStopped ? "ОСТАНОВЛЕНА" : "работает"));
        }

        static void ShowProgram()
        {
            if (machine.CommandCount == 0)
            {
                Console.WriteLine(">> Программа пуста.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("ПРОГРАММА:");

            int current = machine.CurrentLine;

            for (int i = 1; i <= machine.CommandCount; i++)
            {
                string cmd = machine.GetCommand(i).ToString();
                string marker = (i == current && !machine.IsStopped)
                    ? " > " : "   ";
                Console.WriteLine($"{marker}{i,3}. {cmd}");
            }
        }

        // ---------- Действия ----------

        static void CreateMachine()
        {
            machine = new PostMachine();
            Console.WriteLine(">> Создана пустая машина.");
        }

        static void ParseMachine()
        {
            Console.WriteLine();
            Console.WriteLine("Введите машину построчно. Пустая строка — конец ввода.");
            Console.WriteLine("Формат:");
            Console.WriteLine("  tape: 0 1 2      — ячейки с метками");
            Console.WriteLine("  head: 0          — позиция каретки");
            Console.WriteLine("  R 2              — команды (по одной в строке)");
            Console.WriteLine("  ? 1 3");
            Console.WriteLine("  V 4");
            Console.WriteLine("  !");
            Console.WriteLine();

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                string? line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    break;
                sb.AppendLine(line);
            }

            machine = PostMachine.Parse(sb.ToString());
            Console.WriteLine(">> Машина прочитана.");
            PrintState();
        }

        static void ShowState()
        {
            PrintState();
        }

        static void SetMark()
        {
            int position = ReadInt("Номер ячейки: ");
            int value = ReadInt("1 — поставить метку, 0 — стереть: ");

            bool old = machine.IsMarked(position);
            machine.SetMark(position, value != 0);
            bool now = machine.IsMarked(position);

            Console.WriteLine($">> Ячейка {position}: " +
                              $"было {(old ? "1" : "0")}, стало {(now ? "1" : "0")}.");

            PrintState();
        }

        static void SetHead()
        {
            int position = ReadInt("Новая позиция каретки: ");
            machine.SetHead(position);
            Console.WriteLine($">> Каретка перемещена в {position}.");
            PrintState();
        }

        static void DoStep()
        {
            if (machine.CommandCount == 0)
            {
                Console.WriteLine(">> Программа пуста, шаг невозможен.");
                return;
            }

            if (machine.IsStopped)
            {
                Console.WriteLine(">> Машина уже остановлена.");
                return;
            }

            int before = machine.CurrentLine;
            machine.Step();
            Console.WriteLine($">> Выполнена команда {before}. " +
                              $"Теперь текущая: {machine.CurrentLine}.");

            PrintState();
        }

        static void DoRun()
        {
            if (machine.CommandCount == 0)
            {
                Console.WriteLine(">> Программа пуста, запуск невозможен.");
                return;
            }

            if (machine.IsStopped)
            {
                Console.WriteLine(">> Машина уже остановлена.");
                return;
            }

            int steps = machine.Run();
            Console.WriteLine($">> Программа завершена. Выполнено шагов: {steps}.");

            PrintState();
        }

        static void DoReset()
        {
            machine.Reset();
            Console.WriteLine(">> Машина возвращена к первой команде.");
            PrintState();
        }

        static void CopyAndCompare()
        {
            PostMachine copy = new PostMachine(machine);

            Console.WriteLine($">> Копия создана.");
            Console.WriteLine($"   copy == machine: {copy == machine}");

            int head = copy.GetHead();
            copy.SetMark(head, !copy.IsMarked(head));

            Console.WriteLine($">> В копии изменили метку под кареткой ({head}).");
            Console.WriteLine($"   copy == machine: {copy == machine}");
            Console.WriteLine($"   copy != machine: {copy != machine}");
        }

        static void ShowText()
        {
            Console.WriteLine();
            Console.WriteLine("--- ТЕКСТОВОЕ ПРЕДСТАВЛЕНИЕ ---");
            Console.WriteLine(machine.ToString());
            Console.WriteLine("--- КОНЕЦ ---");
        }

        static void LoadExampleAddOne()
        {
            // Метки 0,1,2. Каретка на 0.
            // Программа: идём вправо до первой пустой ячейки,
            // ставим метку, останавливаемся.
            string text =
                "tape: 0 1 2\n" +
                "head: 0\n" +
                "R 2\n" +
                "? 1 3\n" +
                "V 4\n" +
                "!\n";

            machine = PostMachine.Parse(text);
            Console.WriteLine(">> Загружен пример «прибавить 1».");
            PrintState();
        }

        static void LoadExampleSubOne()
        {
            // Метки 0,1,2. Каретка на 0.
            // Идём вправо до первой пустой, возвращаемся на 1 назад,
            // стираем метку, останавливаемся.
            string text =
                "tape: 0 1 2\n" +
                "head: 0\n" +
                "R 2\n" +
                "? 1 3\n" +
                "L 4\n" +
                "X 5\n" +
                "!\n";

            machine = PostMachine.Parse(text);
            Console.WriteLine(">> Загружен пример «вычесть 1».");
            PrintState();
        }
    }
}