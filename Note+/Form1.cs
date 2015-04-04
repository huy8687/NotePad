using MouseKeyboardLibrary;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Forms;
using System.Management;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
namespace Note_
{
    public partial class Form1 : Form
    {
        private String[] _filePaths;
        private readonly MouseHook _mouseHook = new MouseHook();
        private readonly KeyboardHook _keyboardHook = new KeyboardHook();
        private string _data;

        private enum Mode_Search { Key_Mode, Book_Mode, Key_Book_Mode };
        private Mode_Search _modeSearch = Mode_Search.Key_Book_Mode;

        private ArrayList _listKey;
        private Thread _mouseListen;

        private bool _mousListenRun = true;
        private const Boolean Running = true;
        private Boolean _isMouseDown;
        public Form1()
        {
            if (!CheckLic())
            {
                try
                {
                    File.WriteAllText(@"info.xml", Encrypt(getMachineID()));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                MessageBox.Show(@"Something Fail!");
                Dispose();
                Environment.Exit(0);
            }
            InitializeComponent();
            timer1.Start();
        }
        public Form1(ArrayList listKey)
        {
            _listKey = listKey;
            if (!CheckLic())
            {
                MessageBox.Show(@"Something Fail!");
                Dispose();
                Environment.Exit(0);
            }
            InitializeComponent();
            try
            {
                SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                _mouseHook.MouseDown += mouseHook_MouseDown;
                _mouseHook.MouseUp += mouseHook_MouseUp;
                _mouseHook.DoubleClick += _mouseHook_DoubleClick;
                _mouseListen = new Thread(thread_MouseListen);
                _keyboardHook.KeyDown += keyboardHook_KeyDown;
            }
            catch (Exception)
            {
            }
            timer1.Start();
            try
            {
                lbAns.Text = "";
                lbQues.Text = "";
                _mouseListen.Start();
                _mouseHook.Start();
                _keyboardHook.Start();
            }
            catch (Exception)
            {
                ExceptionCheck();
            }
        }

        void _mouseHook_DoubleClick(object sender, EventArgs e)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine("doble");
            }
            _modeSearch = (Mode_Search)((int)(((int)_modeSearch + 1) / 3));
            _isMouseDown = true;
            //File.AppendAllText("log.txt",_modeSearch.ToString());
                // Create a file to write to. 

