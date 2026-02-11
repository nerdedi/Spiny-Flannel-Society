using System;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Core
{
    /// <summary>
    /// ScriptableObject containing demo/prototype data from the Python demo.py.
    /// View this in the Inspector to preview narrative content, mechanics, and systems.
    /// Create instances via: Assets > Create > SFS > Demo Data
    /// </summary>
    [CreateAssetMenu(fileName = "DemoData", menuName = "SFS/Demo Data", order = 1)]
    public class DemoData : ScriptableObject
    {
        [Header("Game Info")]
        public string GameTitle = "Spiny Flannel Society";
        public string Version = "0.2.0";

        [TextArea(3, 5)]
        public string Description = "A hybrid 3D platformer about translation, systems, and diversity. Neuroaffirming | Non-violent combat | Universal design as world law";

        [Header("Narrative Foundation")]
        [TextArea(5, 10)]
        public string Setting = @"Spiny Flannel Society is a living settlement suspended in permanent wind currents above an Australian coastline. It is not a school—it is a place people live.

Windgap Academy is one precinct inside: a learning commons, workshop district, and navigation hub—the Society's 'translation engine.'";

        [TextArea(5, 10)]
        public string SpinyFlannelAxiom = @"The original principle that celebrated diversity:
• Multiple valid paths through spaces
• Adaptive systems responding to individuals
• Signals maintaining integrity across interpretation
• Environment supports people, not corrects them";

        [TextArea(5, 10)]
        public string TheDrift = @"When rigid 'standard defaults' were adopted, the Axiom withdrew. The Drift is systemic bias made physical:

• Distortions — Glitched rules (ramps retract if you pause)
• Echo Forms — Social scripts given motion (coercive routines)
• Noise Beasts — Sensory overload as weather";

        [TextArea(5, 10)]
        public string PlayerRole = @"You are THE TRANSLATOR. You perceive 'Windprints'—how assumptions embed into space. Your mission: make the Society fit people again.

Communication modes (all equal outcomes):
• Direct speech    • Scripted responses
• Icon-based       • Minimal speech";

        [Header("Windprint Rig")]
        public List<WindprintMode> WindprintModes = new()
        {
            new WindprintMode
            {
                Name = "Cushion",
                Description = "Expand personal space, hold during sensory moments, create safe distance from overwhelming stimuli",
                UseCase = "Defense / Comfort"
            },
            new WindprintMode
            {
                Name = "Guard",
                Description = "Project protective barriers, maintain translation threads, resist forced normalization",
                UseCase = "Protection / Intervention"
            }
        };

        [Header("Combat Verbs (Non-Violent)")]
        public List<CombatVerb> CombatVerbs = new()
        {
            new CombatVerb { Name = "PULSE", Description = "Radiate calming energy to de-escalate", Unlocked = true },
            new CombatVerb { Name = "THREAD LASH", Description = "Connect translation threads to reveal assumptions", Unlocked = true },
            new CombatVerb { Name = "RADIANT HOLD", Description = "Freeze coercive scripts in place", Unlocked = false },
            new CombatVerb { Name = "EDGE CLAIM", Description = "Assert boundaries with clarity", Unlocked = false },
            new CombatVerb { Name = "RETUNE", Description = "Convert hostile noise into harmonious signal", Unlocked = false }
        };

        [Header("Districts")]
        public List<DistrictInfo> Districts = new()
        {
            new DistrictInfo { Name = "Windgap Academy", Description = "The learning commons and translation engine", DriftLevel = 0.8f },
            new DistrictInfo { Name = "Charter Stone Plaza", Description = "Where the Axiom was first inscribed", DriftLevel = 0.6f },
            new DistrictInfo { Name = "Signal Heights", Description = "Communication and broadcast district", DriftLevel = 0.7f },
            new DistrictInfo { Name = "The Understory", Description = "Quiet refuge beneath the main levels", DriftLevel = 0.3f }
        };

        [Header("Chapters")]
        public int TotalChapters = 12;
        public List<ChapterInfo> ChapterPreviews = new()
        {
            new ChapterInfo { Number = 1, Title = "Arrival", Summary = "First day. Learn to see Windprints." },
            new ChapterInfo { Number = 2, Title = "The Atrium", Summary = "Encounter your first Drift manifestation." },
            new ChapterInfo { Number = 3, Title = "Translation", Summary = "Begin restoring corrupted civic rules." }
        };
    }

    [Serializable]
    public class WindprintMode
    {
        public string Name;
        [TextArea(2, 4)]
        public string Description;
        public string UseCase;
    }

    [Serializable]
    public class CombatVerb
    {
        public string Name;
        [TextArea(1, 3)]
        public string Description;
        public bool Unlocked;
    }

    [Serializable]
    public class DistrictInfo
    {
        public string Name;
        [TextArea(1, 3)]
        public string Description;
        [Range(0f, 1f)]
        public float DriftLevel;
    }

    [Serializable]
    public class ChapterInfo
    {
        public int Number;
        public string Title;
        [TextArea(1, 3)]
        public string Summary;
    }
}
