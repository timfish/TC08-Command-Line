using System.ComponentModel;
using Args;
using Pico.Device.TC08;

namespace TC08CmdLine
{
  [Description("TC08 Command line application")]
  [ArgsModel(SwitchDelimiter = "-")]
  class CommandArgs
  {
    public CommandArgs()
    {
      Log = false;
      Channel = 1;
      Type = "J";
      RejFreq = 50;
      Unit = "C";
    }

    [Description("Switch to continuous logging mode. Loops until closed.")]
    public bool Log { get; set; }

    [Description("List devices.")]
    public bool Devices { get; set; }

    [Description("Serial number. Default is first device found.")]
    public string Serial { get; set; }

    [Description("Type or types of thermocouple. Default is 'J'. " +
                 "For multiple thermocouples use '-' to denote disabled channels. " +
                 "For example '---j--k-' would set channels 4 and 7 to J and K types respectively.")]
    public string Type { get; set; }

    [Description("Channel number. Default is 1. Ignored when specifying multiple channel types.")]
    public int Channel { get; set; }

    [Description("Mains rejection frequency. Default is 50")]
    public int RejFreq { get; set; }

    [Description("Temperature units C/F/K. Default is C (centigrade).")]
    public string Unit { get; set; }
  }
}