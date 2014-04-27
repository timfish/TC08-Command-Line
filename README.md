Pico USB TC08 Command Line 
===============================

A simple command line application to fetch data from the Pico Technology USB TC08 thermocouple data logger, without having to dig into the SDK.
It requires .NET 4.0 and the driver dll is embedded within so there are no other files required. You may have to install PicoLog so that the kernel driver is installed.
 
It has some sensible defaults and a help command so you can get temperatures quickly with no knowledge of the driver API. You can also redirect the output to a file (>) and log the results continuously.
 
Check out some examples:
 
**Default: Channel 1, J type and degC, single result**

    > TC08CmdLine.exe
    > 20.13
 
**Channel 2**

    > TC08CmdLine.exe -c 2
    > 18.13
 
**Channel 2, K type**

    > TC08CmdLine.exe -c 2 -t K
    > 18.13
 
**Channel 4 and 7 as J and K**

    > TC08CmdLine.exe -t ---j--k-
    > 18.13, 20.75
 
**List devices**

    > TC08CmdLine.exe -d
    > QJY22/387
 
**Device QJY22/387 Channel 4 and 7 as J and K**

    > TC08CmdLine.exe -s QJY22/387 -t ---j--k-
    > 18.13, 20.75
 
**Log all 8 channels to CSV file until stopped (includes a header row!)**

    > TC08CmdLine.exe –l -t jjjjjjjj > output.csv
 
**Help**

    > TC08CmdLine.exe -?
    > TC08 Command line application
    > <command> [-Log|-L] [-Serial|-S] [-Type|-T] [-Channel|-C] [-RejFreq|-R]
    >           [-Unit|-U]
    > 
    > 
    > [-Log|-L]         Switch to continuous logging mode. Loops until closed.
    > [-Devices|-D]     List devices.
    > [-Serial|-S]      Serial number. Default is first device found.
    > [-Type|-T]        Type or types of thermocouple. Default is 'J'. For multiple
    >                   thermocouples use '-' to denote disabled channels. For
    >                   example '---j--k-' would set channels 4 and 7 to J and K
    >                   types respectively.
    > [-Channel|-C]     Channel number. Default is 1. Ignored when specifying
    >                   multiple channel types.
    > [-RejFreq|-R]     Mains rejection frequency. Default is 50
    > [-Unit|-U]        Temperature units C/F/K. Default is C (centigrade).
