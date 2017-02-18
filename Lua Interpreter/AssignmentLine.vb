Public Class AssignmentLine
    Inherits GenericStatement

    Public Sub New(Text As String, Filename As String, LineNumber As Integer, ParentChunk As Chunk, Debugger As LuaDebugger)
        MyBase.New(Text, Filename, LineNumber, ParentChunk, Debugger)
        Me.VariableNames = New List(Of String)
        Me.VariableStrings = New List(Of String)
        Me.VariableValues = New List(Of LuaObject)
    End Sub

    Protected Property IsLocal As Boolean
    Protected Property VariableNames As List(Of String)
    Protected Property VariableStrings As List(Of String)
    Protected Property VariableValues As List(Of LuaObject)

    Public Overrides Sub Load()
        IsLocal = Text.ToLower.StartsWith("local ")
        VariableNames = New List(Of String)
        VariableValues = New List(Of LuaObject)
        If IsLocal Then
            Text = Text.Split(" ".ToCharArray, 2)(1)
        End If
        Dim parts = Text.Split("=".ToCharArray, 2)
        For Each item In parts(0).Split(",")
            VariableNames.Add(item.Trim)
        Next
        'If parts(1).StartsWith("function") Then
        '    VariableValues.Add(New LuaObject(New FunctionAssignment(parts(1), ParentChunk)))
        'Else
        For Each item In parts(1).Split(",")
            VariableStrings.Add(item.Trim)
        Next
        'End If
    End Sub

    Public Overrides Sub Run()
        MyBase.Run()

        If VariableStrings.Count > 0 Then
            VariableValues.Clear()
            For Each item In VariableStrings
                VariableValues.Add(LuaObject.Parse(item, ParentChunk, Debugger))
            Next
        End If
        For count = 0 To VariableNames.Count - 1
            If VariableValues.Count > count Then
                ParentChunk.SetVariable(VariableNames(count), VariableValues(count), IsLocal)
            Else
                ParentChunk.SetVariable(VariableNames(count), LuaObject.Nil, IsLocal)
            End If
        Next
    End Sub

End Class
