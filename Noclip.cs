using BrokeProtocol.Client.UI;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Utility;
using BrokeProtocolClient.settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BrokeProtocolClient.modules.movement
{
    class Noclip : Module
    {
        NumberSetting speed = new NumberSetting("Speed", 1, 1000, 180, 1);
        NumberSetting speedBoost = new NumberSetting("Speed boost multiplier", 1, 10, 5, 0.5);

        Rigidbody rb;
        Vector2 sensitivity = MonoBehaviourSingleton<ClManager>.Instance.sensitivity;

        public Noclip() : base(Categories.Movement, "Noclip", "Allows to fly and phase through walls")
        {
            addSetting(speed);
            addSetting(speedBoost);
        }

        public override void onActivate()
        {
            if (!getClient().ClManager.myPlayer) return;

            rb = getClient().ClManager.myPlayer.GetComponent<Rigidbody>();

            rb.isKinematic = true;
            getClient().ClManager.myPlayer.capsule.isTrigger = true;
            getClient().ClManager.myPlayer.headCollider.isTrigger = true;
            getClient().ClManager.sensitivity.x = sensitivity.x * 3;
        }

        public override void onDeactivate()
        {
            if (!getClient().ClManager.myPlayer) return;
            if (!rb) return;

            rb.isKinematic = false;
            getClient().ClManager.myPlayer.capsule.isTrigger = false;
            getClient().ClManager.myPlayer.headCollider.isTrigger = false;
            getClient().ClManager.sensitivity = sensitivity;
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            if (!getClient().ClManager.myPlayer) return;
            if (!rb) return;

            ShPlayer player = getClient().ClManager.myPlayer;
            rb.isKinematic = true;

            Vector3 vector = Vector3.zero;
            var boost = 1.0f;

            if (Keyboard.current.wKey.isPressed)
                vector += player.transform.forward;
            if (Keyboard.current.sKey.isPressed)
                vector -= player.transform.forward;
            if (Keyboard.current.aKey.isPressed)
                vector -= player.transform.right;
            if (Keyboard.current.dKey.isPressed)
                vector += player.transform.right;
            if (Keyboard.current.spaceKey.isPressed)
                vector += Vector3.up;
            if (Keyboard.current.ctrlKey.isPressed)
                vector += Vector3.down;
            if (Keyboard.current.shiftKey.isPressed)
                boost = speedBoost.getValueFloat();

            rb.MovePosition(player.GetPosition + vector * speed.getValueFloat() * boost * Time.deltaTime);
        }
    }
}
