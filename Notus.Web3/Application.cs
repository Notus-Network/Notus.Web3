using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Notus.Web3
{
    public class Application
    {
        private string Val_PrivateKeyHex;
        public string PrivateKeyHex
        {
            get { return Val_PrivateKeyHex; }
            set { Val_PrivateKeyHex = value; }
        }
        public class LocalWalletList
        {
            public Notus.Core.Variable.EccKeyPair Wallet { get; set; }
            public string WalletName { get; set; }
            public string WalletImage { get; set; }
            public DateTime DaysAgo { get; set; }
        }

        public static async Task<List<Notus.Core.Variable.CurrencyList>> GetCurrencyList(Notus.Core.Variable.NetworkType currentNetwork = Core.Variable.NetworkType.MainNet)
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("currency/list/", currentNetwork);
            return JsonSerializer.Deserialize<List<Notus.Core.Variable.CurrencyList>>(tmpResult);
        }
        
        public static async Task<Dictionary<string, string>> Balance(
            string WalletKey, 
            Notus.Core.Variable.NetworkType currentNetwork = Core.Variable.NetworkType.MainNet
        )
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("balance/" + WalletKey + "/", currentNetwork);
            Notus.Core.Variable.WalletBalanceStruct tmpBalanceVal = JsonSerializer.Deserialize<Notus.Core.Variable.WalletBalanceStruct>(tmpResult);
            return tmpBalanceVal.Balance;
        }
        
        public static async Task<Notus.Core.Variable.CryptoTransactionResult> AirDrop(string WalletKey, Notus.Core.Variable.NetworkType currentNetwork = Core.Variable.NetworkType.MainNet)
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("airdrop/" + WalletKey + "/", currentNetwork);
            Notus.Core.Variable.CryptoTransactionResult tmpAirDrop = JsonSerializer.Deserialize<Notus.Core.Variable.CryptoTransactionResult>(tmpResult);
            return tmpAirDrop;
        }
        public static async Task<Notus.Core.Variable.BlockStatusCode> GetStatus(string WalletKey, Notus.Core.Variable.NetworkType CurrentNetwork)
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("block/status/" + WalletKey + "/", CurrentNetwork);
            Notus.Core.Variable.BlockStatusCode tmpAirDrop = JsonSerializer.Deserialize<Notus.Core.Variable.BlockStatusCode>(tmpResult);
            return tmpAirDrop;
        }
        
        public static Notus.Core.Variable.EccKeyPair GenerateKeyPair()
        {
            return Notus.Core.Wallet.ID.GenerateKeyPair();
        }

        public static async Task<Notus.Core.Variable.BlockResponseStruct> GenerateToken(string PrivateKeyHex, Notus.Core.Variable.TokenInfoStruct Obj_TokenInfo, Notus.Core.Variable.SupplyStruct Obj_TokenSupply, Notus.Core.Variable.NetworkType currentNetwork)
        {
            //string PublicKeyHex = Notus.Core.Wallet.ID.GetAddressWithPublicKey(PrivateKeyHex);
            string PublicKeyHex = Notus.Core.Wallet.ID.Generate(PrivateKeyHex);
            string TokenRawDataForSignText = Notus.Core.MergeRawData.TokenGenerate(PublicKeyHex, Obj_TokenInfo, Obj_TokenSupply);
            string SignText = Notus.Core.Wallet.ID.Sign(TokenRawDataForSignText, PrivateKeyHex);

            Notus.Core.Variable.BlockResponseStruct TokenResult = await Notus.Core.Prepare.Token.Generate(PublicKeyHex, SignText, Obj_TokenInfo, Obj_TokenSupply, currentNetwork);
            return TokenResult;
        }

    }
}
