using System;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager man = new Manager();
            do
            {                    
                man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}