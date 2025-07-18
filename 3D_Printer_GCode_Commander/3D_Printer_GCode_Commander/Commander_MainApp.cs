using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    public partial class Commander_MainApp : Form
    {
        //public variables
        public readonly ClassNames_e myClassName = ClassNames_e.Gcode_Commander_Class;
        public readonly byte commander_ver_major = 0;
        public readonly byte commander_ver_minor = 7;
        public readonly byte commander_ver_rev = 1;

        //private variables
        private ModuleInfo_Class ModuleInfo;
        private ModuleConfig_Class ModuleConfig;
        private GCodeFileInfo_Class GCodeFileInfo;
        private SerialComm_Class SerialComm;

        private DateTime msgSent;
        private static List<IntertaskMessage> ittMessageQueue;

        public Commander_MainApp()
        {
            //perform initialization of the Main window
            InitializeMainWindow();
            
            //Get instances of classes to produce ui panels
            ModuleInfo = ModuleInfo_Class.GetInstance();
            ModuleConfig = ModuleConfig_Class.GetInstance();
            GCodeFileInfo = GCodeFileInfo_Class.GetInstance();
            SerialComm = SerialComm_Class.GetInstance();

            //Commander MUST add the panels to the main window
            this.Controls.Add(ModuleInfo.GetPanel());
            this.Controls.Add(ModuleConfig.GetPanel());
            this.Controls.Add(GCodeFileInfo.GetPanel());
            this.Controls.Add(SerialComm.GetSerialConfigPanel());
            this.Controls.Add(SerialComm.GetSerialCommPanel());

            //init class variables
            ittMessageQueue = new List<IntertaskMessage>();
        }
        private void InitializeMainWindow()
        {
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 550);
            this.Name = "Commander_MainAppForm";
            this.Text = "GCODE COMMANDER  Ver. " + commander_ver_major + "." + commander_ver_minor + "." + commander_ver_rev + " @ 2025 Abhiram Lingamsetty. All Rights Reserved.";
            this.ClientSize = new System.Drawing.Size(1086, 510);
            this.ResumeLayout(false);

        }
        public static List<GCodeCommand> GetGCodeCommandList()
        {
            return GCodeFileInfo_Class.GetInstance().GetGCodeCommands();
        }
        public static void RouteIntertaskMessage(IntertaskMessage request)
        {
            if ((request.moduleMsg.baseMessage.CmdType == CommandType_e.ERR))
            {
                //handle error or invalid message here
                MessageBox.Show("Message Error", "Commander found an error.", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (request.messageOwner != ClassNames_e.Serial_Comm_Class)
            {
                //if the message sender is not SerialComm, route the message to SerialComm
                SerialComm_Class.GetInstance().AddMessageToTxQueue(request);
                ittMessageQueue.Add(request);
            }
            else
            {
                //route to class with an open request that has matching txnid
                for (int i = 0; i < ittMessageQueue.Count; i++)
                {
                    //find if a request matched the incomming message transaction id
                    if (request.moduleMsg.baseMessage.TransactID == ittMessageQueue[i].moduleMsg.baseMessage.TransactID)
                    {
                        //call the appropriate class functions
                        switch (ittMessageQueue[i].messageOwner)
                        {
                            case ClassNames_e.Module_Info_Class:
                                ModuleInfo_Class.GetInstance().AddIntertaskMsgToQueue(request);
                                break;
                            case ClassNames_e.Diagnostic_Class:
                                break;
                            case ClassNames_e.Gcode_File_Info_Class:
                                break;
                        }

                        //remove request from list
                        ittMessageQueue.RemoveAt(i);
                        break;
                    }
                }//end for
            }          
        }//end router function
    }
}
