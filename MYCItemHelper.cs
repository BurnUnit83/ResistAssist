using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;

namespace ResistAssist
{
    public static class MYCItemHelper
    {
        public static List<MYCItem> ItemList;
        
        static IntPtr AgentVtable;
        static int offsetAgent; // 0x30
        static int arrayStartOff; //0x918
        static int arrayCountOff; //0x180
        static AgentInterface agent;
        static IntPtr CanUseAction;
        static IntPtr ActionManager;
        static IntPtr UseMYCItem ;
        
        static MYCItemHelper()
        {
            using (var pf = new GreyMagic.PatternFinder(Core.Memory))
            {
                AgentVtable = pf.Find("48 8D 05 ? ? ? ? 48 89 03 48 8D 05 ? ? ? ? 48 89 43 ? 33 C0 48 89 43 ? 48 89 43 ? Add 3 TraceRelative");
                offsetAgent = pf.Find("48 8B 7B ? 33 D2 48 81 C7 ? ? ? ? Add 3 Read8").ToInt32(); // 0x30
                arrayStartOff = pf.Find("48 81 C7 ? ? ? ? 48 8B C8 Add 3 Read32").ToInt32(); //0x918
                arrayCountOff = pf.Find("4C 8D A7 ? ? ? ? 45 8B 04 24 Add 3 Read32").ToInt32(); //0x180
                agent = AgentModule.GetAgentInterfaceById(AgentModule.FindAgentIdByVtable(AgentVtable));
                CanUseAction = pf.Find("48 89 5C 24 ? 4C 89 4C 24 ? 48 89 4C 24 ? 55 56 57 48 81 EC ? ? ? ?");
                ActionManager = pf.Find("48 8D 0D ? ? ? ? 44 8D 42 ? E8 ? ? ? ? 85 C0 Add 3 TraceRelative");
                UseMYCItem = pf.Find("48 89 5C 24 ? 48 89 74 24 ? 57 48 83 EC ? 8B F2 8B D9 E8 ? ? ? ? 48 8B F8");
            }

            ItemList = new List<MYCItem>()
            {
                new MYCItem(34 , 20734, "Dynamis Dice", 0, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Place your faith in the goddess Nymeia as she spins the wheel of fate.
                new MYCItem(35 , 20735, "Resistance Phoenix", 0, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Resurrects target to a weakened state.
                new MYCItem(36 , 20736, "Resistance Reraiser", 2355, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Grants a 70% chance of automatic revival upon KO.
                new MYCItem(37 , 20737, "Resistance Potion Kit", 2342, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Grants Auto-potion to self.
                new MYCItem(38 , 20738, "Resistance Ether Kit", 2343, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Grants Auto-ether to self.
                new MYCItem(39 , 20739, "Resistance Medikit", 2344, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Removes a single detrimental effect from self. When not suffering from detrimental effects, creates a barrier that protects against most status ailments. The barrier is removed after curing the next status ailment suffered.
                new MYCItem(40 , 20740, "Resistance Potion", 0, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Gradually restores HP.
                new MYCItem(41 , 20741, "Essence of the Aetherweaver", 2311, new byte[] {24,28,33} ), //Increases damage dealt by 80%.
                new MYCItem(42 , 20742, "Essence of the Martialist", 2312, new byte[] {19,21,32,37} ), //Increases damage dealt by 60%.
                new MYCItem(43 , 20743, "Essence of the Savior", 2313, new byte[] {19,20,21,22,23,25,27,30,31,32,34,35,37,38} ), //Increases healing potency by 60%.
                new MYCItem(44 , 20744, "Essence of the Veteran", 2314, new byte[] {24,25,27,28,33,35} ), //Increases physical defense by 150%, magic defense by 45%, and maximum HP by 60%.
                new MYCItem(45 , 20745, "Essence of the Platebearer", 2315, new byte[] {20,22,23,30,31,34,38} ), //Increases defense by 80% and maximum HP by 45%.
                new MYCItem(46 , 20746, "Essence of the Guardian", 2316, new byte[] {19,21,32,37} ), //Increases defense by 30% and maximum HP by 10%.
                new MYCItem(47 , 20747, "Essence of the Ordained", 2317, new byte[] {24,27,28,33,35} ), //Increases damage dealt by 20%, healing potency by 25%, and maximum MP by 50%.
                new MYCItem(48 , 20748, "Essence of the Skirmisher", 2318, new byte[] {20,22,23,25,27,30,31,34,35,38} ), //Increases damage dealt by 20% and critical hit rate by 15%.
                new MYCItem(49 , 20749, "Essence of the Watcher", 2319, new byte[] {20,22,23,25,27,30,31,34,35,38} ), //Reduces maximum HP by 5% while increasing evasion by 40%.
                new MYCItem(50 , 20750, "Essence of the Profane", 2320, new byte[] {24,28,33} ), //Reduces healing potency by 70% while increasing damage dealt by 100%.
                new MYCItem(51 , 20751, "Essence of the Irregular", 2321, new byte[] {19,21,32,37} ), //Increases damage dealt by 90% and damage taken by 200% while reducing maximum HP by 30%.
                new MYCItem(52 , 20752, "Essence of the Breathtaker", 2322, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Increases poison resistance and movement speed, including mount speed, and increases evasion by 10%.
                new MYCItem(53 , 20753, "Essence of the Bloodsucker", 2323, new byte[] {19,21,32,37} ), //Increases damage dealt by 40%.
                new MYCItem(54 , 20754, "Essence of the Beast", 2324, new byte[] {20,22,23,25,27,30,31,34,35,38} ), //Increases defense by 50% and maximum HP by 45%.
                new MYCItem(55 , 20755, "Essence of the Templar", 2325, new byte[] {24,28,33} ), //Increases defense by 50%, maximum HP by 45%, and damage dealt by 60%.
                new MYCItem(56 , 20756, "Deep Essence of the Aetherweaver", 2311, new byte[] {24,28,33} ), //Increases damage dealt by 96%.
                new MYCItem(57 , 20757, "Deep Essence of the Martialist", 2312, new byte[] {19,21,32,37} ), //Increases damage dealt by 72%.
                new MYCItem(58 , 20758, "Deep Essence of the Savior", 2313, new byte[] {19,20,21,22,23,25,27,30,31,32,34,35,37,38} ), //Increases healing potency by 72%.
                new MYCItem(59 , 20759, "Deep Essence of the Veteran", 2314, new byte[] {24,25,27,28,33,35} ), //Increases physical defense by 180%, magic defense by 54%, and maximum HP by 72%.
                new MYCItem(60 , 20760, "Deep Essence of the Platebearer", 2315, new byte[] {20,22,23,30,31,34,38} ), //Increases defense by 96% and maximum HP by 54%.
                new MYCItem(61 , 20761, "Deep Essence of the Guardian ", 2316, new byte[] {19,21,32,37} ), //Increases defense by 36% and maximum HP by 12%.
                new MYCItem(62 , 20762, "Deep Essence of the Ordained", 2317, new byte[] {24,27,28,33,35} ), //Increases damage dealt by 24%, healing potency by 30%, and maximum MP by 60%.
                new MYCItem(63 , 20763, "Deep Essence of the Skirmisher", 2318, new byte[] {20,22,23,25,27,30,31,34,35,38} ), //Increases damage dealt by 24% and critical hit rate by 18%.
                new MYCItem(64 , 20764, "Deep Essence of the Watcher", 2319, new byte[] {20,22,23,25,27,30,31,34,35,38} ), //Reduces maximum HP by 3% while increasing evasion by 48%.
                new MYCItem(65 , 20765, "Deep Essence of the Profane", 2320, new byte[] {24,28,33} ), //Reduces healing potency by 70% while increasing damage dealt by 120%.
                new MYCItem(66 , 20766, "Deep Essence of the Irregular", 2321, new byte[] {19,21,32,37} ), //Increases damage dealt by 108% and damage taken by 200% while reducing maximum HP by 30%.
                new MYCItem(67 , 20767, "Deep Essence of the Breathtaker", 2322, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Increases poison resistance and movement speed, including mount speed, and increases evasion by 20%.
                new MYCItem(68 , 20768, "Deep Essence of the Bloodsucker", 2323, new byte[] {19,21,32,37} ), //Increases damage dealt by 48%.
                new MYCItem(69 , 20769, "Deep Essence of the Beast", 2324, new byte[] {20,22,23,25,27,30,31,34,35,38} ), //Increases defense by 60% and maximum HP by 54%.
                new MYCItem(70 , 20770, "Deep Essence of the Templar", 2325, new byte[] {24,28,33} ), //Increases defense by 60%, maximum HP by 54%, and damage dealt by 72%.
                new MYCItem(73 , 22346, "Pure Essence of the Gambler", 2434, new byte[] {20,22,23,25,27,30,31,34,35,38} ), //Increases evasion by 11%, critical hit rate by 77%, and direct hit rate by 77%.
                new MYCItem(74 , 22347, "Pure Essence of the Elder", 2435, new byte[] {25,27,35} ), //Increases defense by 25%, damage dealt by 50%, and maximum HP by 100%.
                new MYCItem(75 , 22348, "Pure Essence of the Duelist", 2436, new byte[] {20,22,30,34} ), //Increases defense by 60%, damage dealt by 60%, and maximum HP by 81%.
                new MYCItem(76 , 22349, "Pure Essence of the Fiendhunter", 2437, new byte[] {23,31,38} ), //Increases defense by 60%, damage dealt by 50%, and maximum HP by 81%.
                new MYCItem(77 , 22350, "Pure Essence of the Indomitable", 2438, new byte[] {19,21,32,37} ), //Increases defense by 40%, damage dealt by 72%, and maximum HP by 50%.
                new MYCItem(78 , 22351, "Pure Essence of the Divine", 2439, new byte[] {24,28,33} ), //Increases defense by 25%, damage dealt by 35%, and maximum HP by 100%.
                new MYCItem(84 , 23907, "Lodestone", 0, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Instantly return to the starting point of the area.
                new MYCItem(88 , 23911, "Light Curtain", 2337, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Grants the effect of Lost Reflect to self.
                new MYCItem(99 , 23922, "Resistance Elixir", 0, new byte[] {19,20,21,22,23,24,25,27,28,30,31,32,33,34,35,37,38} ), //Restores own HP and MP to maximum.
            };
        }

        public static bool CanCast(int itemId)
        {
            var item = ItemList.FirstOrDefault(i => i.ID == itemId);
            
            if (item == null) return false;

            if (!HaveItem(itemId)) return false;
            
            //ResistAssist.Log($"Have Item");

            if (!item.Jobs.Contains((byte) Core.Me.CurrentJob)) return false;
            
            //ResistAssist.Log($"Is job");
            
            long target = (long) 0xE0000000;
            
            if (GameObjectManager.Target != null)
            {
                target = GameObjectManager.Target.ObjectId;
            }
            
            var result = Core.Memory.CallInjected64<uint>(CanUseAction, ActionManager,1,(uint)item.SpellID,(long)target, 1,1);
            
            //ResistAssist.Log($"Can Cast {result}");

            return (result == 0);
        }

        public static async Task Cast(int itemId)
        {
            if (!CanCast(itemId)) return;
            
            var item = ItemList.FirstOrDefault(i => i.ID == itemId);
            
            if (item == null) return;

            if (Core.Me.IsMounted)
            {
                ff14bot.Managers.ActionManager.Dismount();
                await Coroutine.Sleep(500);
            }

            Core.Memory.CallInjected64<byte>(UseMYCItem, item.ID,0);

            await Coroutine.Sleep(500);
        }

        public static bool HaveAura(int itemId)
        {
            var item = ItemList.FirstOrDefault(i => i.ID == itemId);
            
            if (item == null) return false;

            return Core.Me.HasAura(item.Aura);
        }

        public static MYCTemporaryItem[] GetCurrentItems()
        {
            var arraystart = Core.Memory.Read<IntPtr>(agent.Pointer + offsetAgent) + arrayStartOff;
            var arrayCount = Core.Memory.Read<byte>(arraystart + arrayCountOff);
            var array = Core.Memory.ReadArray<MYCTemporaryItem>(arraystart, arrayCount);
            return array;
        }

        public static bool HaveItem(int itemId)
        {
            return GetCurrentItems().Any(i => i.Id == itemId && i.Count > 0);
        }

        public static async Task CastIfNoAura(int itemId)
        {
            var item = ItemList.FirstOrDefault(i => i.ID == itemId);
            
            if (item == null) return;

            if (!HaveAura(itemId))
            {
                //ResistAssist.Log($"Read to cast");
                await Cast(itemId);
            }
        }
    }
    
    public class MYCItem
    {
        public int ID { get; }

        public uint SpellID { get; }
        public string Name { get; }
        public uint Aura { get; }
        public byte[] Jobs { get; }

        public MYCItem(int id, uint spellId, string name, uint aura, byte[] jobs)
        {
            ID = id;
            SpellID = spellId;
            Name = name;
            Aura = aura;
            Jobs = jobs;
        }
    }
    [StructLayout(LayoutKind.Sequential, Size = 0x8)]
    public struct MYCTemporaryItem
    {
        public readonly int Id;
        public readonly int Count;
    }
}