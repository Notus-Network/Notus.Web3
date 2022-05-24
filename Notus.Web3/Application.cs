// Copyright (C) 2020-2022 Notus Network
// 
// Notus Network is free software distributed under the MIT software license, 
// see the accompanying file LICENSE in the main directory of the
// project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Notus.Web3
{
    /// <summary>
    /// Represents Web3 dApps.
    /// </summary>
    public class Application
    {
        private string Val_PrivateKeyHex;
        /// <summary>
        /// Represents local user's private key to use Web3 dApps.
        /// </summary>
        public string PrivateKeyHex
        {
            get { return Val_PrivateKeyHex; }
            set { Val_PrivateKeyHex = value; }
        }
        /// <summary>
        /// Used to store wallet's local variables. Such as <see cref="Notus.Core.Variable.EccKeyPair"/>, Wallet Name, Wallet Image and Last Fetch Date.
        /// </summary>
        public class LocalWalletList
        {
            /// <value>
            /// The variable to store wallet properties. Ref: <see cref="Notus.Core.Variable.EccKeyPair"/>
            /// </value>
            public Notus.Core.Variable.EccKeyPair Wallet { get; set; }
            /// <value>
            /// User's Wallet Name.
            /// </value>
            public string WalletName { get; set; }
            /// <value>
            /// User's Wallet Image's URL or Base64.
            /// </value>
            public string WalletImage { get; set; }
            /// <value>
            /// The variable to store last fetch time.
            /// </value>
            public DateTime DaysAgo { get; set; }
        }

        /// <summary>
        /// Gets Currency List with given network via HTTP request. 
        /// </summary>
        /// <param name="currentNetwork">Current Network for Request.</param>
        /// <returns>Returns <see cref="Notus.Core.Variable.CurrencyList"/>.</returns>
        public static async Task<List<Notus.Core.Variable.CurrencyList>> GetCurrencyList(Notus.Core.Variable.NetworkType currentNetwork = Core.Variable.NetworkType.MainNet)
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("currency/list/", currentNetwork, Notus.Core.Variable.NetworkLayer.Layer1);
            return JsonSerializer.Deserialize<List<Notus.Core.Variable.CurrencyList>>(tmpResult);
        }
        /// <summary>
        /// Gets Balance with given network and wallet key via HTTP request. 
        /// </summary>
        /// <param name="WalletKey">Wallet key of the wallet whose balance will be shown.</param>
        /// <param name="currentNetwork">Current Network for Request.</param>
        /// <returns>Returns <see cref="Dictionary{TKey, TValue}"/>.</returns>
        public static async Task<Dictionary<string, string>> Balance(
            string WalletKey,
            Notus.Core.Variable.NetworkType currentNetwork = Core.Variable.NetworkType.MainNet
        )
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("balance/" + WalletKey + "/", currentNetwork, Notus.Core.Variable.NetworkLayer.Layer1);
            Notus.Core.Variable.WalletBalanceStruct tmpBalanceVal = JsonSerializer.Deserialize<Notus.Core.Variable.WalletBalanceStruct>(tmpResult);
            return tmpBalanceVal.Balance;
        }
        /// <summary>
        /// It performs the airdrop operation with the given wallet address and network type via HTTP request.
        /// </summary>
        /// <param name="WalletKey">The wallet key of the wallet to be airdropped.</param>
        /// <param name="currentNetwork">Current Network for Request.</param>
        /// <returns>Returns <see cref="Notus.Core.Variable.CryptoTransactionResult"/>.</returns>
        public static async Task<Notus.Core.Variable.CryptoTransactionResult> AirDrop(string WalletKey, Notus.Core.Variable.NetworkType currentNetwork = Core.Variable.NetworkType.MainNet)
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("airdrop/" + WalletKey + "/", currentNetwork, Notus.Core.Variable.NetworkLayer.Layer1);
            Notus.Core.Variable.CryptoTransactionResult tmpAirDrop = JsonSerializer.Deserialize<Notus.Core.Variable.CryptoTransactionResult>(tmpResult);
            return tmpAirDrop;
        }
        /// <summary>
        /// TO DO.
        /// </summary>
        public static async Task<Notus.Core.Variable.BlockStatusCode> GetStatus(string BlockUid, Notus.Core.Variable.NetworkType CurrentNetwork, Notus.Core.Variable.NetworkLayer CurrentLayer = Notus.Core.Variable.NetworkLayer.Layer1)
        {
            string tmpResult = await Notus.Core.Function.FindAvailableNode("block/status/" + BlockUid + "/", CurrentNetwork, CurrentLayer);
            Notus.Core.Variable.BlockStatusCode tmpAirDrop = JsonSerializer.Deserialize<Notus.Core.Variable.BlockStatusCode>(tmpResult);
            return tmpAirDrop;
        }
        /// <summary>
        /// TO DO.
        /// </summary>
        public static Notus.Core.Variable.BlockStatusCode StoreFileOnChain(string PrivateKeyHex, string FileAddress, bool LocalFile, Notus.Core.Variable.NetworkType CurrentNetwork=Core.Variable.NetworkType.MainNet)
        {
            int sleepTime = 2500;
            byte errorCountForSleepTime = 0;

            using MemoryStream ms = new MemoryStream();
            string fileName = "";
            if (LocalFile == false)
            {
                WebClient client = new WebClient();
                Stream imgstream = client.OpenRead(FileAddress);

                byte[] buffer = new byte[4096];
                int read;
                while ((read = imgstream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                fileName = System.IO.Path.GetFileName(new Uri(FileAddress).LocalPath);
            }
            else
            {
                fileName = System.IO.Path.GetFileName(FileAddress);
                using (FileStream file = new FileStream(FileAddress, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                }
            }
            byte[] fileArray = ms.ToArray();
            uint fileSize = (uint)ms.Length;

            Notus.Core.Variable.StorageOnChainStruct storageObj = new Notus.Core.Variable.StorageOnChainStruct()
            {
                Name = fileName,
                Size = fileSize,
                Hash = new Notus.Hash().CommonHash("sasha", ms.ToArray()),
                Encrypted = false,
                PublicKey = Notus.Core.Wallet.ID.Generate(PrivateKeyHex),
                Sign = "",
                Balance = new Notus.Core.Variable.BalanceAfterBlockStruct()
                {
                    Wallet = "",
                    Balance = new Dictionary<string, string>(),
                    Fee = "0",
                    RowNo = 0,
                    UID = ""
                }
            };
            storageObj.Sign = Notus.Core.Wallet.ID.Sign(Notus.Core.MergeRawData.StorageOnChain(storageObj), PrivateKeyHex);

            string responseData = Notus.Core.Function.FindAvailableNodeSync(
                "storage/file/new",
                new Dictionary<string, string>()
                {
                    {
                        "data",
                        JsonSerializer.Serialize(storageObj)
                    }
                },
                Notus.Core.Variable.NetworkType.MainNet,
                Notus.Core.Variable.NetworkLayer.Layer1
            );

            Console.WriteLine(responseData);
            Notus.Core.Variable.BlockResponse tmpStartObj = JsonSerializer.Deserialize<Notus.Core.Variable.BlockResponse>(responseData);

            if (tmpStartObj.UID.Length > 0 && tmpStartObj.Result == Notus.Core.Variable.BlockStatusCode.AddedToQueue)
            {
                Console.WriteLine("Pre-Wait");
                Thread.Sleep(4000);

                bool exitWhileLoop = false;
                while (exitWhileLoop == false)
                {
                    string controlResponse = Notus.Core.Function.FindAvailableNodeSync(
                        "block/" + tmpStartObj.UID,
                        Notus.Core.Variable.NetworkType.MainNet,
                        Notus.Core.Variable.NetworkLayer.Layer1
                    );
                    Console.WriteLine(controlResponse);
                    if (controlResponse.Length > 100)
                    {
                        // block oluşturulmuş demektir.
                        exitWhileLoop = true;
                    }
                    else
                    {
                        if (errorCountForSleepTime < 10)
                        {
                            errorCountForSleepTime++;
                        }
                        else
                        {
                            sleepTime = 10000;
                        }
                        Console.WriteLine("Sleep for wait : " + sleepTime.ToString());
                        Thread.Sleep(sleepTime);
                    }
                }
            }
            // Console.ReadLine();

            int chunkSize = Notus.Core.Variable.DefaultChunkSize;
            int chunkLength = (int)Math.Ceiling(Convert.ToDouble(fileArray.Length / chunkSize));
            int chunk = 0;
            for (int i = 0; i < chunkLength; i++)
            {
                sleepTime = 2500;
                errorCountForSleepTime = 0;

                byte[] tmpArray = new byte[chunkSize];
                Array.Copy(fileArray, chunk, tmpArray, 0, tmpArray.Length);
                string tmpBaseStr = System.Convert.ToBase64String(tmpArray);
                string tmpDataStr = System.Uri.EscapeDataString(tmpBaseStr);
                string sendDataStr = JsonSerializer.Serialize(
                    new Notus.Core.Variable.FileChunkStruct()
                    {
                        Count = i,
                        Data = tmpDataStr,
                        UID = tmpStartObj.UID
                    }
                );
                bool innerLoop = false;
                while (innerLoop == false)
                {
                    string responseChunk = Notus.Core.Function.FindAvailableNodeSync(
                        "storage/file/update",
                        new Dictionary<string, string>() {
                            {
                                "data", sendDataStr
                            }
                        },
                        Notus.Core.Variable.NetworkType.MainNet,
                        Notus.Core.Variable.NetworkLayer.Layer3
                    );
                    Notus.Core.Variable.BlockResponse tmpChunkObj = JsonSerializer.Deserialize<Notus.Core.Variable.BlockResponse>(responseChunk);
                    // Console.WriteLine(responseChunk);
                    // Console.WriteLine("-------------------------");
                    // Console.ReadLine();
                    if (tmpChunkObj.Result == Notus.Core.Variable.BlockStatusCode.AddedToQueue)
                    {
                        innerLoop = true;
                    }
                    else
                    {
                        if (errorCountForSleepTime < 10)
                        {
                            errorCountForSleepTime++;
                        }
                        else
                        {
                            sleepTime = 10000;
                        }
                        Console.WriteLine("Sleep for wait : " + sleepTime.ToString());
                        Thread.Sleep(sleepTime);
                    }

                }
                chunk += chunkSize;
            }

            // işlemi beklemeye alıyor ve orada kalıyor.
            // dosya yüklenince tüm dosyaları birleştir ve blok içeriğine al

            bool loop = true;
            sleepTime = 2500;
            errorCountForSleepTime = 0;
            while (loop)
            {
                string response = Notus.Core.Function.FindAvailableNodeSync(
                    $"storage/file/status/{tmpStartObj.UID}",
                    Notus.Core.Variable.NetworkType.MainNet,
                    Notus.Core.Variable.NetworkLayer.Layer3
                );
                Notus.Core.Variable.BlockStatusCode ResStruct = JsonSerializer.Deserialize<Notus.Core.Variable.BlockStatusCode>(response);
                if (ResStruct == Notus.Core.Variable.BlockStatusCode.Completed)
                {
                    loop = false;
                }
                else
                {
                    if (errorCountForSleepTime < 10)
                    {
                        errorCountForSleepTime++;
                    }
                    else
                    {
                        sleepTime = 10000;
                    }
                    Console.WriteLine("Sleep for wait : " + sleepTime.ToString());
                    Thread.Sleep(sleepTime);
                }
            }
            return Notus.Core.Variable.BlockStatusCode.Completed;
        }

        /// <summary>
        /// Generates new wallet and returns wallet details.
        /// </summary>
        /// <returns>Returns <see cref="Notus.Core.Variable.EccKeyPair"/>.</returns>
        public static Notus.Core.Variable.EccKeyPair GenerateKeyPair()
        {
            return Notus.Core.Wallet.ID.GenerateKeyPair();
        }
        /// <summary>
        /// It performs token generate operation with the given wallet address and network type via HTTP request.
        /// </summary>
        /// <param name="PrivateKeyHex">Private key of the main wallet.</param>
        /// <param name="Obj_TokenInfo">The local variable to store name, tag and logo variables.</param>
        /// <param name="Obj_TokenSupply">The local variable to store supply, decimal and resupplyable variables.</param>
        /// <param name="currentNetwork">Current Network for Request.</param>
        /// <returns>Returns <see cref="Notus.Core.Variable.BlockResponseStruct"/>.</returns>
        public static async Task<Notus.Core.Variable.BlockResponseStruct> GenerateToken(string PrivateKeyHex, Notus.Core.Variable.TokenInfoStruct Obj_TokenInfo, Notus.Core.Variable.SupplyStruct Obj_TokenSupply, Notus.Core.Variable.NetworkType currentNetwork)
        {
            string PublicKeyHex = Notus.Core.Wallet.ID.Generate(PrivateKeyHex);
            string TokenRawDataForSignText = Notus.Core.MergeRawData.TokenGenerate(PublicKeyHex, Obj_TokenInfo, Obj_TokenSupply);
            string SignText = Notus.Core.Wallet.ID.Sign(TokenRawDataForSignText, PrivateKeyHex);

            Notus.Core.Variable.BlockResponseStruct TokenResult = await Notus.Core.Prepare.Token.Generate(PublicKeyHex, SignText, Obj_TokenInfo, Obj_TokenSupply, currentNetwork);
            return TokenResult;
        }

    }
}
