using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class Player
    {
        public Player()
        {

        }
        public Player(int health, int damage, int defense)
        {
            this.health = health;
            this.maxHealth = health;
            this.baseDamage = damage;
            this.baseDefense = defense;
            this.level = 1;
            this.EXP = 0;
            this.crit = 0.04;
            this.Inventory = new List<(Item, int)>();
            this.weapon = new Item("拳头");
            this.armor = new Item("无");
        }
        public virtual bool checkDead()
        {
            if (this.health <= 0) return true;
            else return false;
        }
        public void Heal(int HealNum, string ItemName, GameForm form)
        {
            health += HealNum;
            if (health > maxHealth) health = maxHealth;
            form.OutputOnScreen("使用" + ItemName + ",恢复" + HealNum + "点血量");
        }
        public int HealthLevel = 0;
        public int DamageLevel = 0;
        public int DefenseLevel = 0;
        public int health;//生命值
        public int baseDamage = 0;
        public int damage;//伤害
        public int maxHealth;
        public int defense;
        public int baseDefense = 0;
        public int level, EXP;
        public Item weapon;
        public Item armor;
        public double crit;
        public List<(Item, int)> Inventory;
    }
}
