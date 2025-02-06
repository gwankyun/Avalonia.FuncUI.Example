
namespace CounterApp

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Controls.Primitives
open TextCopy
open Common

module ControlExample =
    let controlView () =
        Component(fun ctx ->
            let progressBar = ctx.useState 0.0
            let slider = ctx.useState 50.0
            let menu =
                Menu.create [
                    Menu.viewItems [
                        MenuItem.create [
                            MenuItem.header "文件"
                            MenuItem.viewItems [
                                MenuItem.create [
                                    MenuItem.header "新建文件"
                                ]
                                MenuItem.create [
                                    MenuItem.header "打開文件"
                                ]
                            ]
                        ]
                        MenuItem.create [
                            MenuItem.header "編輯"
                            MenuItem.viewItems [
                                MenuItem.create [
                                    MenuItem.header "剪切"
                                ]
                                MenuItem.create [
                                    MenuItem.header "複製"
                                ]
                                MenuItem.create [
                                    MenuItem.header "黏貼"
                                ]
                            ]
                        ]
                    ]
                ]
            StackPanel.create [
                StackPanel.orientation Orientation.Horizontal
                StackPanel.children [
                    TabControl.create [
                        TabControl.tabStripPlacement Dock.Left
                        TabControl.viewItems [
                            TabItem.create [
                                TabItem.header "Button"
                                TabItem.content (
                                    Button.create [
                                        Button.content "Button"
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "Border"
                                TabItem.content (
                                    Border.create [
                                        Border.background "blue"
                                        Border.borderThickness 1
                                        Border.borderBrush "green"
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "Calendar"
                                TabItem.content (
                                    Calendar.create [
                                        Calendar.selectedDate System.DateTime.Today
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "CalendarDatePicker"
                                TabItem.content (
                                    CalendarDatePicker.create [
                                        CalendarDatePicker.selectedDate System.DateTime.Today
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "CheckBox"
                                TabItem.content (
                                    CheckBox.create [
                                        CheckBox.content "CheckBox"
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "DatePicker"
                                TabItem.content (
                                    DatePicker.create [
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "Expander"
                                TabItem.content (
                                    Expander.create [
                                        Expander.header "Expander"
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "NumericUpDown"
                                TabItem.content (
                                    NumericUpDown.create [
                                        NumericUpDown.minimum 0
                                        NumericUpDown.maxHeight 10
                                        NumericUpDown.increment 1
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "ProgressBar"
                                TabItem.content (
                                    ProgressBar.create [
                                        // ProgressBar.value progressBar.Current
                                        ProgressBar.value 50.
                                        ProgressBar.maximum 100.
                                        // ProgressBar.onValueChanged (fun x -> ())
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "RadioButton"
                                TabItem.content (
                                    RadioButton.create [
                                        RadioButton.content "RadioButton"
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "RepeatButton"
                                TabItem.content (
                                    RepeatButton.create [
                                        RepeatButton.content "RepeatButton"
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "Slider"
                                TabItem.content (
                                    Slider.create [
                                        // Slider.minimum (double 0)
                                        // Slider.maximum (double 100)
                                        // Slider.largeChange 100.0
                                        // Slider.smallChange 100.0
                                        // Slider.tickFrequency (double 1)
                                        // Slider.isSnapToTickEnabled true
                                        // Slider.minimum 0.
                                        // Slider.pers
                                        // Slider.maximum 100.
                                        // Slider.value slider.Current
                                        // Slider.onValueChanged (fun n ->
                                        //     printfn "Slider: %f" n
                                        //     slider.Set n)
                                    //     // Slider.smallChange 1.0
                                    //     Slider.value slider.Current
                                    //     Slider.tickFrequency 1.0
                                    //     Slider.isSnapToTickEnabled true
                                    //     // Slider.onValueChanged (fun x -> slider.Set x)
                                    //     // Slider.onTapped (fun x -> ())
                                    //     Slider.onValueChanged(fun x -> slider.Set x)
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "TextBlock"
                                TabItem.content (
                                    TextBlock.create [
                                        TextBlock.text "TextBlock"
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "TextBox"
                                TabItem.content (
                                    TextBox.create [
                                        TextBox.text "TextBox"
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "TimePicker"
                                TabItem.content (
                                    TimePicker.create [
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "ToggleButton"
                                TabItem.content (
                                    ToggleButton.create [
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "ToggleSwitch"
                                TabItem.content (
                                    ToggleSwitch.create [
                                    ]
                                )
                            ]
                            TabItem.create [
                                TabItem.header "Menu"
                                TabItem.content (
                                    menu
                                )
                            ]
                        ]
                    ]
                ]
            ]
        )
