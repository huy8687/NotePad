using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Note_
{
    public partial class NotePad : Form, Note_.INotePad
    {
        public NotePad()
        {
            InitializeComponent();
            if (CheckLic())
            {
                miStart.Visible = true;
                miRegister.Visible = false;
            }
            else
            {
                miStart.Visible = false;
                miRegister.Visible = true;
            }
            _listKey = new ArrayList();
        }

        private string fname = "";
        private System.Drawing.Printing.PrintDocument docToPrint = new System.Drawing.Printing.PrintDocument();
        private ToolStripMenuItem wordWrapToolStripMenuItem;
        

        public ToolStripMenuItem WordWrapToolStripMenuItem
        {
            get { return wordWrapToolStripMenuItem; }
            set { wordWrapToolStripMenuItem = value; }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fname == "")
            {
                saveFileDialog1.Filter = "Text Files|*.txt";
                DialogResult result = saveFileDialog1.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                fname = saveFileDialog1.FileName;
                StreamWriter s = new StreamWriter(fname);
                s.WriteLine(richTextBox1.Text);
                s.Flush();
                s.Close();
            }
        }
        private static string RemoveSpaces(string s)
        {
            s = s.Replace(Environment.NewLine, " ");
            s = s.Replace("\t", " ");
            s = s.Trim();
            do
            {
                s = s.Replace("  ", " ");
            }
            while (s.Contains("  "));
            do
            {
                s = s.Replace("..", ".");
            }
            while (s.Contains(".."));
            var str = "!@#$%^&*()_+{}:\"<>?[]',./\\;-=";
            foreach (var ch in str.ToCharArray())
            {
                s = s.Replace(ch + " ", ch.ToString());
                s = s.Replace(" " + ch, ch.ToString());
            }
            return s;
        }
        private void openCtrlOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _listKey.Clear();
            richTextBox1.Text = "";
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    string line = "";

                    // Read the file and display it line by line.
                    System.IO.StreamReader file =
                        new System.IO.StreamReader(openFileDialog1.FileName, System.Text.Encoding.GetEncoding(1252));
                    while ((line = file.ReadLine().ToLower()) != null)
                    {
                        
                        try
                        {
                            String ques = RemoveSpaces(line.Substring(0, line.IndexOf("|")));
                            String ans = RemoveSpaces(line.Substring(line.IndexOf("|") + 1));
                            _listKey.Add(new Key(ques, ans));
                            richTextBox1.AppendText(line + "\n");
                        }
                        catch (Exception) { richTextBox1.AppendText("===========ERROOR=="+line + "\n"); }
                    }
                    file.Close();
                    
                }
                else
                {
                    MessageBox.Show(" Not Opened File");
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            richTextBox1.AppendText("LOG + " + _listKey.Count);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
            {
                DialogResult click = MessageBox.Show("The text in the Untitled has changed.\n\n Do you want to save the changes?", " My Notepad", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (click == DialogResult.Yes)
                {
                    if (fname == "")
                    {
                        saveFileDialog1.Filter = "Text Files|*.textBox1";
                        DialogResult result = saveFileDialog1.ShowDialog();
                        if (result == DialogResult.Cancel)
                        {
                            return;
                        }
                        fname = saveFileDialog1.FileName;
                        // MessageBox.Show(fname);
                    }
                    StreamWriter write = new StreamWriter(fname);
                    write.WriteLine(richTextBox1.Text);
                    write.Flush();
                    //  textBox1.Text = "";
                    write.Close();

                    richTextBox1.Text = "";
                    fname = "";
                    // bool flag = false;
                }
                if (click == DialogResult.No)
                {
                    richTextBox1.Text = "";
                    fname = "";
                }
            }
            else
            {
                richTextBox1.Text = "";
                fname = "";
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt";
            saveFileDialog1.ShowDialog();
            fname = saveFileDialog1.FileName;
            if (fname == "")
            {
                saveFileDialog1.Filter = "Text Files|*.txt";
                DialogResult result = saveFileDialog1.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                fname = saveFileDialog1.FileName;
            }
            StreamWriter s = new StreamWriter(fname);
            s.WriteLine(richTextBox1.Text);
            s.Flush();
            s.Close();
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.PageSettings = new System.Drawing.Printing.PageSettings();
            pageSetupDialog1.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            pageSetupDialog1.ShowNetwork = false;
            DialogResult result = pageSetupDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                object[] results = new object[]{
                    pageSetupDialog1.PageSettings.Margins,
                    pageSetupDialog1.PageSettings.PaperSize,
                    pageSetupDialog1.PageSettings.Landscape,
                    pageSetupDialog1.PrinterSettings.PrinterName,
                    pageSetupDialog1.PrinterSettings.PrintRange};
                //richTextBox1.Text.LastIndexOf(results);
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {

            printDialog1.AllowSomePages = true;
            printDialog1.ShowHelp = true;
            printDialog1.Document = docToPrint;
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                docToPrint.Print();
            }

        }
        private void document_PrintPage(object sender,
          System.Drawing.Printing.PrintPageEventArgs e)
        {
            string text = "In document_PrintPage method.";
            System.Drawing.Font printFont = new System.Drawing.Font
                ("Arial", 35, System.Drawing.FontStyle.Regular);
            e.Graphics.DrawString(text, printFont,
                System.Drawing.Brushes.Black, 10, 10);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
        {
            DialogResult click = MessageBox.Show("The text in the Untitled has changed.\n\n Do you want to save the changes?", " My Notepad", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (click == DialogResult.Yes)
            {
                if (fname == "")
                {
                    saveFileDialog1.Filter = "Text Files|*.txt";
                    DialogResult result = saveFileDialog1.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                    fname = saveFileDialog1.FileName;
                    // MessageBox.Show(fname);
                }
                StreamWriter write = new StreamWriter(fname);
                write.WriteLine(richTextBox1.Text);
                write.Flush();
                write.Close();
                Application.Exit();
            }
            if (click == DialogResult.No)
            {
                Application.Exit();
            }
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
                richTextBox1.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != "")
            {
                cutToolStripMenuItem.Enabled = true;
                richTextBox1.Cut();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != "")
            {
                richTextBox1.Copy();
            }
        }

        private void pToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
            {
                richTextBox1.SelectedText = "";
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FindTab tab = new FindTab(this);
            //tab.Show();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
            {
                richTextBox1.SelectAll();
            }
            else
            {
                MessageBox.Show("No Text is there");
            }
        }

        private void timeDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string timeDate;
            timeDate = DateTime.Now.ToShortTimeString() + " " +
            DateTime.Now.ToShortDateString();
            int newSelectionStart = richTextBox1.SelectionStart + timeDate.Length;
            richTextBox1.Text = richTextBox1.Text.Insert(richTextBox1.SelectionStart, timeDate);
            richTextBox1.SelectionStart = newSelectionStart;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowColor = true;
            fontDialog1.ShowDialog();
            richTextBox1.Font = fontDialog1.Font;
            richTextBox1.ForeColor = fontDialog1.Color;
        }

        private void wordToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void miStart_Click(object sender, EventArgs e)
        {
            if (_listKey.Count<=0) _listKey.Add(new Key("a", "a"));
            if (_listKey.Count < 1) MessageBox.Show("Error");
            else {
                this.Hide();
                new Form1(_listKey,_data).Show();
            }
        }
        private ArrayList _listKey;
        private string _data;
        #region MachineID, checkMachineID
        private String getMachineID()
        {
            try
            {
                String cpuID = cpuId();
                String biosID = biosId();
                String mainboardID = baseId();
                return cpuID + biosID + mainboardID;
            }
            catch (Exception)
            {
                MessageBox.Show("Please log out your System !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            return null;
        }

        private bool CheckLic()
        {
            try
            {
                //var file = new StreamReader("lic.dll");
                //var line = Decrypt(file.ReadLine());
                var machineID = getMachineID();
                //file.Close();
                return "BFEBFBFF000206A7Dell Inc.A11FH47MP120120803000000.000000+000DELL   - 1072009Dell Inc.Base Board.FH47MP1.CN7016618R00K1.".Equals(machineID);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Original Device ID Getting Code
        private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
        private static string cpuId()
        {
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "")
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "")
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "")
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
        private static string biosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }
        private static string diskId()
        {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }
        private static string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }
        private static string videoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }
        private static string macId()
        {
            return identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }
        #endregion
        #region Encrypt, decrypt data

        private string p0 = "Q@LW2td1";
        private string p1 = "P@@s9dhni2";
        private string p2 = "S@LT&Ad2";
        private string p3 = "C@@scw2ai2";
        private string p4 = "A@@ad2djg5";
        private string p5 = "@nk1j2b3k1jb23kb12";
        private string p6 = "@n20dk2kjflep20fk3";

        private string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(p1 + "X", Encoding.ASCII.GetBytes(p2 + "X")).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(p5 + "X"));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        private string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(p1, Encoding.ASCII.GetBytes(p2)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(p5));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        #endregion

        private void miRegister_Click(object sender, EventArgs e)
        {
            if (!check) return;
            System.Windows.Forms.Clipboard.SetDataObject((Encrypt(getMachineID())));
            check = false;
            miRegister.Enabled = false;
        }

        private bool check = true;

        private void openBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    // Read the file and display it line by line.
                    System.IO.StreamReader file =
                        new System.IO.StreamReader(openFileDialog1.FileName, System.Text.Encoding.GetEncoding(1252));
                    _data = file.ReadToEnd();
                    richTextBox1.AppendText(_data);
                    file.Close();

                }
                else
                {
                    MessageBox.Show(" Not Opened File");
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
        }
    }
}
