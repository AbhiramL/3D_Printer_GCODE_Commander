using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    public enum CommandType_e : byte
    {
        G = 0,
        M = 1,
        I = 2, //Identify command
        R = 3, //Generic Response
        D = 4, //Diagnostic Report
        C = 5, //in-line Comment
        E = 0xF  //error
    };

    public enum ParameterType_e : byte
    {
        X = (byte)'X', //x axis 
        Y = (byte)'Y', //y axis
        Z = (byte)'Z', //z axis
        E = (byte)'E', //extrude len
        F = (byte)'F', //feed rate
        S = (byte)'S', //spindle speed
        P = (byte)'P', //pause params
        I = (byte)'I', //arc offset x
        J = (byte)'J', //arc offset y
        R = (byte)'R', //arc offset z
        H = (byte)'H', //heater setting
        L = (byte)'L'  //level setting
    }
    public class GCodeCommand
    {
        public string gCodeString;
        public CommandType_e CmdType;
        public short CmdCode; // Numeric Code (eg. 0, 1, 21 ...)
        public Dictionary<ParameterType_e, float> Parameters; //holds x,y,z,f,s...parameters and their value

        /********************************************************
         * public constructors
         *******************************************************/
        public GCodeCommand()
        {
            gCodeString = null;
            CmdType = CommandType_e.E;
            CmdCode = 0;
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
            CmdCode = 0;
            Parameters = new Dictionary<ParameterType_e, float>();
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
                        CmdCode = (short)numCode; // Extract numeric part of the code, eg. G21, extract 21
                    }

                    if (commandParts.Count() > 1)
                    {
                        //parameters present in gcode command...
                        Dictionary<ParameterType_e, float> newParams = new Dictionary<ParameterType_e, float>();

                        // Extract parameters (X, Y, Z, F, S, etc.)
                        for (int i = 1; i < commandParts.Count(); i++)
                        {
                            char key = ' ';
                            if (Enum.IsDefined(typeof(ParameterType_e), (byte)commandParts[i][0]))
                            {
                                key = commandParts[i][0];
                            }
                            else
                            {
                                //unknown parameter
                                //not a valid string, implement more parameter options?
                                CmdType = CommandType_e.E; //error occurred
                                MessageBox.Show("Parameter Error", "Found an unidentified character", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }

                            if (float.TryParse(commandParts[i].Substring(1), out float value)) //parse str to float, output float as value
                            {
                                //extract float value
                                newParams.Add((ParameterType_e)key, value);
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
