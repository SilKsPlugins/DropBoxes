using Cysharp.Threading.Tasks;
using DropBoxes.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("DropBoxes", DisplayName = "Drop Boxes", Author = "SilK")]
namespace DropBoxes
{
    public class DropBoxesPlugin : OpenModUnturnedPlugin
    {
        private readonly DropBoxesDbContext _dbContext;
        private readonly ILogger<DropBoxesPlugin> _logger;

        public DropBoxesPlugin(DropBoxesDbContext dbContext,
            ILogger<DropBoxesPlugin> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async UniTask OnLoadAsync()
        {
            _logger.LogInformation($"{DisplayName} by SilK is loading...");
            _logger.LogInformation($"For support, join my plugin discord: https://discord.gg/zWD6fg2r5b");

            try
            {
                await _dbContext.Database.MigrateAsync();
            }
            catch (ArgumentNullException ex)
            {
                if (ex.ParamName == "connectionString")
                {
                    throw new Exception("No connection string is specified in the configuration.");
                }
            }
            catch (InvalidOperationException ex) when (ex.InnerException?.Message == "Unable to connect to any of the specified MySQL hosts.")
            {
                throw new Exception("The configured connection string is incorrect.");
            }
        }
    }
}