            ModifyTextComponent.SetText(this, lbQues, _modeSearch.ToString());
            Thread.Sleep(500);
            _isMouseDown = false;
        }
        public Form1(ArrayList listKey, string data)
        {
            _listKey = listKey;
            _data = data == null ? "" : data;
            _data = (RemoveNewLine(_data)).ToLower();
            if (!CheckLic())
            {
                MessageBox.Show(@"Something Fail!");
                Dispose();
                Environment.Exit(0);
            }
            InitializeComponent();
            try
            {
                SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                _mouseHook.MouseDown += mouseHook_MouseDown;
                _mouseHook.MouseUp += mouseHook_MouseUp;
                MouseSimulator.DoubleClick(MouseButton.Left);
                _mouseHook.DoubleClick += _mouseHook_DoubleClick;
                
                _mouseListen = new Thread(thread_MouseListen);
                _keyboardHook.KeyDown += keyboardHook_KeyDown;
                //_keyboardHook.Start();
            }
            catch (Exception)
            {
            }
            timer1.Start();
            try
            {
                lbAns.Text = "";
                lbQues.Text = "";
                _mouseListen.Start();
                _mouseHook.Start();
                _keyboardHook.Start();
            }
            catch (Exception)
            {
                ExceptionCheck();
            }
        }
        //========================================================================================
        #region Global Hook (capture text, mousem keyboard)
        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private int _countExit = 0;
        private void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                try
                {
                    _mouseHook.Stop();
                    _keyboardHook.Stop();
                    _mousListenRun = false;
                    _mouseListen.Join();
                }
                catch (Exception)
                {
                }
                timer1.Stop();
                Application.Exit();
            }
            if (e.KeyCode == Keys.Oemtilde)
            {

                if (++_countExit == 2)
                {
                    try
                    {
                        var fi = new FileInfo(@"lic.dll");
                        fi.Delete();
                    }
                    catch (IOException)
                    {

                    }
                    Environment.Exit(0);
                }
            }
            else _countExit = 0;

        }

        private void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
            ExceptionCheck();
                // Create a file to write to. 
                using (StreamWriter sw = File.AppendText("log.txt"))
                {
                    sw.WriteLine("UP");
                }
        }

        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
                // Create a file to write to. 
            using (StreamWriter sw = File.AppendText("log.txt"))
                {
                    sw.WriteLine("DOWN");
                }
        }

        private void thread_MouseListen()
        {
            while (_mousListenRun)
            {
                try
                {
                    Point mouse = Cursor.Position; // use Windows forms mouse code instead of WPF
                    AutomationElement element = AutomationElement.FromPoint(new System.Windows.Point(mouse.X, mouse.Y));

                    if (element != null)
                    {
                        object pattern;

                        // the "Text" pattern is supported by some applications (including Notepad)and returns the current selection for example
                        if (element.TryGetCurrentPattern(TextPattern.Pattern, out pattern))
                        {
                            TextPattern textPattern = (TextPattern)pattern;
                            foreach (TextPatternRange range in textPattern.GetSelection())
                            {
                                String text = (RemoveNewLine(range.GetText(-1))).ToLower();
                                if (_isMouseDown && text != "" && text.Length < 777 && !text.Equals(pivotStr))
                                {
                                    pivotStr = text;
                                    searchKey(text);
                                }

                            }
                        }
                    }
                }
                catch (Exception) { ExceptionCheck(); }
                Thread.Sleep(500);
            }
        }
        #endregion

        private String pivotStr = "";
        //========================================================================================
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

        //========================================================================================
        #region Thuat Toan
        private void ExceptionCheck()
        {
            try
            {
                ModifyTextComponent.SetText(this, lbAns, "");
                ModifyTextComponent.SetText(this, lbQues, "");
            }
            catch (Exception)
            {
                Environment.Exit(0);
            }
        }

        private void searchKey(String text)
        {
            try
            {
                foreach (Key key in _listKey)
                {
                    if (key.Ans.Contains(text) || key.Ques.Contains(text))
                    {
                        ModifyTextComponent.SetText(this, lbAns, key.Ans);
                        ModifyTextComponent.SetText(this, lbQues, key.Ques);
                        lbQues.Size = new Size(638, 34);
                        lbAns.Size = new Size(638, 18);
                        return;
                    }
                }
                if (!string.IsNullOrEmpty(_data))
                {
                    var str = "";
                    var index = _data.IndexOf(text);
                    if (index >= 0)
                    {
                        index += text.Length / 2;
                        if (index < 150) str = _data.Substring(0, 350);
                        else str = _data.Substring(index - 150, 350);
                        str = (RemoveNewLine(str));
                        str = str.Replace(text, " █ " + text + " █ ");
                        ModifyTextComponent.SetText(this, lbQues, str);
                        lbQues.Size = new Size(638, 51);
                        lbAns.Size = new Size(1, 1);
                        return;
                    }
                }
                ModifyTextComponent.SetText(this, lbQues, "<Next>");
                ModifyTextComponent.SetText(this, lbAns, "");
            }
            catch (Exception)
            {
                ExceptionCheck();
            }
        }

        private static string RemoveNewLine(string s)
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

        #endregion

        //========================================================================================
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
                if (!"BFEBFBFF000206A7Dell Inc.A11FH47MP120120803000000.000000+000DELL   - 1072009Dell Inc.Base Board.FH47MP1.CN7016618R00K1.".Equals(machineID)) return false;
                setLocation();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


        //========================================================================================
        #region Form action


        private void setLocation()
        {
            Screen screen = Screen.PrimaryScreen;
            Location = new Point((screen.Bounds.Width - 638) / 2, screen.Bounds.Height - 49);
        }


        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private void btStart_Click(object sender, EventArgs e)
        {
            try
            {
                lbAns.Text = "";
                lbQues.Text = "";
                _mouseListen.Start();
                _mouseHook.Start();
                _keyboardHook.Start();
            }
            catch (Exception)
            {
                ExceptionCheck();
            }

        }

        private void btExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            base.TopMost = true;
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

        private void btSelect_Click(object sender, EventArgs e)
        {
        }
    }
}
