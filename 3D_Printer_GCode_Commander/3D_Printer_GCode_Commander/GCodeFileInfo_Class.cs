using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    internal class GCodeFileInfo_Class
    {
        //public variables
        public readonly ClassNames_e myClassName = ClassNames_e.Gcode_File_Info_Class;

        //private variables
        private static GCodeFileInfo_Class GCodeFileInfo_Instance = null;
        private StreamReader GCFileReader;
        private List<GCodeCommand> GCodeCmdList;
        private string GCFileLocation;

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
        private System.Windows.Forms.Button GCFileUnload_Btn;
        private System.Windows.Forms.CheckBox GCFileValidity_Checkbox;


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
         * Load File Function
         * 
         * loads a gcode file and saves its file location
         *******************************************************/
        public void LoadFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    GCFileLocation = openFileDialog.FileName;
                    GCFileReader = new StreamReader(GCFileLocation);

                    FileInfo fileInfo = new FileInfo(GCFileLocation);
                    GCFileName_Label.Text = GCFileLocation;
                    GCFileSize_Label.Text = fileInfo.Length.ToString() + " bytes";
                    GCFileDate_Label.Text = fileInfo.LastWriteTime.ToString();

                }
            }
        }

        /********************************************************
         * Unload file function
         * 
         * Updates panel and resets fileLocation variable
         *******************************************************/
        public void UnloadFile()
        {
            GCFileName_Label.Text = "";
            GCFileDate_Label.Text = "";
            GCFileSize_Label.Text = "";
            GCFileLocation = "";
            if (GCFileReader != null)
            {
                GCFileReader.Close();
                GCFileReader = null;
            }

            //clear any cmds that were read
            GCodeCmdList.Clear();
            GCodeCmdList = null;

        }

        /********************************************************
         * Validate file function
         * 
         * returns true if the GCODE file is valid
         * builds a List of valid gcode commands
         *******************************************************/
        public bool ReadAndValidateFile()
        {
            GCodeCmdList = new List<GCodeCommand>();
            string line;
            bool isValid = false;

            //perform a check of the GCODE
            while ((GCFileReader != null) && ((line = GCFileReader.ReadLine()) != null))
            {
                //check if it is really a GCODE and build a GCode Command
                GCodeCommand gCodeCommand = new GCodeCommand(line);

                //if the GCode command is not an error,
                if(gCodeCommand.CmdType == CommandType_e.ERR) 
                {
                    //not GCODE
                    GCodeCmdList.Clear();
                    isValid = false;
                    break;
                }
                else if(gCodeCommand.CmdType != CommandType_e.NULL) //and is not a comment...
                {
                    //add cmd to the list
                    GCodeCmdList.Add(gCodeCommand);
                    isValid = true;
                }
            }

            if ((GCFileReader != null))
            {
                //close reader and return
                GCFileReader.Close();
            }
            return isValid;
        }

        /********************************************************
         * Get command list function
         * 
         * returns a List of valid gcode commands that were loaded
         * from a file
         *******************************************************/
        public List<GCodeCommand> GetGCodeCommands()
        {
            return GCodeCmdList;
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
            GCFileUnload_Btn = new System.Windows.Forms.Button();
            GCFileValidity_Checkbox = new System.Windows.Forms.CheckBox();
            GCodeFileInfo_Panel.SuspendLayout();
            //
            // Initialize elements
            // 
            // GCFileInfo_Panel
            // 
            GCodeFileInfo_Panel.BackColor = System.Drawing.SystemColors.ControlLight;
            GCodeFileInfo_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            GCodeFileInfo_Panel.Location = new System.Drawing.Point(3, 285);
            GCodeFileInfo_Panel.Name = "GCodeFileInfo_Panel";
            GCodeFileInfo_Panel.Size = new System.Drawing.Size(300, 220);
            GCodeFileInfo_Panel.TabIndex = 4;
            // 
            // LoadFile_Btn
            // 
            GCFileLoad_Btn.Location = new System.Drawing.Point(8, 180);
            GCFileLoad_Btn.Name = "GCFileLoad_Btn";
            GCFileLoad_Btn.Size = new System.Drawing.Size(122, 29);
            GCFileLoad_Btn.TabIndex = 2;
            GCFileLoad_Btn.Text = "Load Gcode File";
            GCFileLoad_Btn.UseVisualStyleBackColor = true;
            GCFileLoad_Btn.BackColor = System.Drawing.Color.Green;
            GCFileLoad_Btn.Click += new System.EventHandler(GCFileLoad_Btn_Click_Handler);
            // 
            // UnloadFile_Btn
            // 
            GCFileUnload_Btn.Location = new System.Drawing.Point(150, 180);
            GCFileUnload_Btn.Name = "GCFileUnload_Btn";
            GCFileUnload_Btn.Size = new System.Drawing.Size(132, 29);
            GCFileUnload_Btn.TabIndex = 2;
            GCFileUnload_Btn.Text = "Unload Gcode File";
            GCFileUnload_Btn.UseVisualStyleBackColor = true;
            GCFileUnload_Btn.BackColor = System.Drawing.Color.Gray;
            GCFileUnload_Btn.Enabled = false;
            GCFileUnload_Btn.Click += new System.EventHandler(GCFileUnload_Btn_Click_Handler);
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
            GCodeFileInfoPanelHeader_TextBox.Size = new System.Drawing.Size(300, 28);
            GCodeFileInfoPanelHeader_TextBox.TabIndex = 1;
            GCodeFileInfoPanelHeader_TextBox.Text = "GCODE FILE INFORMATION";
            GCodeFileInfoPanelHeader_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FileNameHeader_TextBox
            // 
            GCodeFileNameHeader_TextBox.ReadOnly = true;
            GCodeFileNameHeader_TextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCodeFileNameHeader_TextBox.Location = new System.Drawing.Point(12, 50);
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
            GCodeFileSizeHeader_TextBox.Location = new System.Drawing.Point(12, 100);
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
            GCodeFileDateHeader_TextBox.Location = new System.Drawing.Point(12, 130);
            GCodeFileDateHeader_TextBox.Name = "GCodeFileDateHeader_TextBox";
            GCodeFileDateHeader_TextBox.Size = new System.Drawing.Size(87, 22);
            GCodeFileDateHeader_TextBox.TabIndex = 8;
            GCodeFileDateHeader_TextBox.Text = "Last Updated:";
            GCodeFileDateHeader_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label to hold file name
            // 
            GCFileName_Label.AutoSize = true;
            GCFileName_Label.MaximumSize = new System.Drawing.Size(170, 80); //wraps text if too long
            GCFileName_Label.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCFileName_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            GCFileName_Label.Location = new System.Drawing.Point(105, 50);
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
            GCFileSize_Label.Location = new System.Drawing.Point(105, 100);
            GCFileSize_Label.Name = "GCFileSize_Label";
            GCFileSize_Label.Size = new System.Drawing.Size(0, 16);
            GCFileSize_Label.TabIndex = 10;
            // 
            // Label to hold file date
            // 
            GCFileDate_Label.AutoSize = true;
            GCFileDate_Label.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            GCFileDate_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            GCFileDate_Label.Location = new System.Drawing.Point(105, 130);
            GCFileDate_Label.Name = "GCFileDate_Label";
            GCFileDate_Label.Size = new System.Drawing.Size(2, 18);
            GCFileDate_Label.TabIndex = 11;
            //
            // Validity checkbox
            //
            GCFileValidity_Checkbox.Text = "File is Valid:";
            GCFileValidity_Checkbox.AutoCheck = false;
            GCFileValidity_Checkbox.Location = new System.Drawing.Point(15, 160);
            GCFileValidity_Checkbox.Name = "GCFileValidity_Checkbox";
            GCFileValidity_Checkbox.AutoSize = true;
            GCFileValidity_Checkbox.Visible = false;
            GCFileValidity_Checkbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            GCFileValidity_Checkbox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;


            //Add elements to panel
            GCodeFileInfo_Panel.Controls.Add(this.GCFileLoad_Btn);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileUnload_Btn);
            GCodeFileInfo_Panel.Controls.Add(this.GCodeFileInfoPanelHeader_TextBox);
            GCodeFileInfo_Panel.Controls.Add(this.GCodeFileNameHeader_TextBox);
            GCodeFileInfo_Panel.Controls.Add(this.GCodeFileSizeHeader_TextBox);
            GCodeFileInfo_Panel.Controls.Add(this.GCodeFileDateHeader_TextBox);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileName_Label);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileSize_Label);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileDate_Label);
            GCodeFileInfo_Panel.Controls.Add(this.GCFileValidity_Checkbox);

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
            LoadFile();

            GCFileValidity_Checkbox.Visible = true;
            GCFileUnload_Btn.Enabled = true;
            GCFileLoad_Btn.Enabled = false;
            GCFileUnload_Btn.BackColor = System.Drawing.Color.Red;
            GCFileLoad_Btn.BackColor = System.Drawing.Color.Gray;

            if (ReadAndValidateFile())
            {
                //valid file was loaded, modify ui to show validity
                GCFileValidity_Checkbox.Checked=true;
                GCodeFileInfo_Panel.BackColor = System.Drawing.SystemColors.Info;
            }
            else
            {
                GCFileValidity_Checkbox.Checked = false;
            }
        }

        private void GCFileUnload_Btn_Click_Handler(object sender, EventArgs e)
        {
            UnloadFile();
            GCFileValidity_Checkbox.Visible = false;
            GCFileLoad_Btn.Enabled = true;
            GCFileUnload_Btn.Enabled = false;
            GCodeFileInfo_Panel.BackColor = System.Drawing.SystemColors.ControlLight;
            GCFileUnload_Btn.BackColor = System.Drawing.Color.Gray;
            GCFileLoad_Btn.BackColor = System.Drawing.Color.Green;
        }
    }
}
