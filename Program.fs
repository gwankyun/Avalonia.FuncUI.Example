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
open Avalonia.Controls.ApplicationLifetimes
open System.Collections.ObjectModel
open Avalonia.Data
// open Primitives
open Common

type Person (name, age, male) =
    member val Name = name with get, set
    member val Age = age with get, set
    member val IsMale = male with get, set



module Main =
    let gridComponent () =
        Component.create ("gridComponent", fun ctx ->
            let data = ctx.useState (
                        ObservableCollection [
                            Person("John", 20, true)
                            Person("Jane", 21, false)
                            Person("Bob", 22, true)
                        ]
                    )
                    
            let selectedItem = ctx.useState None

            DockPanel.create [
                DockPanel.children [
                    TextBlock.create [
                        TextBlock.dock Dock.Top
                        TextBlock.margin 10
                        TextBlock.text $"""Selected: {(selectedItem.Current |> Option.defaultValue (Person("", 0, false))).Name}"""
                    ]
                    DataGrid.create [
                        DataGrid.dock Dock.Top
                        DataGrid.isReadOnly false
                        DataGrid.items data.Current
                        DataGrid.onSelectedItemChanged (fun item ->
                            (match box item with
                            | null -> None
                            | :? Person as i -> Some i
                            | _ -> failwith "Something went horribly wrong!")
                            |> selectedItem.Set)

                        DataGrid.columns [
                            DataGridTextColumn.create [
                                DataGridTextColumn.header "Name"
                                DataGridTextColumn.binding (Binding ("Name", BindingMode.TwoWay))
                                DataGridTextColumn.width (DataGridLength(2, DataGridLengthUnitType.Star))
                            ]
                            DataGridTemplateColumn.create [
                                DataGridTemplateColumn.header "Name"
                                DataGridTemplateColumn.cellTemplate (
                                    DataTemplateView<_>.create (fun (data: Person) ->
                                        TextBlock.create [
                                            TextBlock.text data.Name
                                        ]
                                    )
                                )
                                DataGridTemplateColumn.cellEditingTemplate (
                                    DataTemplateView<_>.create (fun (data: Person) ->
                                        TextBox.create [
                                            TextBox.init (fun t ->
                                                t.Bind(TextBox.TextProperty, Binding("Name", BindingMode.TwoWay)) |> ignore
                                            )
                                        ]
                                    )
                                )
                            ]
                            DataGridTextColumn.create [
                                DataGridTextColumn.header "Age"
                                DataGridTextColumn.binding (Binding "Age")
                            ]
                            DataGridCheckBoxColumn.create [
                                DataGridCheckBoxColumn.header "IsMale"
                                DataGridCheckBoxColumn.binding (Binding "IsMale")
                            ]
                        ]
                    ]
                ]
            ]
        )

    let view () =
        Component(fun ctx ->
            TabControl.create [
                TabControl.tabStripPlacement Dock.Left
                TabControl.viewItems [
                    TabItem.create [
                        TabItem.header "VSCode"
                        TabItem.content (
                            VSCode.vscodeComponet()
                        )
                    ]
                    TabItem.create [
                        TabItem.header "DataGrid"
                        TabItem.content (
                            gridComponent()
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
