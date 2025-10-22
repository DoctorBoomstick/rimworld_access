using HarmonyLib;
using UnityEngine;
using Verse;

namespace RimWorldAccess
{
    /// <summary>
    /// Harmony patch to add a hotkey for detailed tile information.
    /// Pressing 'I' key will announce verbose information about the current cursor tile.
    /// </summary>
    [HarmonyPatch(typeof(CameraDriver))]
    [HarmonyPatch("Update")]
    public static class DetailInfoPatch
    {
        private static float lastDetailRequestTime = 0f;
        private const float DetailRequestCooldown = 0.5f; // Prevent spam

        /// <summary>
        /// Postfix patch to check for detail info hotkey after normal camera updates.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)] // Run after other patches
        public static void Postfix(CameraDriver __instance)
        {
            // Only process during normal gameplay with a valid map
            if (Find.CurrentMap == null || !MapNavigationState.IsInitialized)
                return;

            // Don't process if any dialog or window that prevents camera motion is open
            if (Find.WindowStack != null && Find.WindowStack.WindowsPreventCameraMotion)
                return;

            // Check for detail info hotkey (I key only)
            bool detailKeyPressed = Input.GetKeyDown(KeyCode.I);

            if (detailKeyPressed)
            {
                // Cooldown to prevent accidental double-presses from causing spam
                if (Time.time - lastDetailRequestTime < DetailRequestCooldown)
                    return;

                lastDetailRequestTime = Time.time;

                // Get detailed information about the current cursor position
                IntVec3 currentPosition = MapNavigationState.CurrentCursorPosition;
                string detailedInfo = TileInfoHelper.GetDetailedTileInfo(currentPosition, Find.CurrentMap);

                // Copy to clipboard for screen reader
                ClipboardHelper.CopyToClipboard(detailedInfo);

                // Log to console as well for debugging
                MelonLoader.MelonLogger.Msg($"Detailed tile info requested for {currentPosition}");
            }
        }
    }
}
