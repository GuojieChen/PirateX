using System;
using PirateX.GException;
using PirateX.GException.V1;
using PirateX.Protocol;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace PirateX.Filters
{
    /// <summary>
    /// Seed创建之后的动作
    /// </summary>
    public class SeedCreatedFilter : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            var seedCreated = commandContext.Session.Items.GetValue<bool>(KeyStore.FilterSeedCreated);
            if (seedCreated)
                throw new GameException(ServerCode.SeedReCreate); 
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {
            commandContext.Session.Items[KeyStore.FilterSeedCreated] = true;

            ((IGameSession)commandContext.Session).ProtocolPackage.CryptoEnable = true;
        }
    }
}
