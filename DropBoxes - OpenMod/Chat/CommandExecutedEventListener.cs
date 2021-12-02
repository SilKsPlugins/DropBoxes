extern alias JetBrainsAnnotations;
using DropBoxes.Configuration;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.Core.Commands.Events;
using SilK.Unturned.Extras.Configuration;
using System.Drawing;
using System.Threading.Tasks;

namespace DropBoxes.Chat
{
    [UsedImplicitly]
    public class CommandExecutedEventListener : IEventListener<CommandExecutedEvent>
    {
        private readonly IConfigurationParser<DropBoxesConfiguration> _configuration;

        public CommandExecutedEventListener(IConfigurationParser<DropBoxesConfiguration> configuration)
        {
            _configuration = configuration;
        }

        public async Task HandleEventAsync(object? sender, CommandExecutedEvent @event)
        {
            if (@event.CommandContext.Exception is UserFriendlyException ex &&
                @event.CommandContext.CommandRegistration?.Component.GetType() == typeof(DropBoxesPlugin))
            {
                @event.ExceptionHandled = true;
                await @event.Actor.PrintMessageWithIconAsync(ex.Message, _configuration.Instance.IconUrl, Color.DarkRed);
            }
        }
    }
}
