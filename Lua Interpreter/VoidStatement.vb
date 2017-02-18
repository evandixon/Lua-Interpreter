Public Class VoidStatement
    Inherits GenericStatement
    Public Sub New(Text As String, Filename As String, LineNumber As String, ParentChunk As Chunk, Debugger As LuaDebugger)
        MyBase.New(Text, Filename, LineNumber, ParentChunk, Debugger)
    End Sub
    Public Overrides Sub Run()
        MyBase.Run()
        Dim tmp = LuaObject.Parse(Text, ParentChunk, Debugger)
    End Sub
End Class
