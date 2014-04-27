using System;

namespace Pico.Device.TC08
{
  public class TC08Exception : Exception
  {
    public TC08Exception(string message)
      : base(message)
    {
    }

    public TC08Exception(short handle)
      : base(GetLastExceptionMessage(handle))
    {
    }

    private static string GetLastExceptionMessage(short handle)
    {
      TC08DeviceImports.ErrorCode error = (TC08DeviceImports.ErrorCode) TC08DeviceImports.GetLastError(handle);

      switch (error)
      {
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_OK:
          return "No error occurred.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_OS_NOT_SUPPORTED:
          return "The driver supports Windows XP SP2 or later, Windows Vista, and Windows 7.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_NO_CHANNELS_SET:
          return "A call to usb_tc08_set_channel is required.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_INVALID_PARAMETER:
          return "One or more of the function arguments were invalid.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_VARIANT_NOT_SUPPORTED:
          return "The hardware version is not supported. Download the latest driver.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_INCORRECT_MODE:
          return
            "An incompatible mix of legacy and non-legacy functions was called (or usb_tc08_get_single was called while in streaming mode.)";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_ENUMERATION_INCOMPLETE:
          return "usb_tc08_open_unit_async was called again while a background enumeration was already in progress.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_NOT_RESPONDING:
          return "Cannot get a reply from a USB TC-08.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_FW_FAIL:
          return "Unable to download firmware.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_CONFIG_FAIL:
          return "Missing or corrupted EEPROM.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_NOT_FOUND:
          return "Cannot find enumerated device.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_THREAD_FAIL:
          return "A threading function failed.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_PIPE_INFO_FAIL:
          return "Can not get USB pipe information.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_NOT_CALIBRATED:
          return "No calibration date was found.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_PICOPP_TOO_OLD:
          return "An old picopp.sys driver was found on the system.";
        case TC08DeviceImports.ErrorCode.USBTC08_ERROR_COMMUNICATION:
          return "The PC has lost communication with the device.";
        default:
          return "Unknown Error";
      }
    }
  }
}