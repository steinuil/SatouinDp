module SatouinDp.DisplayDevice


open System
open System.Runtime.InteropServices


[<Flags>]
type State =
    | AttachedToDesktop = 0x1u
    | MultiDriver = 0x2u
    | PrimaryDevice = 0x4u
    | MirroringDriver = 0x8u
    | VgaCompatible = 0x10u
    | Removable = 0x20u
    | Disconnect = 0x2000000u
    | Remote = 0x4000000u
    | ModesPruned = 0x8000000u


[<Struct; StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)>]
type DisplayDevice =
    val mutable private size: uint32

    [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)>]
    val name: string

    [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)>]
    val string: string

    val state: State

    [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)>]
    val id: string

    [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)>]
    val key: string

    static member Empty =
        let mutable dev = DisplayDevice()
        dev.size <- uint32 (Marshal.SizeOf dev)
        dev


 [<Flags>]
type private DeviceSettingsField =
    | PaperOrientation = 0x00000001u
    | PaperSize = 0x00000002u
    | PaperLength = 0x00000004u
    | PaperWidth = 0x00000008u
    | Scale = 0x00000010u
    | Position = 0x00000020u
    | Nup = 0x00000040u
    | DisplayOrientation = 0x00000080u
    | Copies = 0x00000100u
    | DefaultSource = 0x00000200u
    | PrintQuality = 0x00000400u
    | PrintColor = 0x00000800u
    | Duplex = 0x00001000u
    | YResolution = 0x00002000u
    | TTOption = 0x00004000u
    | Collate = 0x00008000u
    | FormName = 0x00010000u
    | LogPixels = 0x00020000u
    | BitsPerPixel = 0x00040000u
    | PixelsWidth = 0x00080000u
    | PixelsHeight = 0x00100000u
    | DisplayFlags = 0x00200000u
    | DisplayFrequency = 0x00400000u
    | IcmMethod = 0x00800000u
    | IcmIntent = 0x01000000u
    | MediaType = 0x02000000u
    | DitherType = 0x04000000u
    | PanningWidth = 0x08000000u
    | PanningHeight = 0x10000000u
    | DisplayFixedOutput = 0x20000000u


[<Struct; StructLayout(LayoutKind.Sequential)>]
type Point =
    val x: int32
    val y: int32

    new(x, y) = { x = x; y = y }


type DisplayOrientation =
    | Default = 0x0u
    | Rotated90 = 0x1u
    | Rotated180 = 0x2u
    | Rotated270 = 0x4u


type FixedOutputMode =
    | Default = 0u
    | Stretch = 1u
    | Center = 2u


[<Flags>]
type DisplayMode =
    | GrayScale = 0x1
    | Interlaced = 0x2


[<Struct; StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)>]
type DisplaySettings =
    [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)>]
    val deviceName: string

    val specVersion: int16
    val driverVersion: int16

    val mutable private size: int16

    val driverExtra: int16

    val mutable private fields: DeviceSettingsField

    val position: Point
    val orientation: DisplayOrientation
    val fixedOutput: FixedOutputMode

    // Printer-related fields
    val private color: int16
    val private duplex: int16
    val private yResolution: int16
    val private ttOption: int16
    val private collate: int16
    [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)>]
    val private formName: string

    val pixelsPerInch: int16
    val colorDepth: int32
    val width: int32
    val height: int32
    val displayMode: DisplayMode
    val displayFrequency: int32

    new(settings: DisplaySettings, position: Point) = {
        deviceName = settings.deviceName
        specVersion = settings.specVersion
        driverVersion = settings.driverVersion
        size = settings.size
        driverExtra = settings.driverExtra
        fields = settings.fields
        position = position
        orientation = settings.orientation
        fixedOutput = settings.fixedOutput
        color = settings.color
        duplex = settings.duplex
        yResolution = settings.yResolution
        ttOption = settings.ttOption
        collate = settings.collate
        formName = settings.formName
        pixelsPerInch = settings.pixelsPerInch
        colorDepth = settings.colorDepth
        width = settings.width
        height = settings.height
        displayMode = settings.displayMode
        displayFrequency = settings.displayFrequency
    }

    static member Empty =
        let mutable mode = DisplaySettings()
        mode.size <- int16 (Marshal.SizeOf mode)
        mode


type ChangeSettingsResult =
    | Successful = 0
    | Restart = 1
    | Failed = -1
    | BadMode = -2
    | NotUpdated = -3
    | BadFlags = -4
    | Badparam = -5
    | BadDualView = -6


