using BrokeProtocol.Client.UI;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocolClient.modules.misc;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


namespace BrokeProtocolClient.modules.render
{
    class Esp : Module
    {
        ModeSetting mode = new ModeSetting("Mode", Mode.Adaptive);
        ModeSetting lineMode = new ModeSetting("Line mode", LineMode.Bottom);

        BooleanSetting humans = new BooleanSetting("Players", true);
        BooleanSetting npcs = new BooleanSetting("Npcs", false);

        BooleanSetting boxes = new BooleanSetting("Boxes", true);
        BooleanSetting fill = new BooleanSetting("Fill", true);
        BooleanSetting lines = new BooleanSetting("Lines", true);
        BooleanSetting healthbar = new BooleanSetting("Health bar", true);

        BooleanSetting showVisibility = new BooleanSetting("Show visibility color", true);
        BooleanSetting jobColor = new BooleanSetting("Use color of job", false);

        ColorSetting boxColor = new ColorSetting("Box color", Color.cyan);
        ColorSetting fillColor = new ColorSetting("Fill color", Color.cyan);
        NumberSetting fillAlpha = new NumberSetting("Fill alpha", 0, 1, 0.1, 0.01);
        ColorSetting lineColor = new ColorSetting("Line color", Color.cyan);
        NumberSetting lineWidth = new NumberSetting("Line width", 0, 10, 1, 0.1);
        NumberSetting healthbarWidth = new NumberSetting("Health bar width", 0, 10, 0.2, 0.05);

        BooleanSetting colorFriends = new BooleanSetting("Use friend color", true);
        ColorSetting friendColor = new ColorSetting("Friend color", Color.magenta);

        Color visibleColor = Color.green;
        Color invisibleColor = Color.red;

        List<ShPlayer> entities = new List<ShPlayer>();

        public Esp() : base(Categories.Render, "Esp", "Makes other players visible through walls")
        {
            addSetting(mode);
            addSetting(lineMode);

            addSetting(new InfoSetting("Whitelist:"));
            addSetting(humans);
            addSetting(npcs);

            addSetting(new InfoSetting("Style:"));
            addSetting(boxes);
            addSetting(fill);
            addSetting(lines);
            addSetting(healthbar);
            addSetting(showVisibility);
            addSetting(jobColor);
            addSetting(boxColor);
            addSetting(fillColor);
            addSetting(fillAlpha);
            addSetting(lineColor);
            addSetting(lineWidth);
            addSetting(healthbarWidth);
        }

        public override void onRender()
        {
            entities.Clear();
            if (humans.isEnabled()) entities.AddRange((EntityCollections.Humans as Collection<ShPlayer>).ToList());
            if (npcs.isEnabled()) entities.AddRange((EntityCollections.NPCs as Collection<ShPlayer>).ToList());

            foreach (ShPlayer player in entities)
            {
                drawPlayer(player);
            }
        }

