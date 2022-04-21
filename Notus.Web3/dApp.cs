﻿using System;

namespace Notus.Web3
{
    public class dApp
    {
        public static bool GenerateToken(string PrivateKeyHex, Notus.Variable.Token.TokenInfoStruct Obj_TokenInfo, Notus.Variable.Token.SupplyStruct Obj_TokenSupply)
        {
            //string PublicKeyHex = Notus.Wallet.ID.GetAddressWithPublicKey(PrivateKeyHex);
            string PublicKeyHex = Notus.Wallet.ID.Generate(PrivateKeyHex);
            string TokenRawDataForSignText = Notus.Token.Generate.RawDataForSign(PublicKeyHex, Obj_TokenInfo, Obj_TokenSupply);
            string SignText = Notus.Wallet.ID.Sign(TokenRawDataForSignText, PrivateKeyHex);
            int TokenResultInt = Notus.Token.Generate.Execute(PublicKeyHex, SignText, Obj_TokenInfo, Obj_TokenSupply);
            if (TokenResultInt == 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine("TokenResultInt : " + TokenResultInt);
                return false;
            }
        }
        public static Notus.Variable.Struct.EccKeyPair GenerateKeyPair()
        {
            Notus.Variable.Struct.EccKeyPair GeneratedKeyPair = Notus.Wallet.ID.GenerateKeyPair();
            return GeneratedKeyPair;
        }
    }
}