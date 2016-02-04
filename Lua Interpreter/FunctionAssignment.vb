Imports System.Text.RegularExpressions

Public Class FunctionAssignment
    Inherits AssignmentLine

    Public Overrides Sub Load()
        'function MyFunction (Param1, Param2) 'Name: $3, Params: $8
        Static form1Regex As New Regex("(function)(\s+)(([A-Z]|[a-z])([^?!\s\+\-\*\/\\\^\%#=~<>{}[\];:,.])*?)(\s*)(\()(.*)(\))", RegexOptions.Compiled Or RegexOptions.IgnoreCase)

        'MyFunction = function(Param1, Param2) 'Name: $1, Params: $5
        Static form2Regex As New Regex("(([A-Z]|[a-z])([^?!\s\+\-\*\/\\\^\%#=~<>{}[\];:,.])*?)\s*\=\s*function\s*(\()(.*)(\))", RegexOptions.Compiled Or RegexOptions.IgnoreCase)

        'Body: $1
        Static functionBody As New Regex("function\s.*?\(.*?\)\s+((.|\n)*?)\s+end", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)

        Dim name As String = Nothing
        Dim params As String = Nothing
        Dim body As String = Nothing
        Dim success As Boolean = True
        Dim isLocal As Boolean = Text.ToLower.StartsWith("local")

        Dim f1Match = form1Regex.Match(Text)
        If f1Match.Success Then
            name = f1Match.Groups(3).Value
            params = f1Match.Groups(8).Value
        Else
            Dim f2Match = form2Regex.Match(Text)
            If f2Match.Success Then
                name = f1Match.Groups(1).Value
                params = f1Match.Groups(5).Value
            Else
                success = False
            End If
        End If

        If success Then
            body = functionBody.Match(Text).Groups(1).Value
            Me.VariableNames.Add(name)
            Me.VariableValues.Add(New LuaObject(New FunctionObject(body, params, ParentChunk)))
        End If
    End Sub

    Public Sub New(Text As String, ParentChunk As Chunk)
        MyBase.New(Text, ParentChunk)
    End Sub
End Class
