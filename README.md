# Surface Mining Overlay
<img width="140" height="136" alt="image" src="https://github.com/user-attachments/assets/9ed59006-e51c-4c07-b351-701de0480173" />


A transparent overlay panel for EDDiscovery designed to assist with planetary surface mining in Elite Dangerous. It provides a localized radar map that tracks your position and custom mining deposits relative to a specific surface coordinate.


<img width="3439" height="1439" alt="Screenshot 2026-09-11 210448" src="https://github.com/user-attachments/assets/1b2dd2df-b69a-47cc-92c3-54c7a5600bbc" />

## Features

- **Relative Radar Map:** Visualizes the player's position relative to a chosen map center point. The map automatically scales its zoom up to a 50km radius as you travel.
- **Off-Screen Chevron:** If you travel past the 50km boundary, your player indicator pins to the edge of the map as a chevron pointing towards your actual location.
- **Deposit Tracking:** Log custom mining deposits (including resource type, rig count, distance, and bearing) at your current coordinates. Deposits persist on the map.
- **Active Range Indicator:** The Rhino's 2km scanner range is displayed around your position.
- **Local Persistence:** Data is persited in `surfaceminingmap.csv` file in your AppData directory. This file can be edited or used to port your data to another tool.

## Usage

1. **Set the Center:** Land on your chosen Mining Location Signal #. Click **Add/Resume Location Signal** and add the singal number, coordinates are read automatically.
2. <img width="249" height="119" alt="image" src="https://github.com/user-attachments/assets/bfc435e9-339c-45b2-b65c-a66a2949b96c" />

3. **Log Resources:** Drive your SRV to a mining spot and use **Add Mining Deposit** to drop a resource marker at your current coordinates. Input # of rigs and resource type.
4. <img width="401" height="163" alt="Screenshot 2026-09-11 211652" src="https://github.com/user-attachments/assets/4e47a077-38ef-4e62-b275-56aec4f9de3e" />

5. **Navigate:** The panel will automatically track your ship/SRV position, heading, and distance relative to the center origin.
<img width="675" height="680" alt="image" src="https://github.com/user-attachments/assets/56c1270f-5df6-4721-9d70-d23980d5b7fd" />

## Credits
Inspired and motivated by https://github.com/ReeverOne/EDSurfaceMiningMap
