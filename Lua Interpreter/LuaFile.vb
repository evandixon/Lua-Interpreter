Public Class LuaFile
    Public Property ContainedChunk As Chunk
    Public Sub Run()
        ContainedChunk.Run()
    End Sub
    Public Sub New(Filename As String, Debugger As LuaDebugger)
        ContainedChunk = New Chunk(IO.File.ReadAllText(Filename), Filename, 1, Debugger)
        SystemFunctions.RegisterSystemFunctions(Me)
    End Sub
End Class
