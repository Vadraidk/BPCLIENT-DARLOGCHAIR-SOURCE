using BrokeProtocol.Client.UI;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System;

namespace BrokeProtocolClient.modules.render
{
    class Skeleton : Module
    {
        BooleanSetting humans = new BooleanSetting("Players", true);
        BooleanSetting npcs = new BooleanSetting("Npcs", false);

        NumberSetting lineWidth = new NumberSetting("Line width", 0, 10, 1, 0.1);

        ColorSetting color = new ColorSetting("Color", Color.cyan);

        List<ShPlayer> entities = new List<ShPlayer>();

        public Skeleton() : base(Categories.Render, "Skeleton", "Makes players skeleton visible through walls")
        {
            addSetting(humans);
            addSetting(npcs);

            addSetting(lineWidth);
            addSetting(color);
        }

        public override void onActivate()
        {

        }

        public override void onDeactivate()
        {

        }

        public override void onRender()
        {
            entities.Clear();
            if (humans.isEnabled()) entities.AddRange((EntityCollections.Humans as Collection<ShPlayer>).ToList());
            if (npcs.isEnabled()) entities.AddRange((EntityCollections.NPCs as Collection<ShPlayer>).ToList());

            foreach (ShPlayer player in entities)
            {
                drawSkeleton(player);
            }
        }

        public override void onUpdate()
        {

        }

        private void drawSkeleton(ShPlayer player)
        {
            if (!player) return;
            if (player.IsDead) return;
            if (player.headCollider.bounds.size == Vector3.zero || !player.IsUp) return;
            if (player == getClient().ClManager.myPlayer) return;

            foreach (Transform bone in player.clPlayer.skinnedMeshRenderer.bones)
            {

                // Left leg
                // 0-1 1-2 2-3

                // Right leg
                // 0-4 4-5 5-6

                // Spine
                // 0-7 7-8 8-15

                // Left arm
                // 15-9 9-10 10-11

                // Right arm
                // 15-12 12-13 13-14

                if (bone.parent.name == "Armature") continue;

                Vector3 boneScreenPos = getClient().MainCamera.worldCamera.WorldToScreenPoint(bone.position);
                Vector3 parentBoneScreenPos = getClient().MainCamera.worldCamera.WorldToScreenPoint(bone.parent.position);

                if (boneScreenPos.z < 0f) continue;
                if (parentBoneScreenPos.z < 0f) continue;

                Render.DrawLine(new Vector2(boneScreenPos.x, Screen.height - boneScreenPos.y), new Vector2(parentBoneScreenPos.x, Screen.height - parentBoneScreenPos.y), color.getColor(), lineWidth.getValueFloat());
            }
        }


    }
}
