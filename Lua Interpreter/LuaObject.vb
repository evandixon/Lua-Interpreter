Imports System.Text.RegularExpressions

Public Class LuaObject
    Public Shared Function Parse(InputString As String, ParentChunk As Chunk) As LuaObject
        'FunctionName: $1
        'Params: $5
        Static functionInvoke As New Regex("((([A-Z]|[a-z])([^?!\s\+\-\*\/\\\^\%#=~<>{}[\];,])*))\((.*)\)", RegexOptions.Compiled)
        If functionInvoke.IsMatch(InputString) Then
            Dim m = functionInvoke.Match(InputString)
            Dim parts = m.Value.Split("(".ToCharArray, 2)
            Dim name = parts(0).Trim

            Dim func = ParentChunk.GetVariable(name)
            If Not func.IsNil AndAlso func.IsInvokable Then
                Dim paramList = parts(1).Trim.Remove(parts(1).Length - 1, 1) 'Trims the last ) at the end
                Dim paramStrings = Formatting.SplitParamValues(paramList)
                Dim paramValues As New List(Of LuaObject)
                For Each item In paramStrings
                    paramValues.Add(Parse(item, ParentChunk))
                Next
                Return func.Invoke(paramValues.ToArray)
            Else
                Throw New ArgumentException($"Variable {name} cannot be invoked.")
            End If
        ElseIf InputString.StartsWith("""") And InputString.EndsWith("""") Then
            Return New LuaObject With {.RawValue = InputString.Trim("""")}
        ElseIf IsNumeric(InputString) Then
            Return New LuaObject With {.RawValue = Convert.ToDouble(InputString)}
        ElseIf InputString.ToLower = "true" OrElse InputString.ToLower = "false" Then
            Return New LuaObject With {.RawValue = Boolean.Parse(InputString)}
        Else
            Return ParentChunk.GetVariable(InputString)
        End If
    End Function
    Public Shared Function Nil() As LuaObject
        Return New LuaObject With {.RawValue = Nothing}
    End Function



    Public Property RawValue As Object
    Public Function IsNil() As Boolean
        Return RawValue Is Nothing
    End Function
    Public Overridable Function IsInvokable() As Boolean
        Return TypeOf RawValue Is IInvokable
    End Function
    Public Overridable Function Invoke(Args As LuaObject()) As LuaObject
        Return DirectCast(RawValue, IInvokable).Invoke(Args)
    End Function
    Public Overrides Function ToString() As String
        If RawValue Is Nothing Then
            Return "Nil"
        Else
            Return RawValue.ToString
        End If
    End Function
    Public Overridable Function GetTypeName() As String
        If IsNil() Then
            Return "nil"
        ElseIf TypeOf RawValue Is String Then
            Return "string"
        ElseIf TypeOf RawValue Is FunctionObject Then
            Return "function"
        ElseIf TypeOf RawValue Is Double Then
            Return "number"
        ElseIf TypeOf RawValue Is Boolean Then
            Return "boolean"
        End If
    End Function
    Public Sub New()

    End Sub
    Public Sub New(RawValue As Object)
        Me.RawValue = RawValue
    End Sub
End Class
