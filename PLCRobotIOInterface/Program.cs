using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FRRobot;
using NDesk.Options;

namespace PLCRobotIOInterface
{
    class Program
    {
        public static string sLatestBuildNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static DateTime dtLatestBuild = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

        public static FRCRobot robot;
        public static FRCRobotErrorInfo robotErrorInfo;

        public static string sHostname = string.Empty;
        public static bool bShowDebug = false;
        public static bool bShowHelp = false;

        static void Main(string[] args)
        {
            OptionSet optionSet = new OptionSet()
            {
                { "h=|hostname=",   "The {HOSTNAME} (or IP address) of the robot.", v => sHostname = v },
                { "d|debug",        "Show debug messages.",                         v => bShowDebug = v != null },
                { "?|help",         "Show this help message and exit.",             v => bShowHelp = v != null }
            };

            List<string> lsArgs;
            try
            {
                lsArgs = optionSet.Parse(args);
            }
            catch (OptionException e)
            {
                WriteErrorMessage(e.Message);
                ShowHelpMessage(optionSet);
                ShowExitMessage();
                return;
            }

            if (bShowHelp)
            {
                // User has requested the help message.
                ShowHelpMessage(optionSet);
                ShowExitMessage();
                return;
            }

            if (sHostname == string.Empty)
            {
                // User did not provide required hostname field. 
                WriteErrorMessage("A hostname or IP address must be provided.");
                ShowHelpMessage(optionSet);
                ShowExitMessage();
                return;
            }

            // Unnecessary to check every time, bShowDebug is checked within WriteDebugMessage().
            if (bShowDebug)
            {
                // User has requested debug messages.
                WriteDebugMessage("Debug messages will be shown.");
            }

            // Check to see if sHostname matches an IP Address or Hostname pattern.
            string sRegexIPAddressPattern = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            string sRegexHostnamePattern = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
            Regex rIPAddress = new Regex(sRegexIPAddressPattern);
            Regex rHostname = new Regex(sRegexHostnamePattern);

            if (!rIPAddress.Match(sHostname).Success && !rHostname.Match(sHostname).Success)
            {
                WriteErrorMessage("The hostname/IP address provided is not valid.");
                ShowExitMessage();
                return;
            }

            ShowTitleMessage();

            WriteDebugMessage("User provided a valid hostname/IP address.");
            try
            {
                WriteDebugMessage("Attempting to create a new FRCRobot object using the FRRobot library.");
                robot = new FRCRobot();

                WriteDebugMessage("Attempting to connect to the robot at the hostname/IP address.");
                robot.Connect(sHostname);

                WriteDebugMessage("Checking for potential robot error info.");
                robotErrorInfo = robot.GetErrorInfo();
                WriteRobotErrorMessage(robotErrorInfo.Description);
            }
            catch (Exception e)
            {
                WriteErrorMessage(e.Message);
            }

           ShowExitMessage();
        }

        static void ShowTitleMessage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("PLC Robot I/O Interface");
            Console.WriteLine("By: Brandon Jutras");
            Console.WriteLine("Latest Build Date: " + sLatestBuildNumber + " " + dtLatestBuild.ToShortDateString());
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void ShowHelpMessage(OptionSet optionSet)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            ShowTitleMessage();
            Console.WriteLine();
            Console.Write("USAGE: ");
            Console.Write("PLCRobotIOInterface [OPTIONS]\n");
            Console.WriteLine();
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        static void ShowExitMessage()
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
        static void WriteDebugMessage(string format, params object[] args)
        {
            if (bShowDebug)
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
        static void WriteErrorMessage(string format, params object[] args)
        {
            if (bShowDebug)
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
        }

        /// <summary>
        /// Writes an error message to Console.Out using standard application formatting.
        /// </summary>
        /// <param name="format">Standard C# format string.</param>
        /// <param name="args">Optional object arguments to be formatted into the output string.</param>
        static void WriteRobotErrorMessage(string format, params object[] args)
        {
            if (bShowDebug)
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
            }
        }
    }
}
