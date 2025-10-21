using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorldAccess
{
    public static class ColonistEditorNavigationState
    {
        private static bool initialized = false;
        private static int currentPawnIndex = 0;
        private static NavigationMode currentMode = NavigationMode.PawnList;
        private static int currentSectionIndex = 0;
        private static int currentDetailIndex = 0;

        private enum NavigationMode
        {
            PawnList,      // Navigating between pawns
            SectionList,   // Navigating between sections (Skills, Health, Relations, etc.)
            DetailView     // Viewing details within a section
        }

        private enum Section
        {
            Biography,
            Skills,
            Health,
            Relations,
            Traits,
            Gear
        }

        private static readonly List<Section> availableSections = new List<Section>
        {
            Section.Biography,
            Section.Skills,
            Section.Health,
            Section.Relations,
            Section.Traits,
            Section.Gear
        };

        public static void Initialize()
        {
            if (!initialized)
            {
                currentPawnIndex = 0;
                currentMode = NavigationMode.PawnList;
                currentSectionIndex = 0;
                currentDetailIndex = 0;
                initialized = true;
            }
        }

        public static void Reset()
        {
            initialized = false;
            currentPawnIndex = 0;
            currentMode = NavigationMode.PawnList;
            currentSectionIndex = 0;
            currentDetailIndex = 0;
        }

        public static int CurrentPawnIndex => currentPawnIndex;

        public static void NavigateUp()
        {
            switch (currentMode)
            {
                case NavigationMode.PawnList:
                    NavigatePawnUp();
                    break;
                case NavigationMode.SectionList:
                    NavigateSectionUp();
                    break;
                case NavigationMode.DetailView:
                    NavigateDetailUp();
                    break;
            }
        }

        public static void NavigateDown()
        {
            switch (currentMode)
            {
                case NavigationMode.PawnList:
                    NavigatePawnDown();
                    break;
                case NavigationMode.SectionList:
                    NavigateSectionDown();
                    break;
                case NavigationMode.DetailView:
                    NavigateDetailDown();
                    break;
            }
        }

        public static void EnterMode()
        {
            // Tab key - enter section navigation from pawn list
            if (currentMode == NavigationMode.PawnList)
            {
                currentMode = NavigationMode.SectionList;
                currentSectionIndex = 0;
                CopySectionToClipboard();
            }
        }

        public static void DrillIn()
        {
            // Right arrow - drill into details
            if (currentMode == NavigationMode.SectionList)
            {
                currentMode = NavigationMode.DetailView;
                currentDetailIndex = 0;
                CopyDetailToClipboard();
            }
        }

        public static void DrillOut()
        {
            // Left arrow - go back
            if (currentMode == NavigationMode.DetailView)
            {
                currentMode = NavigationMode.SectionList;
                CopySectionToClipboard();
            }
            else if (currentMode == NavigationMode.SectionList)
            {
                currentMode = NavigationMode.PawnList;
                CopyPawnToClipboard();
            }
        }

        public static void RandomizePawn()
        {
            List<Pawn> pawns = Find.GameInitData.startingAndOptionalPawns;
            if (currentPawnIndex < 0 || currentPawnIndex >= pawns.Count) return;

            StartingPawnUtility.RandomizePawn(currentPawnIndex);
            ClipboardHelper.CopyToClipboard($"Randomized pawn {currentPawnIndex + 1}");
            CopyPawnToClipboard();
        }

        private static void NavigatePawnUp()
        {
            List<Pawn> pawns = Find.GameInitData.startingAndOptionalPawns;
            if (pawns.Count == 0) return;

            currentPawnIndex--;
            if (currentPawnIndex < 0)
                currentPawnIndex = pawns.Count - 1;

            CopyPawnToClipboard();
        }

        private static void NavigatePawnDown()
        {
            List<Pawn> pawns = Find.GameInitData.startingAndOptionalPawns;
            if (pawns.Count == 0) return;

            currentPawnIndex++;
            if (currentPawnIndex >= pawns.Count)
                currentPawnIndex = 0;

            CopyPawnToClipboard();
        }

        private static void NavigateSectionUp()
        {
            currentSectionIndex--;
            if (currentSectionIndex < 0)
                currentSectionIndex = availableSections.Count - 1;

            CopySectionToClipboard();
        }

        private static void NavigateSectionDown()
        {
            currentSectionIndex++;
            if (currentSectionIndex >= availableSections.Count)
                currentSectionIndex = 0;

            CopySectionToClipboard();
        }

        private static void NavigateDetailUp()
        {
            int maxDetails = GetDetailCountForCurrentSection();
            if (maxDetails == 0) return;

            currentDetailIndex--;
            if (currentDetailIndex < 0)
                currentDetailIndex = maxDetails - 1;

            CopyDetailToClipboard();
        }

        private static void NavigateDetailDown()
        {
            int maxDetails = GetDetailCountForCurrentSection();
            if (maxDetails == 0) return;

            currentDetailIndex++;
            if (currentDetailIndex >= maxDetails)
                currentDetailIndex = 0;

            CopyDetailToClipboard();
        }

        private static void CopyPawnToClipboard()
        {
            List<Pawn> pawns = Find.GameInitData.startingAndOptionalPawns;
            if (currentPawnIndex < 0 || currentPawnIndex >= pawns.Count) return;

            Pawn pawn = pawns[currentPawnIndex];
            string status = currentPawnIndex < Find.GameInitData.startingPawnCount ? "Starting" : "Left Behind";

            string name = pawn.Name is NameTriple triple
                ? $"{triple.First} '{triple.Nick}' {triple.Last}"
                : pawn.LabelShort;

            string text = $"[Pawn {currentPawnIndex + 1}/{pawns.Count}] {name} - {pawn.story.TitleCap} ({status}) - Age {pawn.ageTracker.AgeBiologicalYears}";

            ClipboardHelper.CopyToClipboard(text);
        }

        private static void CopySectionToClipboard()
        {
            Section section = availableSections[currentSectionIndex];
            string sectionName = section.ToString();
            int detailCount = GetDetailCountForSection(section);

            string text = $"[Section] {sectionName} ({detailCount} items) - Press Right Arrow to view details, Left Arrow to go back";
            ClipboardHelper.CopyToClipboard(text);
        }

        private static void CopyDetailToClipboard()
        {
            Section section = availableSections[currentSectionIndex];
            string detailText = GetDetailText(section, currentDetailIndex);

            ClipboardHelper.CopyToClipboard(detailText);
        }

        private static int GetDetailCountForCurrentSection()
        {
            return GetDetailCountForSection(availableSections[currentSectionIndex]);
        }

        private static int GetDetailCountForSection(Section section)
        {
            List<Pawn> pawns = Find.GameInitData.startingAndOptionalPawns;
            if (currentPawnIndex < 0 || currentPawnIndex >= pawns.Count) return 0;

            Pawn pawn = pawns[currentPawnIndex];

            switch (section)
            {
                case Section.Biography:
                    return 1; // Just backstory

                case Section.Skills:
                    return pawn.skills?.skills?.Count ?? 0;

                case Section.Health:
                    return pawn.health?.hediffSet?.hediffs?.Count ?? 0;

                case Section.Relations:
                    return pawn.relations?.DirectRelations?.Count ?? 0;

                case Section.Traits:
                    return pawn.story?.traits?.allTraits?.Count ?? 0;

                case Section.Gear:
                    return pawn.equipment?.AllEquipmentListForReading?.Count ?? 0;

                default:
                    return 0;
            }
        }

        private static string GetDetailText(Section section, int index)
        {
            List<Pawn> pawns = Find.GameInitData.startingAndOptionalPawns;
            if (currentPawnIndex < 0 || currentPawnIndex >= pawns.Count)
                return "Invalid pawn index";

            Pawn pawn = pawns[currentPawnIndex];

            switch (section)
            {
                case Section.Biography:
                    return GetBiographyText(pawn);

                case Section.Skills:
                    return GetSkillText(pawn, index);

                case Section.Health:
                    return GetHealthText(pawn, index);

                case Section.Relations:
                    return GetRelationText(pawn, index);

                case Section.Traits:
                    return GetTraitText(pawn, index);

                case Section.Gear:
                    return GetGearText(pawn, index);

                default:
                    return "Unknown section";
            }
        }

        private static string GetBiographyText(Pawn pawn)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Name: {pawn.Name}");
            sb.AppendLine($"Gender: {pawn.gender}");
            sb.AppendLine($"Age: {pawn.ageTracker.AgeBiologicalYears}");

            if (pawn.story != null)
            {
                sb.AppendLine($"Childhood: {pawn.story.Childhood?.title ?? "None"}");
                if (pawn.story.Adulthood != null)
                {
                    sb.AppendLine($"Adulthood: {pawn.story.Adulthood.title}");
                }
            }

            return sb.ToString();
        }

        private static string GetSkillText(Pawn pawn, int index)
        {
            if (pawn.skills?.skills == null || index < 0 || index >= pawn.skills.skills.Count)
                return "Invalid skill index";

            SkillRecord skill = pawn.skills.skills[index];

            string passionText = "";
            if (skill.passion == Passion.Minor) passionText = " (Interested)";
            if (skill.passion == Passion.Major) passionText = " (Burning passion)";

            string disabledText = skill.TotallyDisabled ? " [DISABLED]" : "";

            return $"{skill.def.skillLabel}: Level {skill.Level}{passionText}{disabledText}";
        }

        private static string GetHealthText(Pawn pawn, int index)
        {
            if (pawn.health?.hediffSet?.hediffs == null || index < 0 || index >= pawn.health.hediffSet.hediffs.Count)
                return "Healthy";

            Hediff hediff = pawn.health.hediffSet.hediffs[index];
            string partText = hediff.Part != null ? $" on {hediff.Part.Label}" : "";

            return $"{hediff.LabelCap}{partText} - {hediff.SeverityLabel}";
        }

        private static string GetRelationText(Pawn pawn, int index)
        {
            if (pawn.relations?.DirectRelations == null || index < 0 || index >= pawn.relations.DirectRelations.Count)
                return "No relations";

            DirectPawnRelation relation = pawn.relations.DirectRelations[index];
            int opinion = pawn.relations.OpinionOf(relation.otherPawn);

            return $"{relation.otherPawn.LabelShort}: {relation.def.label} (Opinion: {opinion:+#;-#;0})";
        }

        private static string GetTraitText(Pawn pawn, int index)
        {
            if (pawn.story?.traits?.allTraits == null || index < 0 || index >= pawn.story.traits.allTraits.Count)
                return "No traits";

            Trait trait = pawn.story.traits.allTraits[index];
            return $"{trait.LabelCap}: {trait.TipString(pawn)}";
        }

        private static string GetGearText(Pawn pawn, int index)
        {
            if (pawn.equipment?.AllEquipmentListForReading == null || index < 0 || index >= pawn.equipment.AllEquipmentListForReading.Count)
                return "No equipment";

            ThingWithComps equipment = pawn.equipment.AllEquipmentListForReading[index];
            return $"{equipment.LabelCap} - {equipment.DescriptionDetailed}";
        }

        public static string GetCurrentModeDescription()
        {
            switch (currentMode)
            {
                case NavigationMode.PawnList:
                    return "Pawn List Mode - Use Up/Down to navigate pawns, Tab to enter sections, R to randomize";
                case NavigationMode.SectionList:
                    return "Section Mode - Use Up/Down to navigate sections, Right to drill in, Left to go back";
                case NavigationMode.DetailView:
                    return "Detail Mode - Use Up/Down to navigate items, Left to go back";
                default:
                    return "Unknown mode";
            }
        }
    }
}
