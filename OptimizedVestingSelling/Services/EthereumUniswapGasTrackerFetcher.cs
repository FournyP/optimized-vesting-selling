using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using OptimizedVestingSelling.Settings;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;

namespace OptimizedVestingSelling.Services
{
    public class EthereumUniswapGasTrackerFetcher : IEthereumUniswapGasTrackerFetcher
    {
        private readonly GasTrackerSettings _gasTrackerSettings;

        public EthereumUniswapGasTrackerFetcher(IOptions<GasTrackerSettings> gasTrackerSettings)
        {
            _gasTrackerSettings = gasTrackerSettings.Value;
        }

        public async Task<BigInteger> Fetch()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(_gasTrackerSettings.Url);
                var html = await response.Content.ReadAsStringAsync();

                var document = new HtmlDocument();
                document.LoadHtml(html);

                var node = document.DocumentNode.SelectNodes("//td")
                    .Where(x => x.InnerHtml.Contains(_gasTrackerSettings.GasToken))
                    .FirstOrDefault();

                var nodeValue = node.ParentNode.ChildNodes.ElementAt(2);

                var valueStr = nodeValue.InnerHtml.Replace("$", "");
                var value = decimal.Parse(valueStr.Replace(".", ","));

                node = document.DocumentNode.SelectNodes("//span")
                    .Where(x => x.InnerHtml.Contains(_gasTrackerSettings.EthPriceToken))
                    .FirstOrDefault();

                var ethPriceStr = node.InnerHtml.Replace(_gasTrackerSettings.EthPriceToken, "").Replace("$", "");
                var ethPrice = decimal.Parse(ethPriceStr.Replace(",", "").Replace(".", ","));

                // Return in wei
                return Web3.Convert.ToWei(value / ethPrice);
            }
        }
    }
}
