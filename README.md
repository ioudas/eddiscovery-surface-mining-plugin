# Surface Mining Overlay
<img width="140" height="136" alt="Screenshot 2026-09-11 215423" src="https://github.com/user-attachments/assets/f23234d3-d311-4356-89b9-9defd720728b" />


A transparent overlay panel for EDDiscovery designed to assist with planetary surface mining in Elite Dangerous. It provides a localized radar map that tracks your position and custom mining deposits relative to a specific surface coordinate.

<img width="3436" height="1439" alt="Screenshot 2026-09-12 142626" src="https://github.com/user-attachments/assets/89f8fa22-a698-452c-895b-834bfb9de7fb" />

## Features

- **Relative Radar Map:** Visualizes the player's position relative to a chosen map center point. The map automatically scales its zoom up to a 50km radius as you travel.
- **Off-Screen Chevron:** If you travel past the 50km boundary, your player indicator pins to the edge of the map as a chevron pointing towards your actual location.
- **Deposit Tracking:** Log custom mining deposits (including resource type, rig count, distance, and bearing) at your current coordinates. Deposits persist on the map.
- **Active Range Indicator:** The Rhino's 2km scanner range is displayed around your position.
- **Local Persistence:** Data is persited in `surfaceminingmap.csv` file in your AppData directory. This file can be edited or used to port your data to another tool.

## Installation

1. Download the latest `EDSurfaceMiningOverlay.dll` from the [GitHub Releases](https://github.com/ioudas/eddiscovery-surface-mining-plugin/releases) page.
2. Right-click the file, select Properties and "Unblock" it. This is Windows "feature" when you download binaries from the web.

<img width="399" height="501" alt="image" src="https://github.com/user-attachments/assets/e7c0a186-e23d-45be-85a4-1e42596895da" />

4. Copy the `.dll` file to your EDDiscovery plugins folder, located at: `%LOCALAPPDATA%\EDDiscovery\DLL`
5. Restart EDDiscovery. The new panel will be available in the "Add Tab" or "Pop Outs" menus.

## Usage

1. **Set the Center:** Land on your chosen Mining Location Signal #. Click **Add/Resume Location Signal** and add the singal number, coordinates are read automatically.

<img width="249" height="119" alt="Screenshot 2026-09-11 215446" src="https://github.com/user-attachments/assets/6e278461-4e57-4f1e-bc87-9226157b2a4d" />

2. **Log Resources:** Drive your SRV to a mining spot and use **Add Mining Deposit** to drop a resource marker at your current coordinates. Input # of rigs and resource type.

<img width="367" height="160" alt="Screenshot 2026-09-12 142733" src="https://github.com/user-attachments/assets/5b6a895a-ce4a-403f-b64b-954188772e7c" />

3. **Navigate:** The panel will automatically track your ship/SRV position, heading, and distance relative to the center origin.

<img width="724" height="779" alt="Screenshot 2026-09-12 142646" src="https://github.com/user-attachments/assets/f761e527-04f7-4628-bea4-05a3649f0b48" />


## Credits
Inspired and motivated by https://github.com/ReeverOne/EDSurfaceMiningMap
