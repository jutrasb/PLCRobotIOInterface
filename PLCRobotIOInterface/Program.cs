using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using FRRobot;
using ModbusTCPScan;
using MBTCPCOMMDTMLib;
using MBTCP_InterfacesLib;
using NDesk.Options;

namespace PLCRobotIOInterface
{
    class Program
    {
        public static FRCRobot robot;

        public static string sHostname = string.Empty;
        public static bool bShowDebug = false;
        public static bool bShowHelp = false;

        public static bool bEvent = false;

        public enum EventType
        {
            Change = 0,
            Delete = 1,
            Unsimulate = 2,
        }

        public static EventType eventType;
        public static FRCIOSignal eventSender;
        public static FREIOTypeConstants eventSenderType;

        // Declare objects to the other classes.
        public static UI ui = new UI();
        public static Utility utility = new Utility();

        public enum DINumber : long
        {
            DI_CARR_DAT_RDY = 1,
            DI_SPARE_1      = 2, // Rename when/if used in the future.
            DI_SPARE_2      = 3, // Rename when/if used in the future.
            DI_ON_LINE      = 4,
            DI_LINE_STOP    = 5,
            DI_PE_SIGNAL    = 6,
            DI_NXT_MLD_RDY  = 7,
            DI_MACRO_RUN    = 8,
        }

        public enum DONumber : long
        {
            DO_CARR_DAT_ACK     = 1,
            DO_NXT_MLD_DAT_RDY  = 2,
            DO_PATH_RUNG        = 3,
            DO_SYS_RUNG         = 4,
            DO_FAN_1_REQ        = 5,
            DO_FAN_2_REQ        = 6,
            DO_HI_PUFF_REQ      = 7,
            DO_LO_PUFF_REQ      = 8,

            DO_PH_MOL_BIT0      = 17,
            DO_PH_MOL_BIT1      = 18,
            DO_PH_MOL_BIT2      = 19,
            DO_LINEUP_ERR       = 20,
            DO_PROGRAM          = 21,
            DO_HEAD_OPEN        = 22,
            DO_PROG_RUNG        = 23,
            DO_OPEN_PH          = 24,
        }

        public enum GINumber : long
        {
            GI_CARR = 1,
        }

        public enum GONumber : long
        {
            GO_PH_CARR = 1,
        }

        public enum UOPINumber : long
        {
            UOPI_IMSTP  = 1,
            UOPI_HOLD   = 2,
            UOPI_SFSPD  = 3,
            UOPI_CSTOP  = 4,
            UOPI_RESET  = 5,
            UOPI_START  = 6,
            UOPI_HOME   = 7,
            UOPI_ENABL  = 8,
        }

        public enum UOPONumber : long
        {
            UOPO_CMDEN  = 1,
            UOPO_SYSRD  = 2,
            UOPO_PROGR  = 3,
            UOPO_PAUSD  = 4,
            UOPO_HELD   = 5,
            UOPO_FAULT  = 6,
            UOPO_PERCH  = 7,
            UOPO_TPENB  = 8,
        };

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
                ui.WriteErrorMessage(e.Message);
                ui.ShowHelpMessage(optionSet);
                ui.ShowExitMessage();
                return;
            }

            if (bShowHelp)
            {
                // User has requested the help message.
                ui.ShowHelpMessage(optionSet);
                ui.ShowExitMessage();
                return;
            }

            if (sHostname == string.Empty)
            {
                // User did not provide required hostname field. 
                ui.WriteErrorMessage("A hostname or IP address must be provided.");
                ui.ShowHelpMessage(optionSet);
                ui.ShowExitMessage();
                return;
            }

            // Check to see if sHostname matches an IP Address or Hostname pattern.
            string sRegexIPAddressPattern = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            string sRegexHostnamePattern = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
            Regex rIPAddress = new Regex(sRegexIPAddressPattern);
            Regex rHostname = new Regex(sRegexHostnamePattern);

