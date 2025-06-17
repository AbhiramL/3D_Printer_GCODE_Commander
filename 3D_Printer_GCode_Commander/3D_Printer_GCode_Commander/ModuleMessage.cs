using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static _3D_Printer_GCode_Commander.ModuleMessage;

namespace _3D_Printer_GCode_Commander
{
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
        public float[] Data;             // 4*size bytes long
    }

    public class ModuleMessage
    {
        //public variables
        public BaseMessage_s baseMessage;
        public VarMessage_s varMessage;

        //private variables
        private readonly GCodeCommand gCodeCommand;
        private static byte currTransactID = 0;

        //public variable
        public bool isValid;

        public static void resetTransactionID()
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
            baseMessage.NumBytes = 9;
            baseMessage.Checksum = CalculateChecksum();
            isValid = true;
        }
        public ModuleMessage(CommandType_e cmdType, ushort cmdId)
        {
            //sets up a message with the Command Type
            baseMessage.Sync = 0xB7;
            baseMessage.TransactID = currTransactID++;
            baseMessage.CmdType = cmdType;
            baseMessage.CmdID = cmdId;
            baseMessage.NumBytes = 9;
            baseMessage.Checksum = CalculateChecksum();
            isValid = true;
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
                varMessage.Data = new float[gCode.Parameters.Count];
                count = 0;
                foreach (char key in gCode.Parameters.Keys)
                {
                    varMessage.Data[count] = gCode.Parameters[key];
                    count++;
                }
            }
            else
            {
                varMessage.Data = new float[0];
            }
            baseMessage.Checksum = CalculateChecksum();

            baseMessage.NumBytes = (ushort)(9 + (4*count));

            isValid = true;
        }

        /********************************************************
         * Message Constructor given byte array
         * 
         * requires byte array
         * returns a module message, transaction id is not recalculated.
         *******************************************************/
        public ModuleMessage(byte[] receivedBytes)
        {
            int numBytesRead = 0;

            //assuming invalid message
            isValid = false;

            //find the sync byte, it will be 0xB7
            while ((receivedBytes.Length > numBytesRead) && (receivedBytes[numBytesRead] != 0xB7))
            {
                //if byte is not found, increment numbytes read counter
                numBytesRead++;
            }

            //did we see the Sync byte and are there enough bytes to process base message?
            if ((receivedBytes[numBytesRead] == 0xB7) && ((numBytesRead + 9 ) < receivedBytes.Length) )
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

                //read (float) data.
                for (int i = numBytesRead; i < receivedBytes.Length; i+= sizeof(float))
                {
                    varMessage.Data[i] = BitConverter.ToSingle(receivedBytes, i);
                }

                //verify Checksum
                if(baseMessage.Checksum == CalculateChecksum())
                {
                    //set validity
                    isValid = true;
                }
                else
                {
                    isValid = false;
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
            List<byte> buffer = new List<byte>();

            buffer.Add(baseMessage.Sync);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.NumBytes));
            buffer.Add(baseMessage.TransactID);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.Checksum)); 
            buffer.Add((byte)baseMessage.CmdType);
            buffer.AddRange(BitConverter.GetBytes(baseMessage.CmdID)); 

            if ((varMessage.Data != null) && (varMessage.Data.Length > 0)) //if data isnt empty, add data
            {
                foreach (float value in varMessage.Data)
                {
                    buffer.AddRange(BitConverter.GetBytes(value));
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
            ushort checksum = 0;
           /* checksum = (ushort)((ushort)CmdType + (ushort)CmdID);
            for (int i = 0; i < NumBytes - 8; i++)
            {
                checksum += (byte)Data[i];
            }*/
            return (checksum);
        }
    }
}
