using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GrobalHook;

namespace GrobalHookMouseWinFormCSharp
{
    public partial class FormMain : Form
    {
        private static IntPtr m_hHook = IntPtr.Zero;
        MouseHook.HookProcedureDelegate m_proc;
        private const int WH_MOUSE_LL = 14;

        public FormMain()
        {
            InitializeComponent();

            if (m_hHook == IntPtr.Zero)
            {
                m_proc = new MouseHook.HookProcedureDelegate(MouseHookProc);
                SetMouseHook(m_proc);
            }
            else
            {
                RemoveMouseHook();
            }
        }

        private bool SetMouseHook(MouseHook.HookProcedureDelegate _proc)
        {
            bool bResult = true;

            using (Process process = Process.GetCurrentProcess())
            using (ProcessModule processModule = process.MainModule)
            {
                m_hHook = MouseHook.SetWindowsHookEx(WH_MOUSE_LL, _proc, MouseHook.GetModuleHandle(processModule.ModuleName), 0);
                if (m_hHook == IntPtr.Zero)
                {
                    {
                        MessageBox.Show("SetWindowsHookEx Failed.");
                    }
                }
            }

            return bResult;
        }

        private void RemoveMouseHook()
        {
            if (MouseHook.UnhookWindowsHookEx(m_hHook) == false)
            {
                MessageBox.Show("UnhookWindowsHookEx Failed.");
            }
        }

        public IntPtr MouseHookProc(int _nCode, IntPtr _wParam, IntPtr _lParam)
        {
            if (_nCode >= 0)
            {
                MouseHook.MouseHookStruct mouseHookStruct = (MouseHook.MouseHookStruct)Marshal.PtrToStructure(_lParam, typeof(MouseHook.MouseHookStruct));

                String strText = "x = " + mouseHookStruct.pt.x.ToString("d") + " : y = " + mouseHookStruct.pt.y.ToString("d");
                this.Text = strText;
            }
            return MouseHook.CallNextHookEx(m_hHook, _nCode, _wParam, _lParam);
        }
    }
}