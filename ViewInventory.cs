using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;
using System.Data;
using TShockAPI.DB;
using Terraria.ID;
//usin

namespace ViewInventory
{
    [ApiVersion(2, 1)]
    public class ViewInventory : TerrariaPlugin
    {
        /// <summary>
        /// Gets the author(s) of this plugin
        /// </summary>
        public override string Author => "z枳";

        /// <summary>
        /// Gets the description of this plugin.
        /// A short, one lined description that tells people what your plugin does.
        /// </summary>
        public override string Description => "查阅背包";

        /// <summary>
        /// Gets the name of this plugin.
        /// </summary>
        public override string Name => "ViewInventory";

        /// <summary>
        /// Gets the version of this plugin.
        /// </summary>
        public override Version Version => new Version(1, 0, 0, 0);

        /// <summary>
        /// Initializes a new instance of the TestPlugin class.
        /// This is where you set the plugin's order and perfrom other constructor logic
        ///初始化TestPlugin类的新实例。
        ///这是设置插件顺序和性能的地方，来自其他构造函数逻辑
        /// </summary>
        public ViewInventory(Main game) : base(game)
        {
        }

        /// <summary>
        /// Handles plugin initialization. 
        /// Fired when the server is started and the plugin is being loaded.
        /// You may register hooks, perform loading procedures etc here.
        ///处理插件初始化。
        ///在服务器启动和插件加载时触发。
        ///您可以在此处注册挂钩、执行加载过程等。
        /// </summary>
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("vi", ViewInvent, "vi", "VI", "Vi", "vI", "v i")
            {
                HelpText = "输入 /vi 【玩家名】  来查看该玩家的库存\nEnter /vi [player name] to view the player's inventory"
            });
            Commands.ChatCommands.Add(new Command("vi", ViewInventDisorder, "vid", "VID", "Vid", "vId")
            {
                HelpText = "输入 /vid 【玩家名】  来查看该玩家的库存，不进行排列\nEnter / vid [player name] to view the player's inventory without sorting"
            });
            Commands.ChatCommands.Add(new Command("vi", ViewInventText, "vit", "VIT", "Vit", "vIt")
            {
                HelpText = "输入 /vit 【玩家名】  来查看该玩家的库存，不进行排列\nEnter / vit [player name] to view the player's inventory without sorting"
            });
        }


        /// <summary>
        /// Handles plugin disposal logic.
        /// *Supposed* to fire when the server shuts down.
        /// You should deregister hooks and free all resources here.
        ///处理插件处理逻辑。
        ///*Supposed**应该*在服务器关闭时触发。
        ///您应该取消注册挂钩并释放此处的所有资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        //分类查阅指令
        private void ViewInvent(CommandArgs args)
        {
            if (!args.Parameters.Any())
            {
                args.Player.SendInfoMessage("输入 /vi 【玩家名】  来查看该玩家的库存\nEnter / vi [player name] to view the player's inventory");
                return;
            }
            string name = args.Parameters[0];
            if (args.Parameters.Count > 1)
            {
                name = string.Join(" ", args.Parameters);
            }

            List<TSPlayer> list = TSPlayer.FindByNameOrID(name);
            if (list.Any())
            {
                foreach (var li in list)
                {
                    StringBuilder sb = new StringBuilder();
                    string inventory = GetItemsString(li.TPlayer.inventory, NetItem.InventorySlots);
                    string armor = GetItemsString(li.TPlayer.armor, NetItem.ArmorSlots);
                    string dyestuff = GetItemsString(li.TPlayer.dye, NetItem.DyeSlots);
                    string misc = GetItemsString(li.TPlayer.miscEquips, NetItem.MiscEquipSlots);
                    string miscDye = GetItemsString(li.TPlayer.miscDyes, NetItem.MiscDyeSlots);
                    string trash = string.Format("【[i/s{0}:{1}]】 ", li.TPlayer.trashItem.stack, li.TPlayer.trashItem.netID);

                    string pig = GetItemsFromChestString(li.TPlayer.bank, NetItem.PiggySlots);
                    string safe = GetItemsFromChestString(li.TPlayer.bank2, NetItem.SafeSlots);
                    string forge = GetItemsFromChestString(li.TPlayer.bank3, NetItem.ForgeSlots);
                    string vault = GetItemsFromChestString(li.TPlayer.bank4, NetItem.VoidSlots);

                    sb.AppendLine("玩家 【" + li.Name + "】 的所有库存如下:");
                    if (inventory.Length > 0 && inventory != null && inventory != "")
                    {
                        sb.AppendLine("背包:");
                        sb.AppendLine(FormatArrangement(inventory, 20, " "));
                    }
                    if (armor.Length > 0 && armor != null && armor != "")
                    {
                        sb.AppendLine("盔甲 + 饰品 + 时装:");
                        sb.AppendLine(armor);
                    }
                    if (dyestuff.Length > 0 && dyestuff != null && dyestuff != "")
                    {
                        sb.AppendLine("染料:");
                        sb.AppendLine(dyestuff);
                    }
                    if (misc.Length > 0 && misc != null && misc != "")
                    {
                        sb.AppendLine("宠物 + 矿车 + 坐骑 + 钩爪:");
                        sb.AppendLine(misc);
                    }
                    if (miscDye.Length > 0 && miscDye != null && miscDye != "")
                    {
                        sb.AppendLine("宠物 矿车 坐骑 钩爪 染料:");
                        sb.AppendLine(miscDye);
                    }
                    if (trash != "【[i/s0:0]】 ")
                    {
                        sb.AppendLine("垃圾桶:");
                        sb.AppendLine(trash);
                    }
                    if (pig.Length > 0 && pig != null && pig != "")
                    {
                        sb.AppendLine("猪猪储蓄罐:");
                        sb.AppendLine(FormatArrangement(pig, 20, " "));
                    }
                    if (safe.Length > 0 && safe != null && safe != "")
                    {
                        sb.AppendLine("保险箱:");
                        sb.AppendLine(FormatArrangement(safe, 20, " "));
                    }
                    if (forge.Length > 0 && forge != null && forge != "")
                    {
                        sb.AppendLine("护卫熔炉:");
                        sb.AppendLine(FormatArrangement(forge, 20, " "));
                    }
                    if (vault.Length > 0 && vault != null && vault != "")
                    {
                        sb.AppendLine("虚空金库:");
                        sb.AppendLine(FormatArrangement(vault, 20, " "));
                    }
                    if (sb.Length > 0 && sb != null && sb.ToString() != "")
                        args.Player.SendMessage(sb.ToString(), TextColor());
                    else
                        args.Player.SendInfoMessage("没有任何东西");
                }
            }
            else
            {
                args.Player.SendInfoMessage("所查询玩家不在线，正在查询离线数据");
                string offAll = GetOfflinePlayerInv(TShock.DB, name);
                offAll = FormatArrangement(offAll, 35);
                if (offAll != "")
                {
                    args.Player.SendMessage("玩家 【" + name + "】 的所有库存如下:" + "\n" + offAll, TextColor());
                }
                else
                {
                    args.Player.SendInfoMessage("该玩家不存在！");
                }
            }
        }

        //不分类查阅指令
        private void ViewInventDisorder(CommandArgs args)
        {
            if (!args.Parameters.Any())
            {
                args.Player.SendInfoMessage("输入 /vid 【玩家名】  来查看该玩家的库存，不进行排列\nEnter / vid [player name] to view the player's inventory without sorting");
                return;
            }
            string name = args.Parameters[0];
            if (args.Parameters.Count > 1)
            {
                name = string.Join(" ", args.Parameters);
            }

            List<TSPlayer> list = TSPlayer.FindByNameOrID(name);
            if (list.Any())
            {
                foreach (var li in list)
                {
                    string inventory = GetItemsString(li.TPlayer.inventory, NetItem.InventorySlots);
                    string armor = GetItemsString(li.TPlayer.armor, NetItem.ArmorSlots);
                    string dyestuff = GetItemsString(li.TPlayer.dye, NetItem.DyeSlots);
                    string misc = GetItemsString(li.TPlayer.miscEquips, NetItem.MiscEquipSlots);
                    string miscDye = GetItemsString(li.TPlayer.miscDyes, NetItem.MiscDyeSlots);
                    string trash = string.Format("【[i/s{0}:{1}]】 ", li.TPlayer.trashItem.stack, li.TPlayer.trashItem.netID);

                    string pig = GetItemsFromChestString(li.TPlayer.bank, NetItem.PiggySlots);
                    string safe = GetItemsFromChestString(li.TPlayer.bank2, NetItem.SafeSlots);
                    string forge = GetItemsFromChestString(li.TPlayer.bank3, NetItem.ForgeSlots);
                    string vault = GetItemsFromChestString(li.TPlayer.bank4, NetItem.VoidSlots);

                    if (trash == "【[i/s0:0]】 ")
                        trash = "";

                    string all = inventory + armor + dyestuff + misc + misc + miscDye + trash + pig + safe + forge + vault;
                    all = FormatArrangement(all, 35);
                    if (all != "")
                    {
                        args.Player.SendMessage("玩家 【" + li.Name + "】 的所有库存如下:\n" + all, TextColor());
                    }
                    else
                        args.Player.SendInfoMessage("没有任何东西");
                }
            }
            else
            {
                args.Player.SendInfoMessage("所查询玩家不在线，正在查询离线数据");
                string offAll = GetOfflinePlayerInv(TShock.DB, name);
                offAll = FormatArrangement(offAll, 35);
                if (offAll != "")
                {
                    args.Player.SendMessage("玩家 【" + name + "】 的所有库存如下:" + "\n" + offAll, TextColor());
                }
                else
                {
                    args.Player.SendInfoMessage("该玩家不存在！");
                }
            }
        }

        //返回文本查阅背包
        private void ViewInventText(CommandArgs args)
        {
            if (!args.Parameters.Any())
            {
                args.Player.SendInfoMessage("输入 /vit 【玩家名】  来查看该玩家的库存\nEnter / vit [player name] to view the player's inventory");
                return;
            }
            string name = args.Parameters[0];
            if (args.Parameters.Count > 1)
            {
                name = string.Join(" ", args.Parameters);
            }

            List<TSPlayer> list = TSPlayer.FindByNameOrID(name);
            if (list.Any())
            {
                foreach (var li in list)
                {
                    StringBuilder sb = new StringBuilder();
                    string inventory = GetItemsString(li.TPlayer.inventory, NetItem.InventorySlots,1);
                    string armor = GetItemsString(li.TPlayer.armor, NetItem.ArmorSlots,1);
                    string dyestuff = GetItemsString(li.TPlayer.dye, NetItem.DyeSlots,1);
                    string misc = GetItemsString(li.TPlayer.miscEquips, NetItem.MiscEquipSlots,1);
                    string miscDye = GetItemsString(li.TPlayer.miscDyes, NetItem.MiscDyeSlots,1);
                    string trash = $" [{Lang.prefix[li.TPlayer.trashItem.prefix].Value}.{li.TPlayer.trashItem.Name}:{li.TPlayer.trashItem.stack}] ";

                    string pig = GetItemsFromChestString(li.TPlayer.bank, NetItem.PiggySlots,1);
                    string safe = GetItemsFromChestString(li.TPlayer.bank2, NetItem.SafeSlots,1);
                    string forge = GetItemsFromChestString(li.TPlayer.bank3, NetItem.ForgeSlots,1);
                    string vault = GetItemsFromChestString(li.TPlayer.bank4, NetItem.VoidSlots,1);

                    sb.AppendLine("玩家 【" + li.Name + "】 的所有库存如下:");
                    if (inventory.Length > 0 && inventory != null && inventory != "")
                    {
                        sb.AppendLine("背包:");
                        sb.AppendLine(FormatArrangement(inventory, 20, " "));
                    }
                    if (armor.Length > 0 && armor != null && armor != "")
                    {
                        sb.AppendLine("盔甲 + 饰品 + 时装:");
                        sb.AppendLine(armor);
                    }
                    if (dyestuff.Length > 0 && dyestuff != null && dyestuff != "")
                    {
                        sb.AppendLine("染料:");
                        sb.AppendLine(dyestuff);
                    }
                    if (misc.Length > 0 && misc != null && misc != "")
                    {
                        sb.AppendLine("宠物 + 矿车 + 坐骑 + 钩爪:");
                        sb.AppendLine(misc);
                    }
                    if (miscDye.Length > 0 && miscDye != null && miscDye != "")
                    {
                        sb.AppendLine("宠物 矿车 坐骑 钩爪 染料:");
                        sb.AppendLine(miscDye);
                    }
                    if (trash != " [.:0] ")
                    {
                        sb.AppendLine("垃圾桶:");
                        sb.AppendLine(trash);
                    }
                    if (pig.Length > 0 && pig != null && pig != "")
                    {
                        sb.AppendLine("猪猪储蓄罐:");
                        sb.AppendLine(FormatArrangement(pig, 20, " "));
                    }
                    if (safe.Length > 0 && safe != null && safe != "")
                    {
                        sb.AppendLine("保险箱:");
                        sb.AppendLine(FormatArrangement(safe, 20, " "));
                    }
                    if (forge.Length > 0 && forge != null && forge != "")
                    {
                        sb.AppendLine("护卫熔炉:");
                        sb.AppendLine(FormatArrangement(forge, 20, " "));
                    }
                    if (vault.Length > 0 && vault != null && vault != "")
                    {
                        sb.AppendLine("虚空金库:");
                        sb.AppendLine(FormatArrangement(vault, 20, " "));
                    }
                    if (sb.Length > 0 && sb != null && sb.ToString() != "")
                        args.Player.SendMessage(sb.ToString(), TextColor());
                    else
                        args.Player.SendInfoMessage("没有任何东西");
                }
            }
            else
            {
                args.Player.SendInfoMessage("所查询玩家不在线，正在查询离线数据");
                string offAll = GetOfflinePlayerInv(TShock.DB, name, 1);
                offAll = FormatArrangement(offAll, 35);
                if (offAll != "")
                {
                    args.Player.SendMessage("玩家 【" + name + "】 的所有库存如下:" + "\n" + offAll, TextColor());
                }
                else
                {
                    args.Player.SendInfoMessage("该玩家不存在！");
                }
            }
        }

        //返回玩家身上物品的字符串
        public static string GetItemsString(Item[] items, int slots, int Model = 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < slots; i++)
            {
                Item item = items[i];
                if (Model == 0 && !item.IsAir)
                {
                    if (item.prefix != 0)
                        sb.Append(string.Format("【[i/p{0}:{1}]】 ", item.prefix, item.netID));
                    else
                        sb.Append(string.Format("【[i/s{0}:{1}]】 ", item.stack, item.netID));
                }
                if (Model == 1 && !item.IsAir)
                {
                    if (item.prefix != 0)
                        sb.Append($"[{Lang.prefix[item.prefix].Value}.{item.Name}]");
                    else
                        sb.Append($"[{item.Name}:{item.stack}]");
                }
            }
            return sb.ToString();
        }

        //返回玩家猪猪等储蓄物内的物品的字符串
        public static string GetItemsFromChestString(Chest chest, int slots, int Model = 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < slots; i++)
            {
                Item item = chest.item[i];
                if (Model == 0 && !item.IsAir)
                {
                    if (item.prefix != 0)
                        sb.Append(string.Format("【[i/p{0}:{1}]】 ", item.prefix, item.netID));
                    else
                        sb.Append(string.Format("【[i/s{0}:{1}]】 ", item.stack, item.netID));
                }

                if (Model == 1 && !item.IsAir)
                {
                    if (item.prefix != 0)
                        sb.Append($"[{Lang.prefix[item.prefix].Value}.{item.Name}]");
                    else
                        sb.Append($"[{item.Name}:{item.stack}]");
                }
            }
            return sb.ToString();
        }

        //查询离线玩家sqlite查询
        public static string GetOfflinePlayerInv(IDbConnection db, string plrName, int Model = 0)
        {
            int userAccountID = TShock.UserAccounts.GetUserAccountID(plrName);
            string result;
            using (QueryResult queryResult = DbExt.QueryReader(db, "SELECT * FROM tsCharacter WHERE Account=" + userAccountID))
            {
                if (queryResult.Read())
                {
                    List<NetItem> list = queryResult.Get<string>("Inventory").Split(new char[] { '~' }).Select(new Func<string, NetItem>(NetItem.Parse)).ToList<NetItem>();
                    if (list.Count < NetItem.MaxInventory)
                    {
                        list.InsertRange(67, new NetItem[2]);
                        list.InsertRange(77, new NetItem[2]);
                        list.InsertRange(87, new NetItem[2]);
                        list.AddRange(new NetItem[NetItem.MaxInventory - list.Count]);
                    }
                    result = GetItemsString(list.ToArray(), list.Count, Model);
                }
                else
                {
                    result = "";
                }
            }
            return result;
        }

        //返回离线玩家身上的字符串
        public static string GetItemsString(NetItem[] items, int slots, int Model = 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < slots; i++)
            {
                NetItem item = items[i];
                if (Model == 0 && item.NetId != 0)
                {
                    if (item.PrefixId != 0)
                        sb.Append(string.Format("【[i/p{0}:{1}]】 ", item.PrefixId, item.NetId));
                    else
                        sb.Append(string.Format("【[i/s{0}:{1}]】 ", item.Stack, item.NetId));
                }
                if (Model == 1 && item.NetId != 0)
                {
                    if (item.PrefixId != 0)
                        sb.Append($"[{Lang.prefix[item.PrefixId].Value}.{Lang.GetItemName(item.NetId)}]");
                    else
                        sb.Append($"[{Lang.GetItemName(item.NetId)}:{item.Stack}]");
                }
            }
            return sb.ToString();
        }

        //给出一个字符串和每行几个物品数，返回排列好的字符串
        public static string FormatArrangement(string str, int num, string block = "")
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                List<string> ls = str.Split(' ').ToList();
                for (int i = 0; i < ls.Count; i++)
                {
                    if ((i + 1) % (num + 1) == 0)
                    {
                        ls.Insert(i, "\n");
                    }
                }

                if (block == "")
                    return string.Join(block, ls);
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in ls)
                    {
                        if (s != "\n")
                        {
                            sb.Append(s);
                            sb.Append(block);
                        }
                        else
                        {
                            sb.AppendLine();
                        }
                    }
                    return sb.ToString();
                }
            }
            else
            {
                return "";
            }
        }

        //返回颜色
        public static Color TextColor()
        {
            int r, g, b;
            r = Main.rand.Next(60, 255);
            g = Main.rand.Next(60, 255);
            b = Main.rand.Next(60, 255);
            return new Color(r, g, b);
        }
    }
}