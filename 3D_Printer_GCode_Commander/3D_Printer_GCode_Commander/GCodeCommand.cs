using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Printer_GCode_Commander
{
    public enum CommandType_e : byte
    {
        G = 0,
        M = 1,
        I = 2, //Identify command
        R = 3,
        C = 4, //in-line Comment
        E = 0xF  //error
    };
    public class GCodeCommand
    {
        public string gCodeString;
        public CommandType_e CmdType;
        public short NumCode; // Numeric Code (eg. 0, 1, 21 ...)
        public Dictionary<char, float> Parameters; //holds x,y,z,f,s...parameters and their value

        /********************************************************
         * public constructors
         *******************************************************/
        public GCodeCommand()
        {
            gCodeString = null;
            CmdType = CommandType_e.E;
            NumCode = 0;
            Parameters = null;
        }

        /*
        * This contructor is to be used when you want a default message with a 
        * specific command type
        */
        public GCodeCommand(CommandType_e type) 
        {
            gCodeString = null;
            CmdType = type;
            NumCode = 0;
            Parameters = new Dictionary<char, float>();
        }

        public GCodeCommand(string gCodeString)
        {
            string cmdStr = gCodeString;
            string[] commandParts;
             
            if (gCodeString.Contains(";"))
            {
                cmdStr = gCodeString.Split(';')[0].Trim(); //remove gcode comments
            }

            if (!string.IsNullOrWhiteSpace(cmdStr))
            {
                //store valid string
                this.gCodeString = gCodeString;

                commandParts = cmdStr.Split(' '); // Split by spaces
                switch (commandParts[0].Substring(0, 1)) //get first char
                {
                    case "G":
                        CmdType = CommandType_e.G;
                        break;
                    case "M":
                        CmdType = CommandType_e.M;
                        break;
                    default:
                        CmdType = CommandType_e.E; //error occurred
                        break;
                }

                if (CmdType != CommandType_e.E)
                {
                    if (int.TryParse(commandParts[0].Substring(1), out int numCode))
                    {
                        NumCode = (short)numCode; // Extract numeric part of the code, eg. G21, extract 21
                    }

                    if (commandParts.Count() > 1)
                    {
                        //parameters present in gcode command...
                        Dictionary<char, float> newParams = new Dictionary<char, float>();

                        // Extract parameters (X, Y, Z, F, S, etc.)
                        for (int i = 1; i < commandParts.Count(); i++)
                        {
                            char key = commandParts[i][0]; // First character represents parameter type
                            if (float.TryParse(commandParts[i].Substring(1), out float value)) //parse str to float, output float as value
                            {
                                //extract float value
                                newParams.Add(key, value);
                            }
                        }
                        Parameters = newParams;
                    }
                }
            }
            else if(string.IsNullOrWhiteSpace(cmdStr))
            {
                CmdType = CommandType_e.C; //comment found
            }
            else
            {
                //not a valid string
                CmdType = CommandType_e.E; //error occurred
            }
        }
    }
}
