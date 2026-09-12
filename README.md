# Surface Mining Overlay
<img width="140" height="136" alt="Screenshot 2026-09-11 215423" src="https://github.com/user-attachments/assets/f23234d3-d311-4356-89b9-9defd720728b" />


A transparent overlay panel for EDDiscovery designed to assist with planetary surface mining in Elite Dangerous. It provides a localized radar map that tracks your position and custom mining deposits relative to a specific surface coordinate.

<img width="3439" height="1439" alt="Screenshot 2026-09-11 210448" src="https://github.com/user-attachments/assets/0fefa892-55c2-46a0-80a1-d51e1587e477" />

## Features

- **Relative Radar Map:** Visualizes the player's position relative to a chosen map center point. The map automatically scales its zoom up to a 50km radius as you travel.
- **Off-Screen Chevron:** If you travel past the 50km boundary, your player indicator pins to the edge of the map as a chevron pointing towards your actual location.
- **Deposit Tracking:** Log custom mining deposits (including resource type, rig count, distance, and bearing) at your current coordinates. Deposits persist on the map.
- **Active Range Indicator:** The Rhino's 2km scanner range is displayed around your position.
- **Local Persistence:** Data is persited in `surfaceminingmap.csv` file in your AppData directory. This file can be edited or used to port your data to another tool.

## Installation

1. Download the latest `EDSurfaceMiningOverlay.dll` from the [GitHub Releases]([url](https://github.com/ioudas/eddiscovery-surface-mining-plugin/releases)) page.
2. Copy the `.dll` file to your EDDiscovery plugins folder, located at: `%LOCALAPPDATA%\EDDiscovery\DLL`
3. Restart EDDiscovery. The new panel will be available in the "Add Tab" or "Pop Outs" menus.

## Usage

1. **Set the Center:** Land on your chosen Mining Location Signal #. Click **Add/Resume Location Signal** and add the singal number, coordinates are read automatically.
<img width="249" height="119" alt="Screenshot 2026-09-11 215446" src="https://github.com/user-attachments/assets/6e278461-4e57-4f1e-bc87-9226157b2a4d" />
2. **Log Resources:** Drive your SRV to a mining spot and use **Add Mining Deposit** to drop a resource marker at your current coordinates. Input # of rigs and resource type.
<img width="401" height="163" alt="Screenshot 2026-09-11 211652" src="https://github.com/user-attachments/assets/cd149e0e-37c2-415f-a6c2-d5b369411723" />
3. **Navigate:** The panel will automatically track your ship/SRV position, heading, and distance relative to the center origin.
<img width="672" height="690" alt="Screenshot 2026-09-11 220254" src="https://github.com/user-attachments/assets/a209eb1f-4343-4a62-a0ae-762f396fff9e" />


## Credits
Inspired and motivated by https://github.com/ReeverOne/EDSurfaceMiningMap
