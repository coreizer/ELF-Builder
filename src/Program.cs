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
      private const string DEPENDENT_PROCESS = "make_fself.exe";

      [MTAThread]
      public static void Main(string[] args)
      {
         // Simple ELF to BIN Resigner
         Console.ForegroundColor = ConsoleColor.Green;
         Console.WriteLine("+--------------------+");
         Console.WriteLine("| ELF-Builder        |");
         Console.WriteLine("| by coreizer        |");
         Console.WriteLine("| Version 2.3[BETA]  |");
         Console.WriteLine("+-------------------+\n");

         try {
            if (args.Length < 1) {
               throw new Exception("Error: File cannot be found.");
            }
            if (!args.Any(x => Path.GetExtension(x) == ".elf")) {
               throw new FileLoadException("Error: Unable to open file or invalid the extension.");
            }

            DependentExtract();

            foreach (string file in args) {
               string name = Path.GetFileName(file);
               string timestamp = $"{DateTime.Now.Ticks}--{Path.ChangeExtension(name, "")}bin";

               if (!File.Exists(DEPENDENT_PROCESS)) {
                  throw new FileNotFoundException(DEPENDENT_PROCESS);
               }

               Process process = Process.Start(new ProcessStartInfo
               {
                  FileName = DEPENDENT_PROCESS,
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
            if (File.Exists(DEPENDENT_PROCESS)) {
               File.Delete(DEPENDENT_PROCESS);
            }
         }

         Console.ForegroundColor = ConsoleColor.Green;
         Console.WriteLine("Press any key to exit...");
         Console.Read();
      }

      private static void DependentExtract()
      {
         try {
            if (File.Exists(DEPENDENT_PROCESS)) {
               File.Delete(DEPENDENT_PROCESS);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream resource = assembly.GetManifestResourceStream($"ELFBuilder.Resources.{DEPENDENT_PROCESS}")) {
               byte[] buffer = GetArray(resource);
               using (FileStream fileStream = new FileStream(DEPENDENT_PROCESS, FileMode.CreateNew)) {
                  fileStream.Write(buffer, 0, buffer.Length);
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
         using (MemoryStream bufferStream = new MemoryStream()) {
            baseStream.CopyTo(bufferStream);
            return bufferStream.ToArray();
         }
      }
   }
}
