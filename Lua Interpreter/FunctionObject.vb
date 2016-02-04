Public Class FunctionObject
    Implements IInvokable
    Protected Property ContainedChunk As Chunk
    Protected Property ParamNames As List(Of String)
    Public Function Invoke(ParamValues As LuaObject()) As LuaObject Implements IInvokable.Invoke
        For count = 0 To ParamNames.Count - 1
            If ParamValues.Length > count Then
                ContainedChunk.SetVariable(ParamNames(count), ParamValues(count), True)
            Else
                ContainedChunk.SetVariable(ParamNames(count), LuaObject.Nil, True)
            End If
        Next

        ContainedChunk.Run()

        Return ContainedChunk.ReturnValue
    End Function
    Public Sub New(FunctionBody As String, Params As String, ParentChunk As Chunk)
        Me.ContainedChunk = New Chunk(FunctionBody)
        Me.ContainedChunk.ParentChunk = ParentChunk
        Me.ParamNames = New List(Of String)
        For Each item In Params.Split(",")
            ParamNames.Add(item.Trim)
        Next
    End Sub
End Class