            if (!rIPAddress.Match(sHostname).Success && !rHostname.Match(sHostname).Success)
            {
                ui.WriteErrorMessage("The hostname/IP address provided is not valid.");
                ui.ShowHelpMessage(optionSet);
                ui.ShowExitMessage();
                return;
            }

            // Unnecessary to check every time, bShowDebug is checked within WriteDebugMessage().
            if (bShowDebug)
            {
                // User has requested debug messages.
                ui.WriteDebugMessage("Debug messages will be shown.");
            }

            ui.ShowTitleMessage();

            ui.WriteDebugMessage("User provided a valid hostname/IP address.");
            try
            {
                ui.WriteDebugMessage("Attempting to create a new FRCRobot object using the FRRobot library.");
                robot = new FRCRobot();

                ui.WriteDebugMessage("Attempting to connect to the robot at the hostname/IP address.");
                robot.Connect(sHostname);
            }
            catch (Exception e)
            {
                ui.WriteErrorMessage(e.Message);
            }

            if (robot.IsConnected)
            {
                ui.WriteDebugMessage("Sucessfully connected to robot at {0}.", sHostname);
            }

            try
            {
                /*
                MBTCP_Core c = new MBTCP_Core();
                c.GetCommChannel();
                modbusTCPScanner = new ModbusTCPScanner();
                modbusTCPScanner.Scan("127.0.0.1", 1000);
                */
            }
            catch (Exception e)
            {
                ui.WriteErrorMessage("Problem creating/connecting to Modbus PLC - {0}", e.Message);
            }

