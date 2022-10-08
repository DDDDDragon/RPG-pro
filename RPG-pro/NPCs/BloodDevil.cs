using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG;

namespace RPG.NPCs
{
    public class BloodDevil : RPGNPC
    {
        public override void SetStaticDefaults()
        {
            NPCType = 3;
            base.SetStaticDefaults();
        }
        public override void OnSpawn(GameForm form)
        {
            form.OutputOnScreen("邪恶的屑魔已苏醒！");
            base.OnSpawn(form);
        }
        public override void SetDefaults()
        {
            name = "屑魔";
            damage = 10;
            health = 50;
            level = 2;
            defense = 4;
            base.SetDefaults();
        }
        public override void NPCLoot(GameForm form)
        {
            form.LootItem(ItemLoader.ItemList[1], GameForm.rand.Next(2, 5), name);
        }
    }
}
