using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class Recipe
    {
        public Recipe() { }
        public void AddMaterial(Item item, int stack = 1)
        {
            int n = Materials.FindIndex(c => c.Item1.name == item.name);
            if (n != -1)
            {
                var i = Materials[n];
                i.Item2 += stack;
                Materials.RemoveAt(n);
                Materials.Insert(n, i);
                return;
            }
            Materials.Add((item, stack));
        }
        public void AddMaterial(int ItemType, int stack = 1)
        {
            Item item = ItemLoader.ItemList[ItemType];
            int n = Materials.FindIndex(c => c.Item1.name == item.name);
            if (n != -1)
            {
                var i = Materials[n];
                i.Item2 += stack;
                Materials.RemoveAt(n);
                Materials.Insert(n, i);
                return;
            }
            Materials.Add((item, stack));
        }
        public void SetResult(Item item, int stack = 1)
        {
            Result = (item, stack);
        }

        public bool Available(List<(Item, int)> Inventory)
        {
            bool ret = true;
            foreach (var i in Materials)
            {
                ret &= Inventory.FindAll(c => c.Item1.itemType == i.Item1.itemType && c.Item2 >= i.Item2).Count > 0;
            }
            return ret;
        }
        public List<(Item, int)> Materials = new List<(Item, int)>();
        public (Item, int) Result = (new Item(), 1);
    }
}
