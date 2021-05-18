# NLogWPFViewer
Modernized clone with multi-targeting. Original project: https://github.com/erizet/NlogViewer

It is intentionally not a nuget. I believe its better if you adopt the code for your needs as it is pretty basic but amount of UI tweaks can be confusing if done through dependency properties and most of them would not be needed in most of the use-cases.

So download and re-use the code the way you like it.

Use `TargetName` property for multi-targeting. Omit it completely or set to null if you only need single target.

NLog config example with 2 targets:

```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="Info"
      internalLogFile="..\..\logs\internal-nlog.log">

  <extensions>
    <add assembly="NLogWPFViewer" />
  </extensions>
  
  <targets async="true">
    <target xsi:type="NLogWPFViewerTarget" name="control1" />
    <target xsi:type="NLogWPFViewerTarget" name="control2" />
  </targets>

  <rules>
    <logger name="MyClassA*" minlevel="Trace" writeTo="control1" />
    <logger name="MyClassB*" minlevel="Trace" writeTo="control2" />
  </rules>
  
</nlog>
```

Usage in WPF example (note `TargetName` matches target names in NLog config):

```
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="0.45*"/>
            <ColumnDefinition Width="3"/>
            <ColumnDefinition Width="0.55*"/>
        </Grid.ColumnDefinitions>

        <nvw:NLogWPFViewer x:Name="maxdataCtrl" TargetName="control1" Grid.Column="0" MaxCount="1000" />

        <GridSplitter Grid.Row="1" Grid.Column="1" Width="3" HorizontalAlignment="Center" VerticalAlignment="Stretch" />
        
        <nvw:NLogWPFViewer x:Name="dtcCtrl" TargetName="control2" Grid.Column="2" MaxCount="1000" />

    </Grid>
```

Other useful things:

`MaxCount` property (minimum 50) - is how many lines you want to keep in memory. once the limit is reached the older lines will be deleted as the new ones come.

`LogEventInfo[] GetLogEntries()` - gives you current snapshot of the log entries.

`bool AutoScroll` - gets/sets if the grid auto-scrolls to the newest line.