        private void drawPlayer(ShPlayer player)
        {
            if (!player) return;
            if (player == getClient().ClManager.myPlayer) return;
            if (player.IsDead) return;
            if (player.IsUp && player.headCollider.bounds.size == Vector3.zero) return;

            Rect rect;
            bool isOnScreen;
            if (mode.isMode(Mode.Fixed))
            {
                isOnScreen = getFixed(player, out rect);
            }
            else
            {
                isOnScreen = getAdaptive(player, out rect);
            }
            if (!isOnScreen) return;

            Vector2 pos = new Vector2(rect.x, rect.y);
            Vector2 size = new Vector2(rect.width, rect.height);

            Color _boxColor = boxColor.getColor();
            Color _fillColor = fillColor.getColor();
            Color _lineColor = lineColor.getColor();

            if (jobColor.isEnabled())
            {
                _boxColor = player.GetJobInfoShared().GetColor(_boxColor.a);
                _fillColor = player.GetJobInfoShared().GetColor(_boxColor.a);
                _lineColor = player.GetJobInfoShared().GetColor(_lineColor.a);
            }

            if (showVisibility.isEnabled())
            {
                var color = PlayerUtils.IsVisible(player) ? visibleColor : invisibleColor;
                _boxColor = color;
                _fillColor = color;
                _lineColor = color;
            }

            if (colorFriends.isEnabled())
            {
                bool isFriend = FriendManager.getFriends().Contains(player.username);
                if (isFriend)
                {
                    _boxColor = friendColor.getColor();
                    _fillColor = friendColor.getColor();
                    _lineColor = friendColor.getColor();
                }
            }

            if (lines.isEnabled())
            {
                var startPos = lineMode.isMode(LineMode.Bottom) ? new Vector2(Screen.width / 2, Screen.height) : new Vector2(Screen.width / 2, Screen.height / 2);
                var endPos = new Vector2(pos.x + size.x * 0.5f, pos.y);
                //int b = isOnScreen ? 0 : 1;
                //int sw = pos.x > Screen.width * 0.5f ? 1 : -1;
                //int sh = pos.y > Screen.height * 0.5f ? 1 : -1;

                Render.DrawLine(startPos, endPos, _lineColor, lineWidth.getValueFloat());
            }


            if (fill.isEnabled())
            {
                _fillColor.a = fillAlpha.getValueFloat();
                Render.DrawBox(pos, size, _fillColor, false);
            }

            if (boxes.isEnabled())
                Render.DrawBox(pos.x, pos.y, size.x, size.y, _boxColor, lineWidth.getValueFloat());

            if (healthbar.isEnabled())
            {
                var healthColor = new Color(1 - player.health / player.maxStat, player.health / player.maxStat, 0);

                var _size = new Vector2(Mathf.Abs(size.y / size.x / 2 + size.x / size.y) * healthbarWidth.getValueFloat(), size.y);
                var _pos = new Vector2(pos.x - _size.x, pos.y);

                Render.DrawBox(_pos, _size, Color.black, false);
                Render.DrawBox(_pos, new Vector2(_size.x, _size.y * player.health / player.maxStat), healthColor, false);
            }
        }

        private bool getFixed(ShPlayer player, out Rect rect)
        {
            rect = new Rect();

            Vector3 headPos = player.headCollider.transform.position;
            headPos.y += player.headCollider.bounds.size.y;

            Vector3 feetPos = player.transform.position;

            Vector3 screenHeadPos = getClient().MainCamera.worldCamera.WorldToScreenPoint(headPos);
            Vector3 screenfeetPos = getClient().MainCamera.worldCamera.WorldToScreenPoint(feetPos);

            if (screenfeetPos.z <= 0f || screenHeadPos.z <= 0f) return false;

            float height = screenHeadPos.y - screenfeetPos.y;
            float widthOffset = 3f;
            float width = height / widthOffset;

            Vector2 pos = new Vector2(screenfeetPos.x - width / 2, Screen.height - screenfeetPos.y);
            Vector2 size = new Vector2(width, -height);

            rect = new Rect(pos, size);
            return true;
        }

        private bool getAdaptive(ShPlayer player, out Rect rect)
        {
            rect = new Rect();

            //Bounds bounds = player.capsule.bounds;

            Vector3 cen = player.capsule.bounds.center;
            cen.y += player.headCollider.bounds.extents.y;

            Vector3 ext = player.capsule.bounds.extents;
            ext.y += player.headCollider.bounds.extents.y / 2;

            Vector3[] extentPoints = new Vector3[8]
            {
                 WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
                 WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
                 WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
                 WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
                 WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
                 WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
                 WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
                 WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
            };
            Vector3 min = extentPoints[0];
            Vector3 max = extentPoints[0];
            foreach (Vector3 vec in extentPoints)
            {
                min = Vector3.Min(min, vec);
                max = Vector3.Max(max, vec);
            }

            if (min.z < 0f || max.z < 0f) return false;

            Vector2 pos = new Vector2(min.x, max.y);
            Vector2 size = new Vector2(max.x - min.x, min.y - max.y);

            rect = new Rect(pos, size);
            return true;
        }

        public Vector3 WorldToScreenPoint(Vector3 world)
        {
            Vector3 screenPoint = getClient().MainCamera.worldCamera.WorldToScreenPoint(world);
            screenPoint.y = (float)Screen.height - screenPoint.y;
            return screenPoint;
        }

        enum Mode
        {
            Fixed,
            Adaptive
        }

        enum LineMode
        {
            Bottom,
            Center
        }

    }
}