module private Native =
    [<Flags; RequireQualifiedAccess>]
    type EDDOptions = None = 0 | GetDeviceInterfaceName = 1

    [<DllImport(@"user32.dll", CharSet = CharSet.Unicode)>]
    extern [<return:MarshalAs(UnmanagedType.Bool)>] bool EnumDisplayDevicesW(
        string lpDevice,
        uint32 iDevNum,
        DisplayDevice& lpDisplayDevice,
        EDDOptions dwFlags
    )

    type EDSMode =
        | CurrentSettings = -1
        | RegistrySettings = -2

    [<Flags; RequireQualifiedAccess>]
    type EDSOptions = None = 0 | RawMode = 2 | RotateMode = 4

    [<DllImport(@"user32.dll", CharSet = CharSet.Unicode)>]
    extern [<return:MarshalAs(UnmanagedType.Bool)>] bool EnumDisplaySettingsExW(
        string lpszDeviceName,
        EDSMode iModeNum,
        DisplaySettings& lpDevMode,
        EDSOptions dwFlags
    )

    [<Flags; RequireQualifiedAccess>]
    type CDSOptions =
        | None = 0
        | UpdateRegistry = 0x1
        | Test = 0x2
        | Fullscreen = 0x4
        | Global = 0x8
        | SetPrimary = 0x10
        | VideoParameters = 0x20
        | EnableUnsafeModes = 0x100
        | DisableUnsafeModes = 0x200
        | Reset = 0x40000000
        | ResetEx = 0x20000000
        | NoReset = 0x10000000

    [<DllImport(@"user32.dll", CharSet = CharSet.Unicode)>]
    extern ChangeSettingsResult ChangeDisplaySettingsExW(
        string lpszDeviceName,
        DisplaySettings& lpDevMode,
        IntPtr hwnd, // always NULL
        CDSOptions dwflags,
        IntPtr lParam
    )

    [<DllImport(@"user32.dll", CharSet = CharSet.Unicode, EntryPoint = "ChangeDisplaySettingsExW")>]
    extern ChangeSettingsResult ChangeDisplaySettingsExWNull(
        string lpszDeviceName,
        IntPtr lpDevMode,
        IntPtr hwnd, // always NULL
        CDSOptions dwflags,
        IntPtr lParam
    )



let queryAll () =
    0 |> Seq.unfold (fun devNum ->
        let mutable dev = DisplayDevice.Empty

        if Native.EnumDisplayDevicesW(null, uint32 devNum, &dev, Native.EDDOptions.None)
        then Some(dev, devNum + 1)
        else None
    )


let queryDisplayName devName =
    let mutable dev = DisplayDevice.Empty

    if Native.EnumDisplayDevicesW(devName, 0u, &dev, Native.EDDOptions.None)
    then Some(dev)
    else None


let querySettings devName =
    let mutable mode = DisplaySettings.Empty

    if Native.EnumDisplaySettingsExW(devName, Native.EDSMode.CurrentSettings,
                                     &mode, Native.EDSOptions.None)
    then Some(mode)
    else None


let setPrimary name =
    let settings = querySettings name |> Option.get

    let offsetX = settings.position.x
    let offsetY = settings.position.y

    let mutable newSettings = DisplaySettings(settings, Point(0, 0))

    Native.ChangeDisplaySettingsExW(
        name,
        &newSettings,
        IntPtr.Zero,
        Native.CDSOptions.SetPrimary ||| Native.CDSOptions.UpdateRegistry ||| Native.CDSOptions.NoReset,
        IntPtr.Zero
    ) |> ignore

    let otherDisplays =
        queryAll()
        |> Seq.filter (fun d -> d.name <> name && d.state.HasFlag State.AttachedToDesktop)

    for display in otherDisplays do
        let settings = querySettings display.name |> Option.get
        let newPosition = Point(settings.position.x - offsetX, settings.position.y - offsetY)
        let mutable newSettings = DisplaySettings(settings, newPosition)

        Native.ChangeDisplaySettingsExW(
            display.name,
            &newSettings,
            IntPtr.Zero,
            Native.CDSOptions.UpdateRegistry ||| Native.CDSOptions.NoReset,
            IntPtr.Zero
        ) |> ignore

    Native.ChangeDisplaySettingsExWNull(
        null,
        IntPtr.Zero,
        IntPtr.Zero,
        Native.CDSOptions.None,
        IntPtr.Zero
    )
