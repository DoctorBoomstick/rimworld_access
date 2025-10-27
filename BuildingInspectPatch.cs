using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimWorldAccess
{
    /// <summary>
    /// Harmony patch to handle keyboard input for building inspection menus.
    /// Intercepts keyboard events when BuildingInspectState, BillsMenuState, BillConfigState, or ThingFilterMenuState is active.
    /// </summary>
    [HarmonyPatch(typeof(UIRoot))]
    [HarmonyPatch("UIRootOnGUI")]
    public static class BuildingInspectPatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.VeryHigh)] // Run before other patches
        public static void Prefix()
        {
            // Only process keyboard events
            if (Event.current.type != EventType.KeyDown)
                return;

            KeyCode key = Event.current.keyCode;

            // Handle WindowlessFloatMenuState (highest priority - used for recipe selection and other submenus)
            if (WindowlessFloatMenuState.IsActive)
            {
                // This is handled in UnifiedKeyboardPatch, so just return
                MelonLoader.MelonLogger.Msg($"BuildingInspectPatch: WindowlessFloatMenuState is active, key = {key}");
                return;
            }

            // Handle ThingFilterMenuState (second highest priority - it's a submenu)
            if (ThingFilterMenuState.IsActive)
            {
                MelonLoader.MelonLogger.Msg($"BuildingInspectPatch: ThingFilterMenuState is active, key = {key}");
                HandleThingFilterInput();
                return;
            }

            // Handle BillConfigState (third priority)
            if (BillConfigState.IsActive)
            {
                MelonLoader.MelonLogger.Msg($"BuildingInspectPatch: BillConfigState is active, key = {key}");
                HandleBillConfigInput();
                return;
            }

            // Handle BillsMenuState (fourth priority)
            if (BillsMenuState.IsActive)
            {
                MelonLoader.MelonLogger.Msg($"BuildingInspectPatch: BillsMenuState is active, key = {key}");
                HandleBillsMenuInput();
                return;
            }

            // Handle BuildingInspectState (lowest priority)
            if (BuildingInspectState.IsActive)
            {
                MelonLoader.MelonLogger.Msg($"BuildingInspectPatch: BuildingInspectState is active, key = {key}");
                HandleBuildingInspectInput();
                return;
            }
        }

        private static void HandleBuildingInspectInput()
        {
            KeyCode key = Event.current.keyCode;

            switch (key)
            {
                case KeyCode.LeftArrow:
                    BuildingInspectState.SelectPreviousTab();
                    Event.current.Use();
                    break;

                case KeyCode.RightArrow:
                    BuildingInspectState.SelectNextTab();
                    Event.current.Use();
                    break;

                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    BuildingInspectState.OpenCurrentTab();
                    Event.current.Use();
                    break;

                case KeyCode.Escape:
                    BuildingInspectState.Close();
                    ClipboardHelper.CopyToClipboard("Closed building inspection");
                    Event.current.Use();
                    break;
            }
        }

        private static void HandleBillsMenuInput()
        {
            KeyCode key = Event.current.keyCode;
            MelonLoader.MelonLogger.Msg($"HandleBillsMenuInput: key = {key}");

            switch (key)
            {
                case KeyCode.UpArrow:
                    MelonLoader.MelonLogger.Msg("HandleBillsMenuInput: UpArrow pressed");
                    BillsMenuState.SelectPrevious();
                    Event.current.Use();
                    break;

                case KeyCode.DownArrow:
                    MelonLoader.MelonLogger.Msg("HandleBillsMenuInput: DownArrow pressed");
                    BillsMenuState.SelectNext();
                    Event.current.Use();
                    break;

                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    MelonLoader.MelonLogger.Msg("HandleBillsMenuInput: Enter pressed");
                    BillsMenuState.ExecuteSelected();
                    Event.current.Use();
                    break;

                case KeyCode.Delete:
                    MelonLoader.MelonLogger.Msg("HandleBillsMenuInput: Delete pressed");
                    BillsMenuState.DeleteSelected();
                    Event.current.Use();
                    break;

                case KeyCode.C:
                    if (Event.current.control)
                    {
                        MelonLoader.MelonLogger.Msg("HandleBillsMenuInput: Ctrl+C pressed");
                        BillsMenuState.CopySelected();
                        Event.current.Use();
                    }
                    break;

                case KeyCode.Escape:
                    MelonLoader.MelonLogger.Msg("HandleBillsMenuInput: Escape pressed");
                    BillsMenuState.Close();
                    ClipboardHelper.CopyToClipboard("Closed bills menu");
                    Event.current.Use();
                    break;
            }
        }

        private static void HandleBillConfigInput()
        {
            KeyCode key = Event.current.keyCode;

            switch (key)
            {
                case KeyCode.UpArrow:
                    BillConfigState.SelectPrevious();
                    Event.current.Use();
                    break;

                case KeyCode.DownArrow:
                    BillConfigState.SelectNext();
                    Event.current.Use();
                    break;

                case KeyCode.LeftArrow:
                    BillConfigState.AdjustValue(-1);
                    Event.current.Use();
                    break;

                case KeyCode.RightArrow:
                    BillConfigState.AdjustValue(1);
                    Event.current.Use();
                    break;

                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    BillConfigState.ExecuteSelected();
                    Event.current.Use();
                    break;

                case KeyCode.Escape:
                    BillConfigState.Close();
                    ClipboardHelper.CopyToClipboard("Closed bill configuration");

                    // Go back to bills menu
                    if (BuildingInspectState.SelectedBuilding is IBillGiver billGiver)
                    {
                        BillsMenuState.Open(billGiver, BuildingInspectState.SelectedBuilding.Position);
                    }

                    Event.current.Use();
                    break;
            }
        }

        private static void HandleThingFilterInput()
        {
            // Check if range edit submenu is active
            if (RangeEditMenuState.IsActive)
            {
                HandleRangeEditInput();
                return;
            }

            KeyCode key = Event.current.keyCode;

            switch (key)
            {
                case KeyCode.UpArrow:
                    ThingFilterMenuState.SelectPrevious();
                    Event.current.Use();
                    break;

                case KeyCode.DownArrow:
                    ThingFilterMenuState.SelectNext();
                    Event.current.Use();
                    break;

                case KeyCode.RightArrow:
                    ThingFilterMenuState.ExpandOrToggleOn();
                    Event.current.Use();
                    break;

                case KeyCode.LeftArrow:
                    ThingFilterMenuState.CollapseOrToggleOff();
                    Event.current.Use();
                    break;

                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    ThingFilterMenuState.ToggleCurrent();
                    Event.current.Use();
                    break;

                case KeyCode.Escape:
                    ThingFilterMenuState.Close();
                    ClipboardHelper.CopyToClipboard("Closed thing filter menu");
                    Event.current.Use();
                    break;
            }
        }

        private static void HandleRangeEditInput()
        {
            KeyCode key = Event.current.keyCode;

            switch (key)
            {
                case KeyCode.UpArrow:
                    RangeEditMenuState.SelectPrevious();
                    Event.current.Use();
                    break;

                case KeyCode.DownArrow:
                    RangeEditMenuState.SelectNext();
                    Event.current.Use();
                    break;

                case KeyCode.LeftArrow:
                    RangeEditMenuState.DecreaseValue();
                    Event.current.Use();
                    break;

                case KeyCode.RightArrow:
                    RangeEditMenuState.IncreaseValue();
                    Event.current.Use();
                    break;

                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    // Apply changes and return to thing filter menu
                    if (RangeEditMenuState.ApplyAndClose(out var hitPoints, out var quality))
                    {
                        ThingFilterMenuState.ApplyRangeChanges(hitPoints, quality);
                        ClipboardHelper.CopyToClipboard("Applied range changes");
                    }
                    Event.current.Use();
                    break;

                case KeyCode.Escape:
                    RangeEditMenuState.Close();
                    ClipboardHelper.CopyToClipboard("Cancelled range editing");
                    Event.current.Use();
                    break;
            }
        }
    }
}
