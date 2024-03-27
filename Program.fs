namespace CounterApp

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open TextCopy
open Common

module Main =
    type Extension =
        {
            Name: string
            Version: string
            IsX64: bool
        }

    let stack (list: IWritable<Extension list>) (i: Extension) (result: IWritable<string>) =
        Component.create (i.Name, fun ctx ->
            let text = ctx.useState i.Name
            let version = ctx.useState i.Version
            let isX64 = ctx.useState i.IsX64
            let currentItem () =
                { Name = text.Current; Version = version.Current; IsX64 = isX64.Current }
            let save () =
                let current = list.Current
                let idx = List.tryFindIndex (fun x -> x.Name = i.Name) current
                match idx with
                | Some(index) ->
                    list.Set (List.updateAt index (currentItem ()) current)
                | None -> ()
            StackPanel.create [
                StackPanel.orientation Orientation.Horizontal
                StackPanel.onLostFocus (fun _ -> save ()) // 失焦就保存
                StackPanel.children [
                    TextBox.create [
                        TextBox.text text.Current
                        TextBox.width 400
                        TextBox.onTextChanged (fun t -> text.Set t)
                    ]
                    TextBox.create [
                        TextBox.text version.Current
                        TextBox.width 200
                        TextBox.onTextChanged (fun t -> version.Set t)
                    ]
                    ToggleSwitch.create [
                        ToggleSwitch.isChecked isX64.Current
                        ToggleSwitch.onChecked (fun _ -> isX64.Set true)
                        ToggleSwitch.onUnchecked (fun _ -> isX64.Set false)
                    ]
                    Button.create [
                        Button.content "刪除"
                        Button.onClick (fun _ ->
                            list.Set (List.filter (fun j -> j.Name <> i.Name) list.Current))
                    ]
                    Button.create [
                        Button.content "更新"
                        Button.onClick (fun _ -> save ())
                    ]
                    Button.create [
                        Button.content "生成"
                        Button.onClick (fun _ ->
                            let testArray = text.Current |> String.split "."
                            let url =
                                let host = "https://marketplace.visualstudio.com/_apis/public/gallery/publishers"
                                sprintf "%s/%s/vsextensions/%s/%s/vspackage" host testArray[0] testArray[1] version.Current
                            let result_ =
                                match isX64.Current with
                                | true ->
                                    sprintf "%s?targetPlatform=win32-x64" url
                                | false -> sprintf "%s" url
                            result.Set result_
                            ClipboardService.SetText result_
                        )
                    ]
                ]
            ]
        )

    let jsonPath =
        let currentDir = Directory.current
        Path.join currentDir "data.json"

    let view () =
        Component(fun ctx ->
            let list: IWritable<Extension list> = ctx.useState []
            let result = ctx.useState ""
            let saveJson () =
                let json = JsonSerializer.serialize list.Current
                File.writeAllText jsonPath json
            ctx.useEffect (
                (fun _ ->
                    if File.exists jsonPath |> not then
                        saveJson ()
                    let json = File.readAllText jsonPath
                    list.Set <| JsonSerializer.deserialize json),
                    [EffectTrigger.AfterInit]
            )

            StackPanel.create [
                StackPanel.children [
                    ListBox.create [
                        ListBox.viewItems (
                            list.Current |> List.map (fun i -> stack list i result)
                        )
                    ]
                    StackPanel.create [
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.children [
                            Button.create [
                                Button.content "新增"
                                Button.onClick (fun _ ->
                                    list.Set <| { Name = ""; Version = ""; IsX64 = false } :: list.Current)
                            ]
                            Button.create [
                                Button.content "保存"
                                Button.onClick (fun _ ->
                                    saveJson ())
                            ]
                            TextBox.create [
                                TextBox.text result.Current
                                TextBox.width 1000
                            ]
                        ]
                    ]
                ]
            ]
        )

type MainWindow() =
    inherit HostWindow()
    do
        base.Title <- "Counter Example"
        base.Content <- Main.view ()

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
