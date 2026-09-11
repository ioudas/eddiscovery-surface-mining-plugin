# Starting state of the project
This project is direct clone from https://github.com/EDDiscovery/EDDiscovery, however it has another project cloned into it in directory EDSurfaceMiningMap (source https://github.com/ReeverOne/EDSurfaceMiningMap).

Start by understanding both projects, their purpose and how the plugin system works in EDDiscovery.

# Goal
The goal is to create a new plugin for EDDiscovery for the newly added surface mining using Rhino vehicle (google it).
The plugin should utilize the mapping functionality from EDSurfaceMiningMap, but instead of manually inputing coordinates (longtitude/latitude), 
they should be read automatically, utlizing EDDiscovery's ability to do so (see e.g. the Compass plugin, which does it).

Name of plugin is Surface Mining Overlay.

# Typical user flows
## Flow 1 - surveying
1. user performs DSS of a planet. This reveals new navigational locations such as:
- PLANETARY MINING LOCATION SIGNAL (1)
- PLANETARY MINING LOCATION SIGNAL (2)
- PLANETARY MINING LOCATION SIGNAL (X) - up to 99 is valid
2. user picks one location flies to it and deploys the Rino mining vehicle
3. User interacts with the plugin to save the selected MINING LOCATION SIGNAL (they input only the 2 digit number). The new location is now centered on the map overlay (see below)
3. user performs Surface Scan, which reveals the resources available at the deposit in a 2km radius
4. user drives to a chosen deposit (thus making exact coordinates available to EDDiscovery) 
5. user interacts with the plugin to save the mining deposit in a persistent way (research how EDDiscovery persists user data)

## Flow 2 - mining
1. user opens the plugin overlay and browses previously saved data to select a deposit
2. user flies to the location and deploys the Rhino mining vehicle
3. user uses the map overlay to drive to the exact deposit

# Overlay
the plugin should provide an overlay with:
1. the map itself. The map is very similar to the one from EDSurfaceMiningMap. The visual can be copied. If there is no mining location currently selected, the map is empty. When mining location is selected, the following is displayed on the map:
  - red dot in the center representing the mining location signal with the given number. The center dot is red and reads "LOCATION X" where is is the 2-digit number.
  - player's current position on the map
  - the mining deposits inputed by the player (if any)

2. button "Add/Resume new Mining location signal" 
  2a. opens new window with Save/Cancel button
  2a. player will input the signal number (2 digit number)
  2b. this will create a new mining location and start the resource mapping process. The map will reset to this mining location.
  2c. the player can also select previously added mining location from dropdown, which will load the historical data for it.

3. button "Add new Mining Deposit", when clicked opens a window:
  2a. will require these inputs from user: 
    - Resource: drop down from possible values (see Possible resources), with option for free text. Make the possible resources  easily editable. Research how EDDiscovery handles static data like this and use the same approach. The free text should not be added to possible resources automatically, only saved for purposes of rendering the map from historical data.
    - Rigs (number 1-10)
  2b. Save and Cancel button
  2c. Save will store the information in a  persistent way (research how EDDiscovery does it and use  the same approach)

# Considirations:
- when player is not on ground, there are no valid surface coordinates. The plugin can still display any of the stored data on the overlay map, but the player position is absent.


# Possible resources
Palladium, Gold, Silver, Bertrandite, Indite, Gallite, Coltan, Uraninite, Lepidolite, Cobalt, Rutile, Water,
Iridium, Helium, Helium-3, Bastnasite, Deuterium, Thortveitite, Quartz Pyroxenite, Olivine, Periclase Dunite, Sapphire, Diamond, Ruby, Magnesite