using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Items
{
    public class Gel : Item
    {
        public override void SetStaticDefaults()
        {
            itemType = 1;
            description = "史莱姆的掉落物,粘稠而有韧性\n使用可以恢复10点血量";
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "凝胶";
            damage = 1;
            defense = 1;
            base.SetDefaults();
        }
        public override bool Use(Player player)
        {
            player.Heal(10, name, ModUI.GetGameForm());
            return true;
        }
    }
}
