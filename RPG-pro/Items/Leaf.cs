using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Items
{
    class Leaf : Item
    {
        public override void SetStaticDefaults()
        {
            itemType = 3;
            description = "木魔像的掉落物,带有魔力的树叶";
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "树叶";
            damage = 0;
            defense = 1;
            base.SetDefaults();
        }
    }
}
