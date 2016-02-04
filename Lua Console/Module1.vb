Module Module1

    Sub Main()
        Dim args = Environment.GetCommandLineArgs()
        If IO.File.Exists(args(1)) Then
            Dim lua = New Lua_Interpreter.LuaFile(args(1))
            lua.Run()
        End If
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

End Module
