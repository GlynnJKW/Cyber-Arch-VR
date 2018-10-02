Please note that you must be running on a 64 bit operating system!

Setup:
Grab all files from the CAVEKiosk folder in the OneDrive and put them in a folder on the intended computer. Double-click "Cyber-Arch-CAVE.exe" and then click "Play!" to launch. You should see the earth on a starry background, as well as a CAVEKiosk logo and a controls popup. At this point you will need to be added to the database and change the config files to work with your own setup.

Database:
To use the new version of the kiosk app, you must first provide the IP of the computer the app will run on so that it can access the database containing data and configuration files for the data.

Configuration:
There are two local config files inside the Config_Files folder: cave_settings.json and screen_config.txt
cave_settings.json controls 3 basic aspects of the application, and screen_config.txt controls how everything is displayed.
DO NOT CHANGE THE NAMES OR LOCATIONS OF THESE FILES. They are referred to by path in the application, and removing one without replacing it can break the application.

cave_settings.json
 - "pathToDataJSONFile": the path to the site configuration file. This should be "//lib-staging.ucsd.edu/cavebase-public/unity-data/config_3.json" and should not be changed unless you want to point to local data only.
 - "runIn3D": determines if the application will display in 3D or not.
 - "loadAllDataOnStart": when true, the application will attempt to load the data for all sites upon launch. This can take a long time, but will greatly reduce site load times after finishing.

screen_config.txt
screen_config.txt is formatted like an XML file. The provided screen_config is set up for a 6-screen CAVE system. If you have a different setup, there are two sections that you may want to edit:
 - <ViewerPosition>, inside <GLOBAL>: ViewerPosition has x, y, and z attributes, which determine the default position of the camera in world space.
 - <Screen>, inside <ScreenConfig>: Each <Screen> tag defines a screen that will be rendered to by the application.
     - height: the height of the screen.
     - width: the width of the screen.
     - originX: screen's distance from the camera on the camera's X axis (left/right).
     - originY: screen's distance from the camera on the camera's Z axis (forward/back). This should be >1000 as otherwise the planes that are being rendered onto will show up in the application.
     - originZ: screen's distance from the camera on the camera's Y axis (up/down).
     - h: horizontal rotation of the screen compared to the camera (in degrees).
     - r: rotation of the screen on its Z-axis. This should always be 0/180 for landscape or 90/-90 for portrait.
     - p/comment/name: you can safely ignore these attributes, although comment/name can be helpful for organization purposes.

If you want to use your own local data in the application, the public (//lib-staging.ucsd.edu/cavebase-public/unity-data/config_3.json) config file provides a good base to be copied and modified as needed.