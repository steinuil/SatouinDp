﻿module SatouinDp.Main

open System
open System.Windows.Forms
open Microsoft.Win32


[<EntryPoint; STAThread>]
let main _ =
    // Console.alloc() |> ignore

    Application.EnableVisualStyles()

    let ctx = new ApplicationContext()

    let menu = new ContextMenuStrip()

    let mutable displayMenuItems = []

    let rec update () =
        displayMenuItems |> Seq.iter menu.Items.Remove

        displayMenuItems <-
            DisplayDevice.queryAll()
            |> Seq.filter (fun d -> d.state.HasFlag DisplayDevice.State.AttachedToDesktop)
            |> Seq.map (fun display ->
                let settings = DisplayDevice.querySettings display.name |> Option.get
                let item =
                    new ToolStripMenuItem(
                        Text = sprintf "%s %dx%d@%dhz"
                            display.name settings.width
                            settings.height settings.displayFrequency
                    )
    
                let isPrimary = display.state.HasFlag DisplayDevice.State.PrimaryDevice
                if isPrimary then
                    item.Checked <- true
                else
                    item.Click.Add (fun _ ->
                        DisplayDevice.setPrimary display.name |> ignore
                        update ()
                    )
    
                item
            )
            |> Seq.toList

        displayMenuItems |> Seq.iteri (fun i item ->
            menu.Items.Insert(i, item)
        )

    update ()

    new ToolStripSeparator() |> menu.Items.Add |> ignore

    let refreshItem = new ToolStripMenuItem(Text = "&Refresh")
    refreshItem.Click.Add (fun _ -> update ())
    menu.Items.Add refreshItem |> ignore

    let exitItem = new ToolStripMenuItem(Text = "&Exit")
    exitItem.Click.Add (fun _ -> Application.Exit())
    menu.Items.Add exitItem |> ignore

    SystemEvents.DisplaySettingsChanged.Add (fun _ -> update ())

    let notifyIcon =
        new NotifyIcon(
            Text = "SatouinDp",
            ContextMenuStrip = menu,
            Visible = true,
            Icon = AppIcon.get()
        )

    ctx.ThreadExit.Add (fun _ ->
        notifyIcon.Icon.Dispose()
        notifyIcon.Dispose()
        menu.Dispose()
    )

    Application.Run ctx

    0
