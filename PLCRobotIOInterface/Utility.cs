using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FRRobot;

namespace PLCRobotIOInterface
{
    class Utility
    {
        public static UI ui = new UI();
        
        /// <summary>
        /// Sets the FRCDigitalIOSignal at dioIndex. Should only be used to override robot IO.
        /// </summary>
        /// <param name="robot">FRCRobot object that is connected to the robot.</param>
        /// <param name="dioIndex">The index of the digital out signal to set.</param>
        public void SetDigitalOutSignal(FRCRobot robot, long dioIndex, bool dioValue)
        {
            try
            {
                FRCDigitalIOType dioType = (FRCDigitalIOType)robot.IOTypes[FREIOTypeConstants.frDOutType];
                FRCDigitalIOSignal dioSignal = (FRCDigitalIOSignal)dioType.Signals[dioIndex];
                dioSignal.Value = dioValue;
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// Gets the FRCIOSignal of FREIOTypeConstants at ioIndex. It is recommended that you typecast the returned FRCIOSignal to the correct IO signal type.
        /// </summary>
        /// <param name="robot">FRCRobot object that is connected to the robot.</param>
        /// <param name="ioTypeConstant">The intended IO type constant from FREIOTypeConstants.</param>
        /// <param name="dioIndex">The index of the digital out signal to get.</param>
        /// <returns></returns>
        public FRCIOSignal GetRobotIOSignal(FRCRobot robot, FREIOTypeConstants ioTypeConstant, long ioIndex)
        {
            try
            {
                if (robot.IsConnected)
                {
                    // Stacked case statements create the logical OR condition.
                    // http://stackoverflow.com/questions/848472/how-add-or-in-switch-statements
                    switch (ioTypeConstant)
                    {
                        case FREIOTypeConstants.frAInType:
                        case FREIOTypeConstants.frAOutType:
                            ((FRCAnalogIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCAnalogIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frDInType:
                        case FREIOTypeConstants.frDOutType:
                            ((FRCDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frFlagType:
                            ((FRCFlagType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCFlagType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frGPInType:
                        case FREIOTypeConstants.frGPOutType:
                            ((FRCGroupIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCGroupIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frLAInType:
                        case FREIOTypeConstants.frLAOutType:
                            return ((FRCLaserAnalogIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frLDInType:
                        case FREIOTypeConstants.frLDOutType:
                            ((FRCLaserDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCLaserDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frMarkerType:
                            ((FRCMarkerType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCMarkerType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frMaxIOType:
                            return new FRCIOSignal(); // There is no FRCMaxIOType.
                            break;

                        case FREIOTypeConstants.frPLCInType:
                        case FREIOTypeConstants.frPLCOutType:
                            ((FRCPLCIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCPLCIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frRDInType:
                        case FREIOTypeConstants.frRDOutType:
                            return new FRCIOSignal(); // There is no FRCRDIOType.
                            break;

                        case FREIOTypeConstants.frSOPInType:
                        case FREIOTypeConstants.frSOPOutType:
                            ((FRCSOPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCSOPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frTPInType:
                        case FREIOTypeConstants.frTPOutType:
                            ((FRCTPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCTPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frUOPInType:
                        case FREIOTypeConstants.frUOPOutType:
                            ((FRCUOPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCUOPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frWDInType:
                        case FREIOTypeConstants.frWDOutType:
                            ((FRCWeldDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCWeldDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            break;

                        case FREIOTypeConstants.frWSTKInType:
                        case FREIOTypeConstants.frWSTKOutType:
                            ((FRCWeldStickIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex].Refresh();
                            return ((FRCWeldStickIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];

                        default:
                            return new FRCIOSignal();
                            break;
                    }
                }
                return new FRCIOSignal();
            }
            catch (Exception e)
            {
                return new FRCIOSignal();
            }
        }

        /// <summary>
        /// Sets the FRCIOSignal of FREIOTypeConstants at ioIndex.
        /// </summary>
        /// <param name="robot">FRCRobot object that is connected to the robot.</param>
        /// <param name="ioTypeConstant">The intended IO type constant from FREIOTypeConstants.</param>
        /// <param name="dioIndex">The index of the digital out signal to get.</param>
        /// <returns></returns>
        public void SetRobotIOSignal(FRCRobot robot, FREIOTypeConstants ioTypeConstant, long ioIndex, FRCIOSignal ioSignal)
        {
            try
            {
                if (robot.IsConnected)
                {
                    // Stacked case statements create the logical OR condition.
                    // http://stackoverflow.com/questions/848472/how-add-or-in-switch-statements
                    switch (ioTypeConstant)
                    {
                        case FREIOTypeConstants.frAInType:
                        case FREIOTypeConstants.frAOutType:
                            FRCAnalogIOSignal aioSignal = (FRCAnalogIOSignal)((FRCAnalogIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            aioSignal.Comment = ((FRCAnalogIOSignal)ioSignal).Comment;
                            aioSignal.Value = ((FRCAnalogIOSignal)ioSignal).Value;
                            aioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frDInType:
                        case FREIOTypeConstants.frDOutType:
                            FRCDigitalIOSignal dioSignal = (FRCDigitalIOSignal)((FRCDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            dioSignal.Comment = ((FRCDigitalIOSignal)ioSignal).Comment;
                            dioSignal.Value = ((FRCDigitalIOSignal)ioSignal).Value;
                            dioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frFlagType:
                            FRCFlagSignal fioSignal = (FRCFlagSignal)((FRCFlagType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            fioSignal.Comment = ((FRCFlagSignal)ioSignal).Comment;
                            fioSignal.Value = ((FRCFlagSignal)ioSignal).Value;
                            fioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frGPInType:
                        case FREIOTypeConstants.frGPOutType:
                            FRCGroupIOSignal gioSignal = (FRCGroupIOSignal)((FRCGroupIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            gioSignal.Comment = ((FRCGroupIOSignal)ioSignal).Comment;
                            gioSignal.Value = ((FRCGroupIOSignal)ioSignal).Value;
                            gioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frLAInType:
                        case FREIOTypeConstants.frLAOutType:
                            FRCLaserAnalogIOSignal laioSignal = (FRCLaserAnalogIOSignal)((FRCLaserAnalogIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            laioSignal.Comment = ((FRCLaserAnalogIOSignal)ioSignal).Comment;
                            laioSignal.Value = ((FRCLaserAnalogIOSignal)ioSignal).Value;
                            laioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frLDInType:
                        case FREIOTypeConstants.frLDOutType:
                            FRCLaserDigitalIOSignal ldioSignal = (FRCLaserDigitalIOSignal)((FRCLaserDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            ldioSignal.Comment = ((FRCLaserDigitalIOSignal)ioSignal).Comment;
                            ldioSignal.Value = ((FRCLaserDigitalIOSignal)ioSignal).Value;
                            ldioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frMarkerType:
                            FRCMarkerSignal mrkioSignal = (FRCMarkerSignal)((FRCMarkerType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            mrkioSignal.Comment = ((FRCMarkerSignal)ioSignal).Comment;
                            mrkioSignal.Value = ((FRCMarkerSignal)ioSignal).Value;
                            mrkioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frMaxIOType:
                            // There is no FRCMaxIOType.
                            return;
                            break;

                        case FREIOTypeConstants.frPLCInType:
                        case FREIOTypeConstants.frPLCOutType:
                            FRCPLCIOSignal plcioSignal = (FRCPLCIOSignal)((FRCPLCIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            plcioSignal.Comment = ((FRCPLCIOSignal)ioSignal).Comment;
                            plcioSignal.Value = ((FRCPLCIOSignal)ioSignal).Value;
                            plcioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frRDInType:
                        case FREIOTypeConstants.frRDOutType:
                            // There is no FRCRDIOType.
                            return;
                            break;

                        case FREIOTypeConstants.frSOPInType:
                        case FREIOTypeConstants.frSOPOutType:
                            FRCSOPIOSignal sopioSignal = (FRCSOPIOSignal)((FRCSOPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            sopioSignal.Comment = ((FRCSOPIOSignal)ioSignal).Comment;
                            sopioSignal.Value = ((FRCSOPIOSignal)ioSignal).Value;
                            sopioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frTPInType:
                        case FREIOTypeConstants.frTPOutType:
                            FRCTPIOSignal tpioSignal = (FRCTPIOSignal)((FRCTPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            tpioSignal.Comment = ((FRCTPIOSignal)ioSignal).Comment;
                            tpioSignal.Value = ((FRCTPIOSignal)ioSignal).Value;
                            tpioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frUOPInType:
                        case FREIOTypeConstants.frUOPOutType:
                            FRCUOPIOSignal uopioSignal = (FRCUOPIOSignal)((FRCUOPIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            uopioSignal.Comment = ((FRCUOPIOSignal)ioSignal).Comment;
                            uopioSignal.Value = ((FRCUOPIOSignal)ioSignal).Value;

                            return;
                            break;

                        case FREIOTypeConstants.frWDInType:
                        case FREIOTypeConstants.frWDOutType:
                            FRCWeldDigitalIOSignal wdioSignal = (FRCWeldDigitalIOSignal)((FRCWeldDigitalIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            wdioSignal.Comment = ((FRCWeldDigitalIOSignal)ioSignal).Comment;
                            wdioSignal.Value = ((FRCWeldDigitalIOSignal)ioSignal).Value;
                            wdioSignal.Update();
                            return;
                            break;

                        case FREIOTypeConstants.frWSTKInType:
                        case FREIOTypeConstants.frWSTKOutType:
                            FRCWeldStickIOSignal wsioSignal = (FRCWeldStickIOSignal)((FRCWeldStickIOType)robot.IOTypes[ioTypeConstant]).Signals[ioIndex];
                            wsioSignal.Comment = ((FRCWeldStickIOSignal)ioSignal).Comment;
                            wsioSignal.Value = ((FRCWeldStickIOSignal)ioSignal).Value;
                            wsioSignal.Update();
                            return;
                            break;

                        default:
                            return;
                            break;
                    }
                }
                return;
            }
            catch (Exception e)
            {
                return;
            }
        }

        public void RobotIO_Change(FRCIOSignal ioSender, FREIOTypeConstants ioTypeConstant)
        {
            switch (ioTypeConstant)
            {
                case FREIOTypeConstants.frAInType:
                case FREIOTypeConstants.frAOutType:
                    break;

                case FREIOTypeConstants.frDInType:
                case FREIOTypeConstants.frDOutType:
                    break;

                case FREIOTypeConstants.frFlagType:
                    break;

                case FREIOTypeConstants.frGPInType:
                case FREIOTypeConstants.frGPOutType:
                    break;

                case FREIOTypeConstants.frLAInType:
                case FREIOTypeConstants.frLAOutType:
                    break;

                case FREIOTypeConstants.frLDInType:
                case FREIOTypeConstants.frLDOutType:
                    break;

                case FREIOTypeConstants.frMarkerType:
                    break;

                case FREIOTypeConstants.frMaxIOType:
                    break;

                case FREIOTypeConstants.frPLCInType:
                case FREIOTypeConstants.frPLCOutType:
                    break;

                case FREIOTypeConstants.frRDInType:
                case FREIOTypeConstants.frRDOutType:
                    break;

                case FREIOTypeConstants.frSOPInType:
                case FREIOTypeConstants.frSOPOutType:
                    break;

                case FREIOTypeConstants.frTPInType:
                case FREIOTypeConstants.frTPOutType:
                    break;

                case FREIOTypeConstants.frUOPInType:
                case FREIOTypeConstants.frUOPOutType:
                    break;

                case FREIOTypeConstants.frWDInType:
                case FREIOTypeConstants.frWDOutType:
                    break;

                case FREIOTypeConstants.frWSTKInType:
                case FREIOTypeConstants.frWSTKOutType:
                    break;

                default:
                    return;
                    break;
            }
        }

        public void SetUnityPLCIO()
        {
            /* Placeholder */
        }
    }
}
