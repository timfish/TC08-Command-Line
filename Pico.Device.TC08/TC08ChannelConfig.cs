using System;

namespace Pico.Device.TC08
{
  public class TC08ChannelConfig
  {
    public TC08ChannelConfig(int number)
    {
      Number = number;
    }

    public TC08ChannelConfig(int number, char type)
      : this(number)
    {
      ThermoType = type;
    }

    public int Number { get; set; }
    public char ThermoType { get; set; }

    public override bool Equals(object obj)
    {
      TC08ChannelConfig newOne = obj as TC08ChannelConfig;

      if(newOne == null)
        throw new ArgumentException("Not TC08ChannelConfig", "obj");

      return Number.Equals(newOne.Number) && ThermoType.Equals(newOne.ThermoType);
    }
  }
}