﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlicingFile
{
    public class SlicingFile
    {
        public static void Main()
        {
            Console.Title = "File Slicer";

            while (true)
            {
                Console.WriteLine("What do you like to do: slice/assemble/exit");
                string operation = Console.ReadLine();
                while (operation != "slice" && operation != "assemble" && operation != "exit")
                {
                    Console.WriteLine($"Wrong command!{Environment.NewLine}Try again here:");
                    operation = Console.ReadLine();
                }

                switch (operation)
                {
                    case "slice":
                        Console.WriteLine("Which file would you like to slice?");
                        Console.WriteLine("Please, enter the full path to it here:");
                        string sourceFilePath = Path.GetFullPath(Console.ReadLine());

                        int startIndex = sourceFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                        int length = sourceFilePath.Length - startIndex;
                        string fileName = sourceFilePath.Substring(startIndex, length);

                        Console.WriteLine("Slices will be saved in the 'Sliced' folder!");
                        string destinationDirectory = @"..\..\..\Sliced";

                        Console.WriteLine($"Please, enter the number of slices you want to cut '{fileName}' into:");
                        int parts = int.Parse(Console.ReadLine());

                        Console.Clear();
                        Console.WriteLine($"'{fileName}' is being sliced now...");
                        Slice(sourceFilePath, destinationDirectory, parts);
                        Console.WriteLine("Completed!");
                        break;
                    case "assemble":
                        Console.WriteLine("Please, enter the path to the directory containing the files to assemble:");
                        string assemblingDirectory = Path.GetFullPath(Console.ReadLine());
                        List<string> files = Directory.GetFiles(assemblingDirectory).ToList();

                        Console.WriteLine("Assembled file will be saved in 'Assembled' directory");
                        char dirSeparator = Path.DirectorySeparatorChar;
                        destinationDirectory = $"..{dirSeparator}..{dirSeparator}..{dirSeparator}Assembled";

                        Console.WriteLine("Assembling...");
                        Assemble(files, destinationDirectory);
                        Console.WriteLine("Completed!");
                        break;
                    case "exit":
                        Console.Clear();
                        Console.WriteLine("Good bye!");
                        return;
                }
            }
        }

        private static void Assemble(List<string> files, string destinationDirectory)
        {
            Directory.CreateDirectory(destinationDirectory);

            string assembledFileExtension = Path.GetExtension(files[0]);
            string assembledFileName = "assembled";
            string assembledFilePath = Path.Combine(destinationDirectory, assembledFileName + assembledFileExtension);

            for (int i = 0; i < files.Count; i++)
            {
                using (var reader = new FileStream(files[i], FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var writer = new FileStream(assembledFilePath, FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[reader.Length];
                        int readBytes = reader.Read(buffer, 0, buffer.Length);

                        writer.Write(buffer);
                    }
                }
            }
        }

        private static void Slice(string sourceFile, string destinationDirectory, int parts)
        {
            Directory.CreateDirectory(destinationDirectory);

            using (var reader = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string fileExtension = Path.GetExtension(sourceFile);

                long sourceFileBytes = reader.Length;
                long newFilesSize = (long)Math.Ceiling(sourceFileBytes / (double)parts);

                for (int i = 0; i < parts; i++)
                {
                    string currentSlice = $"slice{i + 1}{fileExtension}";
                    string outputPath = Path.Combine(destinationDirectory, currentSlice);

                    byte[] buffer = new byte[newFilesSize];
                    int readBytes = reader.Read(buffer, 0, buffer.Length);

                    using (var writer = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        writer.Write(buffer, 0, readBytes);
                    }
                }
            }
        }
    }
}
