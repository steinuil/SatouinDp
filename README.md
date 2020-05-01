# SatouinDp

The greatest tool to ever exist. Sits in your taskbar and gives you the power
to switch primary display in two clicks.

![Screenshot of SatouinDp in action](./screenshot.jpg)

I think I can consider this feature complete. Unless there's bugs expect no further development.

> How do I run this on startup?

Go to `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup` and drop the exe there.
Or just a shortcut if you prefer.

> Why is the exe file so big?

Because it bundles the .NET Core runtime and all the libraries. Those things are pretty big.
You can compile it yourself easily enough if you don't trust the releases.

> Why are the display names not the same as the display numbers in the control panel?

Hell if I know. If you know of an easy way to get the display numbers like they are shown there
I could fix this, but otherwise it's fine as it is.

## Download

[Download it from here](https://github.com/steinuil/SatouinDp/releases/latest).

## Running and building

SatouinDp requires the [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/).

To run:

```powershell
dotnet run
```

To build a self-contained executable:

```powershell
dotnet publish -c Release -r win-x64 --self-contained /p:PublishSingleFile=true -o out
```

Could also serve as reference to somebody implementing something more substantial
handling displays and resolutions in F#. The code's [unlicensed](https://unlicense.org/)
so feel free to take it.
