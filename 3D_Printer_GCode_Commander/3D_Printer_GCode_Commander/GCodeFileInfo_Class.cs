using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    internal class GCodeFileInfo_Class
    {
        //public variables

        //private variables
        private static GCodeFileInfo_Class GCodeFileInfo_Instance = null;

        //private ui element variables
        private System.Windows.Forms.Panel   GCodeFileInfo_Panel;
        private System.Windows.Forms.TextBox GCodeFileInfoPanelHeader_TextBox;
        private System.Windows.Forms.TextBox GCodeFileNameHeader_TextBox;
        private System.Windows.Forms.TextBox GCodeFileSizeHeader_TextBox;
        private System.Windows.Forms.TextBox GCodeFileDateHeader_TextBox;
        private System.Windows.Forms.Label   GCFileName_Label;
        private System.Windows.Forms.Label   GCFileSize_Label;
        private System.Windows.Forms.Label   GCFileDate_Label;
        private System.Windows.Forms.Button  GCFileLoad_Btn;
        private System.Windows.Forms.Button  GCFileValidate_Btn;


        /********************************************************
         * Private constructors
         *******************************************************/
        private GCodeFileInfo_Class() 
        {
            Build_GCodeInfo_Panel();
        }

        /********************************************************
         * GetInstance function
         * 
         * returns instance of GCodeFileInfo_Class
         *******************************************************/
        public static GCodeFileInfo_Class GetInstance()
        {
            if (GCodeFileInfo_Instance == null)
            {
                GCodeFileInfo_Instance = new GCodeFileInfo_Class();
            }
            return GCodeFileInfo_Instance;
        }

        /********************************************************
         * Build Panel Function
         * 
         * initializes Textboxes, Labels, a Panel, and buttons
         *******************************************************/
        private void Build_GCodeInfo_Panel()
        {
            GCodeFileInfo_Panel = new System.Windows.Forms.Panel();
            GCodeFileInfoPanelHeader_TextBox = new System.Windows.Forms.TextBox();
            GCodeFileNameHeader_TextBox = new System.Windows.Forms.TextBox();
            GCodeFileSizeHeader_TextBox = new System.Windows.Forms.TextBox();
            GCodeFileDateHeader_TextBox = new System.Windows.Forms.TextBox();
            GCFileName_Label = new System.Windows.Forms.Label();
            GCFileSize_Label = new System.Windows.Forms.Label();
            GCFileDate_Label = new System.Windows.Forms.Label();
            GCFileLoad_Btn = new System.Windows.Forms.Button();
            GCFileValidate_Btn = new System.Windows.Forms.Button();
            GCodeFileInfo_Panel.SuspendLayout();

            // Initialize elements
            // 
            // GCFileInfo_Panel
            // 
            GCodeFileInfo_Panel.BackColor = System.Drawing.SystemColors.ControlLight;
            GCodeFileInfo_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            GCodeFileInfo_Panel.Location = new System.Drawing.Point(340, 185);
            GCodeFileInfo_Panel.Name = "GCodeFileInfo_Panel";
            GCodeFileInfo_Panel.Size = new System.Drawing.Size(328, 323);
            GCodeFileInfo_Panel.TabIndex = 4;
            // 
            // LoadFile_Btn
            // 
            GCFileLoad_Btn.Location = new System.Drawing.Point(105, 52);
            GCFileLoad_Btn.Name = "GCFileLoad_Btn";
            GCFileLoad_Btn.Size = new System.Drawing.Size(122, 31);
            GCFileLoad_Btn.TabIndex = 2;
            GCFileLoad_Btn.Text = "Load Gcode File";
            GCFileLoad_Btn.UseVisualStyleBackColor = true;
            GCFileLoad_Btn.Click += new System.EventHandler(GCFileLoad_Btn_Click_Handler);
            // 
            // ValidateFile_Btn
            // 
            GCFileValidate_Btn.Location = new System.Drawing.Point(105, 250);
            GCFileValidate_Btn.Name = "GCFileValidate_Btn";
            GCFileValidate_Btn.Size = new System.Drawing.Size(122, 31);
            GCFileValidate_Btn.TabIndex = 9;
            GCFileValidate_Btn.Text = "Validate File";
            GCFileValidate_Btn.UseVisualStyleBackColor = true;
            GCFileValidate_Btn.Click += new System.EventHandler(GCFileValidate_Btn_Click_Handler);
            // 
            // PanelHeader_TextBox
            // 
            GCodeFileInfoPanelHeader_TextBox.ReadOnly = true;
            GCodeFileInfoPanelHeader_TextBox.BackColor = System.Drawing.SystemColors.Window;
            GCodeFileInfoPanelHeader_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.912F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            GCodeFileInfoPanelHeader_TextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            GCodeFileInfoPanelHeader_TextBox.Location = new System.Drawing.Point(0, 3);
            GCodeFileInfoPanelHeader_TextBox.Multiline = true;
            GCodeFileInfoPanelHeader_TextBox.Name = "GCodeFileInfoPanelHeader_TextBox";
            GCodeFileInfoPanelHeader_TextBox.Size = new System.Drawing.Size(327, 28);
            GCodeFileInfoPanelHeader_TextBox.TabIndex = 1;
            GCodeFileInfoPanelHeader_TextBox.Text = "GCODE FILE INFORMATION";
            GCodeFileInfoPanelHeader_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FileNameHeader_TextBox
            // 
            GCodeFileNameHeader_TextBox.ReadOnly = true;
            GCodeFileNameHeader_TextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCodeFileNameHeader_TextBox.Location = new System.Drawing.Point(12, 103);
            GCodeFileNameHeader_TextBox.Name = "GCodeFileNameHeader_TextBox";
            GCodeFileNameHeader_TextBox.Size = new System.Drawing.Size(87, 22);
            GCodeFileNameHeader_TextBox.TabIndex = 6;
            GCodeFileNameHeader_TextBox.Text = "File Name:";
            GCodeFileNameHeader_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FileSizeHeader_TextBox
            // 
            GCodeFileSizeHeader_TextBox.ReadOnly = true;
            GCodeFileSizeHeader_TextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCodeFileSizeHeader_TextBox.Location = new System.Drawing.Point(12, 150);
            GCodeFileSizeHeader_TextBox.Name = "GCodeFileSizeHeader_TextBox";
            GCodeFileSizeHeader_TextBox.Size = new System.Drawing.Size(87, 22);
            GCodeFileSizeHeader_TextBox.TabIndex = 7;
            GCodeFileSizeHeader_TextBox.Text = "File Size: ";
            GCodeFileSizeHeader_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FileDateHeader_TextBox
            // 
            GCodeFileDateHeader_TextBox.ReadOnly = true;
            GCodeFileDateHeader_TextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCodeFileDateHeader_TextBox.Location = new System.Drawing.Point(12, 200);
            GCodeFileDateHeader_TextBox.Name = "GCodeFileDateHeader_TextBox";
            GCodeFileDateHeader_TextBox.Size = new System.Drawing.Size(87, 22);
            GCodeFileDateHeader_TextBox.TabIndex = 8;
            GCodeFileDateHeader_TextBox.Text = "Last Updated:";
            GCodeFileDateHeader_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label to hold file name
            // 
            GCFileName_Label.AutoSize = true;
            GCFileName_Label.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCFileName_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            GCFileName_Label.Location = new System.Drawing.Point(105, 105);
            GCFileName_Label.Name = "GCFileName_Label";
            GCFileName_Label.Size = new System.Drawing.Size(0, 16);
            GCFileName_Label.TabIndex = 3;
            GCFileName_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label to hold file size
            // 
            GCFileSize_Label.AutoSize = true;
            GCFileSize_Label.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCFileSize_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            GCFileSize_Label.Location = new System.Drawing.Point(105, 152);
            GCFileSize_Label.Name = "GCFileSize_Label";
            GCFileSize_Label.Size = new System.Drawing.Size(0, 16);
            GCFileSize_Label.TabIndex = 10;
            // 
            // Label to hold file date
            // 
            GCFileDate_Label.AutoSize = true;
            GCFileDate_Label.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCFileDate_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            GCFileDate_Label.Location = new System.Drawing.Point(105, 202);
            GCFileDate_Label.Name = "GCFileDate_Label";
            GCFileDate_Label.Size = new System.Drawing.Size(2, 18);
            GCFileDate_Label.TabIndex = 11;

            //Add elements to panel
            GCodeFileInfo_Panel.Controls.Add(this.GCFileLoad_Btn);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileValidate_Btn);
            GCodeFileInfo_Panel.Controls.Add(this.GCodeFileInfoPanelHeader_TextBox);
            GCodeFileInfo_Panel.Controls.Add(this.GCodeFileNameHeader_TextBox);
            GCodeFileInfo_Panel.Controls.Add(this.GCodeFileSizeHeader_TextBox);
            GCodeFileInfo_Panel.Controls.Add(this.GCodeFileDateHeader_TextBox);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileName_Label);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileSize_Label);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileDate_Label);

            //Allow panel to update itself
            GCodeFileInfo_Panel.ResumeLayout(false);
            GCodeFileInfo_Panel.PerformLayout();
        }

        /********************************************************
         * Get Panel Function
         * 
         * returns GCodeFile Info panel
         *******************************************************/
        public Panel GetPanel()
        {
            return GCodeFileInfo_Panel;
        }

        /********************************************************
         * Button Click Handler
         * 
         * initializes Textboxes, Labels, a Panel, and a button
         *******************************************************/
        private void GCFileLoad_Btn_Click_Handler(object sender, EventArgs e)
        {


        }

        private void GCFileValidate_Btn_Click_Handler(object sender, EventArgs e)
        {


        }

    }
}
