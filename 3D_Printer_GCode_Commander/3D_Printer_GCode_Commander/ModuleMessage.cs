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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BaseMessage_s
    {
        public byte Sync;                // 1 byte - Sync identifier
        public ushort NumBytes;          // 2 byte - Message size 
        public byte TransactID;          // 1 byte - Current Message Identifier
        public ushort Checksum;          // 2 bytes - Checksum 
        public CommandType_e CmdType;      // 1 byte - 'G' or 'M'
        public ushort CmdID;             // 2 byte int
    }
    
    public struct VarMessage_s
    {
        public byte[] DataTypes;
        public float[] Data;             // 4*size bytes long
    }

    public class ModuleMessage
    {
        //public constants
        public static readonly byte BASE_MSG_SIZE = (byte)Marshal.SizeOf<BaseMessage_s>();
        public static readonly byte VAR_MSG_PARAMETER_SIZE = (byte)(sizeof(float) + sizeof(byte));
        public static readonly byte GENERIC_RESPONSE_SIZE = (byte)(BASE_MSG_SIZE + sizeof(byte));
        public static readonly byte DIAGNOSTIC_RESPONSE_SIZE = (byte)((4 * VAR_MSG_PARAMETER_SIZE) + BASE_MSG_SIZE);

        //public variables
        public BaseMessage_s baseMessage;
        public VarMessage_s varMessage;
        public byte genMsg_status;

        //private variables
        private readonly GCodeCommand gCodeCommand;
        private static byte currTransactID = 0;

        //public variable
        public bool isValid;
        public byte? locationIdx; //byte? allows assigning of null values

        public static void ResetTransactionID()
        {
            currTransactID = 0;
        }

        /********************************************************
         * Constructors
         * set up default/new messages
         *******************************************************/
        private ModuleMessage()
        {
            //sets up a message with the Command Type
            baseMessage.Sync = 0xB7;
            baseMessage.TransactID = currTransactID++;
            baseMessage.CmdType = 0;
            baseMessage.CmdID = 0;
            baseMessage.NumBytes = BASE_MSG_SIZE;
            varMessage.DataTypes = null;
            varMessage.Data = null;
            baseMessage.Checksum = CalculateChecksum();
            isValid = true;
            locationIdx = null;
        }
        public ModuleMessage(CommandType_e cmdType, ushort cmdId)
        {
            //sets up a message with the Command Type
            baseMessage.Sync = 0xB7;
            baseMessage.TransactID = currTransactID++;
            baseMessage.CmdType = cmdType;
            baseMessage.CmdID = cmdId;
            baseMessage.NumBytes = BASE_MSG_SIZE;
            varMessage.DataTypes = null;
            varMessage.Data = null;
            baseMessage.Checksum = CalculateChecksum();
            isValid = true;
            locationIdx = null;
        }//end constructor

        public ModuleMessage(GCodeCommand gCode)
        {
            gCodeCommand = gCode;
            baseMessage.Sync = 0xB7;
            baseMessage.TransactID = currTransactID++;
            baseMessage.CmdType = gCode.CmdType;
            baseMessage.CmdID = (ushort)gCode.CmdCode;

            int count = 0;
            if ((gCode.Parameters != null) && (gCode.Parameters.Count > 0))
            {
                varMessage.DataTypes = new byte[gCode.Parameters.Count];
                varMessage.Data = new float[gCode.Parameters.Count];
                count = 0;
                foreach (ParameterType_e key in gCode.Parameters.Keys)
                {
                    varMessage.DataTypes[count] = (byte)key;
                    varMessage.Data[count] = gCode.Parameters[key];
                    count++;
                }
            }
            else
            {
                varMessage.DataTypes = null;
                varMessage.Data = null;
            }

            baseMessage.NumBytes = (ushort)(BASE_MSG_SIZE + (VAR_MSG_PARAMETER_SIZE * count));
            baseMessage.Checksum = CalculateChecksum();

            isValid = true;
            locationIdx = null;
        }

        /********************************************************
         * Message Constructor given byte array
         * 
         * requires byte array
         * returns a module message, transaction id is not recalculated.
         *******************************************************/
        public ModuleMessage(byte[] receivedBytes)
        {
            byte numBytesRead = 0;

            //assuming invalid message
            isValid = false;

            //find the sync byte, it will be 0xB7
            while ((receivedBytes.Length > numBytesRead) && (receivedBytes[numBytesRead] != 0xB7))
            {
                //if byte is not found, increment numbytes read counter
                numBytesRead++;
            }

            //did we see the Sync byte and are there enough bytes to process base message?
            if ((receivedBytes[numBytesRead] == 0xB7) && ((numBytesRead + BASE_MSG_SIZE) <= receivedBytes.Length) )
            {
                //sync found. 
                //read sync byte
                baseMessage.Sync = receivedBytes[numBytesRead++];

                //read 'NumBytes'
                baseMessage.NumBytes = BitConverter.ToUInt16(receivedBytes, numBytesRead);
                numBytesRead += 2;

                //read transactId
                baseMessage.TransactID = receivedBytes[numBytesRead++];

                //read checksum
                baseMessage.Checksum = BitConverter.ToUInt16(receivedBytes, numBytesRead);
                numBytesRead += 2;

                //read Command type
                baseMessage.CmdType = (CommandType_e)receivedBytes[numBytesRead++];

                //read Command Id
                baseMessage.CmdID = BitConverter.ToUInt16(receivedBytes, numBytesRead);
                numBytesRead += 2;

                //init varMessage arrays
                byte numParameters = (byte)((baseMessage.NumBytes - BASE_MSG_SIZE) / (VAR_MSG_PARAMETER_SIZE));
                varMessage.Data = new float[numParameters];
                varMessage.DataTypes = new byte[numParameters];

                //check if this message is a generic response, or module message
                //AND check if there are enough bytes to construct a full response
                if ((baseMessage.CmdType == CommandType_e.R) && ((numBytesRead + (GENERIC_RESPONSE_SIZE - BASE_MSG_SIZE)) <= receivedBytes.Length))
                {
                    //generic response received, set status variable
                    genMsg_status = receivedBytes[numBytesRead++];
                }
                else if ((numBytesRead + (baseMessage.NumBytes - BASE_MSG_SIZE)) <= receivedBytes.Length)
                {
                    //default module message or diagnostics response was sent by the module
                    //read available variable data.
                    for (int i = 0; i < numParameters; i++)
                    {
                        if (Enum.IsDefined(typeof(ParameterType_e), (byte)receivedBytes[i]))
                        {
                            varMessage.DataTypes[i] = receivedBytes[numBytesRead];
                            numBytesRead++;
                        }
                        else
                        {
                            //unknown parameter or not a valid string, implement more parameter options?
                            varMessage.DataTypes[i] = (byte)ParameterType_e.E; //error occurred
                            MessageBox.Show("Parameter Error", "Found an unidentified character", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }

                        varMessage.Data[i] = BitConverter.ToSingle(receivedBytes, numBytesRead);
                        numBytesRead += sizeof(float);
                    }
                }

                //verify Checksum
                if(baseMessage.Checksum == CalculateChecksum())
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
        }

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
                public CommandType_e CmdType;      // 1 byte - 'G' or 'M'
                public ushort CmdID;             // 2 byte int
             */
            List<byte> buffer = new List<byte>();

            buffer.Add(baseMessage.Sync);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.NumBytes));
            buffer.Add(baseMessage.TransactID);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.Checksum)); 
            buffer.Add((byte)baseMessage.CmdType);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.CmdID)); 

            if ((varMessage.Data != null) && (varMessage.Data.Length > 0)) //if data isnt empty, add data to the buffer
            {
                for (byte i = 0; i < (baseMessage.NumBytes - 9)/VAR_MSG_PARAMETER_SIZE; i++) 
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
    }
}
