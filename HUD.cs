using System;
using System.Collections.Generic;
using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using HarmonyLib;
using UnityEngine;

namespace BrokeProtocolClient.modules.render
{
    class HUD : Module
    {
        BooleanSetting clientLogo = new BooleanSetting("Client name", true);
        BooleanSetting moduleList = new BooleanSetting("Active modules", true);
        BooleanSetting fps = new BooleanSetting("FPS", true);

        BooleanSetting rainbow = new BooleanSetting("Rainbow", true);
        NumberSetting rainbowSpeed = new NumberSetting("Rainbow speed", 0.01, 1, 0.15, 0.01);
        NumberSetting rainbowOffset = new NumberSetting("Rainbow offset", 0, 0.1, 0.05, 0.001);

        NumberSetting size = new NumberSetting("Size", 10, 100, 16, 1);
        NumberSetting offsetX = new NumberSetting("Offset X", 0, 500, 5, 1);
        NumberSetting offsetY = new NumberSetting("Offset Y", 0, 2, 0.7, 0.05);

        ColorSetting contentColor = new ColorSetting("Color", Color.cyan);

        BooleanSetting weaponInfo = new BooleanSetting("Weapon info", true);
        BooleanSetting weaponCooldownInfo = new BooleanSetting("Weapon cooldown", true);
        NumberSetting weaponInfoHeight = new NumberSetting("Cooldown height", 0, 1, 0.98, 0.01);

        ActionSetting notifTest;
        InputSetting notifText = new InputSetting("Notification text", 512, "");
        NumberSetting notifTime = new NumberSetting("Notification time", 0, 10, 1, 0.5);


        int offset = 0;
        float framesPerSecond = 0;

        public static string ClientLogo = "DARLOGCHEAT BY VADRA";

        public HUD() : base(Categories.Render, "HUD", "Displays useful info")
        {
            addSetting(clientLogo);
            addSetting(moduleList);
            addSetting(fps);

            addSetting(rainbow);
            addSetting(contentColor);

            addSetting(rainbowSpeed);
            addSetting(rainbowOffset);

            addSetting(size);
            addSetting(offsetX);
            addSetting(offsetY);

            addSetting(new InfoSetting("Weapon info"));
            addSetting(weaponInfo);
            addSetting(weaponCooldownInfo);
            addSetting(weaponInfoHeight);

            notifTest = new ActionSetting("Notification test", Notify);
            addSetting(notifTest);
            addSetting(notifText);
            addSetting(notifTime);
        }

        public override void onRender()
        {
            offset = 0;

            int originalSize = Render.StringStyle.fontSize;
            Render.StringStyle.fontSize = size.getValueInt();

            if (clientLogo.isEnabled()) 
                renderLogo();

            if (fps.isEnabled()) 
                renderFPS();

            if (moduleList.isEnabled()) 
                renderActiveModules();

            if (weaponInfo.isEnabled()) 
                renderWeaponInfo();

            Render.StringStyle.fontSize = originalSize;
        }

        public override void onUpdate()
        {
            framesPerSecond = 1.0f / Time.deltaTime;
        }

        private Color getRainbow()
        {
            return Color.HSVToRGB(Mathf.PingPong(Time.time * rainbowSpeed.getValueFloat() + offset * rainbowOffset.getValueFloat(), 1), 1, 1);
        }

        private void renderActiveModules()
        {
            List<Module> list = new List<Module>(Modules.instance.getActive());
            // Sort list of modules by name width
            list.Sort((a, b) => Render.StringStyle.CalcSize(new GUIContent(b.getName())).x.CompareTo(Render.StringStyle.CalcSize(new GUIContent(a.getName())).x));

            foreach (Module module in list)
            {
                var color = rainbow.isEnabled() ? getRainbow() : contentColor.getColor();
                var textSize = Render.StringStyle.CalcSize(new GUIContent(module.getName()));

                var height = Mathf.CeilToInt(textSize.y * offsetY.getValueFloat());

                Render.DrawString(new Vector2(Screen.width - textSize.x - offsetX.getValueFloat(), height * offset), module.getName(), color, false);
                offset++;
            }
        }

        private void renderLogo()
        {
            Render.StringStyle.fontStyle = FontStyle.BoldAndItalic;
            var color = rainbow.isEnabled() ? getRainbow() : contentColor.getColor();

            var textSize = Render.StringStyle.CalcSize(new GUIContent(ClientLogo));
            var height = Mathf.CeilToInt(textSize.y * offsetY.getValueFloat());

            Render.DrawString(new Vector2(Screen.width - textSize.x - offsetX.getValueFloat(), height * offset), ClientLogo, color, false);
            Render.StringStyle.fontStyle = FontStyle.Normal;
            offset++;
        }

        private void renderFPS()
        {
            var color = rainbow.isEnabled() ? getRainbow() : contentColor.getColor();

            string FPS = framesPerSecond.ToString("0");
            var textSize = Render.StringStyle.CalcSize(new GUIContent(FPS));
            var height = Mathf.CeilToInt(textSize.y * offsetY.getValueFloat());

            Render.DrawString(new Vector2(Screen.width - textSize.x - offsetX.getValueFloat(), height * offset), FPS, color, false);
            offset++;
        }

        AccessTools.FieldRef<ShMountable, float> nextFireTimeRef = AccessTools.FieldRefAccess<ShMountable, float>("nextFire");
        private void renderWeaponInfo()
        {
            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            ShUsable weapon = local.curEquipable as ShUsable;
            if (!weapon) return;

            if (weaponCooldownInfo.isEnabled())
            {
                var holderSize = new Vector2(Screen.width * 0.2f, Screen.height * 0.005f);
                var holderPos = new Vector2(Screen.width / 2 - holderSize.x / 2, Screen.height * weaponInfoHeight.getValueFloat());

                var fireCooldown = nextFireTimeRef(weapon) - Time.time;
                //Log($"fireCooldown: {fireCooldown}");
                var useDelayFill = Mathf.Lerp(0, holderSize.x, fireCooldown / weapon.useDelay + fireCooldown);
                var contentSize = new Vector2(useDelayFill, holderSize.y);
            
                Render.DrawBox(holderPos, holderSize, Color.black, false);
                Render.DrawBox(holderPos, contentSize, Color.cyan, false);
            }

        }

        private void Notify()
        {
            InfoUtils.Notify(notifText.getValue(), notifTime.getValueFloat());
        }
    }
}
