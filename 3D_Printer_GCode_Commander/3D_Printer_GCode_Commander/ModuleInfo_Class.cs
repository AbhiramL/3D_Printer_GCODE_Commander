using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    internal class ModuleInfo_Class
    {
        //public variables
        public readonly ClassNames_e myClassName = ClassNames_e.Module_Info_Class;

        //private class variables
        private static ModuleInfo_Class ModuleInfo_instance = null;
        private List<IntertaskMessage> ittMsgRequestQueue;

        //private ui element variables
        private System.Windows.Forms.Panel ModuleInfo_Panel;
        private System.Windows.Forms.TextBox ModuleVersion_TextBox;
        private System.Windows.Forms.TextBox ConnStatus_TextBox;
        private System.Windows.Forms.TextBox PanelTitle_TextBox;
        private System.Windows.Forms.Label ModuleVersion_Label;
        private System.Windows.Forms.Label ConnStatus_Label;
        private System.Windows.Forms.Button ConnectToModule_Btn;


        /********************************************************
         * Private constructors
         *******************************************************/
        private ModuleInfo_Class()
        {
            //Build Module Info panel and buttons.
            Build_ModuleInfo_Panel();

            ittMsgRequestQueue = new List<IntertaskMessage>();
        }

        /********************************************************
         * GetInstance function
         * 
         * returns instance of ModuleInfo_Class
         *******************************************************/
        public static ModuleInfo_Class GetInstance()
        {
            if (ModuleInfo_instance == null)
            {
                ModuleInfo_instance = new ModuleInfo_Class();
            }
            return ModuleInfo_instance;
        }

        /********************************************************
         * Connect to module function
         * Sends Identify cmd to module and starts a timer
         * 
         *******************************************************/
        public void ConnectToModule()
        {
            GCodeCommand gCodeCommand = new GCodeCommand(CommandType_e.ID);

            //special case 
            gCodeCommand.gCodeString = "Module Identify Command";

            //create a intertask message, with moduleInfo as the owner
            IntertaskMessage serialMessage = new IntertaskMessage(myClassName, gCodeCommand);
            
            //route to serialComm_class
            Commander_MainApp.RouteIntertaskMessage(serialMessage);
        }

        /********************************************************
         * Build Panel Function
         * 
         * initializes Textboxes, Labels, a Panel, and a button
         *******************************************************/
        private void Build_ModuleInfo_Panel()
        {
            ModuleInfo_Panel = new System.Windows.Forms.Panel();
            PanelTitle_TextBox = new System.Windows.Forms.TextBox();
            ConnectToModule_Btn = new System.Windows.Forms.Button();
            ConnStatus_Label = new System.Windows.Forms.Label();
            ConnStatus_TextBox = new System.Windows.Forms.TextBox();
            ModuleVersion_TextBox = new System.Windows.Forms.TextBox();
            ModuleVersion_Label = new System.Windows.Forms.Label();
            ModuleInfo_Panel.SuspendLayout();

            // Initialize elements:
            //
            // Panel init
            //
            ModuleInfo_Panel.BackColor = System.Drawing.SystemColors.ControlLight;
            ModuleInfo_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ModuleInfo_Panel.Location = new System.Drawing.Point(308, 3);
            ModuleInfo_Panel.Name = "ModuleStatus_Panel";
            ModuleInfo_Panel.Size = new System.Drawing.Size(328, 176);
            ModuleInfo_Panel.TabIndex = 6;
            //
            // Title Header Textbox
            //
            PanelTitle_TextBox.ReadOnly = true;
            PanelTitle_TextBox.BackColor = System.Drawing.SystemColors.HighlightText;
            PanelTitle_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.912F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            PanelTitle_TextBox.Location = new System.Drawing.Point(0, 3);
            PanelTitle_TextBox.Multiline = true;
            PanelTitle_TextBox.Name = "ModuleInfoHeader_TextBox";
            PanelTitle_TextBox.Size = new System.Drawing.Size(327, 28);
            PanelTitle_TextBox.TabIndex = 1;
            PanelTitle_TextBox.Text = "PRINTER MODULE STATUS";
            PanelTitle_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ConnectToModule_Btn
            // 
            ConnectToModule_Btn.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            ConnectToModule_Btn.Location = new System.Drawing.Point(90, 123); // Adjusted for new width
            ConnectToModule_Btn.Name = "ConnectToModule_Btn";
            ConnectToModule_Btn.Size = new System.Drawing.Size(150, 25);
            ConnectToModule_Btn.TabIndex = 8;
            ConnectToModule_Btn.Text = "CONNECT TO MODULE";
            ConnectToModule_Btn.UseVisualStyleBackColor = false;
            ConnectToModule_Btn.BackColor = System.Drawing.Color.Green;
            ConnectToModule_Btn.Click += new System.EventHandler(this.Connect_Btn_Click_Handler);
            // 
            // Connection Status TextBox and Status Label
            // 
            ConnStatus_TextBox.ReadOnly = true;
            ConnStatus_TextBox.Location = new System.Drawing.Point(10, 85);
            ConnStatus_TextBox.Name = "ConnStatus_TextBox";
            ConnStatus_TextBox.Size = new System.Drawing.Size(180, 22);
            ConnStatus_TextBox.TabIndex = 6;
            ConnStatus_TextBox.Text = "CONNECTION STATUS :";
            ConnStatus_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            ConnStatus_Label.AutoSize = true;
            ConnStatus_Label.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            ConnStatus_Label.Location = new System.Drawing.Point(210, 88);
            ConnStatus_Label.Name = "ConnStatus_Label";
            ConnStatus_Label.Text = "Not Connected";
            ConnStatus_Label.TabIndex = 7;
            // 
            // Module Version TextBox and Label
            // 
            ModuleVersion_TextBox.ReadOnly = true;
            ModuleVersion_TextBox.Location = new System.Drawing.Point(10, 46);
            ModuleVersion_TextBox.Name = "ModuleVersion_TextBox";
            ModuleVersion_TextBox.Size = new System.Drawing.Size(180, 22);
            ModuleVersion_TextBox.TabIndex = 5;
            ModuleVersion_TextBox.Text = "MODULE VERSION :";
            ModuleVersion_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            ModuleVersion_Label.AutoSize = true;
            ModuleVersion_Label.BackColor = System.Drawing.SystemColors.HighlightText;
            ModuleVersion_Label.Location = new System.Drawing.Point(210, 49);
            ModuleVersion_Label.Name = "ModuleVersion_Label";
            ModuleVersion_Label.Size = new System.Drawing.Size(0, 16);
            ModuleVersion_Label.TabIndex = 3;

            //add elements to Panel
            ModuleInfo_Panel.Controls.Add(ConnectToModule_Btn);
            ModuleInfo_Panel.Controls.Add(ConnStatus_Label);
            ModuleInfo_Panel.Controls.Add(ModuleVersion_Label);
            ModuleInfo_Panel.Controls.Add(ConnStatus_TextBox);
            ModuleInfo_Panel.Controls.Add(ModuleVersion_TextBox);
            ModuleInfo_Panel.Controls.Add(PanelTitle_TextBox);

            //allow panel to update itself
            ModuleInfo_Panel.ResumeLayout(false);
            ModuleInfo_Panel.PerformLayout();
        }

        /********************************************************
         * Get Panel Function
         * 
         * returns Module Info panel
         *******************************************************/
        public Panel GetPanel()
        {
            return ModuleInfo_Panel;
        }

        /********************************************************
         * Add intertask message to ModuleInfo class queue function 
         * 
         *******************************************************/
        public void AddIntertaskMsgToQueue(IntertaskMessage msg)
        {
            ittMsgRequestQueue.Add(msg);
        }

        /********************************************************
         * Update ModuleInfo Panel UI function 
         * 
         *******************************************************/
        private void UpdatePanelUI(bool isConnected, byte major, byte minor, byte rev)
        {
            //update version # only if a valid major number appeared
            if (major != 0) 
            {
                ModuleVersion_Label.Text = isConnected ? (major + "." + minor + "." + rev) : "-";
            }

            ConnStatus_Label.Text = isConnected ? "CONNECTED" : "NOT CONNECTED";
            ModuleInfo_Panel.BackColor = isConnected ? System.Drawing.SystemColors.Info : System.Drawing.SystemColors.ControlLight;
        }

        /********************************************************
         * Button Click Handler
         * 
         * initializes Textboxes, Labels, a Panel, and a button
         *******************************************************/
        private void Connect_Btn_Click_Handler(object sender, EventArgs e)
        {
            //send a command to the module and await response
            ConnectToModule();
        }
    }
}
