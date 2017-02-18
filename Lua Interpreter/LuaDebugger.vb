Public Class LuaDebugger
    Public Sub New()
        Breakpoints = New List(Of Integer)
    End Sub

    Public Property Breakpoints As List(Of Integer)

    Public Overridable Sub Break()
        Debugger.Break()
    End Sub

    ''' <summary>
    ''' Tells the debugger what line number is about to execute
    ''' </summary>
    ''' <param name="number">Line number of the line that is about to execute</param>
    Public Sub RunningLine(filename As String, number As Integer)
        If Breakpoints.Contains(number) Then
            Break()
        End If
    End Sub
End Class
