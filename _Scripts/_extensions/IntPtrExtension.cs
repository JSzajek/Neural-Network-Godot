using Godot;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Extension class for basic IntPtr class.
/// </summary>
public static class IntPtrExtension {
    
    /// <summary>
    /// Retrieves the generic delegate from the pointer with passed string function name
    /// </summary>
    /// <typeparam name="T">The type of delegate</typeparam>
    public static T GetFuncDelegate<T>(this IntPtr obj, string path) where T : class {
        return Marshal.GetDelegateForFunctionPointer(NativeMethods.GetProcAddress(obj, path), typeof(T)) as T;
    }

}