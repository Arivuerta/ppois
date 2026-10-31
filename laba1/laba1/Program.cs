using System;
using System.Net.Sockets;
namespace Laba1
{
    static class MenuVector
    {

        public static void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("РАБОТА С ВЕКТОРАМИ 3D");
                Console.WriteLine("1. Создать вектор");
                Console.WriteLine("2. Показать координаты вектора");
                Console.WriteLine("3. Вычислить длину");
                Console.WriteLine("4. Сложить два вектора");
                Console.WriteLine("5. Вычесть два вектора");
                Console.WriteLine("6. Векторное произведение");
                Console.WriteLine("7. Умножить вектор на число");
                Console.WriteLine("8. Разделить вектор на число");
                Console.WriteLine("9. Косинус угла между векторами");
                Console.WriteLine("10. Сравнить векторы по длине");
                Console.WriteLine("11. Проверить равенство");
                Console.WriteLine("13. Прочитать вектор из текста");
                Console.WriteLine("0. Выход");

                Console.Write("Выберите пункт: ");
                string? choice = Console.ReadLine();//? чтобы не было предупреждения про null

                try
                {
                    switch (choice)
                    {
                        case "1":
                            CreateVector();
                            break;

                        case "2":
                            ShowCoordinates();
                            break;

                        case "3":
                            CalculateLength();
                            break;

                        case "4":
                            AddVectors();
                            break;

                        case "5":
                            SubtractVectors();
                            break;

                        case "6":
                            CrossProduct();
                            break;

                        case "7":
                            MultiplyVector();
                            break;

                        case "8":
                            DivideVector();
                            break;

                        case "9":
                            CalculateCosine();
                            break;

                        case "10":
                            CompareVectors();
                            break;

                        case "11":
                            CheckEquality();
                            break;

                        case "12":
                            CopyVector();
                            break;

                        case "13":
                            ParseVector();
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("Такого пункта нет.");
                            Pause();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Pause();
                }
            }
        }

        static Vector3D ReadVector(string name)
        {
            Console.WriteLine();
            Console.WriteLine($"Введите координаты вектора {name}.");
            Console.WriteLine("Формат: x1 y1 z1 x2 y2 z2");
            Console.Write(">>> ");

            double[] values = new double[6];

            while (true)
            {
                string? input = Console.ReadLine();

                if (input == null)
                    throw new InvalidOperationException("Ввод завершен.");

                string[] parts = input.Split(
                    new[] { ' ', ';', ',', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 6)
                {
                    Console.WriteLine("Нужно ввести ровно 6 чисел.");
                    Console.Write(">>> ");
                    continue;
                }

                bool correct = true;

                for (int i = 0; i < 6; i++)
                {
                    if (!double.TryParse(
                        parts[i],
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out values[i]))
                    {
                        correct = false;
                        break;
                    }
                }

                if (!correct)
                {
                    Console.WriteLine("Ошибка ввода. Попробуйте еще раз.");
                    Console.Write(">>> ");
                    continue;
                }

                return new Vector3D(
                    values[0], values[1], values[2],
                    values[3], values[4], values[5]);
            }
        }

        static void CreateVector()
        {
            Vector3D vector = ReadVector("V");

            Console.WriteLine();
            Console.WriteLine("Вектор создан:");
            Console.WriteLine(vector);

            Pause();
        }

        static void ShowCoordinates()
        {
            Vector3D vector = ReadVector("V");

            double[] start = vector.GetStartPoint();
            double[] end = vector.GetEndPoint();

            Console.WriteLine();
            Console.WriteLine(
                $"Начальная точка: ({start[0]}; {start[1]}; {start[2]})");

            Console.WriteLine(
                $"Конечная точка: ({end[0]}; {end[1]}; {end[2]})");

            Pause();
        }

        static void CalculateLength()
        {
            Vector3D vector = ReadVector("V");

            Console.WriteLine();
            Console.WriteLine($"Длина вектора = {vector.Length():F4}");

            Pause();
        }

        static void AddVectors()
        {
            Vector3D a = ReadVector("A");
            Vector3D b = ReadVector("B");

            Vector3D result = a + b;

            Console.WriteLine();
            Console.WriteLine($"A + B = {result}");

            Pause();
        }

        static void SubtractVectors()
        {
            Vector3D a = ReadVector("A");
            Vector3D b = ReadVector("B");

            Vector3D result = a - b;

            Console.WriteLine();
            Console.WriteLine($"A - B = {result}");

            Pause();
        }

        static void CrossProduct()
        {
            Vector3D a = ReadVector("A");
            Vector3D b = ReadVector("B");

            Vector3D result = a * b;

            Console.WriteLine();
            Console.WriteLine($"A × B = {result}");

            Pause();
        }

        static void MultiplyVector()
        {
            Vector3D vector = ReadVector("V");

            Console.Write("Введите число: ");
            double number = double.Parse(
                Console.ReadLine()!,
                System.Globalization.CultureInfo.InvariantCulture);

            Vector3D result = vector * number;

            Console.WriteLine();
            Console.WriteLine($"V * {number} = {result}");

            Pause();
        }

        static void DivideVector()
        {
            Vector3D vector = ReadVector("V");

            Console.Write("Введите число: ");
            double number = double.Parse(
                Console.ReadLine()!,
                System.Globalization.CultureInfo.InvariantCulture);

            Vector3D result = vector / number;

            Console.WriteLine();
            Console.WriteLine($"V / {number} = {result}");

            Pause();
        }

        static void CalculateCosine()
        {
            Vector3D a = ReadVector("A");
            Vector3D b = ReadVector("B");

            double cosine = Vector3D.Cosine(a, b);

            Console.WriteLine();
            Console.WriteLine($"cos(α) = {cosine:F6}");

            Pause();
        }

        static void CompareVectors()
        {
            Vector3D a = ReadVector("A");
            Vector3D b = ReadVector("B");

            Console.WriteLine();
            Console.WriteLine($"|A| = {a.Length():F4}");
            Console.WriteLine($"|B| = {b.Length():F4}");
            Console.WriteLine();

            Console.WriteLine($"A > B  : {a > b}");
            Console.WriteLine($"A >= B : {a >= b}");
            Console.WriteLine($"A < B  : {a < b}");
            Console.WriteLine($"A <= B : {a <= b}");

            Pause();
        }

        static void CheckEquality()
        {
            Vector3D a = ReadVector("A");
            Vector3D b = ReadVector("B");

            Console.WriteLine();
            Console.WriteLine($"A == B : {a == b}");
            Console.WriteLine($"A != B : {a != b}");

            Pause();
        }

        static void CopyVector()
        {
            Vector3D original = ReadVector("V");

            Vector3D copy = new Vector3D(original);

            Console.WriteLine();
            Console.WriteLine($"Исходный вектор: {original}");
            Console.WriteLine($"Копия:           {copy}");
            Console.WriteLine($"Векторы равны:   {original == copy}");

            Pause();
        }

        static void ParseVector()
        {
            Console.WriteLine();
            Console.WriteLine("Введите 6 координат одной строкой.");
            Console.WriteLine("Например:");
            Console.WriteLine("1 2 3 4 5 6");
            Console.Write(">>> ");//чтение

            string text = Console.ReadLine()!;

            Vector3D vector = Vector3D.Parse(text);

            Console.WriteLine();
            Console.WriteLine($"Получен вектор: {vector}");//запись

            Pause();
        }

        static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }
}
