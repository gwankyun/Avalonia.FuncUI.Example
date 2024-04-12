﻿namespace CounterApp

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Controls.Primitives
open TextCopy
open Common

module VSCode =
    type Extension =
        {
            Name: string
            Version: string
            IsX64: bool
        }

    let stack (list: IWritable<Extension list>) (result: IWritable<string>) (i: Extension) : Types.IView =
        Component.create (i.Name, fun ctx ->
            let text = ctx.useState i.Name
            let version = ctx.useState i.Version
            let isX64 = ctx.useState i.IsX64
            let color = ctx.useState "blue"
            let thickness = ctx.useState 1
            let currentItem () =
                { Name = text.Current; Version = version.Current; IsX64 = isX64.Current }
            let save () =
                let current = list.Current
                let idx = List.tryFindIndex (fun x -> x.Name = i.Name) current
                match idx with
                | Some(index) ->
                    list.Set (List.updateAt index (currentItem ()) current)
                | None -> ()
                color.Set "green"
                thickness.Set 1
            Border.create [
                // Border.borderBrush color.Current
                // Border.borderThickness thickness.Current
                Border.child (
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
                                    if (testArray |> Array.length) >= 2 then
                                        let url =
                                            let ver =
                                                version.Current |> String.trimStart 'v'
                                            let host = "https://marketplace.visualstudio.com/_apis/public/gallery/publishers"
                                            sprintf "%s/%s/vsextensions/%s/%s/vspackage" host testArray[0] testArray[1] ver
                                        let result_ =
                                            match isX64.Current with
                                            | true ->
                                                sprintf "%s?targetPlatform=win32-x64" url
                                            | false -> sprintf "%s" url
                                        result.Set result_
                                        ClipboardService.SetText result_
                                    else
                                        ClipboardService.SetText ""
                                )
                            ]
                        ]
                    ]
                )
            ]
        )

    let jsonPath =
        let baseDir = Directory.baseDir
        Path.join baseDir "data.json"

    let vscodeComponet () =
        Component.create (
            "vscodeComponet",
            fun ctx ->
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
            let newItem : Extension =  { Name = ""; Version = ""; IsX64 = false }

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
                            |> List.map (stack list result)
                        )
                    ]
                ]
            ]
        )
