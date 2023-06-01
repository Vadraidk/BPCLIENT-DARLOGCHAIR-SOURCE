using BrokeProtocol.Client.UI;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrokeProtocolClient.modules.render
{
    class Nametags : Module
    {
        BooleanSetting showVisibility = new BooleanSetting("Show visibility color", true);
        BooleanSetting showName = new BooleanSetting("Show username", true);
        BooleanSetting showDisplayName = new BooleanSetting("Show display name", false);
        BooleanSetting showHealth = new BooleanSetting("Show health", true);
        BooleanSetting showWeapon = new BooleanSetting("Show weapon", true);
        BooleanSetting showId = new BooleanSetting("Show ID", false);
        BooleanSetting showDistance = new BooleanSetting("Show distance", false);

        NumberSetting fontSize = new NumberSetting("Size", 8, 64, 14, 1);
        BooleanSetting jobColor = new BooleanSetting("Use color of job", false);

        BooleanSetting lastPos = new BooleanSetting("Show last seen", true);
        BooleanSetting humans = new BooleanSetting("Players", true);
        BooleanSetting npcs = new BooleanSetting("Npcs", false);
        BooleanSetting items = new BooleanSetting("Items", false);
        BooleanSetting containers = new BooleanSetting("Containers", false);
        BooleanSetting doors = new BooleanSetting("Doors", false);

        Color humanNameColor = Color.white;
        Color npcNameColor = Color.gray;
        Color itemNameColor = Color.green;
        Color containerNameColor = Color.cyan;
        Color doorNameColor = Color.blue;
        Color visibleColor = Color.green;
        Color invisibleColor = Color.red;

        List<ShEntity> itemList = new List<ShEntity>();
        List<ShEntity> containerList = new List<ShEntity>();
        List<ShDoor> doorList = new List<ShDoor>();

        private IEnumerator entityUpdate;
        readonly float playerOffset = 1.8f;

        public Nametags() : base(Categories.Render, "Nametags", "Makes others players info visible through walls")
        {
            addSetting(new InfoSetting("Display:"));
            addSetting(showId);
            addSetting(showName);
            addSetting(showDisplayName);
            addSetting(showHealth);
            addSetting(showWeapon);
            addSetting(showDistance);

            addSetting(new InfoSetting("Style:"));
            addSetting(fontSize);
            addSetting(jobColor);
            addSetting(lastPos);
            addSetting(showVisibility);

            addSetting(new InfoSetting("Whitelist:"));
            addSetting(humans);
            addSetting(npcs);
            addSetting(items);
            addSetting(containers);
            addSetting(doors);
        }

        public override void onActivate()
        {
            if (entityUpdate != null) return;

            entityUpdate = EntityUpdate(0.5f);
            getClient().StartCoroutine(entityUpdate);
        }

        public override void onDeactivate()
        {
            if (entityUpdate == null) return;

            getClient().StopCoroutine(entityUpdate);
        }

        public override void onRender()
        {
            if (humans.isEnabled())
            foreach (ShPlayer player in EntityCollections.Humans)
            {
                drawPlayer(player, humanNameColor);
            }

            if (npcs.isEnabled())
            foreach (ShPlayer player in EntityCollections.NPCs)
            {
                drawPlayer(player, npcNameColor);
            }

            if (items.isEnabled())
            foreach (ShEntity item in itemList)
            {
                drawEntity(item, itemNameColor);
            }

            if (containers.isEnabled())
            foreach (ShEntity container in containerList)
            {
                drawEntity(container, containerNameColor);
            }

            if (doors.isEnabled())
            foreach (ShDoor door in doorList)
            {
                drawEntity(door, doorNameColor);
            }

        }

        private void drawPlayer(ShPlayer player, Color color)
        {
            if (!player) return;
            if (player.transform.position == Vector3.zero) return;
            if (player == getClient().ClManager.myPlayer) return;

            Vector3 pos = player.headCollider.transform.position;
            pos.y += playerOffset;

            Vector3 screenPos = getClient().MainCamera.worldCamera.WorldToScreenPoint(pos);
            if (screenPos.z < 0f) return;

            if (showVisibility.isEnabled() && player.isHuman)
                color = PlayerUtils.IsVisible(player) ? visibleColor : invisibleColor;

            if (jobColor.isEnabled())
                color = player.GetJobInfoShared().GetColor(color.a);

            Color _backgroundColor = Color.black;
            if (player.IsUp && player.headCollider.bounds.size == Vector3.zero)
            {
                if (lastPos.isEnabled())
                {
                    color = new Color(0, 0, 0, 0.25f);
                    _backgroundColor.a = 0.25f;
                }
                else return;
            }

            string display = "";

            if (showId.isEnabled()) display += $" {player.ID}";
            if (showName.isEnabled()) display += $" {player.username}";
            if (showDisplayName.isEnabled()) display += $" {player.displayName}";
            if (showHealth.isEnabled()) display += $" {player.health:0.#}";
            if (showWeapon.isEnabled()) display += $" {player.curEquipable.itemName}";
            if (showDistance.isEnabled()) display += $" {getClient().ClManager.myPlayer.Distance(player):0}";

            Render.DrawString(new Vector2(screenPos.x, Screen.height - screenPos.y), display, color, fontSize.getValueInt());
        }

        private void drawEntity(ShEntity entity, Color color)
        {
            if (!entity) return;
            if (entity.transform.position == Vector3.zero) return;

            Vector3 pos = entity.transform.position;

            Vector3 screenPos = getClient().MainCamera.worldCamera.WorldToScreenPoint(pos);
            if (screenPos.z < 0f) return;

            string display = $"{entity.name}";
            Render.DrawString(new Vector2(screenPos.x, Screen.height - screenPos.y), display, color);
        }

        private IEnumerator EntityUpdate(float delay)
        {
            yield return new WaitForSeconds(delay);

            itemList.Clear();
            containerList.Clear();
            doorList.Clear();
            foreach (ShEntity entity in EntityCollections.Entities)
            {
                if (items.isEnabled())
                {
                    if (getClient().ClManager.myPlayer.CanCollectEntity(entity)) itemList.Add(entity);
                }

                if (containers.isEnabled())
                {
                    if (entity.Shop || entity.inventoryType == InventoryType.Locked) containerList.Add(entity);
                }

                if (doors.isEnabled())
                {
                    if (entity is ShDoor) doorList.Add(entity as ShDoor);
                }
            }
            
            entityUpdate = EntityUpdate(delay);
            getClient().StartCoroutine(entityUpdate);
        }
    }
}
