using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Pico.Device.TC08
{
  public unsafe class TC08DeviceImports
  {
    #region ErrorCode enum

    public enum ErrorCode : short
    {
      USBTC08_ERROR_OK,
      USBTC08_ERROR_OS_NOT_SUPPORTED,
      USBTC08_ERROR_NO_CHANNELS_SET,
      USBTC08_ERROR_INVALID_PARAMETER,
      USBTC08_ERROR_VARIANT_NOT_SUPPORTED,
      USBTC08_ERROR_INCORRECT_MODE,
      USBTC08_ERROR_ENUMERATION_INCOMPLETE,
      USBTC08_ERROR_NOT_RESPONDING,
      USBTC08_ERROR_FW_FAIL,
      USBTC08_ERROR_CONFIG_FAIL,
      USBTC08_ERROR_NOT_FOUND,
      USBTC08_ERROR_THREAD_FAIL,
      USBTC08_ERROR_PIPE_INFO_FAIL,
      USBTC08_ERROR_NOT_CALIBRATED,
      USBTC08_ERROR_PICOPP_TOO_OLD,
      USBTC08_ERROR_COMMUNICATION
    }

    #endregion

    #region FreqRej enum

    public enum FreqRej : short
    {
      Fifty = 0,
      Sixty = 1
    }

    #endregion

    #region TempUnit enum

    public enum TempUnit : short
    {
      USBTC08_UNITS_CENTIGRADE,
      USBTC08_UNITS_FAHRENHEIT,
      USBTC08_UNITS_KELVIN,
      USBTC08_UNITS_RANKINE
    }

    #endregion

    private const string DriverFilename = "usbtc08.dll";
    private const string DriverFilename32 = "usbtc08.32.dll";
    private const string DriverFilename64 = "usbtc08.64.dll";

    static TC08DeviceImports()
    {
      DriverDumper.Dump(DriverFilename, Environment.Is64BitProcess ? DriverFilename64 : DriverFilename32);
    }

    [DllImport(DriverFilename, EntryPoint = "usb_tc08_open_unit")]
    public static extern short OpenUnit();

    [DllImport(DriverFilename, EntryPoint = "usb_tc08_close_unit")]
    public static extern short CloseUnit(short handle);

    [DllImport(DriverFilename, EntryPoint = "usb_tc08_get_formatted_info")]
    public static extern short GetFormattedInfo(short handle, StringBuilder unit_info, short string_length);

    [DllImport(DriverFilename, EntryPoint = "usb_tc08_set_channel")]
    public static extern short SetChannel(short handle, short channel, char tc_type);

    [DllImport(DriverFilename, EntryPoint = "usb_tc08_get_single")]
    public static extern short GetSingle(short handle, float[] temp, short* overflow_flags, TempUnit units);

    [DllImport(DriverFilename, EntryPoint = "usb_tc08_set_mains")]
    public static extern short SetRejectionFreq(short handle, short sixtyHertz);

    [DllImport(DriverFilename, EntryPoint = "usb_tc08_get_last_error")]
    public static extern short GetLastError(short handle);
  }
}