Imports System.Text
Imports System.Text.RegularExpressions

Public Class Formatting
    Public Shared Function TrimComment(Input As String) As String
        Dim lineConstructed As New StringBuilder(Input.Length) 'Our output will be no larger than Input.length, and if there's no comments, will be exactly Input.length
        Dim isString As Boolean = False

        For count = 0 To Input.Length - 1
            Dim item As Char = Input(count)
            If item = """"c Then
                isString = Not isString
            End If
            If Not isString Then
                If item = "-"c AndAlso count + 1 < Input.Length AndAlso Input(count + 1) = "-"c Then
                    'Then we found a string and can stop
                    Exit For
                    'Else we continue to run these checks and append to the constructed line
                End If
            End If
            lineConstructed.Append(item)
        Next

        Return lineConstructed.ToString
    End Function

    Public Shared Function SplitParamValues(Params As String) As List(Of String)
        Dim out As New List(Of String)
        Dim tmp As New Text.StringBuilder
        Dim isString As Boolean = False
        Dim invokeDepth As Integer = 0
        For Each item As Char In Params
            Select Case item
                Case """"c
                    isString = Not isString
                    tmp.Append(item)
                Case "("
                    invokeDepth += 1
                    tmp.Append(item)
                Case ")"
                    invokeDepth -= 1
                    tmp.Append(item)
                Case ","c
                    If isString OrElse invokeDepth > 0 Then
                        tmp.Append(item)
                    Else
                        out.Add(tmp.ToString)
                        tmp = New Text.StringBuilder
                    End If
                Case Else
                    tmp.Append(item)
            End Select
        Next
        out.Add(tmp.ToString)
        Return out
    End Function

    Public Shared Function IsChunkStart(ChunkStart As String, Line As String) As Boolean
        If Not ChunkStart.ToLower = "repeat" Then
            If ContainsWord(Line, "function") Then
                Return True
            Else
                Dim startWords = {"do", "for", "if", "while"}
                Return ((From s In startWords Where Line.ToLower.StartsWith(s)).Any AndAlso Not Line.EndsWith("end"))
            End If
        Else
            Return ContainsWord(Line, "repeat")
        End If
    End Function

    Public Shared Function IsChunkEnd(ChunkStart As String, ChunkEnd As String, Line As String) As Boolean
        If ChunkStart.ToLower = "repeat" Then
            Return ContainsWord(Line, ChunkEnd) 'TrimComment(Line).Contains("function")
        Else
            Dim startWords = {"do", "for", "if", "while"}
            Return Line.ToLower.EndsWith(ChunkEnd.ToLower) AndAlso Not (From s In startWords Where Line.ToLower.StartsWith(s)).Any
        End If
    End Function

    Public Shared Function ContainsWord(Line As String, Word As String) As Boolean
        'Todo: use regex to find if the word is preceded and followed by spaces
        Return Line.ToLower.Contains(Word.ToLower)
    End Function
End Class