            FRCUOPIOSignal UOPI_IMSTP    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPInType, (long)UOPINumber.UOPI_IMSTP);
            FRCUOPIOSignal UOPI_HOLD     = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPInType, (long)UOPINumber.UOPI_HOLD);
            FRCUOPIOSignal UOPI_SFSPD    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPInType, (long)UOPINumber.UOPI_SFSPD);
            FRCUOPIOSignal UOPI_CSTOP    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPInType, (long)UOPINumber.UOPI_CSTOP);
            FRCUOPIOSignal UOPI_RESET    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPInType, (long)UOPINumber.UOPI_RESET);
            FRCUOPIOSignal UOPI_START    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPInType, (long)UOPINumber.UOPI_START);
            FRCUOPIOSignal UOPI_HOME     = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPInType, (long)UOPINumber.UOPI_HOME);
            FRCUOPIOSignal UOPI_ENABL    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPInType, (long)UOPINumber.UOPI_ENABL);
            List<FRCUOPIOSignal> UOPI_SIGNALS = new List<FRCUOPIOSignal> { UOPI_IMSTP, UOPI_HOLD, UOPI_SFSPD, UOPI_CSTOP, UOPI_RESET, UOPI_START, UOPI_HOME, UOPI_ENABL };

            FRCGroupIOSignal GI_CARR = (FRCGroupIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frGPInType, (long)GINumber.GI_CARR);
            List<FRCGroupIOSignal> GI_SIGNALS = new List<FRCGroupIOSignal> { GI_CARR };

            FRCDigitalIOSignal DI_CARR_DAT_RDY = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDInType, (long)DINumber.DI_CARR_DAT_RDY);
            //FRCDigitalIOSignal DI_SPARE_1    = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDInType, (long)DINumber.DI_SPARE_1);
            //FRCDigitalIOSignal DI_SPARE_2    = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDInType, (long)DINumber.DI_SPARE_2);
            FRCDigitalIOSignal DI_ON_LINE      = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDInType, (long)DINumber.DI_ON_LINE);
            FRCDigitalIOSignal DI_LINE_STOP    = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDInType, (long)DINumber.DI_LINE_STOP);
            FRCDigitalIOSignal DI_PE_SIGNAL    = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDInType, (long)DINumber.DI_PE_SIGNAL);
            FRCDigitalIOSignal DI_NXT_MLD_RDY  = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDInType, (long)DINumber.DI_NXT_MLD_RDY);
            FRCDigitalIOSignal DI_MACRO_RUN    = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDInType, (long)DINumber.DI_MACRO_RUN);
            List<FRCDigitalIOSignal> DI_SIGNALS = new List<FRCDigitalIOSignal> { DI_CARR_DAT_RDY, /* DI_SPARE_1, DI_SPARE_2, */ DI_ON_LINE, DI_LINE_STOP, DI_PE_SIGNAL, DI_NXT_MLD_RDY, DI_MACRO_RUN };

            FRCUOPIOSignal UOPO_CMDEN    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPOutType, (long)UOPONumber.UOPO_CMDEN);
            FRCUOPIOSignal UOPO_SYSRD    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPOutType, (long)UOPONumber.UOPO_SYSRD);
            FRCUOPIOSignal UOPO_PROGR    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPOutType, (long)UOPONumber.UOPO_PROGR);
            FRCUOPIOSignal UOPO_PAUSD    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPOutType, (long)UOPONumber.UOPO_PAUSD);
            FRCUOPIOSignal UOPO_HELD     = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPOutType, (long)UOPONumber.UOPO_HELD);
            FRCUOPIOSignal UOPO_FAULT    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPOutType, (long)UOPONumber.UOPO_FAULT);
            FRCUOPIOSignal UOPO_PERCH    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPOutType, (long)UOPONumber.UOPO_PERCH);
            FRCUOPIOSignal UOPO_TPENB    = (FRCUOPIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frUOPOutType, (long)UOPONumber.UOPO_TPENB);
            List<FRCUOPIOSignal> UOPO_SIGNALS = new List<FRCUOPIOSignal> { UOPO_CMDEN, UOPO_SYSRD, UOPO_PROGR, UOPO_PAUSD, UOPO_HELD, UOPO_FAULT, UOPO_PERCH, UOPO_TPENB };

            FRCDigitalIOSignal DO_CARR_DAT_ACK      = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_CARR_DAT_ACK);
            FRCDigitalIOSignal DO_NXT_MLD_DAT_RDY   = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_NXT_MLD_DAT_RDY);
            FRCDigitalIOSignal DO_PATH_RUNG         = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_PATH_RUNG);
            FRCDigitalIOSignal DO_SYS_RUNG          = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_SYS_RUNG);
            FRCDigitalIOSignal DO_FAN_1_REQ         = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_FAN_1_REQ);
            FRCDigitalIOSignal DO_FAN_2_REQ         = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_FAN_2_REQ);
            FRCDigitalIOSignal DO_HI_PUFF_REQ       = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_HI_PUFF_REQ);
            FRCDigitalIOSignal DO_LO_PUFF_REQ       = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_LO_PUFF_REQ);

            FRCGroupIOSignal GO_PH_CARR = (FRCGroupIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frGPOutType, (long)GONumber.GO_PH_CARR);
            List<FRCGroupIOSignal> GO_SIGNALS = new List<FRCGroupIOSignal> { GO_PH_CARR };

            FRCDigitalIOSignal DO_PH_MOL_BIT0   = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_PH_MOL_BIT0);
            FRCDigitalIOSignal DO_PH_MOL_BIT1   = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_PH_MOL_BIT1);
            FRCDigitalIOSignal DO_PH_MOL_BIT2   = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_PH_MOL_BIT2);
            FRCDigitalIOSignal DO_LINEUP_ERR    = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_LINEUP_ERR);
            FRCDigitalIOSignal DO_PROGRAM       = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_PROGRAM);
            FRCDigitalIOSignal DO_HEAD_OPEN     = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_HEAD_OPEN);
            FRCDigitalIOSignal DO_PROG_RUNG     = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_PROG_RUNG);
            FRCDigitalIOSignal DO_OPEN_PH       = (FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)DONumber.DO_OPEN_PH);
            List<FRCDigitalIOSignal> DO_SIGNALS = new List<FRCDigitalIOSignal> { DO_CARR_DAT_ACK, DO_NXT_MLD_DAT_RDY, DO_PATH_RUNG, DO_SYS_RUNG, DO_FAN_1_REQ, DO_FAN_2_REQ, DO_HI_PUFF_REQ, DO_LO_PUFF_REQ,
                DO_PH_MOL_BIT0, DO_PH_MOL_BIT1, DO_PH_MOL_BIT2, DO_LINEUP_ERR, DO_PROGRAM, DO_HEAD_OPEN, DO_PROG_RUNG, DO_OPEN_PH};

            foreach (FRCDigitalIOSignal signal in DI_SIGNALS)
            {
                ui.WriteDebugMessage("{0,18} = \"{1}\"", Enum.GetName(typeof(DINumber), signal.LogicalNum), signal.Value.ToString());
            }

            foreach (FRCDigitalIOSignal signal in DO_SIGNALS)
            {
                signal.StartMonitor(1);
                //signal.Change += delegate () { eventType = EventType.Change; eventSender = (FRCIOSignal)signal; eventSenderType = FREIOTypeConstants.frDOutType; bEvent = true; };
                signal.Change += delegate () { utility.RobotIO_Change((FRCIOSignal)signal, FREIOTypeConstants.frDOutType); };
                ui.WriteDebugMessage("{0,18} = \"{1}\"", Enum.GetName(typeof(DONumber), signal.LogicalNum), signal.Value.ToString());
            }

            foreach (FRCGroupIOSignal signal in GI_SIGNALS)
            {
                ui.WriteDebugMessage("{0,18} = \"{1}\"", Enum.GetName(typeof(GINumber), signal.LogicalNum), signal.Value.ToString());
            }

            foreach (FRCGroupIOSignal signal in GO_SIGNALS)
            {
                //signal.StartMonitor(1);
                ui.WriteDebugMessage("{0,18} = \"{1}\"", Enum.GetName(typeof(GONumber), signal.LogicalNum), signal.Value.ToString());
            }

            foreach (FRCUOPIOSignal signal in UOPI_SIGNALS)
            {
                ui.WriteDebugMessage("{0,18} = \"{1}\"", Enum.GetName(typeof(UOPINumber), signal.LogicalNum), signal.Value.ToString());
            }

            foreach (FRCUOPIOSignal signal in UOPO_SIGNALS)
            {
                ui.WriteDebugMessage("{0,18} = \"{1}\"", Enum.GetName(typeof(UOPONumber), signal.LogicalNum), signal.Value.ToString());
                //signal.StartMonitor(1);
            }

            ui.WriteDebugMessage("Robot Time: {0}", robot.SysInfo.Clock.ToLongTimeString());

            ui.WriteDebugMessage("DO[2] = \"{0}\"", ((FRCDigitalIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frDOutType, (long)2)).Value.ToString());
            ui.WriteDebugMessage("GO[1] = \"{0}\"", ((FRCGroupIOSignal)utility.GetRobotIOSignal(robot, FREIOTypeConstants.frGPOutType, (long)1)).Value.ToString());

            //ioSignal = ioType.
            //ioTypes.Add(ioType);

            while (!bEvent)
            {
                if (bEvent)
                {
                    ui.WriteDebugMessage("****** EVENT TRIGGERED ******");
                    ui.WriteDebugMessage("Event Type:        {0}", Enum.GetName(typeof(EventType), eventType));
                    ui.WriteDebugMessage("Event Sender:      {0}", Enum.GetName(typeof(DONumber), eventSender.LogicalNum));
                    ui.WriteDebugMessage("Event Sender Type: {0}", Enum.GetName(typeof(FREIOTypeConstants), eventSenderType));
                    ui.WriteDebugMessage("*****************************");
                }
            }

            /*
            foreach (FRCDigitalIOSignal signal in DO_SIGNALS)
            {
                ui.WriteDebugMessage(".Equals {0,18}?: {1}", Enum.GetName(typeof(DONumber), signal.LogicalNum), LastChangeEventSender.Equals(signal));
            }
            */

            ui.ShowExitMessage();
        }
    }
}
