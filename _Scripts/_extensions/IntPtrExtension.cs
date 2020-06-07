using Godot;
using System;
using System.Runtime.InteropServices;

public static class IntPtrExtension {
    public static T GetFuncDelegate<T>(this IntPtr obj, string path) where T : class {
        return Marshal.GetDelegateForFunctionPointer(NativeMethods.GetProcAddress(obj, path), typeof(T)) as T;
    }

}