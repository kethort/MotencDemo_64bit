using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace MotencDemo
{
    public class MotencAPI
    {
        [DllImport(@"WINMOTENC.dll")]
        public static extern int vitalInit();

        [DllImport(@"WINMOTENC.dll")]
        public static extern int vitalQuit();

        [DllImport(@"WINMOTENC.dll")]
        public static extern int vitalReadReg(int reg, out int regData);

        [DllImport(@"WINMOTENC.dll")]
        public static extern int vitalWriteReg(int reg, int regData);

        [DllImport(@"WINMOTENC.dll")]
        public static extern int vitalDacWrite(int axis, double volts);

        [DllImport(@"WINMOTENC.dll")]
        public static extern int vitalReadAnalogInputs(int nBank, IntPtr values);

        private enum PlxStatusCode
        {
            PLX_STATUS_OK = 512,
            PLX_STATUS_FAILED = 513,
            PLX_STATUS_NULL_PARAM = 514,
            PLX_STATUS_UNSUPPORTED = 515,
            PLX_STATUS_NO_DRIVER = 516,
            PLX_STATUS_INVALID_OBJECT = 517,
            PLX_STATUS_VER_MISMATCH = 518,
            PLX_STATUS_INVALID_OFFSET = 519,
            PLX_STATUS_INVALID_DATA = 520,
            PLX_STATUS_INVALID_SIZE = 521,
            PLX_STATUS_INVALID_ADDR = 522,
            PLX_STATUS_INVALID_ACCESS = 523,
            PLX_STATUS_INSUFFICIENT_RES = 524,
            PLX_STATUS_TIMEOUT = 525,
            PLX_STATUS_CANCELED = 526,
            PLX_STATUS_COMPLETE = 527,
            PLX_STATUS_PAUSED = 528,
            PLX_STATUS_IN_PROGRESS = 529,
            PLX_STATUS_PAGE_GET_ERROR = 530,
            PLX_STATUS_PAGE_LOCK_ERROR = 531,
            PLX_STATUS_LOW_POWER = 532,
            PLX_STATUS_IN_USE = 533,
            PLX_STATUS_DISABLED = 534,
            PLX_STATUS_PENDING = 535,
            PLX_STATUS_NOT_FOUND = 536,
            PLX_STATUS_INVALID_STATE = 537,
            PLX_STATUS_BUFF_TOO_SMALL = 538,
            PLX_STATUS_RSVD_LAST_ERROR = 539
        }

        private void reportError(PlxStatusCode errorCode)
        {
            string output = "";
            switch(errorCode)
            {
                case PlxStatusCode.PLX_STATUS_FAILED:
                    output += "PLX_STATUS_FAILED";
                    break;
                case PlxStatusCode.PLX_STATUS_NULL_PARAM:
                    output += "PLX_STATUS_NULL_PARAM";
                    break;
                case PlxStatusCode.PLX_STATUS_UNSUPPORTED:
                    output += "PLX_STATUS_UNSUPPORTED";
                    break;
                case PlxStatusCode.PLX_STATUS_NO_DRIVER:
                    output += "PLX_STATUS_NO_DRIVER";
                    break;
                case PlxStatusCode.PLX_STATUS_INVALID_OBJECT:
                    output += "PLX_STATUS_INVALID_OBJECT";
                    break;
                case PlxStatusCode.PLX_STATUS_VER_MISMATCH:
                    output += "PLX_STATUS_VER_MISMATCH";
                    break;
                case PlxStatusCode.PLX_STATUS_INVALID_OFFSET:
                    output += "PLX_STATUS_INVALID_OFFSET";
                    break;
                case PlxStatusCode.PLX_STATUS_INVALID_DATA:
                    output += "PLX_STATUS_INVALID_DATA";
                    break;
                case PlxStatusCode.PLX_STATUS_INVALID_SIZE:
                    output += "PLX_STATUS_INVALID_SIZE";
                    break;
                case PlxStatusCode.PLX_STATUS_INVALID_ADDR:
                    output += "PLX_STATUS_INVALID_ADDR";
                    break;
                case PlxStatusCode.PLX_STATUS_INVALID_ACCESS:
                    output += "PLX_STATUS_INVALID_ACCESS";
                    break;
                case PlxStatusCode.PLX_STATUS_INSUFFICIENT_RES:
                    output += "PLX_STATUS_INSUFFICIENT_RES";
                    break;
                case PlxStatusCode.PLX_STATUS_TIMEOUT:
                    output += "PLX_STATUS_TIMEOUT";
                    break;
                case PlxStatusCode.PLX_STATUS_CANCELED:
                    output += "PLX_STATUS_CANCELED";
                    break;
                case PlxStatusCode.PLX_STATUS_COMPLETE:
                    output += "PLX_STATUS_COMPLETE";
                    break;
                case PlxStatusCode.PLX_STATUS_PAUSED:
                    output += "PLX_STATUS_PAUSED";
                    break;
                case PlxStatusCode.PLX_STATUS_IN_PROGRESS:
                    output += "PLX_STATUS_IN_PROGRESS";
                    break;
                case PlxStatusCode.PLX_STATUS_PAGE_GET_ERROR:
                    output += "PLX_STATUS_PAGE_GET_ERROR";
                    break;
                case PlxStatusCode.PLX_STATUS_PAGE_LOCK_ERROR:
                    output += "PLX_STATUS_PAGE_LOCK_ERROR";
                    break;
                case PlxStatusCode.PLX_STATUS_LOW_POWER:
                    output += "PLX_STATUS_LOW_POWER";
                    break;
                case PlxStatusCode.PLX_STATUS_IN_USE:
                    output += "PLX_STATUS_IN_USE";
                    break;
                case PlxStatusCode.PLX_STATUS_DISABLED:
                    output += "PLX_STATUS_DISABLED";
                    break;
                case PlxStatusCode.PLX_STATUS_PENDING:
                    output += "PLX_STATUS_PENDING";
                    break;
                case PlxStatusCode.PLX_STATUS_NOT_FOUND:
                    output += "PLX_STATUS_NOT_FOUND";
                    break;
                case PlxStatusCode.PLX_STATUS_INVALID_STATE:
                    output += "PLX_STATUS_INVALID_STATE";
                    break;
                case PlxStatusCode.PLX_STATUS_BUFF_TOO_SMALL:
                    output += "PLX_STATUS_BUFF_TOO_SMALL";
                    break;
                case PlxStatusCode.PLX_STATUS_RSVD_LAST_ERROR:
                    output += "PLX_STATUS_RSVD_LAST_ERROR";
                    break;
            }
            MessageBoxResult rs = MessageBox.Show(output, "Confirm", MessageBoxButton.OK, MessageBoxImage.Question);
            Environment.Exit(0);
            this.Dispose();
        }

        public MotencAPI()
        {
            PlxStatusCode errorCode = (PlxStatusCode)vitalInit();
            if (errorCode != PlxStatusCode.PLX_STATUS_OK)
            {
                reportError(errorCode);
                throw new Exception("vitalInit()");
            }
        }

        public virtual void Dispose()
        {
            vitalQuit();
        }

        public void SetDAC(int channel, double volts)
        {
            if (vitalDacWrite(channel, volts) == 0)
                throw new Exception(string.Format("vitalWriteReg( {0} )", (24 + channel)));
        }

        public void ResetEncoder(int encoder)
        {
            if (encoder < 0 || encoder >= 4)
                throw new Exception(String.Format("Invalid encoder index: {0}", encoder));

            int regData = (0x00000001 << encoder);
            if (vitalWriteReg(5, regData) == 0)
                throw new Exception("vitalWriteReg( 5 )");
        }

        public int ReadEncoder(int encoder)
        {
            if (encoder < 0 || encoder >= 4)
                throw new Exception(String.Format("Invalid encoder index: {0}", encoder));

            int count = 0;
            if (vitalReadReg(encoder, out count) == 0)
                throw new Exception(String.Format("vitalReadReg( {0} )", encoder));
            return count;
        }

        public double ReadAnalogIN(int nBank, int channel)
        {
            double[] values = {0, 0, 0, 0};
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeof(double) * values.Length);
            if (vitalReadAnalogInputs(nBank, ptr) == 0)
                throw new Exception("vitalReadAnalogIN( 32 )");
            Marshal.Copy(ptr, values, 0, values.Length); 
            Marshal.FreeHGlobal(ptr);
            return values[channel];
        }

        public ushort DigitalOutputs
        {
            get
            {
                int regData = 0;
                if (vitalReadReg(4, out regData) == 0)
                    throw new Exception("vitalReadReg( 4 )");
                return (ushort)(regData & 0x0000FFFF);
            }

            set
            {
                int regData = value;
                if (vitalWriteReg(4, regData) == 0)
                    throw new Exception("vitalWriteReg( 4 )");
            }
        }

        public uint DigitalInputs
        {
            get
            {
                int j4Data = 0; // inputs 0..15 in bits 16..31
                if (vitalReadReg(4, out j4Data) == 0)
                    throw new Exception("vitalReadReg( 4 )");

                int j5Data = 0; // inputs 16..31 in bits 0..15
                if (vitalReadReg(5, out j5Data) == 0)
                    throw new Exception("vitalReadReg( 5 )");

                return (((uint)j4Data & 0xFFFF0000) >> 16) | (((uint)j5Data & 0x0000FFFF) << 16);
            }

        }

        public int ReadRegister(int register)
        {
            int regData;
            if (vitalReadReg(register, out regData) == 0)
                throw new Exception(String.Format("ReadRegister( {0} )", register));
            return regData;
        }
    }
}
