using System;
using System.Linq;
using System.Threading.Tasks;
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
using ff14bot.Objects;

namespace ResistAssist
{
    public class ResistAssist : BotPlugin
    {
	    private static ClassJobType[] Jobs = new ClassJobType[] { ClassJobType.WhiteMage, ClassJobType.Scholar, ClassJobType.Summoner, ClassJobType.RedMage, ClassJobType.Astrologian };
	    
        private static readonly string name = "ResistAssist";
        
        private Composite ResistAssistRoutine;

        public override string Author { get; } = " NeonNeo86, DomesticWarlord86, nt153133";

        public override Version Version => new Version(1, 0);

        public override string Name { get; } = name;

        public override bool WantButton => true;
        
        public static ResistSettings Settings = ResistSettings.Instance;
        private ResistSettingsForm settings;
        
        private static Dictionary<ClassJobType, uint> RezSpells = new Dictionary<ClassJobType, uint>()
        {
	        {ClassJobType.WhiteMage, 125},
	        {ClassJobType.Astrologian, 3603},
	        {ClassJobType.Scholar, 173},
	        {ClassJobType.Summoner, 173},
	        {ClassJobType.RedMage, 7523},
	        {ClassJobType.Sage, 24287}
        };

        private int[] maps = new int[] { 975, 920, 732, 763, 795, 827 };
		private bool CanLostActionsCast() => maps.Contains(WorldManager.ZoneId);
        public override void OnInitialize()
        {
            ResistAssistRoutine = new Decorator(c => CanLostActionsCast(),new ActionRunCoroutine(ctx => LostActionsCast()));
        }

        public override void OnButtonPress()
        {
	        if (settings == null || settings.IsDisposed)
		        settings = new ResistSettingsForm();
	        try
	        {
		        settings.Show();
		        settings.Activate();
	        }
	        catch (ArgumentOutOfRangeException ee)
	        {
	        }
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
            Logging.Write(Colors.Aquamarine, "Removing Lost Action Casters Hook");
            TreeHooks.Instance.RemoveHook("TreeStart", ResistAssistRoutine);
        }
		
		private void OnBotStart(BotBase bot) { AddHooks(); }

        private void OnHooksCleared(object sender, EventArgs e) { RemoveHooks(); }

