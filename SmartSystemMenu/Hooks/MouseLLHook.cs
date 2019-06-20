﻿using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SmartSystemMenu.Hooks
{
    class MouseLLHook : Hook
    {
        private int msgID_MouseLL;
        private int msgID_MouseLL_HookReplaced;

        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_RBUTTONDBLCLK = 0x0206;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MBUTTONUP = 0x0208;
        private const int WM_MBUTTONDBLCLK = 0x0209;
        private const int WM_MOUSEWHEEL = 0x020A;

        public event EventHandler<EventArgs> HookReplaced;
        public event EventHandler<BasicHookEventArgs> MouseLLEvent;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseUp;

        struct MSLLHOOKSTRUCT
        {
            #pragma warning disable 0649

            public System.Drawing.Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;

            #pragma warning restore 0649
        };

        public MouseLLHook(IntPtr handle) : base(handle)
        {
        }

        protected override void OnStart()
        {
            msgID_MouseLL = NativeMethods.RegisterWindowMessage("SMART_SYSTEM_MENU_HOOK_MOUSELL");
            msgID_MouseLL_HookReplaced = NativeMethods.RegisterWindowMessage("SMART_SYSTEM_MENU_HOOK_MOUSELL_REPLACED");

            if (Environment.OSVersion.Version.Major >= 6)
            {
                NativeMethods.ChangeWindowMessageFilter(msgID_MouseLL, NativeConstants.MSGFLT_ADD);
                NativeMethods.ChangeWindowMessageFilter(msgID_MouseLL_HookReplaced, NativeConstants.MSGFLT_ADD);
            }
            NativeHookMethods.InitializeMouseLLHook(0, handle);
        }

        protected override void OnStop()
        {
            NativeHookMethods.UninitializeMouseLLHook();
        }

        public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == msgID_MouseLL)
            {
                RaiseEvent(MouseLLEvent, new BasicHookEventArgs(m.WParam, m.LParam));

                MSLLHOOKSTRUCT M = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(m.LParam, typeof(MSLLHOOKSTRUCT));
                if (m.WParam.ToInt64() == WM_MOUSEMOVE)
                {
                    RaiseEvent(MouseMove, new MouseEventArgs(MouseButtons.None, 0, M.pt.X, M.pt.Y, 0));
                }
                else if (m.WParam.ToInt64() == WM_LBUTTONDOWN)
                {
                    RaiseEvent(MouseDown, new MouseEventArgs(MouseButtons.Left, 0, M.pt.X, M.pt.Y, 0));
                }
                else if (m.WParam.ToInt64() == WM_RBUTTONDOWN)
                {
                    RaiseEvent(MouseDown, new MouseEventArgs(MouseButtons.Right, 0, M.pt.X, M.pt.Y, 0));
                }
                else if (m.WParam.ToInt64() == WM_LBUTTONUP)
                {
                    RaiseEvent(MouseUp, new MouseEventArgs(MouseButtons.Left, 0, M.pt.X, M.pt.Y, 0));
                }
                else if (m.WParam.ToInt64() == WM_RBUTTONUP)
                {
                    RaiseEvent(MouseUp, new MouseEventArgs(MouseButtons.Right, 0, M.pt.X, M.pt.Y, 0));
                }
            }
            else if (m.Msg == msgID_MouseLL_HookReplaced)
            {
                RaiseEvent(HookReplaced, EventArgs.Empty);
            }
        }
    }
}
