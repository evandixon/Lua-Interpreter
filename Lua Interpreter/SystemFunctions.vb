Public Class SystemFunctions
    Public Shared Sub RegisterSystemFunctions(LuaFile As LuaFile)
        LuaFile.ContainedChunk.SetVariable("dofile", New LuaObject(New VbInvokable(AddressOf DoFile, LuaFile.ContainedChunk)), True)
        LuaFile.ContainedChunk.SetVariable("print", New LuaObject(New VbInvokable(AddressOf Print, LuaFile.ContainedChunk)), True)
        LuaFile.ContainedChunk.SetVariable("type", New LuaObject(New VbInvokable(AddressOf Type, LuaFile.ContainedChunk)), True)
    End Sub
    Protected Shared Function Print(Arguments As LuaObject(), ParentChunk As Chunk, Debugger As LuaDebugger) As LuaObject
        Console.WriteLine(Arguments(0).ToString)
        Return LuaObject.Nil
    End Function
    Protected Shared Function Type(Arguments As LuaObject(), ParentChunk As Chunk, Debugger As LuaDebugger) As LuaObject
        Return New LuaObject(Arguments(0).GetTypeName)
    End Function
    Protected Shared Function DoFile(Arguments As LuaObject(), ParentChunk As Chunk, Debugger As LuaDebugger) As LuaObject
        Dim l As New LuaFile(Arguments(0).RawValue, Debugger)
        l.ContainedChunk.ParentChunk = ParentChunk
        l.Run()
        Return LuaObject.Nil
    End Function
End Class
