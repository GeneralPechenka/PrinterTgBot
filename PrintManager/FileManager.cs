using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager
{
    public class FileManager
    {
        public string DestinationDirectory { get; } = "C:\\PrinterTgBot";

        public FileManager()
        {
            if(!Directory.Exists(DestinationDirectory))
                Directory.CreateDirectory(DestinationDirectory);
        }
        /// <summary>
        /// This method returnes path of the pdf-files and automatically changes if such file has already exsisted
        /// </summary>
        /// <param name="filename">Name of file with extention</param>
        /// <returns></returns>
        public string GetDestinationPath(string filename)
        {
            string destinationPath = DestinationDirectory + '\\' + filename;
            int incr = 1;
            while(File.Exists(destinationPath))
            {
                var fname=destinationPath.Substring(0,destinationPath.LastIndexOf('.'))+"_"+incr.ToString();
                destinationPath = fname + ".pdf";
                incr++;
            }
            return destinationPath;
        }
        //TODO: сделать метод удаления файла.
    }
}
