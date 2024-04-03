namespace Common
open System.IO
open System.Text.Json

module File =
    let exists path =
        File.Exists path

    let writeAllText path contents =
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
