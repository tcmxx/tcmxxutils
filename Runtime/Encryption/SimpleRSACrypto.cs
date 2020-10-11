using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace TCUtils
{
    //from https://stackoverflow.com/questions/17128038/c-sharp-rsa-encryption-decryption-with-transmission
    public static class SimpleRSACrypto
    {

        public enum EncryptionBits
        {
            Bits1024,
            Bits2048
        }
        /// <summary>
        /// generate public and private keys
        /// </summary>
        /// <param name="bits">1024 or 2048</param>
        /// <returns></returns>
        public static (string publicKey, string privateKey) GenerateKeys(EncryptionBits bits)
        {
            //lets take a new CSP with a new bits bit rsa key pair
            var csp = new RSACryptoServiceProvider(bits == EncryptionBits.Bits1024 ? 1024 : 2048);

            //how to get the private key
            var privKey = csp.ExportParameters(true);

            //and the public key ...
            var pubKey = csp.ExportParameters(false);

            //we need some buffer
            var sw = new System.IO.StringWriter();
            //we need a serializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //serialize the key into the stream
            xs.Serialize(sw, pubKey);
            //get the string from the stream
            var pubKeyString = sw.ToString();

            //serialize the key into the stream
            var sw2 = new System.IO.StringWriter();
            xs.Serialize(sw2, privKey);
            //get the string from the stream
            var privKeyString = sw2.ToString();

            return (pubKeyString, privKeyString);

        }

        public static string Encrypt(string valueToEncrypt, string keyString)
        {
            var sr = new System.IO.StringReader(keyString);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            var key = (RSAParameters)xs.Deserialize(sr);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(key);

            //for encryption, always handle bytes...
            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(valueToEncrypt);
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, true);    //OAEP padding is better
            var cypherText = Convert.ToBase64String(bytesCypherText);

            return cypherText;
        }

        public static string Decrypt(string valueToDecrypt, string keyString)
        {
            var sr = new System.IO.StringReader(keyString);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            var key = (RSAParameters)xs.Deserialize(sr);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(key);

            //for encryption, always handle bytes...
            var bytesCypherText = Convert.FromBase64String(valueToDecrypt);
            var bytesPlainTextData = csp.Decrypt(bytesCypherText, true);    //OAEP padding is better
            var plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

            return plainTextData;
        }
    }
}