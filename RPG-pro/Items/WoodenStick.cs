using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Items
{
    public class WoodenStick : Item
    {
        public override void SetStaticDefaults()
        {
            itemType = 2;
            description = "木魔像的掉落物,一根带有魔力的木棍";
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "木棍";
            damage = 2;
            defense = 1;
            base.SetDefaults();
        }
    }
}
