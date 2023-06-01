using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocolClient.settings;
using BrokeProtocolClient.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokeProtocolClient.modules.misc
{
    class FriendManager : Module
    {
        InputSetting friendInput = new InputSetting("Friends username/ID", 16, "");
        ActionSetting addFriendButton;
        FileSetting friendsFile = new FileSetting("Friends file", FileManager.MainFolderPath + FileManager.FriendsFile);

        static List<string> friendlyPlayers = new List<string>();

        public FriendManager() : base(Categories.Misc, "Friend Manager", "Allows to manage friends")
        {
            addSetting(friendInput);

            addFriendButton = new ActionSetting("Add friend", addFriendFunc);
            addSetting(addFriendButton);

            addSetting(friendsFile);
            friendlyPlayers = friendsFile.readLines();
        }

        public override void onActivate()
        {
            
        }

        public override void onDeactivate()
        {
            
        }

        public override void onRender()
        {
            
        }

        public override void onUpdate()
        {
            
        }

        public static List<string> getFriends()
        {
            return friendlyPlayers;
        }

        private void addFriendFunc()
        {
            addFriend(friendInput.getValue());
        }

        private void addFriend(string usernameOrId)
        {
            ShPlayer target;
            if (!EntityCollections.TryGetPlayerByNameOrID(usernameOrId, out target))
            {
                ConsoleBase.WriteLine($"{usernameOrId} not found!");
                return;
            }

            friendlyPlayers.Add(target.username);
            friendsFile.writeLine(target.username);

        }
    }
}
