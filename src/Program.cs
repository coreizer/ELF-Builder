/*
 * Copyright (c) 2017 AlphaNyne
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
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ELFBuilder
{
  public class Program
  {
    [MTAThread]
    public static void Main(string[] args)
    {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("+----------------------------+");
      Console.WriteLine("| Simple ELF to BIN Resigner |");
      Console.WriteLine("| by AlphaNyne               |");
      Console.WriteLine("| Version 2.3 Beta           |");
      Console.WriteLine("+----------------------------+\n");

      try {
        if (args.Length < 1) {
          throw new Exception("Unable to find *.elf file");
        }

        if (!args.Any(x => Path.GetExtension(x) == ".elf")) {
          throw new FileLoadException("File extension is not allowed");
        }

        foreach (string arg in args) {

          string fileName = Path.GetFileName(arg);
          string exportName = $"{DateTime.Now.Ticks}-{Path.ChangeExtension(fileName, "")}bin";

          Console.WriteLine($"[INFO] Loaded filename: {fileName}");
          Console.WriteLine($"[INFO] Export name: {exportName}\n");

          ProcessStartInfo startInfo = new ProcessStartInfo {
            FileName = "make_fself.exe",
            WorkingDirectory = Application.StartupPath,
            CreateNoWindow = false,
            UseShellExecute = false,
            Arguments = $"{fileName} {exportName}"
          };

          Process process = Process.Start(startInfo);
          process.WaitForExit();
          process.Close();
        }
      }
      catch (Exception ex) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
      }
      finally {

        // make_fself.exeが存在する場合は、削除します。
        if (File.Exists("make_fself.exe")) {
          File.Delete("make_fself.exe");
        }
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Press any key to exit...");
      Console.Read();
    }
  }
}
