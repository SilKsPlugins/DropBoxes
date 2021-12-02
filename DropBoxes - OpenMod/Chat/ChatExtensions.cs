using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System.Drawing;
using System.Threading.Tasks;

namespace DropBoxes.Chat
{
    public static class ChatExtensions
    {
        public static async Task PrintMessageWithIconAsync(this ICommandActor actor, string message, string iconUrl)
        {
            if (actor is UnturnedUser user)
            {
                await user.PrintMessageAsync(message, Color.White, true, iconUrl);
            }
        }

        public static async Task PrintMessageWithIconAsync(this ICommandActor actor, string message, string iconUrl, Color color)
        {
            if (actor is UnturnedUser user)
            {
                await user.PrintMessageAsync(message, color, true, iconUrl);
            }
        }

        public static async Task PrintMessageWithIconAsync(this CommandBase command, string message, string iconUrl)
        {
            if (command.Context.Actor is UnturnedUser user)
            {
                await user.PrintMessageAsync(message, Color.White, true, iconUrl);
            }
            else
            {
                await command.PrintAsync(message);
            }
        }
    }
}
