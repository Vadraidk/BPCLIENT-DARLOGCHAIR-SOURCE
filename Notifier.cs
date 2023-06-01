using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrokeProtocolClient.modules.misc
{
    class Notifier : Module
    {
        BooleanSetting onEnterExitView = new BooleanSetting("Entering and leaving your view", true);
        BooleanSetting onDie = new BooleanSetting("Player dying", true);

        List<ShPlayer> lastPlayers = new List<ShPlayer>();

        IEnumerator CheckPlayersThread = null;

        public Notifier() : base(Categories.Misc, "Notifier", "")
        {
            addSetting(onEnterExitView);
            addSetting(onDie);
        }

        public override void onActivate()
        {
            if (CheckPlayersThread == null)
            {
                CheckPlayersThread = CheckPlayers();
                getClient().StartCoroutine(CheckPlayersThread);
            }
        }

        public override void onDeactivate()
        {
            if (CheckPlayersThread != null)
            {
                getClient().StopCoroutine(CheckPlayersThread);
            }
        }

        IEnumerator CheckPlayers()
        {
            var delay = new WaitForSeconds(1f);

            while (true)
            {
                yield return delay;

                if (!onEnterExitView.isEnabled()) continue;

                var local = getClient().ClManager.myPlayer;
                if (!local) continue;

                var players = PlayersInView();

                var enteredPlayers = players.Except(lastPlayers);
                var leftPlayers = lastPlayers.Except(players);

                foreach (var player in enteredPlayers)
                {
                    InfoUtils.Notify($"{player.displayName} has entered your view!", 3);
                }

                foreach (var player in leftPlayers)
                {
                    InfoUtils.Notify($"{player.displayName} has left your view!", 3);
                }


                lastPlayers.Clear();
                foreach (var player in PlayersInView())
                {
                    lastPlayers.Add(player);
                }

            }
        }

        public void OnDie(ShPlayer player, ShPlayer attacker)
        {
            if (!onDie.isEnabled()) return;
            if (!player.isHuman) return;

            var local = getClient().ClManager.myPlayer;
            if (!local) return;

            string label;
            if (attacker) label = $"{player.displayName} has been killed by {attacker.displayName}!";
            else label = $"{player.displayName} has died!";

            InfoUtils.Notify(label, 3);
        }

        IEnumerable<ShPlayer> PlayersInView()
        {
            return (from p in EntityCollections.Humans where p.isActiveAndEnabled where p != getClient().ClManager.myPlayer select p);
        }
    }
}
