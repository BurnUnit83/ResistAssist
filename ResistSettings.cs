using System;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using System.Windows.Media;
using TreeSharp;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using ff14bot.Enums;
using ff14bot.Helpers;

namespace ResistAssist
{
    public partial class ResistSettings : JsonSettings
    {
        
        private static ResistSettings _settings;
        
        public static ResistSettings Instance => _settings ?? (_settings = new ResistSettings());

        private EssenceSelectionEnum _essencechoice;
        public enum EssenceSelectionEnum
        {
            Not_Selected = -1,
            Essence_of_the_Aetherweaver = 41,
            Essence_of_the_Martialist = 42,
            Essence_of_the_Savior = 43,
            Essence_of_the_Veteran = 44,
            Essence_of_the_Platebearer = 45,
            Essence_of_the_Guardian = 46,
            Essence_of_the_Ordained = 47,
            Essence_of_the_Skirmisher = 48,
            Essence_of_the_Watcher = 49,
            Essence_of_the_Profane = 50,
            Essence_of_the_Irregular = 51,
            Essence_of_the_Breathtaker = 52,
            Essence_of_the_Bloodsucker = 53,
            Essence_of_the_Beast = 54,
            Essence_of_the_Templar = 55,
            Deep_Essence_of_the_Aetherweaver = 56,
            Deep_Essence_of_the_Martialist = 57,
            Deep_Essence_of_the_Savior = 58,
            Deep_Essence_of_the_Veteran = 59,
            Deep_Essence_of_the_Platebearer = 60,
            Deep_Essence_of_the_Guardian  = 61,
            Deep_Essence_of_the_Ordained = 62,
            Deep_Essence_of_the_Skirmisher = 63,
            Deep_Essence_of_the_Watcher = 64,
            Deep_Essence_of_the_Profane_= 65,
            Deep_Essence_of_the_Irregular = 66,
            Deep_Essence_of_the_Breathtaker = 67,
            Deep_Essence_of_the_Bloodsucker = 68,
            Deep_Essence_of_the_Beast = 69,
            Deep_Essence_of_the_Templar = 70,
            Pure_Essence_of_the_Gambler = 73,
            Pure_Essence_of_the_Elder = 74,
            Pure_Essence_of_the_Duelist = 75,
            Pure_Essence_of_the_Fiendhunter = 76,
            Pure_Essence_of_the_Indomitable = 77,
            Pure_Essence_of_the_Divine = 78,
        };          

        [Description("Choose which Essence you'd like to use.")]
        [DefaultValue(EssenceSelectionEnum.Not_Selected)]
        public EssenceSelectionEnum EssenceSelection
        {
            get => _essencechoice;
            set
            {
                if (_essencechoice != value)
                {
                    _essencechoice = value;
                    Save();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }
        
        public ResistSettings() : base(Path.Combine(CharacterSettingsDirectory, "ResistSettings.json"))
        {
            
        }        
    }
}