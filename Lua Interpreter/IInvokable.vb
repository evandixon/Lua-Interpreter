Public Interface IInvokable
    Function Invoke(Arguments As LuaObject(), debugger As LuaDebugger) As LuaObject
End Interface
