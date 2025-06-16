using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Printer_GCode_Commander
{
    public enum Owner_e : byte
    {
        Gcode_Commander_Class,
        Module_Info_Class,
        Diagnostic_Class,
        Gcode_File_Info_Class,
        Serial_Comm_Class
    }
    public class SerialCommMessage
    {
        //public variables
        public Owner_e messageOwner;
        public ModuleMessage moduleMsg;

        //private variables

        private SerialCommMessage() { }
        
        public SerialCommMessage(Owner_e messageOwner, GCodeCommand data)
        {
            this.messageOwner = messageOwner;
            this.moduleMsg = new ModuleMessage(data);
        }
        public SerialCommMessage(Owner_e messageOwner, ModuleMessage data)
        {
            this.messageOwner = messageOwner;
            this.moduleMsg = data;
        }
    }
}
