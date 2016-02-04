Imports System.Text

Public Class Chunk
    Public Property ChildLines As List(Of GenericStatement)
    Public Property ParentChunk As Chunk

    Public Property ReturnValue As LuaObject

    Public Property Variables As Dictionary(Of String, LuaObject)

    ''' <summary>
    ''' Sets the value of VariableName, or creates the variable if it doesn't exist.
    ''' </summary>
    ''' <param name="VariableName">Name of the variable to set or create.</param>
    ''' <param name="VariableValue">Value of the variable to set or create.</param>
    ''' <param name="IsLocal">Whether or not the variable is a local variable.  If true, variable will be created in the current chunk if it does not exist, instead of the root chunk.</param>
    Public Sub SetVariable(VariableName As String, VariableValue As LuaObject, IsLocal As Boolean)
        If Variables.ContainsKey(VariableName) Then
            Variables(VariableName) = VariableValue
        ElseIf IsLocal Then
            Variables.Add(VariableName, VariableValue)
        ElseIf ParentChunk IsNot Nothing Then
            ParentChunk.SetVariable(VariableName, VariableValue, False)
        Else
            Variables.Add(VariableName, VariableValue)
        End If
    End Sub

    Public Function GetVariable(VariableName As String) As LuaObject
        If Variables.ContainsKey(VariableName) Then
            Return Variables(VariableName)
        ElseIf ParentChunk IsNot Nothing Then
            Return ParentChunk.GetVariable(VariableName)
        Else
            Return LuaObject.Nil
        End If
    End Function

    Public Sub Run()
        For Each item In ChildLines
            item.Run()
            If TypeOf item Is ReturnLine Then
                ReturnValue = DirectCast(item, ReturnLine).ReturnValue
            End If
        Next
    End Sub

    Public Sub New()
        ChildLines = New List(Of GenericStatement)
        Variables = New Dictionary(Of String, LuaObject)
        ReturnValue = LuaObject.Nil
    End Sub
    Public Sub New(Text As String)
        Me.New
        Dim lines = Text.Split(vbLf)
        Dim currentChunkStart As String = ""
        Dim currentChunkEnd As String = ""
        Dim chunkDepth As Integer = 0
        Dim currentChunkText As New StringBuilder
        For Each line In lines
            Dim trimmed = line.Trim

            'Are we currently reading a chunk?
            If chunkDepth > 0 Then
                currentChunkText.AppendLine(trimmed)
                If Formatting.IsChunkStart(currentChunkStart, trimmed) Then
                    chunkDepth += 1
                ElseIf Formatting.IsChunkEnd(currentChunkStart, currentChunkEnd, trimmed) Then 'trimmed = currentChunkEnd Then
                    chunkDepth -= 1

                    If chunkDepth = 0 Then
                        'Add this chunk to the child chunks if it's not a comment
                        If Not currentChunkStart = "--[[" Then
                            Select Case currentChunkStart
                                Case "function"
                                    ChildLines.Add(New FunctionAssignment(currentChunkText.ToString, Me))
                                Case Else
                                    ChildLines.Add(New GenericStatement(currentChunkText.ToString, ParentChunk))
                            End Select
                        End If
                    End If
                End If
            Else
                'If not, we continue looking for things in the current chunk.

                If line.Trim.StartsWith("--[[") Then
                    'Then this is a multi-line comment.

                    currentChunkStart = "--[["
                    currentChunkEnd = "--]]"
                    chunkDepth = 1
                    currentChunkText = New StringBuilder
                    currentChunkText.AppendLine("--[[")

                Else
                    'Now that we know it's not a multi-line comment, then we can remove all comments
                    trimmed = Formatting.TrimComment(trimmed)

                    If Not String.IsNullOrWhiteSpace(trimmed) Then
                        If Formatting.ContainsWord(trimmed, "function") Then
                            If trimmed.EndsWith("end") Then
                                'Then this is a one-line function
                                ChildLines.Add(New FunctionAssignment(trimmed, Me))
                            Else
                                'Then this is a function chunk.
                                currentChunkStart = "function"
                                currentChunkEnd = "end"
                                chunkDepth = 1
                                currentChunkText = New StringBuilder
                                currentChunkText.AppendLine(trimmed)
                            End If
                        ElseIf trimmed.EndsWith("{") Then
                            currentChunkStart = "{"
                            currentChunkEnd = "}"
                            chunkDepth = 1
                            currentChunkText = New StringBuilder
                            currentChunkText.AppendLine(trimmed)
                        Else
                            'Then this is NOT a function, so we can go to a simple select statement.  We can't do this with functions because they can be assigned to things
                            Dim firstWord = trimmed.Split(" ".ToCharArray, 2)(0)
                            Select Case firstWord.ToLower
                                Case "do"
                                    currentChunkStart = "do"
                                    currentChunkEnd = "end"
                                    chunkDepth = 1
                                    currentChunkText = New StringBuilder
                                    currentChunkText.AppendLine(trimmed)
                                Case "for"
                                    currentChunkStart = "for"
                                    currentChunkEnd = "end"
                                    chunkDepth = 1
                                    currentChunkText = New StringBuilder
                                    currentChunkText.AppendLine(trimmed)
                                Case "if"
                                    currentChunkStart = "if"
                                    currentChunkEnd = "end"
                                    chunkDepth = 1
                                    currentChunkText = New StringBuilder
                                    currentChunkText.AppendLine(trimmed)
                                Case "while"
                                    currentChunkStart = "while"
                                    currentChunkEnd = "end"
                                    chunkDepth = 1
                                    currentChunkText = New StringBuilder
                                    currentChunkText.AppendLine(trimmed)
                                Case "repeat"
                                    currentChunkStart = "repeat"
                                    currentChunkEnd = "until"
                                    chunkDepth = 1
                                    currentChunkText = New StringBuilder
                                    currentChunkText.AppendLine(trimmed)
                                Case Else
                                    ChildLines.Add(GenericStatement.GetStatement(trimmed, Me))
                            End Select
                        End If
                    End If
                End If
            End If
        Next
    End Sub
End Class
