using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Users;
using System;
using System.Threading.Tasks;

namespace DropBoxes.API
{
    public interface ILootBoxReward
    {
        Task<IItemAsset?> GetItemAsset(IServiceProvider serviceProvider);

        Task<IItemInstance?> GiveUser(UnturnedUser user, IServiceProvider serviceProvider);
    }
}
