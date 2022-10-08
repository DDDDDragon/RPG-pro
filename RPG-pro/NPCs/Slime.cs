using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG;

namespace RPG.NPCs
{
    public class Slime : RPGNPC
    {
        public override void SetStaticDefaults()
        {
            NPCType = 1;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "史莱姆";
            damage = 6;
            health = 10;
            level = 1;
            defense = 2;
            base.SetDefaults();
        }
        public override void NPCLoot(GameForm form)
        {
            form.LootItem(ItemLoader.ItemList[1], GameForm.rand.Next(2, 5), name);
        }
    }
}
