namespace CounterApp

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open System.Text.Json
open System.IO
open TextCopy

module Main =
    type Extension =
        {
            Name: string
            Version: string
            IsX64: bool
        }

    module File =
        let exists path =
            File.Exists(path)

        let writeAllText path contents =
            File.WriteAllText(path, contents)

        let readAllText path =
            File.ReadAllText(path)

    module String =
        let split (separator: string) (str: string) =
            str.Split(separator)

    let stack (list: IWritable<Extension list>) (i: Extension) (result: IWritable<string>) =
        Component.create (i.Name, fun ctx ->
            let text = ctx.useState i.Name
            let version = ctx.useState i.Version
            let isX64 = ctx.useState i.IsX64
            StackPanel.create [
                StackPanel.orientation Orientation.Horizontal
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
                        Button.onClick (fun _ ->
                            let current = list.Current
                            let idx = List.findIndex (fun x -> x.Name = i.Name) current
                            list.Set (List.updateAt idx
                                { Name = text.Current; Version = version.Current; IsX64 = isX64.Current }
                                current))
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
        let currentDir = Directory.GetCurrentDirectory()
        Path.Join(currentDir, "data.json")

    let view () =
        Component(fun ctx ->
            let state = ctx.useState 0
            let list: IWritable<Extension list> = ctx.useState []
            // let id = ctx.useState 0
            let result = ctx.useState ""
            ctx.useEffect (
                (fun _ ->
                    if File.exists jsonPath |> not then
                        let json = JsonSerializer.Serialize(list.Current)
                        File.writeAllText jsonPath json
                    let json = File.readAllText jsonPath
                    list.Set (JsonSerializer.Deserialize<Extension list>(json))),
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
                                    list.Set ({ Name = ""; Version = ""; IsX64 = false } :: list.Current))
                            ]
                            Button.create [
                                Button.content "保存"
                                Button.onClick (fun _ ->
                                    let json = JsonSerializer.Serialize(list.Current)
                                    File.writeAllText jsonPath json)
                            ]
                        ]
                    ]
                    StackPanel.create [
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.children [
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
