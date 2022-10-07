using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class Item
    {
        public Item() { this.name = "none"; }
        public Item(string Name, int damage = 0, int defense = 0)
        {
            this.name = Name;
            this.damage = damage;
            this.defense = defense;
        }
        public virtual void SetStaticDefaults()
        {

        }
        public virtual void SetDefaults()
        {

        }
        public virtual bool Use(Player player)
        {
            return false;
        }
        public virtual (bool, Recipe) AddRecipe()
        {
            return (false, new Recipe());
        }
        public string name;
        public string description = "none";
        public int itemType;
        public int damage = 0;
        public int defense = 0;
        public int rare = 1;
    }
}
