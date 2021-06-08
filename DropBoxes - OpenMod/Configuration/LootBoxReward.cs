using DropBoxes.API;
using DropBoxes.Items;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Users;
using System;
using System.Threading.Tasks;

namespace DropBoxes.Configuration
{
    [Serializable]
    public class LootBoxReward : ILootBoxReward
    {
        public string Item { get; set; } = "";

        public float Weight { get; set; } = 0;

        private IItemAsset? _itemAsset;

        public async Task<IItemAsset?> GetItemAsset(IServiceProvider serviceProvider)
        {
            var itemDirectory = serviceProvider.GetRequiredService<IItemDirectory>();

            _itemAsset ??= await itemDirectory.FindByIdAsync(Item) ?? await itemDirectory.FindByNameAsync(Item);

            return _itemAsset;
        }

        public async Task<IItemInstance?> GiveUser(UnturnedUser user, IServiceProvider serviceProvider)
        {
            var itemSpawner = serviceProvider.GetRequiredService<IItemSpawner>();
            var itemAsset = await GetItemAsset(serviceProvider) as UnturnedItemAsset ??
                            throw new Exception("Invalid item asset");

            return await itemSpawner.GiveItemAsync(user.Player.Inventory, itemAsset, new AdminItemState(itemAsset));
        }
    }
}
