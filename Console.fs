module SatouinDp.Console

open System.Runtime.InteropServices

[<DllImport(@"kernel32.dll", SetLastError = true)>]
extern [<return:MarshalAs(UnmanagedType.Bool)>] bool private AllocConsole()

let alloc () =
    if not <| AllocConsole () then
        Error(Marshal.GetLastWin32Error())
    else
        Ok()
