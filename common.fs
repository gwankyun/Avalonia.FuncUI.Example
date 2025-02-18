namespace Common
open System.IO
open System.Text.Json
open Avalonia.FuncUI
open Avalonia.Controls
open Avalonia.FuncUI.DSL

module File =
    let exists path =
        File.Exists path

    let writeAllText path (contents: string) =
        File.WriteAllText(path, contents)

    let readAllText path =
        File.ReadAllText path

module Path =
    let join (path1: string) path2 =
        Path.Join(path1, path2)

module String =
    let split (separator: string) (str: string) =
        str.Split separator

    let trimStart (c: char) (s: string) =
        s.TrimStart(c)

    let toLower (str: string) =
        str.ToLower()

module Directory =
    let current =
        Directory.GetCurrentDirectory()

    let baseDir =
        System.AppContext.BaseDirectory

module JsonSerializer =
    let serialize value =
        JsonSerializer.Serialize value

    let deserialize (json: string) =
        JsonSerializer.Deserialize<'a> json

module Control =
    let example () =
        Component.create ("custom", fun ctx ->
            let state = ctx.useState 0
            let current = state.Current
            DockPanel.create [
                DockPanel.children [
                    Button.create [
                        Button.onClick (fun _ -> state.Set (current + 1))
                        Button.content "+"
                    ]
                    TextBlock.create [
                        TextBlock.text (string state.Current)
                    ]
                ]
            ]
        )
