using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;

namespace PLCRobotIOInterface
{
    class UI
    {
        public string sLatestBuildNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public DateTime dtLatestBuild = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

        /// <summary>
        /// Prints the application title message.
        /// </summary>
        public void ShowTitleMessage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("PLC Robot I/O Interface");
            Console.WriteLine("By: Brandon Jutras");
            Console.WriteLine("Build: {0} {1}", sLatestBuildNumber, dtLatestBuild.ToShortDateString());
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Prints the application usage message detailing the options within optionSet
        /// </summary>
        /// <param name="optionSet">NDesk.Options OptionSet object holding all commandline options for the application.</param>
        public void ShowHelpMessage(OptionSet optionSet)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            ShowTitleMessage();
            Console.Write("USAGE: ");
            Console.Write("PLCRobotIOInterface [OPTIONS]\n");
            Console.WriteLine();
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        /// <summary>
        /// Prints "Press any key to exit..." and waits for user input. This function holds the console window open.
        /// </summary>
        public void ShowExitMessage()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Press any key to exit...");
            Console.ReadKey();
            Console.WriteLine();
        }

        /// <summary>
        /// Writes a debug message to Console.Out using standard application formatting.
        /// </summary>
        /// <param name="format">Standard C# format string.</param>
        /// <param name="args">Optional object arguments to be formatted into the output string.</param>
        public void WriteDebugMessage(string format, params object[] args)
        {
            if (Program.bShowDebug)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("DEBUG: ");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(string.Format(format, args) + "\n");
                }
                catch (Exception e)
                {
                    // Worst case scenario: bad string format. Try parsing format as literal string.
                    Console.Write(format.ToString() + "\n");
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// Writes an error message to Console.Out using standard application formatting.
        /// </summary>
        /// <param name="format">Standard C# format string.</param>
        /// <param name="args">Optional object arguments to be formatted into the output string.</param>
        public void WriteErrorMessage(string format, params object[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("ERROR: ");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(string.Format(format, args) + "\n");
            }
            catch (Exception e)
            {
                // Worst case scenario: bad string format. Try parsing format as literal string.
                Console.Write(format.ToString() + "\n");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Writes an error message to Console.Out using standard application formatting.
        /// </summary>
        /// <param name="format">Standard C# format string.</param>
        /// <param name="args">Optional object arguments to be formatted into the output string.</param>
        public void WriteRobotErrorMessage(string format, params object[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("ROBOT ERROR: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(string.Format(format, args) + "\n");
            }
            catch (Exception e)
            {
                // Worst case scenario: bad string format. Try parsing format as literal string.
                Console.Write(format.ToString() + "\n");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
