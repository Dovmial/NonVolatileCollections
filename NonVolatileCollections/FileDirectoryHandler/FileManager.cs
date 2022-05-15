using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonVolatileCollections.FileDirectoryHandler
{
    internal class FileManager
    {
        public string StartWorkPath { get; }
        public string CurrentPathDirectory { get; set; }
        public FileManager()
        {
            CurrentPathDirectory = StartWorkPath = Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathDirectory"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public FileManager CreateDirectory(string pathDirectory)
        {
            CurrentPathDirectory = pathDirectory;
            try
            {
                _ = Directory.CreateDirectory(pathDirectory);
                return this;
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
            
        }

        public void CreateFile(string filename)
        {
           _ = File.Create(Path.Combine(CurrentPathDirectory, filename));
        }

    }
}
