# ShellyForWindows
A little tool for displaying the current power consumption of Shelly Gen1 API devices in the Windows system tray

![2022-06-03 07_01_45-](https://user-images.githubusercontent.com/106725788/171790851-6a66ce45-4ccf-40f6-ae2e-c5fd2d825bfb.png)

## Description
The tool is for all Shelly Devices who support either Gen1 API or Gen2 API and have a /meter/ or Switch endpoint for receiving the current power consumption e.g. a Shelly PlugS or Shelly Plus 1 PM.

Refer to https://shelly-api-docs.shelly.cloud/gen1/#shelly-family-overview for an overview of Gen1 API Devices.

Refer to https://shelly-api-docs.shelly.cloud/gen2/ (Section "Devices") for an overview of Gen2 API Devices.

The config.ini file contains 1-to-n Shelly devices which are declared by its Name as the Section name, IP address, color for distinguishing in the System Tray and update interval in seconds.

Multiple sensors per Shelly is supported (like for Shelly 2.5 etc.).

The number of Sensors and the API Generation has to be declared in the config.ini

```
[Balkonkraftwerk1]
IPAddress=192.168.2.66
Color=red
UpdateInterval=5
NumberOfSensors=1
APIgeneration=1
```

The possible values for "Color" refer to the possible .NET color values.
For adding more Shelly devices, just copy-paste the whole block and change the values accordingly.

Refer to http://www.flounder.com/csharp_color_table.htm for possible values. Use them without the Dot like in the INI example above.

***The tool uses the IP address of the Shelly device in your Local Area Network! No cloud support (yet)***

## How-To and Best Practice
* Download the releae or build the solution by yourself
* Edit the .ini to your needs
* Run the ShellyTray.exe

***It is highly recommended to assign a static IP address to your Shelly device because DHCP servers could assign another IP address what will break your config.ini configuration***

If a Shelly device is not reachable on startup, a message box will appear and the device is skipped. If all devices are not reachable, the application exits.

### How-to exit...
Just right-click on the tray icon and use "Exit".
If you have more than one devices configured, the Application exits after the last Shelly icon is closed.

## Donations

Whoever likes to donate some Euros...

https://www.paypal.com/paypalme/hadez16Tools
