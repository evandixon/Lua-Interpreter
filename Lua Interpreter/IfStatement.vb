Imports System.Text
Imports System.Text.RegularExpressions

Public Class IfStatement
    Inherits GenericStatement

    Public Sub New(Text As String, Filename As String, LineNumber As Integer, ParentChunk As Chunk, Debugger As LuaDebugger)
        MyBase.New(Text, Filename, LineNumber, ParentChunk, Debugger)
    End Sub

    Public Property Entries As List(Of IfEntry)

    Public Overrides Sub Load()
        '$1: condition; $2: body
        Static ifLine As Regex = New Regex("if\s(.*)\sthen\s(.*)\send", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        '$2: condition
        Static ifBlock As Regex = New Regex("(else)?if\s(.*)\sthen|else", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)

        Entries = New List(Of IfEntry)

        Dim ifMatch = ifLine.Match(Text)
        If ifMatch.Success Then
            'Parse line
            Entries.Add(New IfEntry With {.IsDefault = False, .ConditionText = ifMatch.Groups(1).Value, .LineNumber = LineNumber, .Body = New Chunk(ifMatch.Groups(2).Value, Filename, LineNumber, Debugger)})
        Else
            'Parse block
            Dim currentEntry As IfEntry = Nothing
            Dim chunkDepth As Integer = 0
            Dim chunkEntry As New StringBuilder

            Dim lines = Text.Split(vbLf)
            For i = 0 To lines.Length - 1
                Dim line = lines(i).Trim

                If chunkDepth = 0 OrElse (chunkDepth = 1 AndAlso line.StartsWith("else")) Then

                    If currentEntry IsNot Nothing Then
                        'Add the previous entry if applicable
                        currentEntry.Body = New Chunk(chunkEntry.ToString, Filename, currentEntry.LineNumber + 1, Debugger)
                        Entries.Add(currentEntry)
                        currentEntry = Nothing
                        chunkEntry = New StringBuilder
                    End If

                    'Look for an entry in the if statement
                    Dim match = ifBlock.Match(lines(i))
                    If match.Success Then
                        'Entry found
                        chunkDepth = 1
                        currentEntry = New IfEntry
                        currentEntry.LineNumber = i + LineNumber
                        currentEntry.IsDefault = (line = "else")
                        If Not currentEntry.IsDefault Then
                            'Set the condition if this is not simply an "else" part
                            currentEntry.ConditionText = match.Groups(2).Value
                        End If
                    End If
                Else
                    If line = "end" Then
                        chunkDepth -= 1
                        If chunkDepth = 0 Then
                            'Add the completed entry
                            currentEntry.Body = New Chunk(chunkEntry.ToString, currentEntry.LineNumber + 1, ParentChunk)
                            Entries.Add(currentEntry)
                            currentEntry = Nothing
                            chunkEntry = New StringBuilder
                        End If
                    Else
                        chunkEntry.AppendLine(line)
                        If line.ToLower.StartsWith("if ") Then
                            chunkDepth += 1
                        End If
                    End If
                End If
            Next
        End If
    End Sub

    Public Overrides Sub Run()
        For Each item In Entries
            ReportExecution()
            If item.IsDefault OrElse LuaObject.Parse(item.ConditionText, ParentChunk, Debugger).IsTrue Then
                item.Body.Run()
            End If
        Next
    End Sub
End Class
