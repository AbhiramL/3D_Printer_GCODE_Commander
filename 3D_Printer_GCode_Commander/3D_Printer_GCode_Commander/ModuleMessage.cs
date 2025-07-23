using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace _3D_Printer_GCode_Commander
{
    public class ModuleMessage
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BaseMessage_s
        {
            public byte Sync;                // 1 byte - Sync identifier
            public ushort NumBytes;          // 2 byte - Message size 
            public byte TransactID;          // 1 byte - Current Message Identifier
            public ushort Checksum;          // 2 bytes - Checksum 
            public CommandType_e CmdType;    // 1 byte - 'G' or 'M'
            public ushort CmdID;             // 2 byte int
        }
        public struct VarMessage_s
        {
            public byte[] DataTypes;
            public float[] Data;             // 4*size bytes long
        }

        //class constants
        protected static readonly byte BASE_MSG_SIZE = (byte)Marshal.SizeOf<BaseMessage_s>();
        protected static readonly byte VAR_MSG_PARAMETER_SIZE = (byte)(sizeof(float) + sizeof(byte));

        //public message related variables
        public BaseMessage_s baseMessage;
        public VarMessage_s varMessage;
        public byte? status;

        //private variables
        private readonly GCodeCommand gCodeCommand;
        private static byte currTransactID = 0;

        //public variable
        public bool isValid;
        public byte? locationIdx; //byte? allows assigning of null values

        /********************************************************
         * Constructors
         * set up default/new messages
         *******************************************************/
        protected ModuleMessage()
        {
            //sets up a message with the Command Type
            baseMessage.Sync = 0xB7;
            baseMessage.TransactID = currTransactID++;
            baseMessage.CmdType = 0;
            baseMessage.CmdID = 0;
            baseMessage.NumBytes = BASE_MSG_SIZE;
            baseMessage.Checksum = 0;
            isValid = true;
            locationIdx = null;
            status = null;

            varMessage.Data = null;
            varMessage.DataTypes = null;
        }
        public ModuleMessage(CommandType_e cmdType)
        {
            //sets up a message with the Command Type
            baseMessage.Sync = 0xB7;
            baseMessage.CmdType = cmdType;

            switch (cmdType)
            {
                case CommandType_e.ID:
                    baseMessage.CmdID = 0;
                    ResetTransactionID();
                    break;
                case CommandType_e.MCF:
                    baseMessage.CmdID = 1;
                    break;
                case CommandType_e.BEGIN:
                case CommandType_e.END:
                case CommandType_e.PAUSE:
                case CommandType_e.GEN_RSP:
                case CommandType_e.ERR:
                    baseMessage.CmdID = 2;
                    break;
                case CommandType_e.DX:
                    baseMessage.CmdID = 4;
                    break;
                case CommandType_e.PD:
                    baseMessage.CmdID = 5;
                    break;
                default:
                    baseMessage.CmdID = 0xFFFF;
                    break;
            }

            baseMessage.TransactID = currTransactID++;
            baseMessage.NumBytes = BASE_MSG_SIZE;
            baseMessage.Checksum = CalculateChecksum();
            isValid = true;
            status = null;

            varMessage.Data = null;
            varMessage.DataTypes = null;
        }
        public ModuleMessage(GCodeCommand gCode)
        {
            gCodeCommand = gCode;

            //set up message base
            baseMessage.Sync = 0xB7;
            baseMessage.CmdType = gCode.CmdType;
            baseMessage.CmdID = (ushort)gCode.CmdCode;

            if (gCode.Parameters != null)
            {
                baseMessage.NumBytes = (ushort)(BASE_MSG_SIZE + (gCode.Parameters.Count * VAR_MSG_PARAMETER_SIZE));
            }
            else
            {
                baseMessage.NumBytes = (ushort)BASE_MSG_SIZE;
            }

            if ((gCode.Parameters != null) && (gCode.Parameters.Count > 0))
            {
                varMessage.DataTypes = new byte[gCode.Parameters.Count];
                varMessage.Data = new float[gCode.Parameters.Count];

                int count = 0;
                foreach (ParameterType_e key in gCode.Parameters.Keys)
                {
                    varMessage.DataTypes[count] = (byte)key;
                    varMessage.Data[count] = gCode.Parameters[key];
                    count++;
                }
            }

            if (baseMessage.CmdType == CommandType_e.ID)
            {
                ResetTransactionID();
            }
            baseMessage.TransactID = currTransactID++;
            baseMessage.Checksum = CalculateChecksum();
            isValid = true;
            locationIdx = null;
            status = null;
        }

        /********************************************************
         * Message builder given byte array
         * 
         * requires byte array
         * returns a module message, transaction id is not recalculated.
         *******************************************************/
        public ModuleMessage(byte[] receivedBytes)
        {
            byte numBytesRead = 0;
            
            //assume invalid message
            isValid = false;

            //find the sync byte, it will be 0xB7
            while ((receivedBytes.Length > numBytesRead) && (receivedBytes[numBytesRead] != 0xB7))
            {
                //if byte is not found, increment numbytes read counter
                numBytesRead++;
            }

            //did we see the Sync byte and are there enough bytes to process base message?
            if ((receivedBytes[numBytesRead] == 0xB7) && ((numBytesRead + BASE_MSG_SIZE) <= receivedBytes.Length))
            {
                //sync found. 
                //read sync byte
                baseMessage.Sync = receivedBytes[numBytesRead++];

                //read 'NumBytes'
                baseMessage.NumBytes = BitConverter.ToUInt16(receivedBytes, numBytesRead);
                numBytesRead += sizeof(UInt16);

                //read transactId
                baseMessage.TransactID = receivedBytes[numBytesRead++];

                //read checksum
                baseMessage.Checksum = BitConverter.ToUInt16(receivedBytes, numBytesRead);
                numBytesRead += sizeof(UInt16); 

                //read Command type
                baseMessage.CmdType = (CommandType_e)receivedBytes[numBytesRead++];

                //read Command Id
                baseMessage.CmdID = BitConverter.ToUInt16(receivedBytes, numBytesRead);
                numBytesRead += sizeof(UInt16);

                //check there are enough bytes to construct a full response
                if ((numBytesRead + (baseMessage.NumBytes - BASE_MSG_SIZE)) <= receivedBytes.Length)
                {
                    if ((baseMessage.NumBytes - BASE_MSG_SIZE) == 1)
                    {
                        //Generic Message with Status Byte
                        //Module Config Response, Module Command Response will be handled here...
                        
                        status = receivedBytes[numBytesRead++];
                    }
                    else
                    {
                        //Message with multiple parameters...
                        //Identify responses, Diagnosis Responses, Power Down responses will be handled here...
                        byte numParameters = (byte)((baseMessage.NumBytes - BASE_MSG_SIZE) / (VAR_MSG_PARAMETER_SIZE));

                        //create module command response message arrays with exact size of numParameters
                        varMessage.Data = new float[numParameters];
                        varMessage.DataTypes = new byte[numParameters];

                        //Module Command Response 
                        for (int i = 0; i < numParameters; i++)
                        {
                            if (Enum.IsDefined(typeof(ParameterType_e), (byte)receivedBytes[numBytesRead]))
                            {
                                varMessage.DataTypes[i] = receivedBytes[numBytesRead];
                                numBytesRead++;
                            }
                            else
                            {
                                //unknown parameter type or not a valid string, implement more parameter options?
                                varMessage.DataTypes[i] = (byte)ParameterType_e.E; //error occurred
                                numBytesRead++;
                                MessageBox.Show("Parameter Error", "Found an unidentified character", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }

                            //important: uint32 values must be BitConverted to UInt32 to access each byte later
                            //since the float values have a different format than normal integers
                            varMessage.Data[i] = BitConverter.ToSingle(receivedBytes, numBytesRead);
                            numBytesRead += sizeof(float);
                        }
                    }
                }//end if                

                //verify Checksum, set validity
                if (baseMessage.Checksum == CalculateChecksum())
                {
                    //set validity
                    isValid = true;
                    locationIdx = (byte)(numBytesRead - baseMessage.NumBytes);
                }
                else
                {
                    isValid = false;
                    locationIdx = null;
                }
            }
        }//end function


        /********************************************************
         * Convert message to byte array function
         * 
         * returns byte array
         *******************************************************/
        public byte[] GetByteArray()
        {
            /* Base message struct:
             *  public byte Sync;                // 1 byte - Sync identifier
                public ushort NumBytes;          // 2 byte - Message size 
                public byte TransactID;          // 1 byte - Current Message Identifier
                public ushort Checksum;          // 2 bytes - Checksum 
                public CommandType_e CmdType;    // 1 byte - 'G' or 'M'
                public ushort CmdID;             // 2 byte int
             */
            List<byte> buffer = new List<byte>();

            buffer.Add(baseMessage.Sync);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.NumBytes));
            buffer.Add(baseMessage.TransactID);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.Checksum)); 
            buffer.Add((byte)baseMessage.CmdType);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.CmdID));

            if(baseMessage.NumBytes - BASE_MSG_SIZE == 1)
            {
                //status message, add status byte to list
                buffer.Add((byte)status);
            }
            else if ((varMessage.Data != null) && (varMessage.Data.Length > 0)) 
            {
                //parameter fields detected... dynamic message
                for (byte i = 0; i < varMessage.DataTypes.Length; i++)
                {
                    buffer.Add((byte)varMessage.DataTypes[i]);
                    buffer.AddRange(BitConverter.GetBytes(varMessage.Data[i]));
                }
            }

            return buffer.ToArray(); 
        }//end convert function

        /********************************************************
         * Checksum calculate function
         * 
         * returns 2 byte int (ushort)
         *******************************************************/
        private ushort CalculateChecksum()
        {
            /* Base message struct:
             *  public byte Sync;                // 1 byte - Sync identifier
                public ushort NumBytes;          // 2 byte - Message size 
                public byte TransactID;          // 1 byte - Current Message Identifier
                public ushort Checksum;          // 2 bytes - Checksum 
                public CommandType_e CmdType;      // 1 byte - 'G' or 'M'
                public ushort CmdID;             // 2 byte int
             */
            ushort originalChecksum;
            int calculatedChecksum = 0;
            ushort numBytes;
            byte[] messageBytes;

            //save the received checksum value
            originalChecksum = baseMessage.Checksum;

            //zero out msg's checksum
            baseMessage.Checksum = 0;

            //get the byte array of the full message (base msg + var msg)
            numBytes = baseMessage.NumBytes;
            messageBytes = GetByteArray();

            for (int i = 0; i < numBytes; i++)
            {
                calculatedChecksum += messageBytes[i];
                calculatedChecksum += ((calculatedChecksum & 0xFF) << 8) + 0x100;
                calculatedChecksum = (calculatedChecksum ^ (calculatedChecksum >> 16)) & 0xFFFF;
            }

            //restore orig checksum to msg structure
            baseMessage.Checksum = originalChecksum;

            //return truncated checksum
            return (ushort)calculatedChecksum;
        }

        /********************************************************
         * Reset Transaction ID function
         * 
         *******************************************************/
        public static void ResetTransactionID()
        {
            currTransactID = 0;
        }


    }//end of module message class


}
