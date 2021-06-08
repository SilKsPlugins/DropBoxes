extern alias JetBrainsAnnotations;
using DropBoxes.API;
using DropBoxes.Configuration;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API.Commands;
using SilK.Unturned.Extras.Configuration;
using System;
using System.Threading.Tasks;

namespace DropBoxes.LootBoxes
{
    [UsedImplicitly]
    public class LootBoxAssetCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        private readonly IConfigurationParser<DropBoxesConfiguration> _configuration;

        public LootBoxAssetCommandParameterResolveProvider(
            IConfigurationParser<DropBoxesConfiguration> configuration)
        {
            _configuration = configuration;
        }

        public bool Supports(Type type)
        {
            return type == typeof(LootBoxAsset) || type == typeof(ILootBoxAsset);
        }

        public Task<object?> ResolveAsync(Type type, string input)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(input))
            {
                return Task.FromResult((object?)null);
            }

            if (!Supports(type))
            {
                throw new ArgumentException("The given type is not supported", nameof(type));
            }

            var configuration = _configuration.Instance;

            return Task.FromResult<object?>(configuration.GetLootBoxAssetById(input) ??
                                            configuration.GetLootBoxAssetByName(input));
        }

    }
}
