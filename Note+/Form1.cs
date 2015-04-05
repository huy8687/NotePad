﻿using MouseKeyboardLibrary;
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

        //public Form1(ArrayList listKey)
        //{
        //    _listKey = listKey;
        //    if (!CheckLic())
        //    {
        //        MessageBox.Show(@"Something Fail!");
        //        Dispose();
        //        Environment.Exit(0);
        //    }
        //    InitializeComponent();
        //    try
        //    {
        //        SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        //        _mouseHook.MouseDown += mouseHook_MouseDown;
        //        _mouseHook.MouseUp += mouseHook_MouseUp;
        //        _mouseListen = new Thread(thread_MouseListen);
        //        _keyboardHook.KeyDown += keyboardHook_KeyDown;
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    timer1.Start();
        //    try
        //    {
        //        lbAns.Text = "";
        //        lbQues.Text = "";
        //        _mouseListen.Start();
        //        _mouseHook.Start();
        //        _keyboardHook.Start();
        //    }
        //    catch (Exception)
        //    {
        //        ExceptionCheck();
        //    }
        //}

        public Form1(ArrayList listKey, string data)
        {
            _listKey = listKey;
            _data = data == null ? "" : data;
            _data = (RemoveRedundancy(_data)).ToLower();
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
                _mouseHook.MouseMove += _mouseHook_MouseMove;
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


        //========================================================================================
        #region Global Hook (capture text, mousem keyboard)
        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private int _countExit = 0;
        private DateTime _dtFirstClick;
        private MouseEventArgs _firstClickEvent;
        private bool _moveOne = false;

        void _mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && !_moveOne && e.X - _firstClickEvent.X > 200)
            {
                SearchData(pivotStr,true);
                _moveOne = true;
            }
        }
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
            _moveOne = false;
            ExceptionCheck();
        }
        
        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                _isMouseDown = true;
                _moveOne = false;
                if (DateTime.Now.Subtract(_dtFirstClick).TotalMilliseconds < 150 && _firstClickEvent != null && e.X == _firstClickEvent.X && e.Y == _firstClickEvent.Y)
                {
                    _modeSearch = (Mode_Search)((int)(_modeSearch + 1) % 3);
                    ModifyTextComponent.SetText(this, lbQues, _modeSearch.ToString());
                }
                _firstClickEvent = e;
                _dtFirstClick = DateTime.Now;
            }
            catch
            {
                // ignored
            }
        }

        private void thread_MouseListen()
        {
            while (_mousListenRun)
            {
                try
                {
                    var mouse = Cursor.Position; // use Windows forms mouse code instead of WPF
                    var element = AutomationElement.FromPoint(new System.Windows.Point(mouse.X, mouse.Y));

                    if (element != null)
                    {
                        object pattern;

                        // the "Text" pattern is supported by some applications (including Notepad)and returns the current selection for example
                        if (element.TryGetCurrentPattern(TextPattern.Pattern, out pattern))
                        {
                            var textPattern = (TextPattern)pattern;
                            foreach (var range in textPattern.GetSelection())
                            {
                                var text = (RemoveRedundancy(range.GetText(-1))).ToLower();
                                if (_isMouseDown && text != "" && text.Length < 777 && !text.Equals(pivotStr))
                                {
                                    pivotStr = text;
                                    SearchData(text,false);
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
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var keyBytes = new Rfc2898DeriveBytes(p1 + "X", Encoding.ASCII.GetBytes(p2 + "X")).GetBytes(256 / 8);
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
            var cipherTextBytes = Convert.FromBase64String(encryptedText);
            var keyBytes = new Rfc2898DeriveBytes(p1, Encoding.ASCII.GetBytes(p2)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(p5));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];

            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
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

        private void SearchData(String text, bool isNext)
        {
            try
            {
                switch (_modeSearch)
                {
                    case Mode_Search.Key_Mode:
                        if (SearchKeyBank(text, isNext)) return;
                        if (_listKey.Count < 2)
                        {
                            if (SearchBook(text, isNext)) return;
                        }
                        break;
                    case Mode_Search.Book_Mode:
                        if (SearchBook(text, isNext)) return;
                        if (_data.Length < 5)
                        {
                            if (SearchKeyBank(text, isNext)) return;
                        }
                        break;
                    case Mode_Search.Key_Book_Mode:
                        if (SearchKeyBank(text, isNext)) return;
                        if (SearchBook(text, isNext)) return;
                        break;
                }

                // default
                ModifyTextComponent.SetText(this, lbQues, "<Next>");
                ModifyTextComponent.SetText(this, lbAns, "");
            }
            catch (Exception)
            {
                ExceptionCheck();
            }
        }

        private int _indexKey=-1;
        private int _indexBook=-1;

        private bool SearchBook(String text, bool isNext)
        {
            if (!string.IsNullOrEmpty(_data))
            {
                var index = _data.IndexOf(text);

                if (isNext && _indexBook!=-1)
                {
                    index = _data.IndexOf(text,_indexBook);
                    
                } else if (!isNext)
                {
                    _indexBook = -1;
                }

                if (index >= 0)
                {
                    index += text.Length / 2;
                    var str = index < 150 ? _data.Substring(0, 350) : _data.Substring(index - 150, 350);
                    str = (RemoveRedundancy(str));
                    str = str.Replace(text, " █ " + text + " █ ");
                    ModifyTextComponent.SetText(this, lbQues, str);
                    lbQues.Size = new Size(638, 51);
                    lbAns.Size = new Size(1, 1);
                    _indexBook = index;
                    return true;
                }
            }
            _indexBook = -1;
            return false;
        }

        private bool SearchKeyBank(String text, bool isNext)
        {
            var indexStart = 0;

            if (isNext && _indexKey != -1 && _indexKey != _listKey.Count)
            {
                indexStart = _indexKey + 1;
            }
            else if (!isNext)
            {
                _indexKey = -1;
            }

            for (var index = indexStart; index < _listKey.Count; index++)
            {
                var key = (Key) _listKey[index];
                if (key.Ans.Contains(text) || key.Ques.Contains(text))
                {
                    ModifyTextComponent.SetText(this, lbAns, key.Ans);
                    ModifyTextComponent.SetText(this, lbQues, key.Ques);
                    lbQues.Size = new Size(638, 34);
                    lbAns.Size = new Size(638, 18);
                    _indexKey = index;
                    return true;
                }
            }
            _indexKey = -1;
            return false;
        }

        private static string RemoveRedundancy(string s)
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
                var cpuID = cpuId();
                var biosID = biosId();
                var mainboardID = baseId();
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
            var screen = Screen.PrimaryScreen;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            base.TopMost = true;
        }

        #endregion

        #region Original Device ID Getting Code
        private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            var result = "";
            var mc = new System.Management.ManagementClass(wmiClass);
            var moc = mc.GetInstances();
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
            var result = "";
            var mc = new System.Management.ManagementClass(wmiClass);
            var moc = mc.GetInstances();
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
            var retVal = identifier("Win32_Processor", "UniqueId");
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
    }
}
