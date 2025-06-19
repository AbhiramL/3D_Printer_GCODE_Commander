using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Printer_GCode_Commander
{
    public enum MessageSender_e : byte
    {
        Gcode_Commander_Class,
        Module_Info_Class,
        Diagnostic_Class,
        Gcode_File_Info_Class,
        Serial_Comm_Class
    }
    public class IntertaskMessage
    {
        //public variables
        public MessageSender_e messageOwner;
        public ModuleMessage moduleMsg;

        //private variables

        private IntertaskMessage() { }
        
        public IntertaskMessage(MessageSender_e messageOwner, GCodeCommand data)
        {
            this.messageOwner = messageOwner;
            this.moduleMsg = new ModuleMessage(data);
        }
        public IntertaskMessage(MessageSender_e messageOwner, ModuleMessage data)
        {
            this.messageOwner = messageOwner;
            this.moduleMsg = data;
        }
    }
}
