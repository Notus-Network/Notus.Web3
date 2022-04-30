using System;
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

        public static async Task<Notus.Core.Variable.BlockResponseStruct> GenerateToken(string PrivateKeyHex, Notus.Core.Variable.TokenInfoStruct Obj_TokenInfo, Notus.Core.Variable.SupplyStruct Obj_TokenSupply)
        {
            //string PublicKeyHex = Notus.Core.Wallet.ID.GetAddressWithPublicKey(PrivateKeyHex);
            string PublicKeyHex = Notus.Core.Wallet.ID.Generate(PrivateKeyHex);
            string TokenRawDataForSignText = Notus.Core.MergeRawData.TokenGenerate(PublicKeyHex, Obj_TokenInfo, Obj_TokenSupply);
            string SignText = Notus.Core.Wallet.ID.Sign(TokenRawDataForSignText, PrivateKeyHex);

            
            Notus.Core.Variable.BlockResponseStruct TokenResult = await Notus.Core.Prepare.Token.Generate(PublicKeyHex, SignText, Obj_TokenInfo, Obj_TokenSupply);
            return TokenResult;
        }
        public static async Task<Notus.Core.Variable.WalletBalanceResponseStruct> Balance(string WalletKey)
        {   
            string tmpResult = await Notus.Core.Function.FindAvailableNode("balance/" + WalletKey + "/");
            Notus.Core.Variable.WalletBalanceResponseStruct tmpBalanceVal = JsonSerializer.Deserialize<Notus.Core.Variable.WalletBalanceResponseStruct>(tmpResult);
            return tmpBalanceVal;
        }
        public static async Task<Notus.Core.Variable.CryptoTransactionResult> AirDrop(string WalletKey)
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("airdrop/" + WalletKey + "/");
            Notus.Core.Variable.CryptoTransactionResult tmpAirDrop = JsonSerializer.Deserialize<Notus.Core.Variable.CryptoTransactionResult>(tmpResult);
            return tmpAirDrop;
        }
        public static Notus.Core.Variable.EccKeyPair GenerateKeyPair()
        {
            Notus.Core.Variable.EccKeyPair GeneratedKeyPair = Notus.Core.Wallet.ID.GenerateKeyPair();
            return GeneratedKeyPair;
        }
    }
}
