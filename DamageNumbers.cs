using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrokeProtocolClient.modules.render
{
    class DamageNumbers : Module
    {
        static NumberSetting verticalOffset = new NumberSetting("Vertical offset", 0, 10, 1, 0.5);

        BooleanSetting onlyPlayers = new BooleanSetting("Only players", true);
        NumberSetting lifetime = new NumberSetting("Number lifetime", 0.1, 5, 1, 0.1);
        NumberSetting size = new NumberSetting("Number size", 1, 64, 16, 1);
        NumberSetting randomOffset = new NumberSetting("Random offset", 0, 5, 1, 0.5);
        ColorSetting color = new ColorSetting("Number color", Color.green);

        List<NumberParticle> numbers = new List<NumberParticle>();

        public DamageNumbers() : base(Categories.Render, "Damage Numbers", "Shows taken damage")
        {
            addSetting(onlyPlayers);
            addSetting(lifetime);
            addSetting(verticalOffset);
            addSetting(randomOffset);
            addSetting(size);
            addSetting(color);
        }

        public override void onRender()
        {
            foreach(NumberParticle number in numbers)
            {
                number.render();
            }
        }

        public void TookDamage(ShDestroyable destroyable, float damage)
        {
            // if damage is positive destroyable got healed
            if (damage <= 0) return;

            if (onlyPlayers.isEnabled() && !(destroyable is ShPlayer)) return;

            Vector3 position = destroyable.GetOrigin.Random(randomOffset.getValueFloat());
            getClient().StartCoroutine(CreateDamageNumber(damage, position));
        }

        IEnumerator CreateDamageNumber(float number, Vector3 position)
        {
            NumberParticle particle = new NumberParticle(number, position, size.getValueInt(), lifetime.getValueFloat(), color.getColor());
            numbers.Add(particle);
            yield return new WaitForSeconds(particle.lifetime);
            numbers.Remove(particle);
        }

        class NumberParticle
        {
            public float lifetime;
            public float number;

            Vector3 startPos;
            Vector3 pos;
            Vector3 endPos;

            float timealive;
            int size;
            Color color;

            public NumberParticle(float number, Vector3 startPos, int size, float lifetime, Color color)
            {
                this.number = number;
                this.lifetime = lifetime;
                this.startPos = startPos;
                this.size = size;
                this.color = color;

                pos = startPos;
                endPos = startPos;
                endPos.y += verticalOffset.getValueFloat();

                timealive = 0;
            }

            public void render()
            {
                pos = Vector3.Lerp(pos, endPos, Time.deltaTime / lifetime);

                timealive += Time.deltaTime;
                // Start to fade out after 75% of the lifetime
                float timefraction = 0.75f;
                if (timealive > lifetime * timefraction)
                    color.a = Mathf.Lerp(color.a, 0, Time.deltaTime / lifetime / (1 - timefraction));
                
                Color backgroundColor = new Color(0, 0, 0, color.a);

                string label = number.ToString("0.#");
                Render.DrawWorldString(pos, label, backgroundColor, size + 2, true);
                Render.DrawWorldString(pos, label, color, size, true);
            }

        }
    }
}
