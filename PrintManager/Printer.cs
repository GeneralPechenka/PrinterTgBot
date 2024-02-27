using PDFtoPrinter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager
{
    public class Printer
    {
        public string Name { get; private set; }
        [System.Text.Json.Serialization.JsonIgnore]
        private Queue<string> files;
        
        //TODO:сделать парсинг имени принтера из JSON или другого файла
        public Printer() 
        {
#if DEBUG
            Name = "Microsoft Print to PDF";
#else
            
#endif
           files=new Queue<string>();
        }
        public Printer(string printerName)
        {
            Name = printerName;
            files = new Queue<string>();
        }
        public Printer(string printerName, Queue<string> files)
        {
            Name=printerName;
            files = new Queue<string>();
        }
        private void SerializePrinter()
        {

        }
        public async Task PrintAsync(string absoluteFilePath) 
        {
            var printer = new PDFtoPrinterPrinter();
            await printer.Print(new PrintingOptions(Name, absoluteFilePath));
        }
        public void AddFileToQueue(string filePath) => files.Enqueue(filePath);

        public async Task PrintQueue()
        {
            if (files.Count == 0) return;
            for(int i=0; i<files.Count; i++)
            {
                await PrintAsync(files.Dequeue());
            }
            
        }
    }
}
