using System;
using System.Text.Json;
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

        public static Notus.Core.Variable.BlockResponseStruct GenerateToken(string PrivateKeyHex, Notus.Core.Variable.TokenInfoStruct Obj_TokenInfo, Notus.Core.Variable.SupplyStruct Obj_TokenSupply)
        {
            //string PublicKeyHex = Notus.Core.Wallet.ID.GetAddressWithPublicKey(PrivateKeyHex);
            string PublicKeyHex = Notus.Core.Wallet.ID.Generate(PrivateKeyHex);
            string TokenRawDataForSignText = Notus.Core.SignRawData.TokenGenerate(PublicKeyHex, Obj_TokenInfo, Obj_TokenSupply);
            string SignText = Notus.Core.Wallet.ID.Sign(TokenRawDataForSignText, PrivateKeyHex);

            
            Notus.Core.Variable.BlockResponseStruct TokenResult = Notus.Core.Prepare.Token.Generate(PublicKeyHex, SignText, Obj_TokenInfo, Obj_TokenSupply);
            return TokenResult;
        }
        public static Notus.Core.Variable.WalletBalanceResponseStruct Balance(string WalletKey)
        {   
            string tmpResult = Notus.Core.Function.FindAvailableNode("balance/" + WalletKey + "/");
            Notus.Core.Variable.WalletBalanceResponseStruct tmpBalanceVal = JsonSerializer.Deserialize<Notus.Core.Variable.WalletBalanceResponseStruct>(tmpResult);
            return tmpBalanceVal;
        }
        public static Notus.Core.Variable.CryptoTransactionResult AirDrop(string WalletKey)
        {
            string tmpResult = Notus.Core.Function.FindAvailableNode("airdrop/" + WalletKey + "/");
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
