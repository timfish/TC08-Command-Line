using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Pico.Device.TC08
{
  /// <summary>
  /// Device discovery and life-time manager for TC08 devices
  /// </summary>
  public class TC08DeviceManager : IDisposable
  {
    private readonly ConcurrentDictionary<short, TC08Device> _openDevices = new ConcurrentDictionary<short, TC08Device>();

    /// <summary>
    /// Gets the avalable TC08 devices.
    /// </summary>
    /// <value>
    /// The devices.
    /// </value>
    public IEnumerable<TC08Device> Devices
    {
      get
      {
        foreach (var openDevice in CheckForNewDevices())
          _openDevices[openDevice.Handle] = openDevice;

        return _openDevices.Select(k => k.Value).ToList();
      }
    }

    /// <summary>
    /// Checks for new devices.
    /// </summary>
    /// <returns></returns>
    private IEnumerable<TC08Device> CheckForNewDevices()
    {
      short handle = 0;
      while ((handle = TC08DeviceImports.OpenUnit()) > 0)
      {
        TC08Device device = new TC08Device(handle, OnClosed);
        Trace.WriteLine("Device Found - \r\n" + device.DeviceInfo);
        yield return device;
      }
    }

    /// <summary>
    /// Called when a device closes and wants to notify the device manager
    /// </summary>
    /// <param name="handle">The handle.</param>
    private void OnClosed(short handle)
    {
      Trace.WriteLine("Device " + handle + " closed");
      TC08DeviceImports.CloseUnit(handle);

        TC08Device device;
        _openDevices.TryRemove(handle, out device);
    }

    #region Implementation of IDisposable

    /// <summary>
    /// Ensures all devices are closed
    /// </summary>
    public void Dispose()
    {
      foreach (var device in _openDevices)
        device.Value.Dispose();
    }

    #endregion
  }
}
