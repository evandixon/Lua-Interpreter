Imports System.Text.RegularExpressions

Public Class GenericStatement

    Protected Property ParentChunk As Chunk

    Protected Property Text As String

    Public Overridable Sub Run()
        Dim tmp = LuaObject.Parse(Text, ParentChunk)
    End Sub

    Public Shared Function GetStatement(Text As String, ParentChunk As Chunk) As GenericStatement
        'Name: $1, Value: $5
        Static assignmentRegex As New Regex("((([A-Z]|[a-z])([^?!\s\+\-\*\/\\\^\%#=~<>{}[\];:,.])*)\s*)\=\s*(.+)", RegexOptions.Compiled)

        If assignmentRegex.IsMatch(Text) Then
            Return New AssignmentLine(Text, ParentChunk)
        ElseIf Text.tolower.StartsWith("return") Then
            Return New ReturnLine(Text, ParentChunk)
        Else
            Return New GenericStatement(Text, ParentChunk)
        End If
    End Function
    Public Overridable Sub Load()

    End Sub

    Public Sub New()

    End Sub
    Public Sub New(Text As String, ParentChunk As Chunk)
        Me.ParentChunk = ParentChunk
        Me.Text = Text
        Load()
    End Sub
End Class
