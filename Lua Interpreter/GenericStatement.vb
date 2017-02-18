Imports System.Text.RegularExpressions

Public MustInherit Class GenericStatement

    Public Shared Function GetStatement(Text As String, Filename As String, LineNumber As Integer, ParentChunk As Chunk, Debugger As LuaDebugger) As GenericStatement
        'Name: $1, Value: $5
        Static assignmentRegex As New Regex("((([A-Z]|[a-z])([^?!\s\+\-\*\/\\\^\%#=~<>{}[\];:,.])*)\s*)\=\s*(.+)", RegexOptions.Compiled)

        If assignmentRegex.IsMatch(Text) Then
            Dim assignment = New AssignmentLine(Text, Filename, LineNumber, ParentChunk, Debugger)
            assignment.Load()
            Return assignment
        ElseIf Text.ToLower.StartsWith("return") Then
            Dim line = New ReturnLine(Text, Filename, LineNumber, ParentChunk, Debugger)
            line.Load()
            Return line
        Else
            Return New VoidStatement(Text, Filename, LineNumber, ParentChunk, Debugger)
        End If
    End Function

    Public Sub New()

    End Sub
    Public Sub New(Text As String, Filename As String, LineNumber As String, ParentChunk As Chunk, Debugger As LuaDebugger)
        Me.ParentChunk = ParentChunk
        Me.Text = Text
        Me.LineNumber = LineNumber
        Me.Filename = Filename
        Me.Debugger = Debugger
    End Sub

    Protected Property ParentChunk As Chunk

    Protected Property Text As String

    Public Property LineNumber As Integer

    Public Property Filename As String

    Public Property Debugger As LuaDebugger

    Public Overridable Sub Load()

    End Sub

    Protected Sub ReportExecution()
        Debugger?.RunningLine(Filename, LineNumber)
    End Sub

    Protected Sub ReportExecution(lineNumber As Integer)
        Debugger?.RunningLine(Filename, lineNumber)
    End Sub

    Public Overridable Sub Run()
        ReportExecution()
    End Sub

End Class
