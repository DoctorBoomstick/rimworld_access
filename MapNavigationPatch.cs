using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimWorldAccess
{
    /// <summary>
    /// Harmony patch for CameraDriver.Update() to add accessible map navigation.
    /// Intercepts arrow key input to move a cursor tile-by-tile instead of panning the camera.
    /// The camera follows the cursor, keeping it centered on screen.
    /// </summary>
    [HarmonyPatch(typeof(CameraDriver))]
    [HarmonyPatch("Update")]
    public static class MapNavigationPatch
    {
        private static bool hasAnnouncedThisFrame = false;

        /// <summary>
        /// Prefix patch that intercepts arrow key input before the camera's normal panning behavior.
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix(CameraDriver __instance)
        {
            // Reset per-frame flag
            hasAnnouncedThisFrame = false;

            // Only process input during normal gameplay with a valid map
            if (Find.CurrentMap == null)
            {
                MapNavigationState.Reset();
                return;
            }

            // Don't process arrow keys if any dialog or window that prevents camera motion is open
            if (Find.WindowStack != null && Find.WindowStack.WindowsPreventCameraMotion)
            {
                return;
            }

            // Initialize cursor position if needed
            if (!MapNavigationState.IsInitialized)
            {
                MapNavigationState.Initialize(Find.CurrentMap);

                // Announce starting position
                string initialInfo = TileInfoHelper.GetTileSummary(MapNavigationState.CurrentCursorPosition, Find.CurrentMap);
                ClipboardHelper.CopyToClipboard(initialInfo);
                MapNavigationState.LastAnnouncedInfo = initialInfo;
                hasAnnouncedThisFrame = true;
                return;
            }

            // Check for arrow key input
            IntVec3 moveOffset = IntVec3.Zero;
            bool keyPressed = false;

            // Check each arrow key direction
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                moveOffset = IntVec3.North; // North is positive Z
                keyPressed = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveOffset = IntVec3.South; // South is negative Z
                keyPressed = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveOffset = IntVec3.West; // West is negative X
                keyPressed = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveOffset = IntVec3.East; // East is positive X
                keyPressed = true;
            }

            // If an arrow key was pressed, move the cursor and update camera
            if (keyPressed)
            {
                // Move the cursor position
                bool positionChanged = MapNavigationState.MoveCursor(moveOffset, Find.CurrentMap);

                if (positionChanged)
                {
                    // Get the new cursor position
                    IntVec3 newPosition = MapNavigationState.CurrentCursorPosition;

                    // Move camera to center on new cursor position
                    __instance.JumpToCurrentMapLoc(newPosition);

                    // Get tile information and announce it
                    string tileInfo = TileInfoHelper.GetTileSummary(newPosition, Find.CurrentMap);

                    // Only announce if different from last announcement (avoids spam when hitting map edge)
                    if (tileInfo != MapNavigationState.LastAnnouncedInfo)
                    {
                        ClipboardHelper.CopyToClipboard(tileInfo);
                        MapNavigationState.LastAnnouncedInfo = tileInfo;
                        hasAnnouncedThisFrame = true;
                    }
                }
                else
                {
                    // Cursor at map boundary - optionally announce boundary
                    if (!hasAnnouncedThisFrame)
                    {
                        ClipboardHelper.CopyToClipboard("Map boundary");
                        hasAnnouncedThisFrame = true;
                    }
                }

                // Consume the arrow key event to prevent default camera panning
                // This is done by preventing the KeyBindingDefOf checks from succeeding
                // Note: We're using Input.GetKeyDown instead of KeyBindingDefOf to intercept earlier
            }
        }

        /// <summary>
        /// Postfix patch to prevent default camera dolly movement when we've handled arrow keys.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(CameraDriver __instance)
        {
            // If we announced something this frame, it means we handled arrow key input
            // The camera jump already happened in Prefix, but we need to ensure
            // no residual dolly movement occurs
            if (hasAnnouncedThisFrame)
            {
                // Reset velocity to prevent any accumulated movement
                Traverse.Create(__instance).Field("velocity").SetValue(Vector3.zero);
                Traverse.Create(__instance).Field("desiredDollyRaw").SetValue(Vector2.zero);
            }
        }
    }
}
