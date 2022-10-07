using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG;
using System.Windows.Forms;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace ExampleMod
{
    public class Mod1 : Mod
    {
        public override void OnEnterGame(GameForm form)
        {
            form.OutputOnScreen("> 示例mod已加载！");
            base.OnEnterGame(form);
        }
    }
    public class Zombie : NPC
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "僵尸";
            damage = 6;
            health = 10;
            level = 1;
            defense = 2;
            base.SetDefaults();
        }
        public override void NPCLoot(GameForm form)
        {
            form.LootItem(ModLoader.ItemType<Gel2>(), GameForm.rand.Next(2, 5), name);
        }
    }
    public class papy : NPC
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "小天使";
            damage = 6;
            health = 10;
            level = 1;
            defense = 2;
            base.SetDefaults();
        }
        public override void NPCLoot(GameForm form)
        {
            form.LootItem(ModLoader.ItemType<papyPenis>(), GameForm.rand.Next(2, 5), name);
        }
    }
    public class papyPenis : Item
    {
        public override void SetStaticDefaults()
        {
            description = "小天使的牛至 嘎嘣脆\n使用可以恢复1000点血量";
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "小天使的牛至";
            damage = 10;
            defense = 10;
            base.SetDefaults();
        }
        public override bool Use(Player player)
        {
            player.Heal(1000, name, ModUI.GetGameForm());
            return true;
        }
    }
    public class Gel2 : Item
    {
        public override void SetStaticDefaults()
        {
            description = "僵尸的掉落物,粘稠而有韧性\n使用可以恢复10点血量";
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            name = "僵尸粑粑";
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
