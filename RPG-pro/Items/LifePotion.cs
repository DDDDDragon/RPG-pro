using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Items
{
    class LifePotion : Item
    {
        public override void SetStaticDefaults()
        {
            itemType = 4;
            description = "富含生命力的粘稠药水\n使用可以恢复30点血量";
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "生命药水";
            damage = 0;
            defense = 1;
            base.SetDefaults();
        }
        public override bool Use(Player player)
        {
            player.Heal(30, name, ModUI.GetGameForm());
            return true;
        }
        public override (bool, Recipe) AddRecipe()
        {
            Recipe recipe = new Recipe();
            recipe.AddMaterial(ItemLoader.ItemList[1], 2);
            recipe.AddMaterial(ItemLoader.ItemList[3], 2);
            recipe.SetResult(this);
            return (true, recipe);
        }
    }
}
