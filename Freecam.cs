using BrokeProtocol.Utility;
using BrokeProtocolClient.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BrokeProtocolClient.modules.render
{
    class Freecam : Module
    {
        public NumberSetting speed = new NumberSetting("Speed", 1, 1000, 180, 1);
        public NumberSetting speedBoost = new NumberSetting("Speed boost multiplier", 1, 10, 5, 0.5);

        Transform originalCameraParent = null;
        CameraMode originalCameraMode = CameraMode.FirstPerson;

        bool shouldFreecam;

        public Freecam() : base(Categories.Render, "Freecam", "Allows to move camera away from the player")
        {
            addSetting(speed);
            addSetting(speedBoost);
        }

        public override void onActivate()
        {
            if (!getClient().ClManager.myPlayer) return;
            if (!getClient().MainCamera) return;

            originalCameraParent = getClient().ClManager.myPlayer.originT;
            originalCameraMode = getClient().MainCamera.cameraMode;

            shouldFreecam = true;

            getClient().MainCamera.worldCameraT.SetParent(null);
            getClient().MainCamera.cameraMode = CameraMode.FirstPerson;
        }

        public override void onDeactivate()
        {
            if (!getClient().ClManager.myPlayer) return;
            if (!getClient().MainCamera) return;

            shouldFreecam = false;


            getClient().MainCamera.worldCameraT.SetParent(originalCameraParent);
            getClient().MainCamera.worldCameraT.localPosition = Vector3.zero;
            getClient().MainCamera.worldCameraT.localRotation = Quaternion.identity;
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            if (!getClient().ClManager.myPlayer) return;
            if (!getClient().MainCamera) return;

            getClient().MainCamera.cameraMode = CameraMode.FirstPerson;

            var player = getClient().ClManager.myPlayer;

            Vector3 vector = Vector3.zero;
            if (Keyboard.current.wKey.isPressed)
                vector += new Vector3(getClient().MainCamera.worldCamera.transform.forward.x, 0, getClient().MainCamera.worldCamera.transform.forward.z).normalized;
            if (Keyboard.current.sKey.isPressed)
                vector -= new Vector3(getClient().MainCamera.worldCamera.transform.forward.x, 0, getClient().MainCamera.worldCamera.transform.forward.z).normalized;
            if (Keyboard.current.aKey.isPressed)
                vector -= getClient().MainCamera.worldCamera.transform.right;
            if (Keyboard.current.dKey.isPressed)
                vector += getClient().MainCamera.worldCamera.transform.right;
            if (Keyboard.current.spaceKey.isPressed)
                vector += Vector3.up;
            if (Keyboard.current.ctrlKey.isPressed)
                vector += Vector3.down;

            var boost = 1f;
            if (Keyboard.current.shiftKey.isPressed)
                boost = speedBoost.getValueFloat();

            getClient().MainCamera.worldCameraT.position += vector * speed.getValueFloat() * boost * Time.deltaTime;
            getClient().MainCamera.worldCameraT.rotation = getClient().ClManager.myPlayer.GetRotation;
            getClient().ClManager.myPlayer.ZeroInputs();
        }

        public bool ShouldFreecam()
        {
            return !shouldFreecam;
        }

    }
}
