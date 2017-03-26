using System;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager man = new Manager();
            DateTime currentDate = DateTime.Now;
            do
            {
                Console.WriteLine("   {0:N0} ticks", currentDate.Ticks);                
                Console.WriteLine(currentDate.Ticks); 
                    
                man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}