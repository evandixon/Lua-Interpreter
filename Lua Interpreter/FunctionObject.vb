Public Class FunctionObject
    Implements IInvokable

    Public Sub New(FunctionBody As String, Params As String, LineNumber As Integer, ParentChunk As Chunk)
        Me.ContainedChunk = New Chunk(FunctionBody, LineNumber, ParentChunk)
        Me.ParamNames = New List(Of String)
        For Each item In Params.Split(",")
            ParamNames.Add(item.Trim)
        Next
    End Sub

    Protected Property ContainedChunk As Chunk

    Protected Property ParamNames As List(Of String)

    Public Function Invoke(ParamValues As LuaObject(), debugger As LuaDebugger) As LuaObject Implements IInvokable.Invoke
        For count = 0 To ParamNames.Count - 1
            If ParamValues.Length > count Then
                ContainedChunk.SetVariable(ParamNames(count), ParamValues(count), True)
            Else
                ContainedChunk.SetVariable(ParamNames(count), LuaObject.Nil, True)
            End If
        Next
        ContainedChunk.SetVariable("return", LuaObject.Nil, True)

        ContainedChunk.Run()

        Return ContainedChunk.GetVariable("return")
    End Function

End Class
