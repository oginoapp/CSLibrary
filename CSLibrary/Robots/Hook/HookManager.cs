using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLibrary.Robots.Hook
{
    public static class HookManager
    {
        #region イベント

        public delegate void MouseHookEventHandler(MouseHookEventArgs e);
        public delegate void KeyHookEventHandler(KeyHookEventArgs e);

        [Description("Move イベント")]
        public static event MouseHookEventHandler MouseMove;

        [Description("MouseDown イベント")]
        public static event MouseHookEventHandler MouseDown;

        [Description("MouseUp イベント")]
        public static event MouseHookEventHandler MouseUp;

        [Description("MouseWheel イベント")]
        public static event MouseHookEventHandler MouseWheel;

        [Description("KeyDown イベント")]
        public static event KeyHookEventHandler KeyDown;

        [Description("KeyUp イベント")]
        public static event KeyHookEventHandler KeyUp;

        #endregion

        #region 変数

        //マウス
        private const int HC_ACTION = 0;
        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MOUSEWHEEL = 0x20a;

        //キー
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEY_DOWN = 0x100;
        private const int WM_KEY_UP = 0x101;
        private const int WM_SYSKEY_DOWN = 0x104;
        private const int WM_SYSKEY_UP = 0x105;

        #endregion

        #region Core

        //フラグ
        private static int mouseHookId = 0;
        private static int keyHookId = 0;

        //押下中のキー
        private static List<Keys> pressingKeys = null;

        //コールバック
        private delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static HookProc hookCallBack;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// マウスフック情報の構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MouseHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        /// <summary>
        /// キーフック情報の構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct KeyHookStruct
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

        #region コールバック

        /// <summary>
        /// ローレベルのマウスイベント関数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private static int MouseActionEvent(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseHookStruct hookInfo = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            //値の取得
            int action = wParam.ToInt32();
            Point pos = new Point(hookInfo.pt.x, hookInfo.pt.y);

            //イベント発生
            switch (action)
            {
                case WM_MOUSEMOVE:
                    if (MouseMove != null) MouseMove(new Hook.MouseHookEventArgs(pos, MouseButtons.None));
                    break;
                case WM_LBUTTONDOWN:
                    if (MouseDown != null) MouseDown(new Hook.MouseHookEventArgs(pos, MouseButtons.Left));
                    break;
                case WM_MBUTTONDOWN:
                    if (MouseDown != null) MouseDown(new Hook.MouseHookEventArgs(pos, MouseButtons.Middle));
                    break;
                case WM_RBUTTONDOWN:
                    if (MouseDown != null) MouseDown(new Hook.MouseHookEventArgs(pos, MouseButtons.Right));
                    break;
                case WM_LBUTTONUP:
                    if (MouseUp != null) MouseUp(new Hook.MouseHookEventArgs(pos, MouseButtons.Left));
                    break;
                case WM_MBUTTONUP:
                    if (MouseUp != null) MouseUp(new Hook.MouseHookEventArgs(pos, MouseButtons.Middle));
                    break;
                case WM_RBUTTONUP:
                    if (MouseUp != null) MouseUp(new Hook.MouseHookEventArgs(pos, MouseButtons.Right));
                    break;
                case WM_MOUSEWHEEL:
                    if (MouseWheel != null) MouseWheel(new Hook.MouseHookEventArgs(pos, MouseButtons.None));
                    break;
            }


            return CallNextHookEx(mouseHookId, nCode, wParam, lParam);
        }

        /// <summary>
        /// ローレベルのキーイベント関数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private static int KeyActionEvent(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KeyHookStruct hookInfo = (KeyHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyHookStruct));

            //値の取得
            int action = wParam.ToInt32();
            Keys key = (Keys)hookInfo.vkCode;

            //イベント発生
            lock (pressingKeys)
            {
                switch (action)
                {
                    case WM_KEY_DOWN:
                    case WM_SYSKEY_DOWN:
                        if (!pressingKeys.Contains(key)) pressingKeys.Add(key);
                        if (KeyDown != null) KeyDown(new Hook.KeyHookEventArgs(key, pressingKeys));
                        break;
                    case WM_KEY_UP:
                    case WM_SYSKEY_UP:
                        if (pressingKeys.Contains(key)) pressingKeys.Remove(key);
                        if (KeyUp != null) KeyUp(new Hook.KeyHookEventArgs(key, pressingKeys));
                        break;
                }
            }

            return CallNextHookEx(keyHookId, nCode, wParam, lParam);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// マウスフックを開始する
        /// </summary>
        public static void StartMouseHook()
        {
            hookCallBack = new HookProc(MouseActionEvent);
            mouseHookId = SetWindowsHookEx(WH_MOUSE_LL,
                        hookCallBack,
                        GetModuleHandle(null),
                        0);
        }

        /// <summary>
        /// キーフックを開始する
        /// </summary>
        public static void StartKeyHook()
        {
            pressingKeys = new List<Keys>();
            hookCallBack = new HookProc(KeyActionEvent);
            mouseHookId = SetWindowsHookEx(WH_KEYBOARD_LL,
                        hookCallBack,
                        GetModuleHandle(null),
                        0);
        }

        /// <summary>
        /// マウスフックを解除する
        /// </summary>
        public static void StopMouseHook()
        {
            UnhookWindowsHookEx(mouseHookId);
            mouseHookId = 0;
        }

        /// <summary>
        /// キーフックを解除する
        /// </summary>
        public static void StopKeyHook()
        {
            UnhookWindowsHookEx(keyHookId);
            mouseHookId = 0;
            if(pressingKeys != null) pressingKeys.Clear();
        }

        #endregion
    }
}
