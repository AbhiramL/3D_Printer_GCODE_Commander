using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    internal class SerialComm_Class
    {
        //public variables

        //private variables
        private static SerialComm_Class SerialComm_Instance = null;

        //private ui element variables
        //serialConfig panel elements
        private System.Windows.Forms.Panel   SerialConfig_Panel;
        private System.Windows.Forms.Button  SerialConfig_FindPorts_Btn;
        private System.Windows.Forms.Button  SerialConfig_OpenPort_Btn;
        private System.Windows.Forms.Button  SerialConfig_ClosePort_Btn;
        private System.Windows.Forms.ListBox SerialConfig_Ports_ListBox;
        private System.Windows.Forms.ListBox SerialConfig_BaudRates_ListBox;
        private System.Windows.Forms.ListBox SerialConfig_Parities_ListBox;
        private System.Windows.Forms.ListBox SerialConfig_DataBits_ListBox;
        private System.Windows.Forms.ListBox SerialConfig_StopBits_ListBox;
        private System.Windows.Forms.TextBox SerialConfig_Ports_TextBox;
        private System.Windows.Forms.TextBox SerialConfig_BaudRates_TextBox;
        private System.Windows.Forms.TextBox SerialConfig_Parities_TextBox;
        private System.Windows.Forms.TextBox SerialConfig_DataBits_TextBox;
        private System.Windows.Forms.TextBox SerialConfig_StopBits_TextBox;
        private System.Windows.Forms.TextBox SerialConfig_Header_TextBox;

        //serialCommunication panel elements
        private System.Windows.Forms.Panel SerialComm_Panel;
        private System.Windows.Forms.Button SerialComm_StartComm_Btn;
        private System.Windows.Forms.ListBox SerialComm_SentMsgs_ListBox;
        private System.Windows.Forms.ListBox SerialComm_ReceivedMsgs_ListBox;
        private System.Windows.Forms.TextBox SerialComm_SentMsgsHeader_TextBox;
        private System.Windows.Forms.TextBox SerialComm_ReceivedMsgsHeader_TextBox;
        private System.Windows.Forms.TextBox SerialComm_Header_TextBox;

        /********************************************************
         * Private constructors
         *******************************************************/
        private SerialComm_Class()
        {
            Build_SerialComm_Panel();
            Build_SerialConfig_Panel();
        }

        /********************************************************
         * GetInstance function
         * 
         * returns instance of SerialComm_Class
         *******************************************************/
        public static SerialComm_Class GetInstance()
        {
            if (SerialComm_Instance == null)
            {
                SerialComm_Instance = new SerialComm_Class();
            }
            return SerialComm_Instance;
        }

        /********************************************************
         * Build Panel Function
         * 
         * initializes Textboxes, Labels, a Panel, and buttons
         *******************************************************/
        private void Build_SerialComm_Panel()
        {
            SerialConfig_Panel = new System.Windows.Forms.Panel(); 
            SerialConfig_FindPorts_Btn = new System.Windows.Forms.Button();
            SerialConfig_OpenPort_Btn = new System.Windows.Forms.Button();
            SerialConfig_ClosePort_Btn = new System.Windows.Forms.Button();
            SerialConfig_Ports_ListBox = new System.Windows.Forms.ListBox();
            SerialConfig_BaudRates_ListBox = new System.Windows.Forms.ListBox();
            SerialConfig_Parities_ListBox = new System.Windows.Forms.ListBox();
            SerialConfig_DataBits_ListBox = new System.Windows.Forms.ListBox();
            SerialConfig_StopBits_ListBox = new System.Windows.Forms.ListBox();
            SerialConfig_Ports_TextBox = new System.Windows.Forms.TextBox();
            SerialConfig_BaudRates_TextBox = new System.Windows.Forms.TextBox(); 
            SerialConfig_Parities_TextBox = new System.Windows.Forms.TextBox(); 
            SerialConfig_DataBits_TextBox = new System.Windows.Forms.TextBox(); 
            SerialConfig_StopBits_TextBox = new System.Windows.Forms.TextBox(); 
            SerialConfig_Header_TextBox = new System.Windows.Forms.TextBox(); 

            // 
            // SerComSettings_Panel
            // 
            SerialConfig_Panel.BackColor = System.Drawing.SystemColors.ControlLight;
            SerialConfig_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            SerialConfig_Panel.Location = new System.Drawing.Point(5, 3);
            SerialConfig_Panel.Name = "SerComSettings_Panel";
            SerialConfig_Panel.Size = new System.Drawing.Size(328, 505);
            SerialConfig_Panel.TabIndex = 7;
            SerialConfig_Panel.SuspendLayout();
            // 
            // SerialConfig_ClosePort_Btn
            // 
            SerialConfig_ClosePort_Btn.BackColor = System.Drawing.SystemColors.ActiveCaption;
            SerialConfig_ClosePort_Btn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            SerialConfig_ClosePort_Btn.Location = new System.Drawing.Point(178, 441);
            SerialConfig_ClosePort_Btn.Name = "SerialConfig_ClosePort_Btn";
            SerialConfig_ClosePort_Btn.Size = new System.Drawing.Size(84, 29);
            SerialConfig_ClosePort_Btn.TabIndex = 23;
            SerialConfig_ClosePort_Btn.Text = "CLOSE";
            SerialConfig_ClosePort_Btn.UseVisualStyleBackColor = false;
            SerialConfig_ClosePort_Btn.Click += new System.EventHandler(this.SerialConfig_ClosePort_Btn_Click_Handler);

            // 
            // SerialConfig_FindPorts_Btn
            // 
            SerialConfig_FindPorts_Btn.BackColor = System.Drawing.SystemColors.Info;
            SerialConfig_FindPorts_Btn.Location = new System.Drawing.Point(78, 50);
            SerialConfig_FindPorts_Btn.Name = "SerialConfig_FindPorts_Btn";
            SerialConfig_FindPorts_Btn.Size = new System.Drawing.Size(169, 26);
            SerialConfig_FindPorts_Btn.TabIndex = 20;
            SerialConfig_FindPorts_Btn.Text = "FIND NEW PORTS";
            SerialConfig_FindPorts_Btn.UseVisualStyleBackColor = false;
            SerialConfig_FindPorts_Btn.Click += new System.EventHandler(this.SerialConfig_FindPorts_Btn_Click_Handler);
            // 
            // SerialConfig_OpenPort_Btn
            // 
            SerialConfig_OpenPort_Btn.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            SerialConfig_OpenPort_Btn.Location = new System.Drawing.Point(56, 440);
            SerialConfig_OpenPort_Btn.Name = "SerialConfig_OpenPort_Btn";
            SerialConfig_OpenPort_Btn.Size = new System.Drawing.Size(83, 29);
            SerialConfig_OpenPort_Btn.TabIndex = 11;
            SerialConfig_OpenPort_Btn.Text = "OPEN";
            SerialConfig_OpenPort_Btn.UseVisualStyleBackColor = false;
            SerialConfig_OpenPort_Btn.Click += new System.EventHandler(this.SerialConfig_OpenPort_Btn_Click_Handler);
            // 
            // SerialConfig_DataBits_TextBox
            // 
            SerialConfig_DataBits_TextBox.ReadOnly = true;
            SerialConfig_DataBits_TextBox.BackColor = System.Drawing.SystemColors.Window;
            SerialConfig_DataBits_TextBox.Cursor = System.Windows.Forms.Cursors.SizeAll;
            SerialConfig_DataBits_TextBox.Location = new System.Drawing.Point(31, 310);
            SerialConfig_DataBits_TextBox.Name = "SerialConfig_DataBits_TextBox";
            SerialConfig_DataBits_TextBox.Size = new System.Drawing.Size(93, 22);
            SerialConfig_DataBits_TextBox.TabIndex = 21;
            SerialConfig_DataBits_TextBox.Text = "Data Bits";
            SerialConfig_DataBits_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SerialConfig_StopBits_TextBox
            // 
            SerialConfig_StopBits_TextBox.ReadOnly = true;
            SerialConfig_StopBits_TextBox.BackColor = System.Drawing.SystemColors.Window;
            SerialConfig_StopBits_TextBox.Location = new System.Drawing.Point(31, 380);
            SerialConfig_StopBits_TextBox.Name = "SerialConfig_StopBits_TextBox";
            SerialConfig_StopBits_TextBox.Size = new System.Drawing.Size(93, 22);
            SerialConfig_StopBits_TextBox.TabIndex = 16;
            SerialConfig_StopBits_TextBox.Text = "Stop Bit";
            SerialConfig_StopBits_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SerialConfig_Parities_TextBox
            // 
            SerialConfig_Parities_TextBox.ReadOnly = true;
            SerialConfig_Parities_TextBox.BackColor = System.Drawing.SystemColors.Window;
            SerialConfig_Parities_TextBox.Location = new System.Drawing.Point(31, 240);
            SerialConfig_Parities_TextBox.Name = "SerialConfig_Parities_TextBox";
            SerialConfig_Parities_TextBox.Size = new System.Drawing.Size(93, 22);
            SerialConfig_Parities_TextBox.TabIndex = 15;
            SerialConfig_Parities_TextBox.Text = "Parity";
            SerialConfig_Parities_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SerialConfig_BaudRates_TextBox
            // 
            SerialConfig_BaudRates_TextBox.ReadOnly = true;
            SerialConfig_BaudRates_TextBox.BackColor = System.Drawing.SystemColors.Window;
            SerialConfig_BaudRates_TextBox.Location = new System.Drawing.Point(31, 170);
            SerialConfig_BaudRates_TextBox.Name = "SerialConfig_BaudRates_TextBox";
            SerialConfig_BaudRates_TextBox.Size = new System.Drawing.Size(93, 22);
            SerialConfig_BaudRates_TextBox.TabIndex = 14;
            SerialConfig_BaudRates_TextBox.Text = " Baud Rate";
            SerialConfig_BaudRates_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SerialConfig_Ports_TextBox
            // 
            SerialConfig_Ports_TextBox.ReadOnly = true;
            SerialConfig_Ports_TextBox.BackColor = System.Drawing.SystemColors.Window;
            SerialConfig_Ports_TextBox.Location = new System.Drawing.Point(31, 100);
            SerialConfig_Ports_TextBox.Name = "SerialConfig_Ports_TextBox";
            SerialConfig_Ports_TextBox.Size = new System.Drawing.Size(93, 22);
            SerialConfig_Ports_TextBox.TabIndex = 12;
            SerialConfig_Ports_TextBox.Text = "Serial Port";
            SerialConfig_Ports_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // SerialConfig_Header_TextBox
            // 
            SerialConfig_Header_TextBox.ReadOnly = true;
            SerialConfig_Header_TextBox.BackColor = System.Drawing.SystemColors.Window;
            SerialConfig_Header_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.912F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            SerialConfig_Header_TextBox.Location = new System.Drawing.Point(1, 3);
            SerialConfig_Header_TextBox.Multiline = true;
            SerialConfig_Header_TextBox.Name = "SerialConfig_Header_TextBox";
            SerialConfig_Header_TextBox.Size = new System.Drawing.Size(324, 29);
            SerialConfig_Header_TextBox.TabIndex = 5;
            SerialConfig_Header_TextBox.Text = "SERIAL COMM SETTINGS";
            SerialConfig_Header_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // 
            // SerialConfig_DataBits_ListBox
            // 
            SerialConfig_DataBits_ListBox.FormattingEnabled = true;
            SerialConfig_DataBits_ListBox.Location = new System.Drawing.Point(151, 310);
            SerialConfig_DataBits_ListBox.Name = "SerialConfig_DataBits_ListBox";
            SerialConfig_DataBits_ListBox.Size = new System.Drawing.Size(154, 24);
            SerialConfig_DataBits_ListBox.TabIndex = 22;
            // 
            // SerialConfig_StopBits_ListBox
            // 
            SerialConfig_StopBits_ListBox.FormattingEnabled = true;
            SerialConfig_StopBits_ListBox.Location = new System.Drawing.Point(151, 380);
            SerialConfig_StopBits_ListBox.Name = "SerialConfig_StopBits_ListBox";
            SerialConfig_StopBits_ListBox.Size = new System.Drawing.Size(155, 24);
            SerialConfig_StopBits_ListBox.TabIndex = 19;
            // 
            // SerialConfig_Parities_ListBox
            // 
            SerialConfig_Parities_ListBox.FormattingEnabled = true;
            SerialConfig_Parities_ListBox.Location = new System.Drawing.Point(150, 240);
            SerialConfig_Parities_ListBox.Name = "SerialConfig_Parities_ListBox";
            SerialConfig_Parities_ListBox.Size = new System.Drawing.Size(155, 24);
            SerialConfig_Parities_ListBox.TabIndex = 18;
            // 
            // SerialConfig_BaudRates_ListBox
            // 
            SerialConfig_BaudRates_ListBox.FormattingEnabled = true;
            SerialConfig_BaudRates_ListBox.Location = new System.Drawing.Point(148, 170);
            SerialConfig_BaudRates_ListBox.Name = "SerialConfig_BaudRates_ListBox";
            SerialConfig_BaudRates_ListBox.Size = new System.Drawing.Size(155, 24);
            SerialConfig_BaudRates_ListBox.TabIndex = 17;
            // 
            // SerialConfig_Ports_ListBox
            // 
            SerialConfig_Ports_ListBox.FormattingEnabled = true;
            SerialConfig_Ports_ListBox.Location = new System.Drawing.Point(148, 100);
            SerialConfig_Ports_ListBox.Name = "SerialConfig_Ports_ListBox";
            SerialConfig_Ports_ListBox.Size = new System.Drawing.Size(155, 24);
            SerialConfig_Ports_ListBox.TabIndex = 13;


            //Add elements to panel
            SerialConfig_Panel.Controls.Add(SerialConfig_FindPorts_Btn);
            SerialConfig_Panel.Controls.Add(SerialConfig_OpenPort_Btn);
            SerialConfig_Panel.Controls.Add(SerialConfig_ClosePort_Btn);
            SerialConfig_Panel.Controls.Add(SerialConfig_Ports_ListBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_BaudRates_ListBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_Parities_ListBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_DataBits_ListBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_StopBits_ListBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_Ports_TextBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_BaudRates_TextBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_Parities_TextBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_DataBits_TextBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_StopBits_TextBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_Header_TextBox);

            //allow panel to update itself
            SerialConfig_Panel.ResumeLayout(false);
            SerialConfig_Panel.PerformLayout();
        }

        private void Build_SerialConfig_Panel()
        {
            SerialComm_Panel = new System.Windows.Forms.Panel();
            SerialComm_StartComm_Btn = new System.Windows.Forms.Button();   
            SerialComm_SentMsgs_ListBox = new System.Windows.Forms.ListBox();
            SerialComm_ReceivedMsgs_ListBox = new System.Windows.Forms.ListBox();
            SerialComm_SentMsgsHeader_TextBox = new System.Windows.Forms.TextBox();
            SerialComm_ReceivedMsgsHeader_TextBox = new System.Windows.Forms.TextBox();
            SerialComm_Header_TextBox = new System.Windows.Forms.TextBox();
            SerialConfig_Panel.SuspendLayout();

            // 
            // SerCommStatus_Panel
            // 
            SerialComm_Panel.BackColor = System.Drawing.SystemColors.ControlLight;
            SerialComm_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            SerialComm_Panel.Location = new System.Drawing.Point(673, 3);
            SerialComm_Panel.Name = "SerCommStatus_Panel";
            SerialComm_Panel.Size = new System.Drawing.Size(409, 505);
            SerialComm_Panel.TabIndex = 5;
            // 
            // SerialComm_ReceivedMsgs_ListBox
            // 
            SerialComm_ReceivedMsgs_ListBox.FormattingEnabled = true;
            SerialComm_ReceivedMsgs_ListBox.ItemHeight = 16;
            SerialComm_ReceivedMsgs_ListBox.Location = new System.Drawing.Point(12, 313);
            SerialComm_ReceivedMsgs_ListBox.Name = "SerialComm_ReceivedMsgs_ListBox";
            SerialComm_ReceivedMsgs_ListBox.Size = new System.Drawing.Size(375, 180);
            SerialComm_ReceivedMsgs_ListBox.TabIndex = 0;
            // 
            // SerialComm_SentMsgs_ListBox
            // 
            SerialComm_SentMsgs_ListBox.FormattingEnabled = true;
            SerialComm_SentMsgs_ListBox.ItemHeight = 16;
            SerialComm_SentMsgs_ListBox.Location = new System.Drawing.Point(12, 115);
            SerialComm_SentMsgs_ListBox.Name = "SerialComm_SentMsgs_ListBox";
            SerialComm_SentMsgs_ListBox.Size = new System.Drawing.Size(375, 164);
            SerialComm_SentMsgs_ListBox.TabIndex = 0;
            // 
            // SerialComm_StartComm_Btn
            // 
            SerialComm_StartComm_Btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.912F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            SerialComm_StartComm_Btn.Location = new System.Drawing.Point(164, 43);
            SerialComm_StartComm_Btn.Name = "SerialComm_StartComm_Btn";
            SerialComm_StartComm_Btn.Size = new System.Drawing.Size(87, 33);
            SerialComm_StartComm_Btn.TabIndex = 5;
            SerialComm_StartComm_Btn.Text = "Start";
            SerialComm_StartComm_Btn.UseVisualStyleBackColor = true;
            SerialComm_StartComm_Btn.Click += new System.EventHandler(this.SerialComm_StartComm_Btn_Click_Handler);
            // 
            // SerialComm_ReceivedMsgsHeader_TextBox
            // 
            SerialComm_ReceivedMsgsHeader_TextBox.ReadOnly = true;
            SerialComm_ReceivedMsgsHeader_TextBox.Location = new System.Drawing.Point(12, 285);
            SerialComm_ReceivedMsgsHeader_TextBox.Name = "SerialComm_ReceivedMsgsHeader_TextBox";
            SerialComm_ReceivedMsgsHeader_TextBox.Size = new System.Drawing.Size(375, 22);
            SerialComm_ReceivedMsgsHeader_TextBox.TabIndex = 4;
            SerialComm_ReceivedMsgsHeader_TextBox.Text = "RECEIVED MESSAGES:";
            SerialComm_ReceivedMsgsHeader_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SerialComm_SentMsgsHeader_TextBox
            // 
            SerialComm_SentMsgsHeader_TextBox.ReadOnly = true;
            SerialComm_SentMsgsHeader_TextBox.Location = new System.Drawing.Point(12, 86);
            SerialComm_SentMsgsHeader_TextBox.Name = "SerialComm_SentMsgsHeader_TextBox";
            SerialComm_SentMsgsHeader_TextBox.Size = new System.Drawing.Size(375, 22);
            SerialComm_SentMsgsHeader_TextBox.TabIndex = 3;
            SerialComm_SentMsgsHeader_TextBox.Text = "SENT MESSAGES:";
            SerialComm_SentMsgsHeader_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SerialComm_Header_TextBox
            // 
            SerialComm_Header_TextBox.ReadOnly = true;
            SerialComm_Header_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.912F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            SerialComm_Header_TextBox.Location = new System.Drawing.Point(3, 3);
            SerialComm_Header_TextBox.Multiline = true;
            SerialComm_Header_TextBox.Name = "SerialComm_Header_TextBox";
            SerialComm_Header_TextBox.Size = new System.Drawing.Size(401, 29);
            SerialComm_Header_TextBox.TabIndex = 0;
            SerialComm_Header_TextBox.Text = "SERIAL COMMUNICATION STATUS";
            SerialComm_Header_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            //Add elements to Panel
            SerialComm_Panel.Controls.Add(SerialComm_ReceivedMsgs_ListBox);
            SerialComm_Panel.Controls.Add(SerialComm_SentMsgs_ListBox);
            SerialComm_Panel.Controls.Add(SerialComm_StartComm_Btn);
            SerialComm_Panel.Controls.Add(SerialComm_ReceivedMsgsHeader_TextBox);
            SerialComm_Panel.Controls.Add(SerialComm_SentMsgsHeader_TextBox);
            SerialComm_Panel.Controls.Add(SerialComm_Header_TextBox);

            //allow panel to update itself
            SerialComm_Panel.ResumeLayout(false);
            SerialComm_Panel.PerformLayout();
        }

        /********************************************************
         * Get Panel Function
         * 
         * returns SerialComm panel
         *******************************************************/
        public Panel GetSerialConfigPanel()
        {
            return SerialConfig_Panel;
        }
        public Panel GetSerialCommPanel()
        {
            return SerialComm_Panel;
        }

        /********************************************************
         * Button Click Handler
         * 
         * initializes Textboxes, Labels, a Panel, and a button
         *******************************************************/
        private void SerialConfig_ClosePort_Btn_Click_Handler(object sender, EventArgs e)
        {


        }
        private void SerialConfig_FindPorts_Btn_Click_Handler(object sender, EventArgs e)
        {


        }
        private void SerialConfig_OpenPort_Btn_Click_Handler(object sender, EventArgs e)
        {


        }

        private void SerialComm_StartComm_Btn_Click_Handler(object sender, EventArgs e)
        {


        }
       
    }
}
