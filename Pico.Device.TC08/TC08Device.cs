using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Pico.Device.TC08
{
  public class TC08Device : IDisposable
  {
    private const int TC08MaxChannels = 8;
    private const int TC08MaxInfoLength = 256;
    public static readonly char[] SupportedThermocouples = new[] {'B', 'E', 'J', 'K', 'N', 'R', 'S', 'T'};
    internal readonly short Handle;
    private readonly Timer _devicePoll;
    private readonly Action<short> _onClosed;
    private string _serial;

    public TC08Device(short handle, Action<short> onClosed)
    {
      Handle = handle;
      _onClosed = onClosed;
      StringBuilder builder = new StringBuilder(TC08MaxInfoLength);
      TC08DeviceImports.GetFormattedInfo(Handle, builder, TC08MaxInfoLength);

      DeviceInfo = builder.ToString();
      EnabledChannels = new List<TC08ChannelConfig>();

      _devicePoll = new Timer(2222);
      _devicePoll.Elapsed += DevicePollOnElapsed;
      _devicePoll.Start();

      Connected = true;
    }

    /// <summary>
    /// Gets the device information string.
    /// </summary>
    public string DeviceInfo { get; private set; }

    /// <summary>
    /// Gets the device serial number.
    /// </summary>
    public string Serial
    {
      get { return _serial ?? (_serial = GetSerial()); }
    }

    /// <summary>
    /// Gets or sets the list of enabled channels.
    /// </summary>
    public IList<TC08ChannelConfig> EnabledChannels { get; set; }

    /// <summary>
    /// Gets or sets the device frequency rejection.
    /// </summary>
    public TC08DeviceImports.FreqRej FrequencyRejection { get; set; }

    /// <summary>
    /// Gets a value indicating whether the device is [connected].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [connected]; otherwise, <c>false</c>.
    /// </value>
    public bool Connected { get; private set; }

    /// <summary>
    /// Devices poll to detect disconnect
    /// </summary>
    private void DevicePollOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
    {
      TC08DeviceImports.SetChannel(Handle, 0, 'X');
      short error = TC08DeviceImports.GetLastError(Handle);

      if (error > 6 && Connected)
      {
        Connected = false;
        _devicePoll.Stop();
        _onClosed(Handle);
      }
    }

    /// <summary>
    /// Configures the device to capture.
    /// </summary>
    public void Configure()
    {
      TC08DeviceImports.SetRejectionFreq(Handle, (short) FrequencyRejection);

      for (int i = 1; i < TC08MaxChannels + 1; i++)
      {
        TC08ChannelConfig found = EnabledChannels.FirstOrDefault(dev => dev.Number == i);
        SetChannel(i, found != null ? found.ThermoType : ' ');
      }
    }

    /// <summary>
    /// Gets the temperature values.
    /// </summary>
    /// <param name="unit">The temperature unit.</param>
    /// <returns></returns>
    /// <exception cref="TC08Exception"></exception>
    public unsafe float[] GetValues(TC08DeviceImports.TempUnit unit)
    {
      float[] data = new float[9];
      short overflows;
      short result = TC08DeviceImports.GetSingle(Handle, data, &overflows, unit);

      if (result == 0)
        throw new TC08Exception(result);

      for (int i = 0; i < data.Length; i++)
        data[i] = (float) Math.Round(data[i], 2);

      return data;
    }

    private string GetSerial()
    {
      string[] parts = DeviceInfo.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

      return parts.Length > 4 && parts[4].Contains(":")
               ? parts[4].Split(':').Last().Trim()
               : "Unknown";
    }

    private void SetChannel(int channel, char thermoType)
    {
      short result = TC08DeviceImports.SetChannel(Handle, (short) channel, thermoType);

      if (result == 0)
        throw new TC08Exception(result);
    }

    #region Overrides

    public override bool Equals(object obj)
    {
      TC08Device device = obj as TC08Device;
      return device != null && Handle.Equals(device.Handle);
    }

    public override int GetHashCode()
    {
      return Handle.GetHashCode();
    }


    public override string ToString()
    {
      return Serial;
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~TC08Device()
    {
      Dispose(false);
    }

    public void Dispose(bool disposing)
    {
      if (disposing)
      {
        /* Release managed resources here... */
      }

      _onClosed(Handle);
    }

    #endregion
  }
}