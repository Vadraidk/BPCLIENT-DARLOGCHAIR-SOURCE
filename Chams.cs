using BrokeProtocol.Client.UI;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;

namespace BrokeProtocolClient.modules.render
{
    class Chams : Module
    {
        BooleanSetting humans = new BooleanSetting("Players", true);
        BooleanSetting npcs = new BooleanSetting("Npcs", false);

        BooleanSetting rainbow = new BooleanSetting("Rainbow", true);
        ColorSetting color = new ColorSetting("Color", Color.green);
        NumberSetting alpha = new NumberSetting("Opacility", 0, 1, 0.1, 0.05);

        Renderer renderer;
        Material ChamsMaterial = new Material(Shader.Find("GUI/Text Shader"));

        List<ShEntity> entities = new List<ShEntity>();
        private IEnumerator chamsUpdate;

        public Chams() : base(Categories.Render, "Chams", "Makes other players glow through walls")
        {
            addSetting(humans);
            addSetting(npcs);

            addSetting(rainbow);
            addSetting(color);
            addSetting(alpha);
        }

        public override void onActivate()
        {
            addChams();

            if (chamsUpdate != null) return;

            chamsUpdate = ChamsUpdate(2f);
            getClient().StartCoroutine(chamsUpdate);
        }

        public override void onDeactivate()
        {
            if (chamsUpdate == null) return;

            getClient().StopCoroutine(chamsUpdate);
            removeChams();
        }

        public override void onRender()
        {

        }

        public override void onUpdate()
        {
            Color materialColor = rainbow.isEnabled() ? getClient().Rainbow : color.getColor();
            materialColor.a = alpha.getValueFloat();

            ChamsMaterial.color = materialColor;
        }

        private IEnumerator ChamsUpdate(float delay)
        {
            yield return new WaitForSeconds(delay);

            entities.Clear();
            if (humans.isEnabled()) entities.AddRange((EntityCollections.Humans as Collection<ShPlayer>).ToList());
            if (npcs.isEnabled()) entities.AddRange((EntityCollections.NPCs as Collection<ShPlayer>).ToList());

            removeChams();
            addChams();

            chamsUpdate = ChamsUpdate(delay);
            getClient().StartCoroutine(chamsUpdate);
        }

        private void addChams()
        {
            foreach (ShPlayer player in entities)
            {
                if (!player) continue;
                if (player == getClient().ClManager.myPlayer) continue;

                foreach (var ren in player.GetComponentsInChildren<Renderer>())
                {
                    renderer = ren;

                    Material[] materials = new Material[(renderer.materials.Length + 1)];
                    renderer.materials.CopyTo(materials, 0);

                    materials[materials.Length - 1] = ChamsMaterial;
                    renderer.materials = materials;
                }
            }
        }

        private void removeChams()
        {
            foreach (ShPlayer player in entities)
            {
                if (!player) continue;

                renderer = player.gameObject.GetComponent<Renderer>();
                renderer.materials = getClient().ClManager.myPlayer.GetComponent<Renderer>().materials;
            }
        }
    }
}
