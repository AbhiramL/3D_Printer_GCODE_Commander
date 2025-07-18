using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    internal class ModuleConfig_Class
    {
        //public variables
        public readonly ClassNames_e myClassName = ClassNames_e.Module_Config_Class;

        //private variables
        private static ModuleConfig_Class ModuleConfig_instance = null;

        //private UI element variables
        private System.Windows.Forms.Panel ModuleConfig_Panel;
        private System.Windows.Forms.TextBox PanelTitle_TextBox;
        private System.Windows.Forms.TextBox ModuleConfig_XAxis_TextBox;
        private System.Windows.Forms.TextBox ModuleConfig_YAxis_TextBox;
        private System.Windows.Forms.TextBox ModuleConfig_ZAxis_TextBox;
        private System.Windows.Forms.TextBox ModuleConfig_NeutralPos_TextBox;
        private System.Windows.Forms.TextBox ModuleConfig_Tool_TextBox;
        private System.Windows.Forms.ListBox ModuleConfig_XAxis_ListBox;
        private System.Windows.Forms.ListBox ModuleConfig_YAxis_ListBox;
        private System.Windows.Forms.ListBox ModuleConfig_ZAxis_ListBox;
        private System.Windows.Forms.ListBox ModuleConfig_NeutralPos_ListBox;
        private System.Windows.Forms.ListBox ModuleConfig_Tool_ListBox;
        private System.Windows.Forms.Button ModuleConfig_Send_Btn;

        //constructor
        private ModuleConfig_Class() 
        {
            //build panel
            Build_ModuleConfig_Panel();

            //init private member variables


        }

        /********************************************************
         * GetInstance function
         * 
         * returns instance of ModuleInfo_Class
         *******************************************************/
        public static ModuleConfig_Class GetInstance()
        {
            if (ModuleConfig_instance == null)
            {
                ModuleConfig_instance = new ModuleConfig_Class();
            }
            return ModuleConfig_instance;
        }

        /********************************************************
         * Get Panel Function
         * 
         * returns Module Info panel
         *******************************************************/
        public Panel GetPanel()
        {
            return ModuleConfig_Panel;
        }

        /********************************************************
         * Build Panel Function
         * 
         * initializes Textboxes, Labels, a Panel, and a button
         *******************************************************/
        private void Build_ModuleConfig_Panel()
        {
            ModuleConfig_Panel = new System.Windows.Forms.Panel();
            PanelTitle_TextBox = new System.Windows.Forms.TextBox();
            ModuleConfig_XAxis_TextBox = new System.Windows.Forms.TextBox();
            ModuleConfig_YAxis_TextBox = new System.Windows.Forms.TextBox();
            ModuleConfig_ZAxis_TextBox = new System.Windows.Forms.TextBox();
            ModuleConfig_YAxis_ListBox = new System.Windows.Forms.ListBox();
            ModuleConfig_ZAxis_ListBox = new System.Windows.Forms.ListBox();
            ModuleConfig_NeutralPos_ListBox = new System.Windows.Forms.ListBox();
            ModuleConfig_Tool_ListBox = new System.Windows.Forms.ListBox();
            ModuleConfig_Send_Btn = new System.Windows.Forms.Button();

        //
        // Initialize elements:
        //
        // Panel init
        //
            ModuleConfig_Panel.BackColor = System.Drawing.SystemColors.ControlLight;
            ModuleConfig_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ModuleConfig_Panel.Location = new System.Drawing.Point(308, 183);
            ModuleConfig_Panel.Name = "ModuleConfig_Panel";
            ModuleConfig_Panel.Size = new System.Drawing.Size(328, 321);
            ModuleConfig_Panel.TabIndex = 6;
            //
            // Title Header Textbox
            //
            PanelTitle_TextBox.ReadOnly = true;
            PanelTitle_TextBox.BackColor = System.Drawing.SystemColors.HighlightText;
            PanelTitle_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.912F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            PanelTitle_TextBox.Location = new System.Drawing.Point(0, 3);
            PanelTitle_TextBox.Multiline = true;
            PanelTitle_TextBox.Name = "ModuleConfigHeader_TextBox";
            PanelTitle_TextBox.Size = new System.Drawing.Size(327, 28);
            PanelTitle_TextBox.TabIndex = 1;
            PanelTitle_TextBox.Text = "PRINTER CONFIG SETTINGS";
            PanelTitle_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // X axis Size textbox
            //
            ModuleConfig_XAxis_TextBox.ReadOnly = true;
            ModuleConfig_XAxis_TextBox.BackColor = System.Drawing.SystemColors.Window;
            ModuleConfig_XAxis_TextBox.Location = new System.Drawing.Point(13, 45);
            ModuleConfig_XAxis_TextBox.Name = "ModuleConfig_X_TextBox";
            ModuleConfig_XAxis_TextBox.Size = new System.Drawing.Size(100, 23);
            ModuleConfig_XAxis_TextBox.TabIndex = 12;
            ModuleConfig_XAxis_TextBox.Text = "X Axis Dimensions";
            ModuleConfig_XAxis_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // Y axis Size textbox
            //
            ModuleConfig_YAxis_TextBox.ReadOnly = true;
            ModuleConfig_YAxis_TextBox.BackColor = System.Drawing.SystemColors.Window;
            ModuleConfig_YAxis_TextBox.Location = new System.Drawing.Point(13, 95);
            ModuleConfig_YAxis_TextBox.Name = "ModuleConfig_Y_TextBox";
            ModuleConfig_YAxis_TextBox.Size = new System.Drawing.Size(100, 23);
            ModuleConfig_YAxis_TextBox.TabIndex = 12;
            ModuleConfig_YAxis_TextBox.Text = "Y Axis Dimensions";
            ModuleConfig_YAxis_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // Z axis Size textbox
            //
            ModuleConfig_ZAxis_TextBox.ReadOnly = true;
            ModuleConfig_ZAxis_TextBox.BackColor = System.Drawing.SystemColors.Window;
            ModuleConfig_ZAxis_TextBox.Location = new System.Drawing.Point(13, 145);
            ModuleConfig_ZAxis_TextBox.Name = "ModuleConfig_Z_TextBox";
            ModuleConfig_ZAxis_TextBox.Size = new System.Drawing.Size(100, 23);
            ModuleConfig_ZAxis_TextBox.TabIndex = 12;
            ModuleConfig_ZAxis_TextBox.Text = "Z Axis Dimensions";
            ModuleConfig_ZAxis_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            //
            // Send Configuration button
            //
            ModuleConfig_Send_Btn.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            ModuleConfig_Send_Btn.Location = new System.Drawing.Point(90, 200); // Adjusted for new width
            ModuleConfig_Send_Btn.Name = "ModuleConfig_Send_Btn";
            ModuleConfig_Send_Btn.Size = new System.Drawing.Size(150, 25);
            ModuleConfig_Send_Btn.TabIndex = 8;
            ModuleConfig_Send_Btn.Text = "CONFIGURE MODULE";
            ModuleConfig_Send_Btn.UseVisualStyleBackColor = false;
            ModuleConfig_Send_Btn.BackColor = System.Drawing.Color.Green;
            ModuleConfig_Send_Btn.Click += new System.EventHandler(this.Configure_Btn_Click_Handler);

            //add elements to Panel
            ModuleConfig_Panel.Controls.Add(PanelTitle_TextBox);
            ModuleConfig_Panel.Controls.Add(ModuleConfig_XAxis_TextBox);
            ModuleConfig_Panel.Controls.Add(ModuleConfig_YAxis_TextBox);
            ModuleConfig_Panel.Controls.Add(ModuleConfig_ZAxis_TextBox);
            ModuleConfig_Panel.Controls.Add(ModuleConfig_Send_Btn);

            //allow panel to update itself
            ModuleConfig_Panel.ResumeLayout(false);
            ModuleConfig_Panel.PerformLayout();
        }

        /********************************************************
        * Button Click Handler
        * 
        * initializes Textboxes, Labels, a Panel, and a button
        *******************************************************/
        private void Configure_Btn_Click_Handler(object sender, EventArgs e)
        {
            IntertaskMessage ittmsg = new IntertaskMessage(myClassName, new ModuleMessage(CommandType_e.MCF));
            
            //set the user set valaues in the varMessage of theconfig dispatch
            //add TOD
            //route message
            Commander_MainApp.RouteIntertaskMessage(ittmsg);
        }

    }

       
    }
