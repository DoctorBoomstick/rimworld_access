using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace RimWorldAccess
{
    /// <summary>
    /// Navigation level for the options menu.
    /// </summary>
    public enum OptionsMenuLevel
    {
        CategoryList,    // Top level: choosing a category
        SettingsList     // Inside a category: adjusting settings
    }

    /// <summary>
    /// Manages a windowless accessible options menu.
    /// Two-level navigation: categories -> settings within category.
    /// </summary>
    public static class WindowlessOptionsMenuState
    {
        private static bool isActive = false;
        private static OptionsMenuLevel currentLevel = OptionsMenuLevel.CategoryList;
        private static int selectedCategoryIndex = 0;
        private static int selectedSettingIndex = 0;
        private static List<OptionCategory> categories = new List<OptionCategory>();

        public static bool IsActive => isActive;

        /// <summary>
        /// Opens the options menu at the category list level.
        /// </summary>
        public static void Open()
        {
            isActive = true;
            currentLevel = OptionsMenuLevel.CategoryList;
            selectedCategoryIndex = 0;
            selectedSettingIndex = 0;

            // Close pause menu
            WindowlessPauseMenuState.Close();

            // Build category list
            BuildCategories();

            // Announce first category
            AnnounceCurrentState();
        }

        /// <summary>
        /// Closes the options menu.
        /// </summary>
        public static void Close()
        {
            isActive = false;
            currentLevel = OptionsMenuLevel.CategoryList;
            selectedCategoryIndex = 0;
            selectedSettingIndex = 0;
        }

        /// <summary>
        /// Moves selection to next item (category or setting).
        /// </summary>
        public static void SelectNext()
        {
            if (currentLevel == OptionsMenuLevel.CategoryList)
            {
                selectedCategoryIndex = (selectedCategoryIndex + 1) % categories.Count;
            }
            else // SettingsList
            {
                var settings = categories[selectedCategoryIndex].Settings;
                if (settings.Count > 0)
                {
                    selectedSettingIndex = (selectedSettingIndex + 1) % settings.Count;
                }
            }

            AnnounceCurrentState();
        }

        /// <summary>
        /// Moves selection to previous item (category or setting).
        /// </summary>
        public static void SelectPrevious()
        {
            if (currentLevel == OptionsMenuLevel.CategoryList)
            {
                selectedCategoryIndex = (selectedCategoryIndex - 1 + categories.Count) % categories.Count;
            }
            else // SettingsList
            {
                var settings = categories[selectedCategoryIndex].Settings;
                if (settings.Count > 0)
                {
                    selectedSettingIndex = (selectedSettingIndex - 1 + settings.Count) % settings.Count;
                }
            }

            AnnounceCurrentState();
        }

        /// <summary>
        /// Enters the selected category or toggles/cycles the selected setting.
        /// </summary>
        public static void ExecuteSelected()
        {
            if (currentLevel == OptionsMenuLevel.CategoryList)
            {
                // Enter the selected category
                currentLevel = OptionsMenuLevel.SettingsList;
                selectedSettingIndex = 0;
                AnnounceCurrentState();
            }
            else // SettingsList
            {
                // Toggle or cycle the current setting
                var setting = categories[selectedCategoryIndex].Settings[selectedSettingIndex];
                setting.Toggle();
                AnnounceCurrentState();
            }
        }

        /// <summary>
        /// Adjusts slider/dropdown settings left or right.
        /// </summary>
        public static void AdjustSetting(int direction)
        {
            if (currentLevel != OptionsMenuLevel.SettingsList)
                return;

            var setting = categories[selectedCategoryIndex].Settings[selectedSettingIndex];
            setting.Adjust(direction);
            AnnounceCurrentState();
        }

        /// <summary>
        /// Goes back one level or closes the menu.
        /// </summary>
        public static void GoBack()
        {
            if (currentLevel == OptionsMenuLevel.SettingsList)
            {
                // Go back to category list
                currentLevel = OptionsMenuLevel.CategoryList;
                AnnounceCurrentState();
            }
            else
            {
                // Close menu and return to pause menu
                Close();
                WindowlessPauseMenuState.Open();
            }
        }

        private static void AnnounceCurrentState()
        {
            if (currentLevel == OptionsMenuLevel.CategoryList)
            {
                string categoryName = categories[selectedCategoryIndex].Name;
                ClipboardHelper.CopyToClipboard($"Category: {categoryName}");
            }
            else // SettingsList
            {
                var setting = categories[selectedCategoryIndex].Settings[selectedSettingIndex];
                ClipboardHelper.CopyToClipboard(setting.GetAnnouncement());
            }
        }

        private static void BuildCategories()
        {
            categories.Clear();

            // Audio Category
            var audio = new OptionCategory("Audio");
            audio.Settings.Add(new SliderSetting("Master Volume", () => Prefs.VolumeMaster, v => Prefs.VolumeMaster = v, 0f, 1f, 0.1f, true));
            audio.Settings.Add(new SliderSetting("Game Volume", () => Prefs.VolumeGame, v => Prefs.VolumeGame = v, 0f, 1f, 0.1f, true));
            audio.Settings.Add(new SliderSetting("Music Volume", () => Prefs.VolumeMusic, v => Prefs.VolumeMusic = v, 0f, 1f, 0.1f, true));
            audio.Settings.Add(new SliderSetting("Ambient Volume", () => Prefs.VolumeAmbient, v => Prefs.VolumeAmbient = v, 0f, 1f, 0.1f, true));
            audio.Settings.Add(new SliderSetting("UI Volume", () => Prefs.VolumeUI, v => Prefs.VolumeUI = v, 0f, 1f, 0.1f, true));
            categories.Add(audio);

            // Gameplay Category
            var gameplay = new OptionCategory("Gameplay");
            gameplay.Settings.Add(new CheckboxSetting("Pause On Load", () => Prefs.PauseOnLoad, v => Prefs.PauseOnLoad = v));
            gameplay.Settings.Add(new SliderSetting("Max Player Settlements", () => Prefs.MaxNumberOfPlayerSettlements, v => Prefs.MaxNumberOfPlayerSettlements = Mathf.RoundToInt(v), 1f, 5f, 1f, false));
            gameplay.Settings.Add(new CheckboxSetting("Adaptive Training", () => Prefs.AdaptiveTrainingEnabled, v => Prefs.AdaptiveTrainingEnabled = v));
            categories.Add(gameplay);

            // Interface Category
            var ui = new OptionCategory("Interface");
            ui.Settings.Add(new EnumSetting<TemperatureDisplayMode>("Temperature Mode", () => Prefs.TemperatureMode, v => Prefs.TemperatureMode = v));
            ui.Settings.Add(new CheckboxSetting("Show Realtime Clock", () => Prefs.ShowRealtimeClock, v => Prefs.ShowRealtimeClock = v));
            ui.Settings.Add(new CheckboxSetting("12 Hour Clock", () => Prefs.TwelveHourClockMode, v => Prefs.TwelveHourClockMode = v));
            ui.Settings.Add(new EnumSetting<AnimalNameDisplayMode>("Animal Name Display", () => Prefs.AnimalNameMode, v => Prefs.AnimalNameMode = v));
            ui.Settings.Add(new CheckboxSetting("Hats Only On Map", () => Prefs.HatsOnlyOnMap, v => Prefs.HatsOnlyOnMap = v));
            categories.Add(ui);

            // Graphics Category
            var graphics = new OptionCategory("Graphics");
            graphics.Settings.Add(new CheckboxSetting("Plant Wind Sway", () => Prefs.PlantWindSway, v => Prefs.PlantWindSway = v));
            graphics.Settings.Add(new SliderSetting("Screen Shake Intensity", () => Prefs.ScreenShakeIntensity, v => Prefs.ScreenShakeIntensity = v, 0f, 2f, 0.1f, true));
            graphics.Settings.Add(new CheckboxSetting("Smooth Camera Jumps", () => Prefs.SmoothCameraJumps, v => Prefs.SmoothCameraJumps = v));
            categories.Add(graphics);

            // Controls Category
            var controls = new OptionCategory("Controls");
            controls.Settings.Add(new SliderSetting("Map Drag Sensitivity", () => Prefs.MapDragSensitivity, v => Prefs.MapDragSensitivity = v, 0.8f, 2.5f, 0.1f, true));
            controls.Settings.Add(new CheckboxSetting("Edge Screen Scroll", () => Prefs.EdgeScreenScroll, v => Prefs.EdgeScreenScroll = v));
            controls.Settings.Add(new CheckboxSetting("Zoom To Mouse", () => Prefs.ZoomToMouse, v => Prefs.ZoomToMouse = v));
            categories.Add(controls);
        }

        /// <summary>
        /// Represents a category of settings.
        /// </summary>
        private class OptionCategory
        {
            public string Name { get; }
            public List<OptionSetting> Settings { get; }

            public OptionCategory(string name)
            {
                Name = name;
                Settings = new List<OptionSetting>();
            }
        }

        /// <summary>
        /// Base class for all setting types.
        /// </summary>
        private abstract class OptionSetting
        {
            public string Name { get; }

            protected OptionSetting(string name)
            {
                Name = name;
            }

            public abstract string GetAnnouncement();
            public abstract void Toggle();
            public abstract void Adjust(int direction);
        }

        /// <summary>
        /// Checkbox setting (boolean).
        /// </summary>
        private class CheckboxSetting : OptionSetting
        {
            private readonly Func<bool> getter;
            private readonly Action<bool> setter;

            public CheckboxSetting(string name, Func<bool> getter, Action<bool> setter)
                : base(name)
            {
                this.getter = getter;
                this.setter = setter;
            }

            public override string GetAnnouncement()
            {
                bool value = getter();
                return $"{Name}: {(value ? "On" : "Off")}";
            }

            public override void Toggle()
            {
                bool current = getter();
                setter(!current);
                Prefs.Save();
            }

            public override void Adjust(int direction)
            {
                // Checkboxes toggle on left/right too
                Toggle();
            }
        }

        /// <summary>
        /// Slider setting (float or int).
        /// </summary>
        private class SliderSetting : OptionSetting
        {
            private readonly Func<float> getter;
            private readonly Action<float> setter;
            private readonly float min;
            private readonly float max;
            private readonly float step;
            private readonly bool showAsPercentage;

            public SliderSetting(string name, Func<float> getter, Action<float> setter, float min, float max, float step, bool showAsPercentage)
                : base(name)
            {
                this.getter = getter;
                this.setter = setter;
                this.min = min;
                this.max = max;
                this.step = step;
                this.showAsPercentage = showAsPercentage;
            }

            public override string GetAnnouncement()
            {
                float value = getter();
                if (showAsPercentage)
                {
                    return $"{Name}: {Mathf.RoundToInt(value * 100)}%";
                }
                else
                {
                    return $"{Name}: {value:F1}";
                }
            }

            public override void Toggle()
            {
                // Sliders cycle through values on toggle
                Adjust(1);
            }

            public override void Adjust(int direction)
            {
                float current = getter();
                float newValue = Mathf.Clamp(current + (step * direction), min, max);
                setter(newValue);
                Prefs.Save();
            }
        }

        /// <summary>
        /// Enum dropdown setting.
        /// </summary>
        private class EnumSetting<T> : OptionSetting where T : struct, Enum
        {
            private readonly Func<T> getter;
            private readonly Action<T> setter;
            private readonly T[] values;

            public EnumSetting(string name, Func<T> getter, Action<T> setter)
                : base(name)
            {
                this.getter = getter;
                this.setter = setter;
                this.values = (T[])Enum.GetValues(typeof(T));
            }

            public override string GetAnnouncement()
            {
                T current = getter();
                return $"{Name}: {current}";
            }

            public override void Toggle()
            {
                Adjust(1);
            }

            public override void Adjust(int direction)
            {
                T current = getter();
                int currentIndex = Array.IndexOf(values, current);
                int newIndex = (currentIndex + direction + values.Length) % values.Length;
                setter(values[newIndex]);
                Prefs.Save();
            }
        }
    }
}
