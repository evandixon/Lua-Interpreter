''' <summary>
''' A single entry of an if statement
''' </summary>
Public Class IfEntry
    ''' <summary>
    ''' Whether or not the entry is the default entry (else statement)
    ''' </summary>
    Public Property IsDefault As Boolean

    ''' <summary>
    ''' The text of the condition (for use with <see cref="LuaObject.Parse(String, Chunk)"/>)
    ''' </summary>
    Public Property ConditionText As String

    ''' <summary>
    ''' The line number the condition is on
    ''' </summary>
    Public Property LineNumber As Integer

    ''' <summary>
    ''' The chunk to execute of the condition is true
    ''' </summary>
    Public Property Body As Chunk
End Class
