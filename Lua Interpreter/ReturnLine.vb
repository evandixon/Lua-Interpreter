Public Class ReturnLine
    Inherits GenericStatement

    Public Sub New(Text As String, Filename As String, LineNumber As Integer, ParentChunk As Chunk, Debugger As LuaDebugger)
        MyBase.New(Text, Filename, LineNumber, ParentChunk, Debugger)
    End Sub

    Protected Property ReturnExpression As String

    Public Overrides Sub Run()
        MyBase.Run()
        If ReturnExpression IsNot Nothing Then
            ParentChunk.SetVariable("return", LuaObject.Parse(ReturnExpression, ParentChunk, Debugger), False)
        Else
            ParentChunk.SetVariable("return", LuaObject.Nil, False)
        End If
    End Sub

    Public Overrides Sub Load()
        Dim parts = Text.Split(" ".ToCharArray, 2)
        If parts(0).ToLower = "return" Then
            ReturnExpression = parts(1).Trim
        End If
    End Sub

End Class
