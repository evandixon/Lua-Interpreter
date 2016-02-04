Imports Lua_Interpreter

Public Class VbInvokable
    Implements IInvokable

    Protected Property F As Func(Of LuaObject(), Chunk, LuaObject)
    Protected Property ParentChunk As Chunk

    Public Function Invoke(Arguments() As LuaObject) As LuaObject Implements IInvokable.Invoke
        Return F.Invoke(Arguments, ParentChunk)
    End Function

    Public Sub New(F As Func(Of LuaObject(), Chunk, LuaObject), ParentChunk As Chunk)
        Me.F = F
        Me.ParentChunk = ParentChunk
    End Sub
End Class
