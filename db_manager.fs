namespace DatabaseManage
open Microsoft.Data.Sqlite

type Extension =
    {
        Name: string
        Identify: string
        Version: string
        IsX64: bool
    }

module DatabaseManage =
    let deleteAll (_connection: SqliteConnection) =
        let command = _connection.CreateCommand();
        command.CommandText <-
            @"
                DELETE FROM extension;
            "
        command.ExecuteNonQuery() |> ignore

    let insertMany (_connection: SqliteConnection) (_userList: Extension list) =
        let tran = _connection.BeginTransaction()
        let command = _connection.CreateCommand();
        command.CommandText <-
            @"
                INSERT INTO extension(identify, name, version, arch)
                VALUES
                ($identify, $name, $version, $arch);
            "

        let createParameter name =
            let param = command.CreateParameter()
            param.ParameterName <- name
            command.Parameters.Add(param) |> ignore
            param

        let identify = createParameter "$identify"
        let name = createParameter "$name"
        let version = createParameter "$version"
        let arch = createParameter "$arch"

        for user in _userList do
            identify.Value <- user.Identify
            name.Value <- user.Name
            version.Value <- user.Version
            arch.Value <- if user.IsX64 then "X64" else ""
            command.ExecuteNonQuery() |> ignore
        tran.Commit()

    let selectAll (_connection: SqliteConnection) =
        /// 搜索全部
        let command = _connection.CreateCommand();
        command.CommandText <-
            @"
                SELECT identify, name, version, arch
                FROM extension
                ;
            "

        use reader = command.ExecuteReader()
        let mutable result = []
        while reader.Read() do
            let identify = reader.GetString(0)
            let name = reader.GetString(1)
            let version = reader.GetString(2)
            let arch = reader.GetString(3)
            result <- {
                Identify = identify;
                Name = name;
                Version = version;
                IsX64 = (arch = "X64") } :: result
        result
