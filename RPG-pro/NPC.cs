using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class NPC
    {
        public NPC() { }
        public NPC(string name, int level, int damage, int health, int defense)
        {
            this.damage = damage;
            this.defense = defense;
            this.level = level;
            this.name = name;
            this.health = health;
        }
        public virtual bool checkDead(GameForm form)
        {
            if (this.health <= 0)
            {
                OnDead(form);
                return true;
            }
            else return false;
        }
        public NPC Clone()
        {
            NPC npc = new NPC();
            npc = this.MemberwiseClone() as NPC;
            return npc;
        }
        public virtual void SetStaticDefaults()
        {

        }
        public virtual void SetDefaults()
        {

        }
        public virtual void OnSpawn(GameForm form)
        {
            form.OutputOnScreen("一个" + name + "向你冲过来，等级为" + level + "级");
        }
        public virtual void OnDead(GameForm form)
        {

        }
        public virtual void NPCLoot(GameForm form)
        {
            form.LootItem(new Item("额滴圣剑"), 1, this.name);
        }
        public string name;
        public int level;
        public int damage;
        public int health;
        public int defense;
        public int NPCType;
    }
}
