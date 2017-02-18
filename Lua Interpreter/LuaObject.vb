Imports System.Text.RegularExpressions

Public Class LuaObject

    Public Const NilType = "nil"
    Public Const FunctionType = "function"
    Public Const NumberType = "number"
    Public Const StringType = "string"
    Public Const BooleanType = "boolean"

    Public Shared Function Parse(InputString As String, ParentChunk As Chunk, debugger As LuaDebugger) As LuaObject
        'FunctionName: $1
        'Params: $5
        Static functionInvoke As New Regex("((([A-Z]|[a-z])([^?!\s\+\-\*\/\\\^\%#=~<>{}[\];,])*))\((.*)\)", RegexOptions.Compiled)
        Static operatorInquire As New Regex("(.*)(<|>|<=|>=|==|~=)(.*)", RegexOptions.Compiled)
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
                    paramValues.Add(Parse(item, ParentChunk, debugger))
                Next
                Return func.Invoke(paramValues.ToArray, debugger)
            Else
                Throw New ArgumentException($"Variable {name} cannot be invoked.")
            End If
        ElseIf operatorInquire.IsMatch(InputString) Then
            Dim m = operatorInquire.Match(InputString)
            Dim objA = Parse(m.Groups(1).Value, ParentChunk, debugger)
            Dim op = m.Groups(2).Value
            Dim objB = Parse(m.Groups(3).Value, ParentChunk, debugger)

            Select Case op
                Case ">"
                    Return New LuaObject(objA > objB)
                Case "<"
                    Return New LuaObject(objA < objB)
                Case "<="
                    Return New LuaObject(objA <= objB)
                Case ">="
                    Return New LuaObject(objA >= objB)
                Case "=="
                    Return New LuaObject(objA = objB)
                Case "~="
                    Return New LuaObject(objA <> objB)
                Case Else
                    Throw New Exception("Operator does not match what was retrieved from regex")
            End Select
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

    Public Sub New()

    End Sub
    Public Sub New(RawValue As Object)
        Me.RawValue = RawValue
    End Sub

    Public Property RawValue As Object

    Public Function IsNil() As Boolean
        Return RawValue Is Nothing
    End Function

    Public Function IsTrue() As Boolean
        If TypeOf RawValue Is Boolean Then
            Return RawValue
        ElseIf TypeOf RawValue Is Double Then
            Return RawValue > 0
        Else
            Return Not IsNil()
        End If
    End Function

    Public Overridable Function IsInvokable() As Boolean
        Return TypeOf RawValue Is IInvokable
    End Function

    Public Overridable Function Invoke(Args As LuaObject(), debugger As LuaDebugger) As LuaObject
        Return DirectCast(RawValue, IInvokable).Invoke(Args, debugger)
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
            Return NilType
        ElseIf TypeOf RawValue Is String Then
            Return StringType
        ElseIf TypeOf RawValue Is FunctionObject Then
            Return FunctionType
        ElseIf TypeOf RawValue Is Double Then
            Return NumberType
        ElseIf TypeOf RawValue Is Boolean Then
            Return BooleanType
        End If
    End Function

    Public Shared Operator >(ByVal class1 As LuaObject, ByVal class2 As LuaObject) As Boolean
        Select Case class1.GetTypeName
            Case NumberType
                Return (DirectCast(class1.RawValue, Double) > DirectCast(class2.RawValue, Double))
            Case Else
                Throw New NotImplementedException
        End Select
    End Operator

    Public Shared Operator <(ByVal class1 As LuaObject, ByVal class2 As LuaObject) As Boolean
        Select Case class1.GetTypeName
            Case NumberType
                Return (DirectCast(class1.RawValue, Double) < DirectCast(class2.RawValue, Double))
            Case Else
                Throw New NotImplementedException
        End Select
    End Operator

    Public Shared Operator >=(ByVal class1 As LuaObject, ByVal class2 As LuaObject) As Boolean
        Select Case class1.GetTypeName
            Case NumberType
                Return (DirectCast(class1.RawValue, Double) >= DirectCast(class2.RawValue, Double))
            Case Else
                Throw New NotImplementedException
        End Select
    End Operator

    Public Shared Operator <=(ByVal class1 As LuaObject, ByVal class2 As LuaObject) As Boolean
        Select Case class1.GetTypeName
            Case NumberType
                Return (DirectCast(class1.RawValue, Double) <= DirectCast(class2.RawValue, Double))
            Case Else
                Throw New NotImplementedException
        End Select
    End Operator

    Public Shared Operator =(ByVal class1 As LuaObject, ByVal class2 As LuaObject) As Boolean
        Return class1.RawValue.Equals(class2)
    End Operator

    Public Shared Operator <>(ByVal class1 As LuaObject, ByVal class2 As LuaObject) As Boolean
        Return Not class1.RawValue.Equals(class2)
    End Operator

End Class
