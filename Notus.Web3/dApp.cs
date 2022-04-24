using System;

namespace Notus.Web3
{
    public class dApp
    {
        private string Val_PrivateKeyHex;
        public string PrivateKeyHex
        {
            get { return Val_PrivateKeyHex; }
            set { Val_PrivateKeyHex = value; }
        }

        public static Notus.Variable.Struct.BlockResponseStruct GenerateToken(string PrivateKeyHex, Notus.Variable.Token.TokenInfoStruct Obj_TokenInfo, Notus.Variable.Token.SupplyStruct Obj_TokenSupply)
        {
            //string PublicKeyHex = Notus.Wallet.ID.GetAddressWithPublicKey(PrivateKeyHex);
            string PublicKeyHex = Notus.Wallet.ID.Generate(PrivateKeyHex);
            string TokenRawDataForSignText = Notus.Token.Generate.RawDataForSign(PublicKeyHex, Obj_TokenInfo, Obj_TokenSupply);
            string SignText = Notus.Wallet.ID.Sign(TokenRawDataForSignText, PrivateKeyHex);
            Notus.Variable.Struct.BlockResponseStruct TokenResult = Notus.Token.Generate.Execute(PublicKeyHex, SignText, Obj_TokenInfo, Obj_TokenSupply);
            return TokenResult;
        }
        public static string Balance(string WalletKey)
        {
            string Result = Notus.Web3.Common.FindAvailableNode("balance/" + WalletKey + "/");
            Console.WriteLine(Result);
            return Result;
        }
        public static string AirDrop(string WalletKey)
        {
            string Result=Notus.Web3.Common.FindAvailableNode("airdrop/" + WalletKey + "/");
            Console.WriteLine(Result);
            return Result;
            //WalletKey
        }
        public static Notus.Variable.Struct.EccKeyPair GenerateKeyPair()
        {
            Notus.Variable.Struct.EccKeyPair GeneratedKeyPair = Notus.Wallet.ID.GenerateKeyPair();
            return GeneratedKeyPair;
        }
    }
}
