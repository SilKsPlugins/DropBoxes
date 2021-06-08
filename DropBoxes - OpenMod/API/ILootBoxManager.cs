using OpenMod.API.Ioc;
using OpenMod.Unturned.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DropBoxes.API
{
    [Service]
    public interface ILootBoxManager
    {
        Task<IEnumerable<ILootBoxInstance>> GetLootBoxes(UnturnedUser user);

        Task GiveLootBox(ulong steamId, ILootBoxAsset lootBoxAsset);

        Task<bool> RemoveLootBox(ulong steamId, ILootBoxAsset lootBoxAsset);
    }
}
