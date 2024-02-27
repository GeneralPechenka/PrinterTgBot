using System.Reflection;

namespace PrinterTgBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.SetValue("Your Application Name", Assembly.GetExecutingAssembly().Location);
            Bot bot = new Bot();
            bot.StartReceiving();
            Console.WriteLine("Бот запущен!");
            Console.ReadLine();
            
        }
    }
}