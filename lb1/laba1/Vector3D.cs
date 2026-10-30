using System;
using System.Globalization;
using System.Net.Sockets;
namespace Laba1
{
    /// <summary>
    /// Представляет вектор в трёхмерном пространстве,
    /// заданный координатами начальной и конечной точек.
    /// </summary>
    public class Vector3D// спрашивала у ии, он ответил, что это используется в словарях, списка и в принципе по правилам для object, но я не добавлляла, ведь у меня не используется
    {
        private double startX;
        private double startY;
        private double startZ;

        private double endX;
        private double endY;
        private double endZ;

        /// <summary>
        /// Создает нулевой вектор.
        /// </summary>
        public Vector3D()
        {
        }

        /// <summary>
        /// Создает вектор по координатам начала и конца.
        /// </summary>
        public Vector3D(
            double startX, double startY, double startZ,
            double endX, double endY, double endZ)
        {
            this.startX = startX;
            this.startY = startY;
            this.startZ = startZ;
            this.endX = endX;
            this.endY = endY;
            this.endZ = endZ;
        }

        /// <summary>
        /// Конструктор копирования.
        /// </summary>
        public Vector3D(Vector3D other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            startX = other.startX;
            startY = other.startY;
            startZ = other.startZ;

            endX = other.endX;
            endY = other.endY;
            endZ = other.endZ;
        }

        /// <summary>
        /// Получает координаты начальной точки.
        /// </summary>
        public double[] GetStartPoint()
        {
            return new[] { startX, startY, startZ };
        }

        /// <summary>
        /// Получает координаты конечной точки.
        /// </summary>
        public double[] GetEndPoint()
        {
            return new[] { endX, endY, endZ };
        }

        /// <summary>
        /// Получает компоненты вектора.
        /// </summary>
        private double[] Components()
        {
            return new[]
            {
            endX - startX,
            endY - startY,
            endZ - startZ
        };
        }

        /// <summary>
        /// Вычисляет длину вектора.
        /// </summary>
        public double Length()
        {
            double[] v = Components();

            return Math.Sqrt(
                v[0] * v[0] +
                v[1] * v[1] +
                v[2] * v[2]);
        }

        /// <summary>
        /// Складывает два вектора.
        /// </summary>
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            Check(a, b);

            double[] x = a.Components();
            double[] y = b.Components();

            return FromComponents(
                x[0] + y[0],
                x[1] + y[1],
                x[2] + y[2]);
        }

        /// <summary>
        /// Вычитает два вектора.
        /// </summary>
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            Check(a, b);

            double[] x = a.Components();
            double[] y = b.Components();

            return FromComponents(
                x[0] - y[0],
                x[1] - y[1],
                x[2] - y[2]);
        }

        /// <summary>
        /// Выполняет векторное произведение.
        /// </summary>
        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            Check(a, b);

            double[] x = a.Components();
            double[] y = b.Components();

            return FromComponents(
                x[1] * y[2] - x[2] * y[1],
                x[2] * y[0] - x[0] * y[2],
                x[0] * y[1] - x[1] * y[0]);
        }

        /// <summary>
        /// Умножает вектор на число.
        /// </summary>
        public static Vector3D operator *(Vector3D vector, double number)
        {
            if (vector == null)
                throw new ArgumentNullException(nameof(vector));

            double[] v = vector.Components();

            return FromComponents(
                v[0] * number,
                v[1] * number,
                v[2] * number);
        }

        /// <summary>
        /// Умножает число на вектор.
        /// </summary>
        public static Vector3D operator *(double number, Vector3D vector)
        {
            return vector * number;
        }

        /// <summary>
        /// Делит вектор на число.
        /// </summary>
        public static Vector3D operator /(Vector3D vector, double number)
        {
            if (vector == null)
                throw new ArgumentNullException(nameof(vector));

            if (Math.Abs(number) < double.Epsilon)
                throw new DivideByZeroException(
                    "Деление на ноль невозможно.");

            double[] v = vector.Components();

            return FromComponents(
                v[0] / number,
                v[1] / number,
                v[2] / number);
        }

        /// <summary>
        /// Вычисляет косинус угла между двумя векторами.
        /// </summary>
        public static double Cosine(Vector3D a, Vector3D b)
        {
            Check(a, b);

            double[] x = a.Components();
            double[] y = b.Components();

            double scalar =
                x[0] * y[0] +
                x[1] * y[1] +
                x[2] * y[2];

            double denominator = a.Length() * b.Length();

            if (Math.Abs(denominator) < double.Epsilon)
                throw new InvalidOperationException(
                    "Угол для нулевого вектора не определен.");

            return scalar / denominator;
        }

        /// <summary>
        /// Проверяет равенство двух векторов.
        /// </summary>
        public static bool operator ==(Vector3D? a, Vector3D? b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return Equal(a.startX, b.startX) &&
                   Equal(a.startY, b.startY) &&
                   Equal(a.startZ, b.startZ) &&
                   Equal(a.endX, b.endX) &&
                   Equal(a.endY, b.endY) &&
                   Equal(a.endZ, b.endZ);
        }

        /// <summary>
        /// Проверяет неравенство двух векторов.
        /// </summary>
        public static bool operator !=(Vector3D? a, Vector3D? b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Сравнивает векторы по длине.
        /// </summary>
        public static bool operator >(Vector3D a, Vector3D b)
        {
            Check(a, b);
            return a.Length() > b.Length();
        }

        /// <summary>
        /// Сравнивает векторы по длине.
        /// </summary>
        public static bool operator >=(Vector3D a, Vector3D b)
        {
            Check(a, b);
            return a.Length() >= b.Length();
        }

        /// <summary>
        /// Сравнивает векторы по длине.
        /// </summary>
        public static bool operator <(Vector3D a, Vector3D b)
        {
            Check(a, b);
            return a.Length() < b.Length();
        }

        /// <summary>
        /// Сравнивает векторы по длине.
        /// </summary>
        public static bool operator <=(Vector3D a, Vector3D b)
        {
            Check(a, b);
            return a.Length() <= b.Length();
        }

        /// <summary>
        /// Преобразует вектор в строку.
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,//чтобы точка для дроби была
                "({0}; {1}; {2}) -> ({3}; {4}; {5})",
                startX, startY, startZ,
                endX, endY, endZ);
        }

        /// <summary>
        /// Создает вектор из текстовой строки.
        /// </summary>
        /// <param name="text">
        /// Шесть координат через пробел.
        /// Например: 1 2 3 4 5 6.
        /// </param>
        public static Vector3D Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException(//передает в message
                    "Строка не должна быть пустой.");

            string[] parts = text.Split(
                new[] { ' ', ';', ',', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 6)
                throw new FormatException(
                    "Нужно ввести ровно 6 координат.");

            double[] values = new double[6];

            for (int i = 0; i < 6; i++)
            {
                if (!double.TryParse(
                    parts[i],
                    NumberStyles.Float,//чтобы принимало не только целочисленное
                    CultureInfo.InvariantCulture,
                    out values[i]))
                {
                    throw new FormatException(
                        $"Некорректное число: {parts[i]}");
                }
            }

            return new Vector3D(
                values[0], values[1], values[2],
                values[3], values[4], values[5]);
        }

        private static Vector3D FromComponents(double x, double y, double z)
        {
            return new Vector3D(0, 0, 0, x, y, z);
        }

        private static void Check(Vector3D? a, Vector3D? b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));
        }

        private static bool Equal(double a, double b)
        {
            return Math.Abs(a - b) < 1e-10;
        }
    }
}