# Surface Mining Overlay

A transparent overlay panel for EDDiscovery designed to assist with planetary surface mining in Elite Dangerous. It provides a localized radar map that tracks your position and custom mining deposits relative to a specific surface coordinate.

## Features

- **Relative Radar Map:** Visualizes the player's position relative to a chosen map center point. The map automatically scales its zoom up to a 50km radius as you travel.
- **Off-Screen Chevron:** If you travel past the 50km boundary, your player indicator pins to the edge of the map as a chevron pointing towards your actual location.
- **Deposit Tracking:** Log custom mining deposits (including resource type, rig count, distance, and bearing) at your current coordinates. Deposits persist on the map.
- **Active Range Indicator:** The Rhino's 2km scanner range is displayed around your position.
- **Local Persistence:** Data is persited in `surfaceminingmap.csv` file in your AppData directory. This file can be edited or used to port your data to another tool.

## Usage

1. **Set the Center:** Land on your chosen Mining Location Signal #. Click **Add/Resume Location Signal** and add the singal number, coordinates are read automatically.
2. **Log Resources:** Drive your SRV to a mining spot and use **Add Mining Deposit** to drop a resource marker at your current coordinates. Input # of rigs and resource type.
3. **Navigate:** The panel will automatically track your ship/SRV position, heading, and distance relative to the center origin.

## Credits
Inspired and motivated by https://github.com/ReeverOne/EDSurfaceMiningMap