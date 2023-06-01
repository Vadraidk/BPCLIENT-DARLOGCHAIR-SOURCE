using BrokeProtocol.Client.UI;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using ENet;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BrokeProtocolClient.modules.exploit
{
    class Give : Module
    {
        InputSetting itemName = new InputSetting("Item name", 32, "Colt");
        InputSetting amount = new InputSetting("Item amount", 32, "");
        ActionSetting giveItem;

        BooleanSetting onlyModded = new BooleanSetting("Only modded items", false);
        ActionSetting openMenu;

        BindSetting openMenuBind = new BindSetting(0, "Open menu bind");

        InputSetting kitName = new InputSetting("Kit name", 64, "Default");
        ActionSetting editKit;
        ActionSetting giveKit;


        IEnumerator destroyMenuThread;

        // Get private OptionMenu optionMenu field from ClManager
        static AccessTools.FieldRef<ClManager, OptionMenu> optionMenuRef = AccessTools.FieldRefAccess<ClManager, OptionMenu>("optionMenu");

        readonly string MenuTitle = "BPclient give menu";
        readonly string MenuID = "bpclient.givemenu";

        public Give() : base(Categories.Exploit, "Give", "Give any item")
        {
            addSetting(itemName);
            addSetting(amount);

            giveItem = new ActionSetting("Give Item", give);
            addSetting(giveItem);

            addSetting(onlyModded);
            openMenu = new ActionSetting("Open give menu", OpenGiveMenu);
            addSetting(openMenu);
            addSetting(openMenuBind);

            addSetting(new InfoSetting("Kits"));
            addSetting(kitName);

            editKit = new ActionSetting("edit/create kit", EditKit);
            addSetting(editKit);

            //giveKit = new ActionSetting("Give kit", GiveKit);
            //addSetting(giveKit);

        }

        public override void onUpdate()
        {
            if (openMenuBind.WasPressedThisFrame())
                OpenGiveMenu();
        }



        private void give()
        {
            if (!getClient().ClManager.myPlayer) return;

            string searchedName = itemName.getValue();

            ShItem item;
            if (!getClient().SceneManager.TryGetEntity<ShItem>(searchedName, out item))
            {
                Log($"Item {searchedName} not found, similar items:");
                foreach (ShEntity entity in getClient().SceneManager.entityCollection.Values)
                {
                    if (entity.name.ToLower().Contains(searchedName.ToLower()))
                        ConsoleBase.Write($"{entity.name} ");
                }
                ConsoleBase.WriteLine("");
                return;
            }

            GiveItem(item, Int32.Parse(amount.getValue()));
        }

        private void GiveItem(ShItem item, int amount)
        {
            // Send Drop packet so localplayer.otherEntity != null server-side.
            getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.Drop, new object[] { });

            // Some fucking magic
            getClient().ClManager.SendToServer(PacketFlags.Reliable, SvPacket.TransferView, new object[]
            {
                (byte)1,
                item.index,
                amount
            });

            // Automatically destroy the drop menu
            if (destroyMenuThread == null)
            {
                destroyMenuThread = DestroyNextMenu();
                getClient().StartCoroutine(destroyMenuThread);
            }

            Log($"Successfully gave {amount} {item.itemName}");
        }

        string getCurrentKitPath()
        {
            return FileManager.KitsPath + kitName.getValue() + ".txt";
        }

        private void EditKit()
        {
            var kitPath = getCurrentKitPath();

            if (!File.Exists(kitPath))
            {
                Log($"Kit does not exist, creating {kitPath}");
                using (var file = File.CreateText(kitPath))
                {
                    file.WriteLine("# Add items line by line in the following syntax: ItemName amount");
                    file.WriteLine("# Lines startng with \"#\" are ignored.");
                    if (kitName.getValue() == "Default")
                    {
                        file.WriteLine("# Default file example:");
                        file.WriteLine("Colt 1");
                        file.WriteLine("AmmoPistol 100");
                        file.WriteLine("MedicBox0 2");
                        file.WriteLine("MedicBox1 2");
                    }

                }

            }

            Application.OpenURL(kitPath);
        }

       


        private void OpenGiveMenu()
        {
            /*
            ViewMenu menu = new ViewMenu();
            contentPanelRef(menu) = contentPanelRef(viewInventoryMenuRef(getClient().ClManager));
            try
            {
                foreach (var entity in getClient().SceneManager.entityCollection)
                {
                    ShItem item = entity.Value as ShItem;
                    if (!item) continue;

                    InventoryItem invItem = new InventoryItem(item, 1, 0);
                    if (invItem == null) continue;

                    Log($"Creating button: {invItem.item.itemName}");
                    menu.CreateButton(new ItemInfo(contentPanelRef(menu), invItem, false, invItem.item.GetSortableName, ButtonType.Others));
                }
                getClient().ClManager.ShowMenu(menu, new object[] { });
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
            */

            ClManager manager = getClient().ClManager;
            if (!manager)
            {
                Log("You have to be in game!");
                return;
            }

            List<LabelID> list = new List<LabelID>();

            var itemlist = onlyModded.isEnabled() ? GetModdedItems() : GetItems();
            foreach (ShEntity entity in itemlist)
            {
                LabelID itemLabel = new LabelID(entity.name, entity.name);
                list.Add(itemLabel);
            }

            if (list.Count <= 0)
            {
                Log("No items found!");
                return;
            }

            float xMin = 0.25f;
            float yMin = 0.1f;
            float xMax = 0.75f;
            float yMax = 0.9f;

            int targetID = manager.myID;
            LabelID[] options = list.ToArray();
            LabelID[] actions = new LabelID[]
            {
                new LabelID("Give 1", "1"),
                new LabelID("Give 10", "10"),
                new LabelID("Give 100", "100"),
                new LabelID("Give 1000", "1000"),
                new LabelID("Give 10000", "10000"),
                new LabelID("Give 100000", "100000"),
                new LabelID("Give 1000000", "1000000"),
                new LabelID("Give 10000000", "10000000"),
                new LabelID("Give 100000000", "100000000"),
                new LabelID("Give 1000000000", "1000000000"),
                new LabelID("Give 2147483647", "2147483647"),
            };


            OptionMenu optionMenu = optionMenuRef(manager);
            manager.ShowMenu(optionMenu, new object[] 
            {
                xMin,
                yMin,
                xMax,
                yMax,
                MenuID,
                MenuTitle,
                targetID,
                JsonConvert.SerializeObject(options),
                JsonConvert.SerializeObject(actions)
            });

        }

        public override bool onSendToServer(PacketFlags channel, SvPacket packet, params object[] args)
        {
            if (packet != SvPacket.OptionAction) return true;

            int ID = (int) args[0];
            string menuID = (string) args[1];

            if (ID != getClient().ClManager.myID) return true;
            if (menuID != MenuID) return true;

            string itemname = (string) args[2];
            string action = (string) args[3];

            int amount = Int32.Parse(action);


            ShItem item;
            if (!getClient().SceneManager.TryGetEntity<ShItem>(itemname, out item))
            {
                Log($"Item {itemname} not found!");
                return false;
            }

            GiveItem(item, amount);

            return false;
        }

        IEnumerable<ShItem> GetItems()
        {
            return (from i in getClient().SceneManager.entityCollection.Values where i is ShItem where !(i is ShFurniture) select i).Cast<ShItem>();
        }

        IEnumerable<ShItem> GetModdedItems()
        {
            var customObjects = getClient().SceneManager.customObjects;
            return (from i in getClient().SceneManager.entityCollection.Values where i is ShItem where customObjects.ContainsValue(i.gameObject) select i).Cast<ShItem>();
        }


        
        IEnumerator DestroyNextMenu()
        {
            while (true)
            {
                ViewMenu menu = getClient().ClManager.CurrentMenu as ViewMenu;
                if (menu)
                {
                    menu.CloseMenu(true);
                    destroyMenuThread = null;
                    yield break;
                }
                yield return null;
            }
        }


    }
}
