RimWorld Access - screen reader accessibility for RimWorld
==========================================

OVERVIEW
--------
This mod adds keyboard navigation and screen reader support to RimWorld.
All selections are copied to the clipboard for screen reader accessibility.

Installation:
1. Install RimWorld, and run it once to set up the folder structure correctly.
2. Install mellon loader for RimWorld.
3. Copy the RimWorld_access.dll to the RimWorld mods folder.
4. Launch the game.  UI text should be coppied to your clipboard.

MAIN MENU
---------
Arrow Keys    Navigate menu options
Enter         Select menu item


IN-GAME NAVIGATION
------------------
Arrow Keys    Move map cursor (announces terrain, buildings, items, pawns)
, (comma)     Cycle to previous colonist
. (period)    Cycle to next colonist
Enter         Open inspection menu at cursor position
Escape        Open pause menu

(Note: All menus use Arrow Keys to navigate, Enter to confirm, Escape to close)


TILE INFORMATION (Keys 1-5)
----------------------------
While navigating the map with arrow keys, press these keys to get specific information
about the tile under the cursor:

1             List all items and pawns at cursor position
2             Show flooring details (terrain type, beauty, cleanliness, path cost)
3             Display plant information (species, growth %, harvestable status)
4             Report light level and glow value
5             Show temperature (°C) and indoor/outdoor status


TIME CONTROLS
-------------
Shift+1       Set time speed to Normal (with sound feedback)
Shift+2       Set time speed to Fast (with sound feedback)
Shift+3       Set time speed to Superfast (with sound feedback)
Space         Pause/unpause game


BUILD SYSTEM (Press A)
----------------------
1. Select category (Orders, Structure, Production, etc.)
2. Select build tool (Wall, Door, Bed, etc.)
3. If required, select material (Wood, Steel, etc.)
4. Use Arrow Keys to position, space to place, enter to confirm, or Escape to cancel


ZONE SYSTEM (Press Z)
---------------------
1. Select zone type (Growing zone, Stockpile, etc.)
2. Use Arrow Keys to navigate, space to add/remove cells, enter to confirm, or escape to cancel.


SCHEDULE MENU (Press S)
------------------------
Manage colonist work schedules across all 24 hours:

Arrow Keys    Navigate grid (Up/Down: pawns, Left/Right: hours)
Tab           Cycle assignment type and apply to current cell (Anything/Work/Joy/Sleep/Meditate)
Space         Apply current selection to cell
Shift+Right   Fill rest of row with selected assignment
Ctrl+C        Copy current pawn's entire schedule
Ctrl+V        Paste copied schedule to current pawn
Enter         Save all pending changes and close
Escape        Cancel changes and close



COLONIST ACTIONS
----------------
]             Open order menu (includes draft mode orders like Move, Attack)
R             Toggle draft mode for selected colonist
Alt+M         Display mood information and thoughts
Alt+N         Display needs
Alt+H         Display health
Alt+W         Open work menu (see Work Menu section below)
Alt+G         Display gear
S             Open schedule menu (manage colonist timetables)
Shift+A       Open assign menu (manage outfits, food, drugs, areas, reading)


WORK MENU (Press Alt+W)
-----------------------
Manage colonist work assignments with support for both simple mode and manual priorities.

Common Controls:
Up/Down       Navigate work types (stops at top/bottom)
Tab           Switch to next colonist
Shift+Tab     Switch to previous colonist
M             Toggle between simple mode and manual priorities mode
Enter         Save all changes and close
Escape        Cancel changes and close

Simple Mode (toggle work types on/off):
Space         Toggle selected work type enabled/disabled

Manual Priority Mode (assign priority numbers 1-4):
0-4           Set priority directly (0=disabled, 1=highest, 4=lowest)
Space         Quick toggle between disabled and default (priority 3)
Shift+Up      Move work type up in priority order (affects execution when priorities equal)
Shift+Down    Move work type down in priority order (affects execution when priorities equal)

Notes:
- In manual priority mode, lower numbers execute first (1 before 2 before 3 before 4)
- Work types with the same priority number use list order as tiebreaker
- Changes are marked as "(pending)" until you press Enter to save
- Column reordering (Shift+Up/Down) affects all colonists globally.  As far as I am aware, this is an unavoidable quirk of the game's logic, and not something I can change.


ASSIGN MENU (Press Shift+A)
----------------------------
Manage colonist assignments across five policy categories: Outfit, Food Restrictions,
Drug Policies, Allowed Areas, and Reading Policies (Ideology DLC only).

Navigation:
Left/Right    Switch between policy categories (columns)
Up/Down       Navigate available policies within current category
Enter         Apply selected policy to current colonist
Tab           Switch to next colonist
E         Open policy editor/manager for current category
Escape        Close assign menu

Policy Editor Controls:
When you press E to open a policy editor, you can manage policies:

Up/Down       Navigate policy list (in policy list mode)
Tab           Enter actions menu
escape     Return to policy list
Enter         Execute selected action or confirm
Escape        Go back/close editor

Available Actions:
- New Policy: Create a new policy
- Rename Policy: Rename the selected policy
- Duplicate Policy: Make a copy of the selected policy
- Delete Policy: Delete the selected policy (if not in use)
- Set as Default: Set as the default policy for new colonists
- Edit Filter: Edit apparel/food filters (navigate with arrows, Space to toggle, Enter to expand/collapse)
- Edit Drugs: Configure drug usage settings (navigate drugs, edit dosage schedules and conditions)
- Close: Return to assign menu

Notes:
- Outfit policies let you control what clothes colonists will wear
- Food policies restrict what foods colonists can eat
- Drug policies control when and how colonists use drugs
- Allowed areas restrict where colonists can go
- Reading policies (Ideology DLC) control what colonists read for ideological development.  These policies are untested, and the custom editor is not yet supported.  I do not have the DCLs yet, so I will be saving all DLC features for later.  


QUICK NAVIGATION (Press J)
---------------------------
Opens jump menu with categories:
- Colonists
- Items
- Events
- Buildings

Left/Right    Expand/collapse categories
Up/Down       Navigate items
Enter         Jump to selected item


OTHER SHORTCUTS
---------------
Delete        Delete selected save file (in save/load menu)
p      open research project selection menu.

ADDITIONAL NOTES
----------------
This is a closed beta.  Many features are not supported, and those that are may have unexpected bugs or interactions.  
