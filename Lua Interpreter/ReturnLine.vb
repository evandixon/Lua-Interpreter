Public Class ReturnLine
    Inherits GenericStatement

    Public Property ReturnValue As LuaObject
    Protected Property ReturnExpression As String

    Public Overrides Sub Run()
        If ReturnExpression IsNot Nothing Then
            ReturnValue = LuaObject.Parse(ReturnExpression, ParentChunk)
        Else
            ReturnValue = LuaObject.Nil
        End If
    End Sub

    Public Overrides Sub Load()
        Dim parts = Text.Split(" ".ToCharArray, 2)
        If parts(0).ToLower = "return" Then
            ReturnExpression = parts(1).Trim
        End If
    End Sub

    Public Sub New(Text As String, ParentChunk As Chunk)
        MyBase.New(Text, ParentChunk)
    End Sub
End Class
