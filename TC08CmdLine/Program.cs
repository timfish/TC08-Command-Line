using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Args;
using Args.Help;
using Args.Help.Formatters;
using Pico.Device.TC08;

namespace TC08CmdLine
{
  internal static class Program
  {
    private const char DisabledChannel = '-';

    /// <summary>
    /// Mains the specified arguments.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private static void Main(string[] args)
    {
      IModelBindingDefinition<CommandArgs> bindingDefinition = Configuration.Configure<CommandArgs>();

      // Show help if required
      if (args.Length == 1 && args[0].Contains("?"))
      {
        ModelHelp modelHelp = new HelpProvider().GenerateModelHelp(bindingDefinition);
        Console.Write(new ConsoleHelpFormatter().GetHelp(modelHelp));
        return;
      }

      // Bind cmd line args
      CommandArgs command = bindingDefinition.CreateAndBind(args);

      // Get a DeviceManager instance
      using (var deviceManager = new TC08DeviceManager())
      {
        // List devices if requested
        if (command.Devices)
        {
          foreach (var dev in deviceManager.Devices)
            Console.WriteLine(dev.Serial);
          return;
        }

        // Get the specified device or the first found
        TC08Device device = string.IsNullOrWhiteSpace(command.Serial)
                              ? deviceManager.Devices.FirstOrDefault()
                              : deviceManager.Devices.FirstOrDefault(d => d.Serial.ToLower() == command.Serial.ToLower());

        // Let them know if we couldn't find a device
        if (device == null)
        {
          Console.WriteLine("Device not found");
          return;
        }

        // Set mains rejection to 60Hz if req
        if (command.RejFreq > 55)
          device.FrequencyRejection = TC08DeviceImports.FreqRej.Sixty;

        StringBuilder csvHeader = new StringBuilder();
        csvHeader.Append("Time, ");

        if (command.Type.Length == 1)
        {
          SetChannel(device, command.Channel, command.Type[0]);
          csvHeader.Append("Ch");
          csvHeader.Append(command.Channel);
          csvHeader.Append(", ");
        }
        else if (command.Type.Length == 8)
        {
          for (int i = 0; i < 8; i++)
          {
            char type = command.Type[i];
            if (type == DisabledChannel) continue;

            SetChannel(device, i + 1, type);
            csvHeader.Append("Ch");
            csvHeader.Append(i + 1);
            csvHeader.Append(", ");
          }
        }
        else
        {
          Console.WriteLine(
            "Type should be 1 or 8 characters long. Diasabled channels should be included with a '-'. " +
            "For example '---j--k-' would set channels 4 and 7 to J and K types respectivly.");
          return;
        }

        // Configure device ready for capture
        device.Configure();

        // If we're in logging mode, write the CSV header
        if (command.Log)
          Console.WriteLine(csvHeader.ToString().Trim(' ', ','));

        var tempUnit = GetUnits(command);

        do
        {
          var stopWatch = Stopwatch.StartNew();
          float[] data = device.GetValues(tempUnit);

          StringBuilder csvLine = new StringBuilder();

          if (command.Log)
            csvLine.Append(DateTime.Now + ", ");

          var values = string.Join(", ", data
                                           .Skip(1) // Skip the cold junction value
                                           .Where(w => !float.IsNaN(w))
                                           // Ignore the NaNs as these channels are disabled
                                           .Select(s => s.ToString("F2")));
            // Convert them to string with 2 decimal places

          csvLine.Append(values);
          Console.WriteLine(csvLine);

          // If we're logging, ensure we wait 1 second before continuing...
          if (command.Log)
            Thread.Sleep((int) Math.Max(1, (1000 - stopWatch.ElapsedMilliseconds)));

          // Only loop if we're in log mode
        } while (command.Log);
      }
    }


    private static TC08DeviceImports.TempUnit GetUnits(CommandArgs command)
    {
      TC08DeviceImports.TempUnit units = TC08DeviceImports.TempUnit.USBTC08_UNITS_CENTIGRADE;
      if (command.Unit.Length == 1)
      {
        if (command.Unit.ToLower()[0] == 'k')
          units = TC08DeviceImports.TempUnit.USBTC08_UNITS_KELVIN;
        if (command.Unit.ToLower()[0] == 'f')
          units = TC08DeviceImports.TempUnit.USBTC08_UNITS_FAHRENHEIT;
      }
      return units;
    }

    private static void SetChannel(TC08Device device, int channel, char type)
    {
      var tcType = type.ToString().ToUpper()[0];
      if (tcType == DisabledChannel) return;

      if (TC08Device.SupportedThermocouples.Contains(tcType))
        device.EnabledChannels.Add(new TC08ChannelConfig(channel, tcType));
    }
  }
}