		private void OnBotStop(BotBase bot) { 
		
			RemoveHooks(); 
		
		}
        private async Task<bool> LostActionsCast()
        {

	        // Cast Raise on nearby people.
	        if (DutyManager.InInstance && Core.Me.IsAlive && !Core.Me.InCombat && RezSpells.ContainsKey(Core.Me.CurrentJob) && ResistSettings.Instance.ToRaise)
	        {
		        var deadPeople = GameObjectManager.GetObjectsOfType<BattleCharacter>().Where(p =>
			        !p.IsNpc && !p.HasAura(148) && !p.IsAlive && Core.Me.Distance(p) < 30 && !p.IsMe).ToList();
		        if (deadPeople.Any())
		        {
			        foreach (var partyMember in deadPeople.Where(partyMember => Core.Me.Distance(partyMember) < 30))
			        {
				        await Resurrect(partyMember);
			        }
		        }
	        }
	        
	        if (DutyManager.InInstance && Core.Me.IsAlive && !CommonBehaviors.IsLoading && CanLostActionsCast())
	        {
						  
		        var Actions = new List<(string Action, int Charges, int MaxCharges)>();
		        Actions.Add(MYCItemHelper.DutyAction1);
		        Actions.Add(MYCItemHelper.DutyAction2);
	            
	            
		        //Lost Protect  		Status = 2333, Item = 30908, Action = 20709
		        //Lost Shell    		Status = 2334, Item = 30909, Action = 20710
		        //Lost Protect II 	Status = 2561, Item = 33788, Action = 23915
		        //Lost Shell II 		Status = 2562, Item = 33789, Action = 23916
		        //Lost Bubble		Status = 2563, Item = 33790, Action = 23917
		        //Lost Stone Skin    Status = ??,   Item = 30911, Action = 20712
		        //Lost Stone Skin II Status = ??,   Item = 33781, Action = 23908
		        //Lost Arise										Action = 20730
		        //Lost Bravery									Action = 20713


		        //Lost Arise										Action = 20730
		        if (Actions.Any(i => i.Action == "Lost Arise") && PartyManager.IsInParty)
		        {
			        (var action, var charges, var maxCharges) = Actions.First(i => i.Action == "Lost Arise");
			        if (charges > 0 && maxCharges != 0)
			        {
				        var members = PartyManager.AllMembers.Where(p =>
					        !p.BattleCharacter.HasAura(148) && p.IsInObjectManager && !p.BattleCharacter.IsAlive &&
					        Core.Me.Distance(p.GameObject) < 30 && !p.BattleCharacter.IsMe);
				        if (members.Any())
				        {
					        foreach (var partyMember in members)
					        {
						        if (partyMember.IsInObjectManager && Core.Me.Distance(partyMember.GameObject) < 30)
						        {
							        ActionManager.Dismount();
							        Log(string.Format("Casting Lost Arise on {0}", partyMember.Name));
							        ActionManager.DoAction(20730, partyMember.GameObject);
							        await Coroutine.Sleep(10000);
						        }
					        }
				        }
			        }
		        }

		        if(!Core.Me.InCombat)
		        {
					
			        // Pass in the itemID of the item you want to cast, see item list in MYCItemHelper.cs
			        // int itemID = 44; //Essence of the Veteran
			        // await MYCItemHelper.CastIfNoAura(itemID);
			        if (Settings.EssenceSelection != ResistSettings.EssenceSelectionEnum.Not_Selected)
			        {
				        //Log($"Essence selection made.");
				        int itemID = ((int)Settings.EssenceSelection);
				        if (MYCItemHelper.ItemList.First(i => i.ID == itemID).Jobs.Contains((byte) Core.Me.CurrentJob))
				        {
					        if (MYCItemHelper.HaveItem(itemID) && !MYCItemHelper.HaveAura(itemID))
					        {
						        Log($"Using {Settings.EssenceSelection}");
						        await MYCItemHelper.CastIfNoAura(itemID);
					        }
				        }
			        }


			        /*
			        TODO - Add Party Casting to Visible 
			        PartyManager.VisibleMembers.Any(x => !x.IsMe && x.BattleCharacter.IsAlive && x.BattleCharacter.InCombat
			         foreach (var fate in FateManager.ActiveFates)
			         var units = GameObjectManager.GameObjects;
			        foreach(var unit in units.OrderBy(r=>r.Distance()))
			        */

			        //Lost Protect  		Status = 2333, Item = 30908, Action = 20709
			        if(DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Protect") 
			           || DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Protect"))
			        {
				        if(!Core.Me.HasAura("Lost Protect"))
				        {
					        ActionManager.Dismount();
					        Log("Casting Lost Protect on " +Core.Me.Name);
					        ActionManager.DoAction(20709, Core.Me);
					        await Coroutine.Sleep(5000);
				        }

				        if (PartyManager.IsInParty)
				        {
					        var members = PartyManager.AllMembers.Where(p => !p.BattleCharacter.HasAura("Lost Protect II") && 
					                                                         !p.BattleCharacter.HasAura("Lost Protect") && p.IsInObjectManager && 
					                                                         p.BattleCharacter.IsAlive && Core.Me.Distance(p.GameObject) < 30 &&
					                                                         !p.BattleCharacter.IsMe);
					        if (members.Any())
					        {
						        foreach (var partyMember in members)
						        {
							        if (partyMember.IsInObjectManager && Core.Me.Distance(partyMember.GameObject) < 30)
							        {
								        ActionManager.Dismount();
								        Log(string.Format("Casting Lost Protect on {0}", partyMember.Name));
								        ActionManager.DoAction(20709, partyMember.GameObject);
								        await Coroutine.Sleep(5000);
							        }
						        }
					        }
				        }
			        }				   
				   
			        //Lost Protect II 	Status = 2561, Item = 33788, Action = 23915
			        if(DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Protect II") || 
			           DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Protect II"))
			        {
				        if(!Core.Me.HasAura("Lost Protect II"))
				        {
					        ActionManager.Dismount();
					        Log("Casting Lost Protect II on " +Core.Me.Name);
					        ActionManager.DoAction(23915, Core.Me);
					        await Coroutine.Sleep(5000);
				        }

				        if (PartyManager.IsInParty)
				        {
					        var members = PartyManager.AllMembers.Where(p => !p.BattleCharacter.HasAura("Lost Protect II") && 
					                                                         p.IsInObjectManager && p.BattleCharacter.IsAlive && Core.Me.Distance(p.GameObject) < 30 &&
					                                                         !p.BattleCharacter.IsMe);
					        if (members.Any())
					        {
						        foreach (var partyMember in members)
						        {
							        if (partyMember.IsInObjectManager && Core.Me.Distance(partyMember.GameObject) < 30)
							        {
								        ActionManager.Dismount();
								        Log(string.Format("Casting Lost Protect II on {0}", partyMember.Name));
								        ActionManager.DoAction(23915, partyMember.GameObject);
								        await Coroutine.Sleep(5000);
							        }
						        }
					        }
				        }
			        }

			        //Lost Shell    		Status = 2334, Item = 30909, Action = 20710
			        if(DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Shell") || 
			           DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Shell"))
			        {
				        if(!Core.Me.HasAura("Lost Shell"))
				        {
					        ActionManager.Dismount();
					        Log("Casting Lost Shell on " +Core.Me.Name);
					        ActionManager.DoAction(20710, Core.Me);
					        await Coroutine.Sleep(5000);
				        }

				        if (PartyManager.IsInParty)
				        {
					        var members = PartyManager.AllMembers.Where(p => !p.BattleCharacter.HasAura("Lost Shell II") && 
					                                                         !p.BattleCharacter.HasAura("Lost Shell") && p.IsInObjectManager && p.BattleCharacter.IsAlive && 
					                                                         Core.Me.Distance(p.GameObject) < 30 && !p.BattleCharacter.IsMe);
					        if (members.Any())
					        {
						        foreach (var partyMember in members)
						        {
							        if (partyMember.IsInObjectManager && Core.Me.Distance(partyMember.GameObject) < 30)
							        {
								        ActionManager.Dismount();
								        Log(string.Format("Casting Lost Shell on {0}", partyMember.Name));
								        ActionManager.DoAction(20710, partyMember.GameObject);
								        await Coroutine.Sleep(5000);
							        }
						        }
					        }
				        }
			        }
				   
			        //Lost Shell II 		Status = 2562, Item = 33789, Action = 23916
			        if(DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Shell II") || 
			           DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Shell II"))
			        {
				        if(!Core.Me.HasAura("Lost Shell II"))
				        {
					        ActionManager.Dismount();
					        Log("Casting Lost Shell II on " +Core.Me.Name);
					        ActionManager.DoAction(23916, Core.Me);
					        await Coroutine.Sleep(5000);
				        }

				        if (PartyManager.IsInParty)
				        {
					        var members = PartyManager.AllMembers.Where(p => !p.BattleCharacter.HasAura("Lost Shell II") && 
					                                                         p.IsInObjectManager && p.BattleCharacter.IsAlive && Core.Me.Distance(p.GameObject) < 30 &&
					                                                         !p.BattleCharacter.IsMe);
					        if (members.Any())
					        {
						        foreach (var partyMember in members)
						        {
							        if (partyMember.IsInObjectManager && Core.Me.Distance(partyMember.GameObject) < 30)
							        {
								        ActionManager.Dismount();
								        Log(string.Format("Casting Lost Shell II on {0}", partyMember.Name));
								        ActionManager.DoAction(23916, partyMember.GameObject);
								        await Coroutine.Sleep(5000);
							        }
						        }
					        }
				        }
			        }
				   
			        //Lost Bravery									Action = 20713
			        //if(DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Bravery") || 
			        //   DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Bravery"))
			        if (Actions.Any(i => i.Action == "Lost Bravery"))
			        {
				        (var action, var charges, var maxCharges) = Actions.First(i => i.Action == "Lost Bravery");
				        if (charges > 0 && maxCharges != 0)
				        {
					        if (!Core.Me.HasAura("Lost Bravery"))
					        {
						        ActionManager.Dismount();
						        Log("Casting Lost Bravery on " + Core.Me.Name);
						        ActionManager.DoAction(20713, Core.Me);
						        await Coroutine.Sleep(5000);
					        }

					        if (PartyManager.IsInParty)
					        {
						        var members = PartyManager.AllMembers.Where(p =>
							        !p.BattleCharacter.HasAura("Lost Bravery") &&
							        p.IsInObjectManager && p.BattleCharacter.IsAlive &&
							        Core.Me.Distance(p.GameObject) < 30 &&
							        !p.BattleCharacter.IsMe);
						        if (members.Any())
						        {
							        foreach (var partyMember in members)
							        {
								        if (partyMember.IsInObjectManager &&
								            Core.Me.Distance(partyMember.GameObject) < 30)
								        {
									        ActionManager.Dismount();
									        Log(string.Format("Casting Lost Bravery on {0}", partyMember.Name));
									        ActionManager.DoAction(20713, partyMember.GameObject);
									        await Coroutine.Sleep(5000);
								        }
							        }
						        }
					        }
				        }
			        }
				   
			        //Lost Bubble		Status = 2563, Item = 33790, Action = 23917
			        //if(DutyManager.DutyAction1 == DataManager.GetSpellData("Lost Bubble") || 
			        //   DutyManager.DutyAction2 == DataManager.GetSpellData("Lost Bubble"))
			        if (Actions.Any(i => i.Action == "Lost Bubble"))
			        {
				        (var action, var charges, var maxCharges) = Actions.First(i => i.Action == "Lost Bubble");
				        if (charges > 0 && maxCharges != 0)
				        {
					        if (!Core.Me.HasAura("Lost Bubble"))
					        {
						        ActionManager.Dismount();
						        Log("Casting Lost Bubble on " + Core.Me.Name);
						        ActionManager.DoAction(23917, Core.Me);
						        await Coroutine.Sleep(5000);
					        }

					        if (PartyManager.IsInParty)
					        {
						        var members = PartyManager.AllMembers.Where(p =>
							        !p.BattleCharacter.HasAura("Lost Bubble") &&
							        p.IsInObjectManager && p.BattleCharacter.IsAlive &&
							        Core.Me.Distance(p.GameObject) < 30 &&
							        !p.BattleCharacter.IsMe);
						        if (members.Any())
						        {
							        foreach (var partyMember in members)
							        {
								        if (partyMember.IsInObjectManager &&
								            Core.Me.Distance(partyMember.GameObject) < 30)
								        {
									        ActionManager.Dismount();
									        Log(string.Format("Casting Lost Bubble on {0}", partyMember.Name));
									        ActionManager.DoAction(23917, partyMember.GameObject);
									        await Coroutine.Sleep(5000);
								        }
							        }
						        }
					        }
				        }
			        }

		        }
		        return false;
	        }
	        return false;
            }
        
        internal async Task Resurrect(BattleCharacter partyMember)
        {
	        if (!RezSpells.ContainsKey(Core.Me.CurrentJob)) return;

	        var spell = ActionManager.CurrentActions[RezSpells[Core.Me.CurrentJob]];

	        if (!ActionManager.ActionReady(ActionType.Spell, spell.Id)) return;

	        if (Core.Me.IsMounted)
	        {
		        ActionManager.Dismount();
		        await Coroutine.Wait(10000, () => !Core.Me.IsMounted);
	        }

	        Log(($"Casting {spell.LocalizedName} on {partyMember.Name}"));
	        if (ActionManager.ActionReady(ActionType.Spell, 7561))
	        {
		        ActionManager.DoAction(7561, Core.Me);
		        await Coroutine.Sleep(500);
		        ActionManager.DoAction(spell, partyMember);
		        await Coroutine.Sleep(1000);
	        }
	        else
	        {
		        ActionManager.DoAction(spell, partyMember);
		        await Coroutine.Wait(10000, () => Core.Me.IsCasting);
		        await Coroutine.Wait(10000, () => !Core.Me.IsCasting);
	        }
        }

            public static void Log(string text)
            {
	            var msg = string.Format($"[{name}] " + text);
	            Logging.Write(Colors.Bisque, msg);
            }
    }
}