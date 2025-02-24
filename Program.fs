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
open TextCopy
open FSLogger

type Person (name, age, male) =
    member val Name = name with get, set
    member val Age = age with get, set
    member val IsMale = male with get, set

module ComponentExample =

    type ColorViews () =

        static let random = System.Random()

        static let randomColor () =
            System.String.Format("#{0:X6}", random.Next(0x1000000))

        static member randomColorView () =
            Component.create ("randomColorView", fun ctx ->
                let color = ctx.useState (randomColor())
                
                TextBlock.create [
                    TextBlock.background color.Current
                    TextBlock.text $"Color {color.Current}"
                ]
            )

        static member mainView () =
            Component (fun ctx ->
                let isHidden = ctx.useState false

                DockPanel.create [
                    DockPanel.lastChildFill true
                    DockPanel.children [

                        Button.create [
                            Button.content "hide button"
                            Button.onClick (fun _ -> isHidden.Set true)
                            Button.isVisible (not isHidden.Current)
                        ]

                        ColorViews.randomColorView ()
                    ]
                ]
            )

// module HookExample =
//     let ComponentA id (value: IWritable<string>) =
//         Component.create (id, fun ctx ->
//             // Right here we can use ctx.usePassed to ensure we can both read/update a value
//             let value = ctx.usePassed value
//             StackPanel.create [
//                 StackPanel.children [
//                     TextBlock.create [
//                         TextBlock.text $"This component can read and update this value: \"{value.Current}\""
//                     ]
//                     Button.create [
//                         Button.content "Add 3"
//                         Button.onClick (fun _ -> $"{value.Current}3" |> value.Set )
//                     ]
//                 ]
//             ]
//         )

//     let ComponentB id (value: IReadable<string>) =
//         Component.create (id, fun ctx ->
//             // Right here we can use ctx.usePassedRead to ensure we can only read a value
//             let value = ctx.usePassedRead value
//             StackPanel.create [
//                 StackPanel.children [
//                     TextBlock.create [
//                         TextBlock.text $"This component can only read this value: \"{value.Current}\""
//                     ]
//                 ]
//             ]
//         )

