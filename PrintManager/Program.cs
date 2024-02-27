namespace PrintManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            FileManager fileManager = new FileManager();
            Console.WriteLine(fileManager.GetDestinationPath("Вопросы для практики по 1С.pdf"));
            Printer printer= new Printer();
            printer.PrintAsync("C:\\PrinterTgBot\\" + "Вопросы для практики по 1С.pdf");
        }
    }
}