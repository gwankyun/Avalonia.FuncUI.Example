namespace CounterApp

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Controls.Primitives
// open Primitives
open Common

module Main =
    let jsonPath =
        let baseDir = Directory.baseDir
        Path.join baseDir "data.json"

    let view () =
        Component(fun ctx ->
            let list: IWritable<VSCode.Extension list> = ctx.useState []
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
            let newItem : VSCode.Extension =  { Name = ""; Version = ""; IsX64 = false }

            TabControl.create [
                TabControl.tabStripPlacement Dock.Left
                TabControl.viewItems [
                    TabItem.create [
                        TabItem.header "VSCode"
                        TabItem.content (
                            StackPanel.create [
                                StackPanel.children [
                                    StackPanel.create [
                                        StackPanel.orientation Orientation.Horizontal
                                        StackPanel.children [
                                            Button.create [
                                                Button.content "新增"
                                                Button.onClick (fun _ ->
                                                    list.Set <| newItem :: list.Current)
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
                                    ListBox.create [
                                        ListBox.verticalScrollBarVisibility ScrollBarVisibility.Visible
                                        ListBox.horizontalScrollBarVisibility ScrollBarVisibility.Visible
                                        ListBox.horizontalAlignment HorizontalAlignment.Stretch
                                        ListBox.verticalAlignment VerticalAlignment.Stretch
                                        ListBox.viewItems (
                                            list.Current
                                            |> List.map (VSCode.stack list result)
                                        )
                                    ]
                                ]
                            ]
                        )
                    ]
                    TabItem.create [
                        TabItem.header "DataGrid"
                        TabItem.content (
                            DataGrid.create [
                                // DataGrid.con
                                // datagridco
                                // DataGrid.c
                            ]
                        )
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
