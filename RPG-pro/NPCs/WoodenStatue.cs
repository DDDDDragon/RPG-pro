using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.NPCs
{
    public class WoodenStatue : NPC
    {
        public override void SetStaticDefaults()
        {
            NPCType = 2;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "木魔像";
            damage = 4;
            health = 20;
            level = 1;
            defense = 3;
            base.SetDefaults();
        }
        public override void NPCLoot(GameForm form)
        {
            form.LootItem(ItemLoader.ItemList[2], GameForm.rand.Next(2, 5), name);
            form.LootItem(ItemLoader.ItemList[3], GameForm.rand.Next(1, 4), name);
        }
    }
}
