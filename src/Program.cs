#region License Information (GPL v3)

/**
 * Copyright (C) 2022 coreizer
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

#endregion

namespace ELFBuilder
{
   using System;
   using System.Diagnostics;
   using System.IO;
   using System.Linq;
   using System.Reflection;
   using System.Windows.Forms;

   public class Program
   {
      #region Constant

      private const string DependentProcess = "make_fself.exe";

      #endregion

      private static void AsciiHeader()
      {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine(@"
                                    d8b
                                    Y8P          

    .d8888b .d88b.  888d888 .d88b.  888 88888888 
   d88P""   d88""""88b 888P""  d8P  Y8b 888    d88P  
   888     888  888 888    88888888 888   d88P   
   Y88b.   Y88..88P 888    Y8b.     888  d88P    
    ""Y8888P ""Y88P""  888     ""Y8888  888 88888888
");

         Console.WriteLine("+--------------------+");
         Console.WriteLine("| ELF-Builder        |");
         Console.WriteLine("| by coreizer        |");
         Console.WriteLine("| Version 2.4(BETA)  |");
         Console.WriteLine("+-------------------+\n");
      }

      [STAThread]
      public static void Main(string[] args)
      {
         // Simple ELF to BIN Resigner
         AsciiHeader();

         try {
            var files = OpenFile(args);

            DependentExtract();

            foreach (var file in files) {
               var name = Path.GetFileName(file);
               var timestamp = $"{DateTime.Now.Ticks}--{Path.ChangeExtension(name, "")}bin";

               if (!File.Exists(DependentProcess)) {
                  throw new FileNotFoundException(DependentProcess);
               }

               var process = Process.Start(new ProcessStartInfo {
                  FileName = DependentProcess,
                  WorkingDirectory = Application.StartupPath,
                  CreateNoWindow = false,
                  UseShellExecute = false,
                  Arguments = $"{name} {timestamp}"
               });

               process.WaitForExit();
               process.Close();
            }
         }
         catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
         }
         finally {
            if (File.Exists(DependentProcess)) {
               File.Delete(DependentProcess);
            }
         }

         Console.ForegroundColor = ConsoleColor.White;
         Console.WriteLine("Press any key to exit...");
         Console.Read();
      }

      private static string[] OpenFile(string[] args)
      {
         if (args.Length < 1) {
            Console.WriteLine("Press any key to continue and Open a file dialog...");
            Console.ReadKey();

            using (var OFD = new OpenFileDialog {
               Multiselect = true,
               Filter = "Executable and Linking Format (*.elf)|*.elf"
            }) {
               if (OFD.ShowDialog() != DialogResult.OK) throw new Exception("Invalid the file or no file");

               return OFD.FileNames;
            }
         }

         if (!args.Any(x => Path.GetExtension(x) == ".elf")) {
            throw new FileLoadException("Error: Unable to open file or invalid the extension.");
         }

         return args;
      }

      private static void DependentExtract()
      {
         try {
            if (File.Exists(DependentProcess)) {
               File.Delete(DependentProcess);
            }

            var assembly = Assembly.GetExecutingAssembly();
            using (var resource = assembly.GetManifestResourceStream($"ELFBuilder.Resources.{DependentProcess}")) {
               var buf = GetArray(resource);
               using (var fileStream = new FileStream(DependentProcess, FileMode.CreateNew)) {
                  fileStream.Write(buf, 0, buf.Length);
                  fileStream.Flush();
               }
            }
         }
         catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
         }
      }

      private static byte[] GetArray(Stream baseStream)
      {
         using (var ms = new MemoryStream()) {
            baseStream.CopyTo(ms);
            return ms.ToArray();
         }
      }
   }
}
