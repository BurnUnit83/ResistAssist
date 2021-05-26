﻿using System;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary
{
    public class ResistAssist : BotPlugin
    {
        private static readonly string name = "ResistAssist";
        
        private Composite ResistAssistRoutine;
        
          
        private static NamedPipeClientStream pipe;

        public override string Author { get; } = "NeonNeo86, DomesticWarlord86";

        public override Version Version => new Version(1, 0);

        public override string Name { get; } = name;

        public override bool WantButton => false;

        
		private bool CanLostActionsCast() => Array.IndexOf(new int[] { 975, 920, 732, 763, 795, 827 }, WorldManager.ZoneId) >= 0;
        public override void OnInitialize()
        {
            ResistAssistRoutine = new Decorator(c => CanLostActionsCast(),new ActionRunCoroutine(ctx => LostActionsCast()));
        }

        public override void OnEnabled()
        {
            TreeRoot.OnStart += OnBotStart;
            TreeRoot.OnStop += OnBotStop;
            TreeHooks.Instance.OnHooksCleared += OnHooksCleared;

            if (TreeRoot.IsRunning) { AddHooks(); }
        }

		private void AddHooks()
        {
            Logging.Write(Colors.Aquamarine, "Adding Lost Action Casters Hook");
            TreeHooks.Instance.AddHook("TreeStart", ResistAssistRoutine);
        }

        private void setHooks(BotBase bot)
        {
            Log("Setting Hooks");
            TreeHooks.Instance.AddHook("Lost Action Casters", ResistAssistRoutine);
        }

        public override void OnDisabled()
        {
            TreeRoot.OnStart -= OnBotStart;
            TreeRoot.OnStop -= OnBotStop;
            RemoveHooks();
        }
		
		 private void RemoveHooks()
        {
            Logging.Write(Colors.Aquamarine, "Removing DLost Action Casters Hook");
            TreeHooks.Instance.RemoveHook("TreeStart", ResistAssistRoutine);
        }
		
		private void OnBotStart(BotBase bot) { AddHooks(); }

        private void OnHooksCleared(object sender, EventArgs e) { RemoveHooks(); }

		private void OnBotStop(BotBase bot) { 
		
			RemoveHooks(); 
		
		}
        private async Task<bool> LostActionsCast()
        {
            if (DutyManager.InInstance)
            {
						  
			   //Lost Protect  		Status = 2333, Item = 30908, Action = 20709
			   //Lost Shell    		Status = 2334, Item = 30909, Action = 20710
			   //Lost Protect II 	Status = 2561, Item = 33788, Action = 23915
			   //Lost Shell II 		Status = 2562, Item = 33789, Action = 23916
			   //Lost Bubble		Status = 2563, Item = 33790, Action = 23917
			   //Lost Stone Skin    Status = ??,   Item = 30911, Action = 20712
			   //Lost Stone Skin II Status = ??,   Item = 33781, Action = 23908
			   
				if(!Core.Me.InCombat)
				{
					/*
					TODO - Add Party Casting to Visible 
					PartyManager.VisibleMembers.Any(x => !x.IsMe && x.BattleCharacter.IsAlive && x.BattleCharacter.InCombat
					 foreach (var fate in FateManager.ActiveFates)
					 var units = GameObjectManager.GameObjects;
					foreach(var unit in units.OrderBy(r=>r.Distance()))

					*/
					if((!Core.Me.HasAura("Lost Protect II")) && ((DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Protect II")) || (DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Protect II"))))
					{
						ActionManager.Dismount();
						Log("Casting Lost Protect II");
						ActionManager.DoAction(23915, Core.Me);
						await Coroutine.Sleep(5000);
					}
					if((!Core.Me.HasAura("Lost Shell II")) && ((DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Protect II")) || (DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Shell II"))))
					{
						ActionManager.Dismount();
						Log("Casting Lost Shell II");
						ActionManager.DoAction(23916, Core.Me);
						await Coroutine.Sleep(5000);
					}
					if((!Core.Me.HasAura("Lost Bubble")) && ((DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Bubble")) || (DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Bubble"))))
					{
						ActionManager.Dismount();
						Log("Casting Lost Bubble");
						ActionManager.DoAction(23917, Core.Me);
						await Coroutine.Sleep(5000);
					}
					if((!Core.Me.HasAura("Lost Stoneskin")) && ((DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Stoneskin")) || (DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Stoneskin"))))
					{
						ActionManager.Dismount();
						Log("Casting Lost Stoneskin");
						ActionManager.DoAction(20712, Core.Me);
						await Coroutine.Sleep(5000);
					}
					if((!Core.Me.HasAura("Lost Stoneskin II")) && ((DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Stoneskin II")) || (DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Stoneskin II"))))
					{
						ActionManager.Dismount();
						Log("Casting Lost Stoneskin II");
						ActionManager.DoAction(23908, Core.Me);
						await Coroutine.Sleep(5000);
					}
					
				}
				return false;
			}
			return false;
		}

        private static void Log(string text)
        {
            var msg = string.Format($"[{name}] " + text);
            Logging.Write(Colors.Bisque, msg);
        }
    }
}