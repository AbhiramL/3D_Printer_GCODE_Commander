using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Printer_GCode_Commander
{
    public enum BaudRateSelections_e : int
    {
        b_9600 = 9600,
        b_19200 = 19200,
        b_38400 = 38400,
        b_57600 = 57600,
        b_115200 = 115200,
        b_230400 = 230400,
        b_460800 = 460800,
        b_921600 = 921600

    };
    public enum ParitySelections_e : byte
    {
        p_None = 0,
        p_Odd = 1,
        p_Even = 2
    };
    public enum NumDataBitsSelections_e : byte
    {
        d_7 = 7,
        d_8 = 8,
        d_9 = 9
    };
    public enum NumStopBitsSelections_e : byte
    {
        s_None = 0,
        s_1 = 1,
        s_2 = 2
    };
    internal class SerialComm_Class
    {
        //public variables
        public readonly Owner_e myClassName = Owner_e.Serial_Comm_Class;

        //private variables
        private static SerialComm_Class SerialComm_Instance = null;
        private SerialPort serialPort_Instance = null;
        private List<IntertaskMessage> serialMessageRequestQueue;
        private List<ModuleMessage> serialTransmitQueue; //tx messages are in byte[] format
        private List<byte> serialReceiveQueue; //rx messages are in byte format, to be processed into complete messages
        private string portSelect;
        private ParitySelections_e paritySelect;
        private NumStopBitsSelections_e stopBitSelect;
        private BaudRateSelections_e baudRateSelect;
        private NumDataBitsSelections_e dataBitsSelect;
        private CancellationTokenSource cancelTokenSource;
        private CancellationToken cancelToken;
        private byte currentTransactID;

        //private ui element variables
        //serialConfig panel elements
        private System.Windows.Forms.Panel   SerialConfig_Panel;
        private System.Windows.Forms.Button  SerialConfig_FindPorts_Btn;
        private System.Windows.Forms.Button  SerialConfig_OpenPort_Btn;
        private System.Windows.Forms.Button  SerialConfig_ClosePort_Btn;
        private System.Windows.Forms.ComboBox SerialConfig_Ports_ComboBox;
        private System.Windows.Forms.ComboBox SerialConfig_BaudRates_ComboBox;
        private System.Windows.Forms.ComboBox SerialConfig_Parities_ComboBox;
        private System.Windows.Forms.ComboBox SerialConfig_DataBits_ComboBox;
        private System.Windows.Forms.ComboBox SerialConfig_StopBits_ComboBox;
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
            serialTransmitQueue = new List<ModuleMessage>();
            serialReceiveQueue = new List<byte>();
            serialMessageRequestQueue = new List<IntertaskMessage>();

            Build_SerialComm_Panel();
            Build_SerialConfig_Panel();

            AddPortOptions();
            AddBaudRateOptions();
            AddParityOptions();
            AddDataBitsOptions();
            AddStopBitOptions();
            GetPanelSelections();

            //attach handlers to the combo boxes
            SerialConfig_Ports_ComboBox.SelectedIndexChanged += Port_ComboBox_SelectedIndexChanged_Handler;
            SerialConfig_BaudRates_ComboBox.SelectedIndexChanged += BaudRate_ComboBox_SelectedIndexChanged_Handler;
            SerialConfig_Parities_ComboBox.SelectedIndexChanged += Parity_ComboBox_SelectedIndexChanged_Handler;
            SerialConfig_DataBits_ComboBox.SelectedIndexChanged += DataBits_ComboBox_SelectedIndexChanged_Handler;
            SerialConfig_StopBits_ComboBox.SelectedIndexChanged += StopBits_ComboBox_SelectedIndexChanged_Handler;
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
         * Record Input function
         * 
         * saves user selected ComboBox items
         *******************************************************/
        public void GetPanelSelections()
        {
            portSelect = SerialConfig_Ports_ComboBox.SelectedItem != null ? SerialConfig_Ports_ComboBox.SelectedItem.ToString() : "No Ports Found";
            baudRateSelect = (BaudRateSelections_e)SerialConfig_BaudRates_ComboBox.SelectedItem;
            paritySelect = (ParitySelections_e)SerialConfig_Parities_ComboBox.SelectedItem;
            dataBitsSelect = (NumDataBitsSelections_e)SerialConfig_DataBits_ComboBox.SelectedItem;
            stopBitSelect = (NumStopBitsSelections_e)SerialConfig_StopBits_ComboBox.SelectedItem;
        }

        /********************************************************
         * Open serial port function
         * 
         * opens port with selected comboBox items
         *******************************************************/
        public bool OpenPort()
        {
            bool retVal = false;
            
            //bug occurs if selected serial port dissapears after serial port creation
            if (portSelect != "NONE")
            {
                //check if serial instance isnt null and if its open
                if (serialPort_Instance != null)
                {
                    ClosePort();
                }

                //open new serial port instance
                serialPort_Instance = new SerialPort(portSelect, (int)baudRateSelect, (Parity)paritySelect, (int)dataBitsSelect, (StopBits)stopBitSelect);
                serialPort_Instance.Open();
                serialTransmitQueue.Clear();
                //start async task to send transmit queue messages
                cancelTokenSource = new CancellationTokenSource();
                cancelToken = cancelTokenSource.Token;
                Task.Run(() => SendSerialAsync(cancelToken));
                Task.Run(() => ReceiveSerialAsync(cancelToken));

                //reset transaction id
                ModuleMessage.resetTransactionID();

                retVal = true;
            }
            else
            {
                MessageBox.Show("No serial ports detected, add serial port first.");
            }

            return retVal;
        }
        public void ClosePort()
        {
            serialPort_Instance.Close();
            serialPort_Instance.Dispose();

            //end async tasks
            cancelTokenSource.Cancel();
                        
            ClearCommMenus();

            serialTransmitQueue.Clear();
        }

        /********************************************************
         * Route Message function
         * Called from async task
         * routes valid message to message request owner
         *******************************************************/
        private void RouteReceivedMessage(ModuleMessage incommingMessage)
        {
            //check incomming message validity
            if (incommingMessage.isValid)
            {
                //check the request queue to see if the incomming message is for another class
                for(int i = 0; i < serialMessageRequestQueue.Count; i++) 
                {
                    //find if a request matched the incomming message transaction id
                    if (incommingMessage.baseMessage.TransactID == serialMessageRequestQueue[i].moduleMsg.baseMessage.TransactID)
                    {
                        //found the owner, add the incomming message and route the request
                        serialMessageRequestQueue[i].moduleMsg = incommingMessage;
                        Commander_MainApp.RouteIntertaskMessage(myClassName, serialMessageRequestQueue[i]);

                        //remove request from list
                        serialMessageRequestQueue.RemoveAt(i);
                        break;
                    }
                }

                //else it is a general message and should be printed out on ui
            }
        }

        /********************************************************
         * Add serial message to transmit queue.
         * 
         * adds a serial message to the Transmit queue
         *******************************************************/
        public void AddMessageToTxQueue(IntertaskMessage serialCommMessage)
        {
            if (serialPort_Instance == null)
            {
                MessageBox.Show("Error: Serial Port not setup. Configure a port first, then open the port.");
            }
            else
            {
                if (serialPort_Instance.IsOpen)
                {
                    //debugging
                    //_uiControl.Invoke(new Action(() => AddSentMessage(s_message + $",\t{DateTime.Now:HH:mm:ss}")));

                    //add new request to the request queue
                    serialMessageRequestQueue.Add(serialCommMessage);

                    //add module message to the transmit queue to send out
                    serialTransmitQueue.Add(serialCommMessage.moduleMsg);
                }
                else
                {
                    MessageBox.Show("Error: Attempted to send a message on a closed serial port. \n Open a Port first.");
                }
            }
        }

        /********************************************************
         * Asyncronous Task SendSerial Function handle
         * 
         * sends messages in transmit queue on serial port
         * 
         * 
         * runs on a seperate thread than the commander main app
         * executes automatically, parallel to the main thread
         *******************************************************/
        private async Task SendSerialAsync(CancellationToken token)
        {
            byte[] bytes;

            while(!cancelTokenSource.IsCancellationRequested) //while task isnt cancelled
            {
                if ((serialPort_Instance != null) && (serialPort_Instance.IsOpen))
                {
                    while (serialPort_Instance.BytesToWrite > 0)
                    {
                        await Task.Delay(300,token); 
                    } //waiting for serial port to be ready

                    if (serialTransmitQueue.Count > 0)
                    {
                        //send next module message
                        bytes = serialTransmitQueue[0].GetByteArray();
                        Console.WriteLine(BitConverter.ToString(bytes));
                        serialPort_Instance.Write(bytes, 0, bytes.Length);

                        //add message to screen ui
                        //AddSentMessage(bytes);

                        //dequeue 
                        serialTransmitQueue.RemoveAt(0);
                    }
                }

                await Task.Delay(1000,token); // Pass the token to ensure safe cancellation
            }
        }

        /********************************************************
         * Asyncronous Task ReceiveSerial Function handle
         * 
         * builds module messages based on bytes in the receive queue
         *  
         * runs on a seperate thread than the commander main app
         * executes automatically, parallel to the main thread
         *******************************************************/
        private async Task ReceiveSerialAsync(CancellationToken token)
        {
            while (!cancelTokenSource.IsCancellationRequested) //while task isnt cancelled
            {
                if ((serialPort_Instance != null) && (serialPort_Instance.IsOpen))
                {
                    while ((serialReceiveQueue == null ) || (serialReceiveQueue.Count <= 0))
                    {
                        await Task.Delay(500, token);
                    } //waiting for bytes to appear in receive queue

                    ModuleMessage receivedMessage = new ModuleMessage(serialReceiveQueue.ToArray());

                    //if received message is valid
                    if(receivedMessage.isValid)
                    {
                        //invoke the SerialComm class route message function
                        RouteReceivedMessage(receivedMessage);

                        //add message to the ui screen
                        //AddRecMessage(receivedMessage.GetByteArray());

                        //since the message is valid, remove bytes in receive queue until the message's sync is found
                        while (serialReceiveQueue[0] != receivedMessage.baseMessage.Sync)
                        {
                            serialReceiveQueue.RemoveAt(0);
                        }

                        //remove the sync byte so the message in the buffer is no longer valid
                        serialReceiveQueue.RemoveAt(0);
                    }
                }
                await Task.Delay(500, token); // Pass the token to ensure safe cancellation
            }
        }

        /***************************************************************************************
         * Panel Specific Functions
         *
         ********************************************************
         * Build Panel Function
         * 
         * initializes Textboxes, Labels, a Panel, and buttons
         *******************************************************/
        private void Build_SerialConfig_Panel()
        {
            SerialConfig_Panel = new System.Windows.Forms.Panel(); 
            SerialConfig_FindPorts_Btn = new System.Windows.Forms.Button();
            SerialConfig_OpenPort_Btn = new System.Windows.Forms.Button();
            SerialConfig_ClosePort_Btn = new System.Windows.Forms.Button();
            SerialConfig_Ports_ComboBox = new System.Windows.Forms.ComboBox();
            SerialConfig_BaudRates_ComboBox = new System.Windows.Forms.ComboBox();
            SerialConfig_Parities_ComboBox = new System.Windows.Forms.ComboBox();
            SerialConfig_DataBits_ComboBox = new System.Windows.Forms.ComboBox();
            SerialConfig_StopBits_ComboBox = new System.Windows.Forms.ComboBox();
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
            // SerialConfig_ClosePort_Btn
            // 
            SerialConfig_ClosePort_Btn.BackColor = System.Drawing.SystemColors.ActiveCaption;
            SerialConfig_ClosePort_Btn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            SerialConfig_ClosePort_Btn.Location = new System.Drawing.Point(180, 440);
            SerialConfig_ClosePort_Btn.Name = "SerialConfig_ClosePort_Btn";
            SerialConfig_ClosePort_Btn.Size = new System.Drawing.Size(83, 29);
            SerialConfig_ClosePort_Btn.TabIndex = 23;
            SerialConfig_ClosePort_Btn.Text = "CLOSE";
            SerialConfig_ClosePort_Btn.UseVisualStyleBackColor = false;
            SerialConfig_ClosePort_Btn.Click += new System.EventHandler(this.SerialConfig_ClosePort_Btn_Click_Handler);
            // 
            // SerialConfig_OpenPort_Btn
            // 
            SerialConfig_OpenPort_Btn.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            SerialConfig_OpenPort_Btn.Location = new System.Drawing.Point(60, 440);
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
            SerialConfig_DataBits_TextBox.Location = new System.Drawing.Point(31, 240);
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
            SerialConfig_StopBits_TextBox.Text = "Stop Bits";
            SerialConfig_StopBits_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SerialConfig_Parities_TextBox
            // 
            SerialConfig_Parities_TextBox.ReadOnly = true;
            SerialConfig_Parities_TextBox.BackColor = System.Drawing.SystemColors.Window;
            SerialConfig_Parities_TextBox.Location = new System.Drawing.Point(31, 310);
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
            // SerialConfig_DataBits_ComboBox
            // 
            SerialConfig_DataBits_ComboBox.FormattingEnabled = true;
            SerialConfig_DataBits_ComboBox.Location = new System.Drawing.Point(151, 240);
            SerialConfig_DataBits_ComboBox.Name = "SerialConfig_DataBits_ListBox";
            SerialConfig_DataBits_ComboBox.Size = new System.Drawing.Size(154, 24);
            SerialConfig_DataBits_ComboBox.TabIndex = 22;
            // 
            // SerialConfig_StopBits_ComboBox
            // 
            SerialConfig_StopBits_ComboBox.FormattingEnabled = true;
            SerialConfig_StopBits_ComboBox.Location = new System.Drawing.Point(151, 380);
            SerialConfig_StopBits_ComboBox.Name = "SerialConfig_StopBits_ListBox";
            SerialConfig_StopBits_ComboBox.Size = new System.Drawing.Size(155, 24);
            SerialConfig_StopBits_ComboBox.TabIndex = 19;
            // 
            // SerialConfig_Parities_ComboBox
            // 
            SerialConfig_Parities_ComboBox.FormattingEnabled = true;
            SerialConfig_Parities_ComboBox.Location = new System.Drawing.Point(150, 310);
            SerialConfig_Parities_ComboBox.Name = "SerialConfig_Parities_ListBox";
            SerialConfig_Parities_ComboBox.Size = new System.Drawing.Size(155, 24);
            SerialConfig_Parities_ComboBox.TabIndex = 18;
            // 
            // SerialConfig_BaudRates_ComboBox
            // 
            SerialConfig_BaudRates_ComboBox.FormattingEnabled = true;
            SerialConfig_BaudRates_ComboBox.Location = new System.Drawing.Point(148, 170);
            SerialConfig_BaudRates_ComboBox.Name = "SerialConfig_BaudRates_ListBox";
            SerialConfig_BaudRates_ComboBox.Size = new System.Drawing.Size(155, 24);
            SerialConfig_BaudRates_ComboBox.TabIndex = 17;
            // 
            // SerialConfig_Ports_ComboBox
            // 
            SerialConfig_Ports_ComboBox.FormattingEnabled = true;
            SerialConfig_Ports_ComboBox.Location = new System.Drawing.Point(148, 100);
            SerialConfig_Ports_ComboBox.Name = "SerialConfig_Ports_ListBox";
            SerialConfig_Ports_ComboBox.Size = new System.Drawing.Size(155, 24);
            SerialConfig_Ports_ComboBox.TabIndex = 13;


            //Add elements to panel
            SerialConfig_Panel.Controls.Add(SerialConfig_FindPorts_Btn);
            SerialConfig_Panel.Controls.Add(SerialConfig_OpenPort_Btn);
            SerialConfig_Panel.Controls.Add(SerialConfig_ClosePort_Btn);
            SerialConfig_Panel.Controls.Add(SerialConfig_Ports_ComboBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_BaudRates_ComboBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_Parities_ComboBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_DataBits_ComboBox);
            SerialConfig_Panel.Controls.Add(SerialConfig_StopBits_ComboBox);
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

        private void Build_SerialComm_Panel()
        {
            SerialComm_Panel = new System.Windows.Forms.Panel();
            SerialComm_StartComm_Btn = new System.Windows.Forms.Button();   
            SerialComm_SentMsgs_ListBox = new System.Windows.Forms.ListBox();
            SerialComm_ReceivedMsgs_ListBox = new System.Windows.Forms.ListBox();
            SerialComm_SentMsgsHeader_TextBox = new System.Windows.Forms.TextBox();
            SerialComm_ReceivedMsgsHeader_TextBox = new System.Windows.Forms.TextBox();
            SerialComm_Header_TextBox = new System.Windows.Forms.TextBox();
            SerialComm_Panel.SuspendLayout();

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
            SerialComm_StartComm_Btn.Location = new System.Drawing.Point(155, 43);
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
         * Add ComboBox items functions
         * 
         * fills the ComboBox elements with relevant info
         *******************************************************/
        public void AddPortOptions()
        {
            List<string> portList = new List<string> { };
            portList.AddRange(SerialPort.GetPortNames());  // Add detected ports
            portList.Add("NONE");

            // Populate ComboBox
            SerialConfig_Ports_ComboBox.Items.Clear();
            SerialConfig_Ports_ComboBox.Items.AddRange(portList.ToArray());

            // Default selection
            SerialConfig_Ports_ComboBox.SelectedIndex = 0;
        }
        private void AddBaudRateOptions()
        {
            SerialConfig_BaudRates_ComboBox.Items.Clear();  // Clear previous items
            foreach (BaudRateSelections_e b_rate in Enum.GetValues(typeof(BaudRateSelections_e)))
            {
                SerialConfig_BaudRates_ComboBox.Items.Add(b_rate);  // Add baud rates to ComboBox
            }
            SerialConfig_BaudRates_ComboBox.SelectedIndex = 4;
        }
        private void AddParityOptions()
        {
            SerialConfig_Parities_ComboBox.Items.Clear();
            foreach (ParitySelections_e parity in Enum.GetValues(typeof(ParitySelections_e)))
            {
                SerialConfig_Parities_ComboBox.Items.Add(parity);  // Add parity options to the ComboBox
            }
            SerialConfig_Parities_ComboBox.SelectedIndex = 0;
        }
        private void AddDataBitsOptions()
        {
            SerialConfig_DataBits_ComboBox.Items.Clear();
            foreach (NumDataBitsSelections_e dataBits in Enum.GetValues(typeof(NumDataBitsSelections_e)))
            {
                SerialConfig_DataBits_ComboBox.Items.Add(dataBits);  // Add stop bit options to ComboBox
            }
            SerialConfig_DataBits_ComboBox.SelectedIndex = 1;
        }
        private void AddStopBitOptions()
        {
            SerialConfig_StopBits_ComboBox.Items.Clear();
            foreach (NumStopBitsSelections_e stopBits in Enum.GetValues(typeof(NumStopBitsSelections_e)))
            {
                SerialConfig_StopBits_ComboBox.Items.Add(stopBits);  // Add stop bit options to ComboBox
            }
            SerialConfig_StopBits_ComboBox.SelectedIndex = 2;
        }

        /********************************************************
         * Serial Comm Panel functions
         * 
         * update the ui elements of the comm panel
         *******************************************************/
        private void AddSentMessage(byte[] message)
        {
            SerialComm_SentMsgs_ListBox.Items.Add(BitConverter.ToString(message));

            //maintain 10 messages in the sent message box
            if (SerialComm_SentMsgs_ListBox.Items.Count > 10)
            {
                SerialComm_SentMsgs_ListBox.Items.RemoveAt(0);
            }
        }

        private void AddRecMessage(byte[] message)
        {
            SerialComm_ReceivedMsgs_ListBox.Items.Add(BitConverter.ToString(message));

            //maintain 10 messages in the received message box
            if (SerialComm_ReceivedMsgs_ListBox.Items.Count > 10)
            {
                SerialComm_ReceivedMsgs_ListBox.Items.RemoveAt(0);
            }
        }

        private void ClearCommMenus()
        {
            SerialComm_SentMsgs_ListBox.Items.Clear();
            SerialComm_ReceivedMsgs_ListBox.Items.Clear();
        }

        /********************************************************
         * Serial data received Handler
         * 
         * 
         *******************************************************/
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //read all bytes in serial rx buffer
            byte[] bytesReceived = new byte[serialPort_Instance.BytesToRead];
            serialPort_Instance.Read(bytesReceived, 0, bytesReceived.Length);
            
            //store the bytes in the receive queue
            serialReceiveQueue.AddRange(bytesReceived);
        }

        /********************************************************
         * ComboBox Click Handlers
         * 
         * records user input
         *******************************************************/
        public void Port_ComboBox_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            portSelect = SerialConfig_Ports_ComboBox.SelectedItem != null ? SerialConfig_Ports_ComboBox.SelectedItem.ToString() : "No Ports Found";
        }
        public void BaudRate_ComboBox_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            baudRateSelect = (BaudRateSelections_e)SerialConfig_BaudRates_ComboBox.SelectedItem;
        }
        public void Parity_ComboBox_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            paritySelect = (ParitySelections_e)SerialConfig_Parities_ComboBox.SelectedItem;
        }
        public void DataBits_ComboBox_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            dataBitsSelect = (NumDataBitsSelections_e)SerialConfig_DataBits_ComboBox.SelectedItem;
        }
        public void StopBits_ComboBox_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            stopBitSelect = (NumStopBitsSelections_e)SerialConfig_StopBits_ComboBox.SelectedItem;
        }

        /********************************************************
         * Button Click Handlers
         * 
         *******************************************************/
        private void SerialConfig_FindPorts_Btn_Click_Handler(object sender, EventArgs e)
        {
            AddPortOptions();
        }
        private void SerialConfig_ClosePort_Btn_Click_Handler(object sender, EventArgs e)
        {
            ClosePort();
            SerialConfig_OpenPort_Btn.BackColor = System.Drawing.Color.Gray;
            SerialConfig_ClosePort_Btn.BackColor = System.Drawing.Color.Gray;
        }
        private void SerialConfig_OpenPort_Btn_Click_Handler(object sender, EventArgs e)
        {
            if(OpenPort())
            {
                SerialConfig_OpenPort_Btn.BackColor = System.Drawing.Color.Green;
                SerialConfig_ClosePort_Btn.BackColor = System.Drawing.Color.Red;
            }
        }
        private void SerialComm_StartComm_Btn_Click_Handler(object sender, EventArgs e)
        {
            //get hold of the commander, ask for the gcode command list
            List<GCodeCommand> commandList = Commander_MainApp.GetGCodeCommandList();

            //add all elements of the list into the transmit queue
            ModuleMessage moduleMsg;
            if (commandList != null)
            {
                for (int i = 0; i < commandList.Count; i++)
                {
                    moduleMsg = new ModuleMessage(commandList[i]);
                    serialTransmitQueue.Add(moduleMsg);
                }

                //grey out the start button
                SerialComm_StartComm_Btn.Enabled = false;
            }
        }

    }
}
