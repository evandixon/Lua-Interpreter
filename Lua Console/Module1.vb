Imports Lua_Interpreter

Module Module1

    Sub Main()
        Dim args = Environment.GetCommandLineArgs()

        Dim debug = New LuaDebugger()
        'debug.Breakpoints.Add(9)

        If IO.File.Exists(args(1)) Then
            Dim lua = New LuaFile(args(1), debug)
            lua.Run()
        End If
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

End Module
