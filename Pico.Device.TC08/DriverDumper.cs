using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Pico.Device.TC08
{
  public static class DriverDumper
  {
    /// <summary>
    /// Dumps and loads DLL from embedded resources.
    /// </summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <param name="dllName">Name of the DLL import.</param>
    /// <remarks>
    /// DLL should be at the root namespace
    /// </remarks>
    public static void Dump(string dllName, string resourceName)
    {
      Assembly callingAssembly = Assembly.GetCallingAssembly();
      string tempPath = Assembly.GetExecutingAssembly().GetName().Name + "-" + callingAssembly.GetName().Version;
      string dirName = Path.Combine(Path.GetTempPath(), tempPath);

      if (!Directory.Exists(dirName))
        Directory.CreateDirectory(dirName);

      string dllPath = Path.Combine(dirName, dllName);
      string resourcePath = callingAssembly.GetName().Name + "." + resourceName;

      using (Stream resourceStream = callingAssembly.GetManifestResourceStream(resourcePath))
      {
        try
        {
          using (Stream outFile = File.Create(dllPath))
            resourceStream.CopyTo(outFile);
        }
        catch
        {
        }
      }

      IntPtr h = LoadLibrary(dllPath);
      Debug.Assert(h != IntPtr.Zero, "Unable to load library " + dllPath);
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string dllToLoad);
  }
}