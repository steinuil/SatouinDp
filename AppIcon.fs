module SatouinDp.AppIcon

open System
open System.Drawing

let get =
    let assembly = Reflection.Assembly.GetExecutingAssembly()
    let name = assembly.GetName().Name + ".Resources.icon.ico"
    fun () ->
        new Icon(assembly.GetManifestResourceStream(name))
