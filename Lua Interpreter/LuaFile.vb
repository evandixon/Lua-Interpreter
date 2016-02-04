Public Class LuaFile
    Public Property ContainedChunk As Chunk
    Public Sub Run()
        ContainedChunk.Run()
    End Sub
    Public Sub New(Filename As String)
        ContainedChunk = New Chunk(IO.File.ReadAllText(Filename))
        SystemFunctions.RegisterSystemFunctions(Me)
    End Sub
End Class
