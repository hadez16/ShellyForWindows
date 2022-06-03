# ShellyForWindows
A little tool for displaying the current power consumption of Shelly Gen1 API devices in the Windows system tray

## Description
The tool is for all Gen1 Shelly Devices who support the /meter/0 REST endpoint for receiving the current power consumption e.g. a Shelly PlugS.

Refer to https://shelly-api-docs.shelly.cloud/gen1/#shelly-family-overview for an overview.

The config.ini file contains 1-to-n Shelly devices which are declared by its Name as the Section name, IP address, color for distinguishing in the System Tray and update interval in seconds.

```
[Balkonkraftwerk1]
IPAddress=192.168.2.66
Color=red
UpdateInterval=5
```

The possible values for "Color" refer to the possible .NET color values.
For adding more Shelly devices, just copy-paste the whole block and change the values accordingly.

Refer to http://www.flounder.com/csharp_color_table.htm for possible values. Use them without the Dot like in the INI example above.

## How-To and Best Practice
* Download the releae or build the solution by yourself
* Edit the .ini to your needs
* Run the ShellyGen1Tray.eye

***It is highly recommended to assign a static IP address to your Shelly device because DHCP servers could assign another IP address what will break your config.ini configuration***

If a Shelly device is not reachable on startup, a message box will appear and the device is skipped. If all devices are not reachable, the application exits.
