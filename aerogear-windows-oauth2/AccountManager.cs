using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public sealed class AccountManager
    {
        private IDictionary<string, OAuth2Module> modules = new Dictionary<string, OAuth2Module>();
        private static readonly AccountManager instance = new AccountManager();

        private AccountManager() { }

        public static AccountManager Instance
        {
            get
            {
                return instance;
            }
        }

        public static OAuth2Module AddAccount(Config config)
        {
            if (Instance.modules.ContainsKey(config.accountId))
            {
                return Instance.modules[config.accountId];
            }
            else
            {
                OAuth2Module module = new OAuth2Module(config);
                Instance.modules[config.accountId] = module;
                return module;
            }
        }

        public static OAuth2Module GetAccountByName(string name)
        {
            return Instance.modules[name];
        }

        public static OAuth2Module GetAccountByClientId(string clientId)
        {
            return Instance.modules.Where(entry => entry.Value.config.clientId == clientId).Single().Value;
        }

        public async static Task<OAuth2Module> ParseContinuationEvent(Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args)
        {
            var module = GetAccountByName((string)args.ContinuationData["name"]);
            await module.ExtractCode(new Uri(args.WebAuthenticationResult.ResponseData).Query);
            return module;
        }
    }
}
