using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    internal class ModuleInfo_Class
    {
        //public variables
        public readonly Owner_e myClassName = Owner_e.Module_Info_Class;

        //private class variables
        private static ModuleInfo_Class ModuleInfo_instance = null;
        private DateTime requestSendTime, ackReceivedTime;
        
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

            //Acquire Serial Comm manager instance.
            //SerialComm_Mgr = SerialComm_Class.GetInstance();
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
         * 
         * returns true if connection is made
         *******************************************************/
        public void AttemptModuleConnection()
        {
            GCodeCommand gCodeCommand = new GCodeCommand(CommandType_e.I);

            //special case 
            gCodeCommand.gCodeString = "Module Identify Command";
            SerialCommMessage serialMessage = new SerialCommMessage(myClassName, gCodeCommand);
            
            Commander_MainApp.RouteIntertaskMessage(myClassName, serialMessage);
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
            ModuleInfo_Panel.BackColor = System.Drawing.SystemColors.Info;
            ModuleInfo_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ModuleInfo_Panel.Location = new System.Drawing.Point(340, 3);
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
         * Serial Request Handler
         * 
         * 
         *******************************************************/
        public void SerialRequestAnswered(SerialCommMessage receivedMsg)
        {
            if(receivedMsg.moduleMsg.baseMessage.CmdType == CommandType_e.I)
            {
                //if success, then update panel
                ModuleVersion_Label.Text = "";
                ModuleVersion_TextBox.Text = "Connected";
            }
        }

        /********************************************************
         * Button Click Handler
         * 
         * initializes Textboxes, Labels, a Panel, and a button
         *******************************************************/
        private void Connect_Btn_Click_Handler(object sender, EventArgs e)
        {
            //send a command to the module and await response
            AttemptModuleConnection();
        }
    }
}
