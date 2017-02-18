Imports Lua_Interpreter

Public Class VbInvokable
    Implements IInvokable

    Protected Property F As Func(Of LuaObject(), Chunk, LuaDebugger, LuaObject)
    Protected Property ParentChunk As Chunk

    Public Function Invoke(Arguments() As LuaObject, Debugger As LuaDebugger) As LuaObject Implements IInvokable.Invoke
        Return F.Invoke(Arguments, ParentChunk, Debugger)
    End Function

    Public Sub New(F As Func(Of LuaObject(), Chunk, LuaDebugger, LuaObject), ParentChunk As Chunk)
        Me.F = F
        Me.ParentChunk = ParentChunk
    End Sub
End Class
