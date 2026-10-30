using Laba1;
using System;
using System.Text;

class EntryPoint
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("1 — Машина Поста");
        Console.WriteLine("2 — Вектор 3D");
        Console.Write("Выбор: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1": MenuPost.Run(); break;
            case "2": MenuVector.Run(); break;
            default: Console.WriteLine("Неверный выбор"); break;
        }
    }

}