//     let View =
//         Component(fun ctx ->
//             let value = ctx.useState "This is my value"
//             StackPanel.create [
//                 StackPanel.spacing 12.
//                 StackPanel.children [
//                     // here we can use our components with the existing value
//                     // in other implementations these could also be called
//                     // "Stores"
//                     ComponentA "component-a" value
//                     ComponentB "component-b" value
//                     Button.create [
//                         Button.content "I can add 4 from outside"
//                         // since state is both readable and writable we can also
//                         // modify the values from outside the child components
//                         // as usual
//                         Button.onClick (fun _ -> $"{value.Current}4" |> value.Set )
//                     ]
//                 ]
//             ]
//         )

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

    let itemView (k: string) : Types.IView =
        Component.create (k, fun ctx->
            let text = ctx.useState k
            let version = ctx.useState k
            StackPanel.create [
                StackPanel.orientation Orientation.Horizontal
                StackPanel.spacing 5
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
                        ToggleSwitch.isChecked true
                    ]
                    Button.create [
                        Button.content "生成"
                        Button.onClick (fun _ ->
                            ClipboardService.SetText (text.Current + "." + version.Current))
                    ]
                ]
            ]
        )

    let view () =
        let panelChildren: Types.IView List = [
            TextBox.create [
                TextBox.text "1"
                TextBox.dock Dock.Top
            ]
            TextBox.create [
                TextBox.text "2"
                TextBox.dock Dock.Top
            ]
            TextBox.create [
                TextBox.text "3"
                TextBox.dock Dock.Top
            ]
            TextBox.create [
                TextBox.text "4"
                TextBox.dock Dock.Top
            ]
            TextBox.create [
                TextBox.text "5"
                TextBox.dock Dock.Top
            ]
            TextBox.create [
                TextBox.text "6"
                TextBox.dock Dock.Top
            ]
            TextBox.create [
                TextBox.text "7"
                TextBox.dock Dock.Top
            ]
            TextBox.create [
                TextBox.text "8"
                TextBox.dock Dock.Top
            ]
            TextBox.create [
                TextBox.text "9"
                TextBox.dock Dock.Top
            ]
        ]
        let panel = 
            TabItem.create [
                TabItem.header "佈局"
                TabItem.content (
                    TabControl.create [
                        TabControl.tabStripPlacement Dock.Top
                        TabControl.viewItems [
                            TabItem.create [
                                TabItem.header "DockPanel"
                                TabItem.content (
                                    DockPanel.create [
                                        DockPanel.children panelChildren
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "StackPanel"
                                TabItem.content (
                                    StackPanel.create [
                                        StackPanel.children panelChildren
                                    ]
                                )
                            ]
                        ]
                    ]
                )
            ]
        let listManeger =
            TabItem.create [
                TabItem.header "ListBox"
                TabItem.content (
                    DockPanel.create [
                        // DockPanel.orientation Orientation.Vertical
                        DockPanel.dock Dock.Top
                        DockPanel.children [
                            ListBox.create [
                                ListBox.viewItems (
                                    [ 1..10 ]
                                    |> List.map (fun i ->
                                        // itemView(string i)
                                        let k = string i
                                        Component.create (k, fun ctx->
                                            let text = ctx.useState k
                                            // let version = ctx.useState k
                                            StackPanel.create [
                                                StackPanel.orientation Orientation.Horizontal
                                                StackPanel.spacing 5
                                                StackPanel.children [
                                                    TextBox.create [
                                                        TextBox.text text.Current
                                                        TextBox.width 400
                                                        TextBox.onTextChanged (fun t -> text.Set t)
                                                    ]
                                                    // TextBox.create [
                                                    //     TextBox.text version.Current
                                                    //     TextBox.width 200
                                                    //     TextBox.onTextChanged (fun t -> version.Set t)
                                                    // ]
                                                    // ToggleSwitch.create [
                                                    //     ToggleSwitch.isChecked true
                                                    // ]
                                                    // Button.create [
                                                    //     Button.content "生成"
                                                    //     Button.onClick (fun _ ->
                                                    //         ClipboardService.SetText (text.Current + "." + version.Current))
                                                    // ]
                                                ]
                                            ]
                                        )
                                    )
                                )
                            ]
                        ]
                    ]
                )
            ]
        let customControl =
            Component.create ("custom", fun ctx ->
                let state = ctx.useState 0
                DockPanel.create [
                    DockPanel.children [
                        Button.create [
                            Button.onClick (fun _ ->
                                let current = state.Current + 1
                                state.Set current)
                            Button.content "+"
                        ]
                        TextBlock.create [
                            TextBlock.text (string state.Current)
                        ]
                    ]
                ]
            )
        Component(fun ctx ->
            let customState = ctx.useState 0
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
                    TabItem.create [
                        TabItem.header "ListBox"
                        TabItem.content (
                            DockPanel.create [
                                // DockPanel.orientation Orientation.Vertical
                                DockPanel.dock Dock.Top
                                DockPanel.children [
                                    ListBox.create [
                                        ListBox.viewItems (
                                            [ 1..50 ]
                                            |> List.map (fun i ->
                                                itemView(string i)
                                            )
                                        )
                                    ]
                                ]
                            ]
                        )
                    ]
                    panel
                    TabItem.create [
                        TabItem.header "控件"
                        TabItem.content (
                            ControlExample.controlView()
                        )
                    ]
                    listManeger
                    TabItem.create [
                        TabItem.header "自定義控件"
                        TabItem.content (
                            customControl
                        )
                    ]
                    TabItem.create [
                        TabItem.header "組件生命週期"
                        TabItem.content (
                            ComponentExample.ColorViews.mainView()
                        )
                    ]
                ]
            ]
        )

    let otherView (n: int) =
        Component(fun ctx ->
            let state = ctx.useState n
            let current = state.Current
            DockPanel.create [
                DockPanel.children [
                    Button.create [
                        Button.onClick (fun _ -> state.Set (state.Current + 1))
                        Button.content "+"
                    ]
                    TextBlock.create [
                        TextBlock.text (string state.Current)
                    ]
                ]
            ]
        )

type MainWindow() =
    inherit HostWindow()
    do
        base.Title <- "Counter Example"
        base.Content <- Main.view ()
        // base.Content <- Main.otherView (0)

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add <| FluentTheme()
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        let logger = Logger.ColorConsole
        logger.I "info"
        logger.W "warn"
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
