using Microsoft.Extensions.Options;
using Nethereum.Signer;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using OptimizedVestingSelling.Settings;
using System.IO;

namespace OptimizedVestingSelling.Factories
{
    public interface IWeb3Factory
    {
        IWeb3 Create();
    }

    public class Web3Factory : IWeb3Factory
    {
        private readonly WalletSettings _walletSettings;

        private readonly NetworkSettings _networkSettings;

        public Web3Factory(
            IOptions<WalletSettings> walletSettings,
            IOptions<NetworkSettings> networkSettings)
        {
            _walletSettings = walletSettings.Value;
            _networkSettings = networkSettings.Value;
        }

        public IWeb3 Create()
        {
            var privateKey = File.ReadAllText(_walletSettings.PathToPrivateKey);
            var account = new Account(new EthECKey(privateKey), _networkSettings.ChainId);
            return new Web3(account, _networkSettings.JsonRpcUrl);
        }
    }
}
