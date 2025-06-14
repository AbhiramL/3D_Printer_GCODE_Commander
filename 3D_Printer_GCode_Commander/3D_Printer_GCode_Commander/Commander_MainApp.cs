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
        private readonly double commander_version = 0.5;
        private ModuleInfo_Class ModuleInfo;
        private GCodeFileInfo_Class GCodeFileInfo;
        private SerialComm_Class SerialComm;

        public Commander_MainApp()
        {
            //perform initialization of the Main window
            InitializeMainWindow();
            
            //Get instances of classes to produce ui panels
            ModuleInfo = ModuleInfo_Class.GetInstance();
            GCodeFileInfo = GCodeFileInfo_Class.GetInstance();
            SerialComm = SerialComm_Class.GetInstance();

            //Commander MUST add the panels to the main window
            this.Controls.Add(ModuleInfo.GetPanel());
            this.Controls.Add(GCodeFileInfo.GetPanel());
            this.Controls.Add(SerialComm.GetSerialConfigPanel());
            this.Controls.Add(SerialComm.GetSerialCommPanel());
        }
        private void InitializeMainWindow()
        {
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 550);
            this.Name = "Commander_MainAppForm";
            this.Text = "GCODE COMMANDER  Ver. " + commander_version + " @ 2025 Abhiram Lingamsetty. All Rights Reserved.";
            this.ClientSize = new System.Drawing.Size(1086, 510);
            this.ResumeLayout(false);

        }
        
    }
}
