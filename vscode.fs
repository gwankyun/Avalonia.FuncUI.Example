namespace CounterApp

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Controls.Primitives
open TextCopy
open Common
open Microsoft.Data.Sqlite
open DatabaseManage

module VSCode =

    let stack (list: IWritable<Extension list>) (result: IWritable<string>) (i: Extension) : Types.IView =
        Component.create (i.Identify, fun ctx ->
            let text = ctx.useState i.Identify
            let name = ctx.useState i.Name
            let version = ctx.useState i.Version
            let isX64 = ctx.useState i.IsX64
            let color = ctx.useState "blue"
            let thickness = ctx.useState 1
            let currentItem () : Extension =
                { Name = name.Current; Identify = text.Current; Version = version.Current; IsX64 = isX64.Current }
            let save () =
                let current = list.Current
                let idx = List.tryFindIndex (fun (x : Extension) -> x.Identify = i.Identify) current
                match idx with
                | Some(index) ->
                    list.Set (List.updateAt index (currentItem ()) current)
                | None -> ()
                color.Set "green"
                thickness.Set 1
            // Border.create [
            //     // Border.borderBrush color.Current
            //     // Border.borderThickness thickness.Current
            //     Border.child (
            StackPanel.create [
                StackPanel.orientation Orientation.Horizontal
                StackPanel.onLostFocus (fun _ -> save ()) // 失焦就保存
                StackPanel.spacing 5
                StackPanel.children [
                    TextBox.create [
                        TextBox.text name.Current
                        TextBox.width 200
                        TextBox.onTextChanged (fun t -> name.Set t)
                    ]
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
                            list.Set (List.filter (fun j -> j.Identify <> i.Identify) list.Current))
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
                //     ]
                // )
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
            printfn "current: %s" Directory.current
            printfn "baseDir: %s" Directory.baseDir
            let dbPath = Path.join Directory.current "vscode.db"

            let saveJson () =
                let json =
                    list.Current
                    |> JsonSerializer.serialize
                File.writeAllText jsonPath json

                // 寫入數據庫
                use connection = new SqliteConnection(
                    "Data Source=" + dbPath)
                connection.Open()
                DatabaseManage.deleteAll connection
                DatabaseManage.insertMany connection list.Current

            ctx.useEffect (
                (fun _ ->
                    // 啟動時使用數據庫
                    // if File.exists jsonPath |> not then
                    //     saveJson ()
                    // let json = File.readAllText jsonPath
                    // list.Set <| JsonSerializer.deserialize json

                    use connection = new SqliteConnection(
                        "Data Source=" + dbPath)
                    connection.Open()
                    list.Set <| DatabaseManage.selectAll connection
                    ), [EffectTrigger.AfterInit]
            )
            let newItem : Extension =  { Name = ""; Identify =""; Version = ""; IsX64 = false }

            DockPanel.create [
                DockPanel.children [
                    DockPanel.create [
                        // StackPanel.orientation Orientation.Horizontal
                        DockPanel.dock Dock.Top
                        DockPanel.children [
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
                                // TextBox.width 1000
                            ]
                        ]
                    ]

                    DockPanel.create [
                        DockPanel.dock Dock.Top
                        DockPanel.children [
                            Button.create [
                                Button.dock Dock.Left
                                Button.content "过滤"
                            ]
                            RadioButton.create [
                                RadioButton.groupName "type"
                                RadioButton.content "名称"
                                RadioButton.isChecked true
                            ]
                            RadioButton.create [
                                RadioButton.groupName "type"
                                RadioButton.content "标识符"
                            ]
                            TextBox.create [
                                TextBox.dock Dock.Left
                                TextBox.text ""
                            ]
                        ]
                    ]

                    ListBox.create [
                        ListBox.dock Dock.Top
                        ListBox.viewItems (
                            list.Current
                            |> List.sortBy _.Name.ToLower()
                            |> List.map (stack list result)
                        )
                    ]
                ]
            ]
        )
