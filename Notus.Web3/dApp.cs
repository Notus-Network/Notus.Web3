using System;

namespace Notus.Web3
{
    public class dApp
    {
        public static Notus.Variable.Struct.BlockResponseStruct GenerateToken(string PrivateKeyHex, Notus.Variable.Token.TokenInfoStruct Obj_TokenInfo, Notus.Variable.Token.SupplyStruct Obj_TokenSupply)
        {
            //string PublicKeyHex = Notus.Wallet.ID.GetAddressWithPublicKey(PrivateKeyHex);
            string PublicKeyHex = Notus.Wallet.ID.Generate(PrivateKeyHex);
            string TokenRawDataForSignText = Notus.Token.Generate.RawDataForSign(PublicKeyHex, Obj_TokenInfo, Obj_TokenSupply);
            string SignText = Notus.Wallet.ID.Sign(TokenRawDataForSignText, PrivateKeyHex);
            Notus.Variable.Struct.BlockResponseStruct TokenResult = Notus.Token.Generate.Execute(PublicKeyHex, SignText, Obj_TokenInfo, Obj_TokenSupply);
            return TokenResult;
        }
        public static Notus.Variable.Struct.EccKeyPair GenerateKeyPair()
        {
            Notus.Variable.Struct.EccKeyPair GeneratedKeyPair = Notus.Wallet.ID.GenerateKeyPair();
            return GeneratedKeyPair;
        }
    }
}
