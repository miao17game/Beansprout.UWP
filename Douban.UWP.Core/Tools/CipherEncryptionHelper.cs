using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Douban.UWP.Core.Tools {
    public static class CipherEncryptionHelper {
        
        public static byte[] CollForKeyAndIv = new byte[16] { 234, 123, 231, 44, 25, 16, 7, 68, 11, 206, 137, 44, 95, 67, 173, 108 };

        #region AES CBC PKCS7

        public static IBuffer CipherEncryption(
            string strMsg,
            string strAlgName,
            out BinaryStringEncoding encoding,
            out IBuffer iv,
            out CryptographicKey key,
            uint keyLength = 128) {
            iv = null;  // Initialize the initialization vector because some type encryptions do not need it.
            encoding = BinaryStringEncoding.Utf8;

            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(strMsg, encoding);

            // Open a symmetric algorithm provider for the specified algorithm. 
            var objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(strAlgName);

            // Determine whether the message length is a multiple of the block length.
            // This is not necessary for PKCS #7 algorithms which automatically pad the
            // message to an appropriate length.
            if (!strAlgName.Contains("PKCS7"))
                if ((buffMsg.Length % objAlg.BlockLength) != 0)
                    throw new Exception("Message buffer length must be multiple of block length.");

            // Create a symmetric key.
            // IBuffer keyMaterial = CryptographicBuffer.GenerateRandom(keyLength);     // drop it.
            IBuffer keyMaterial = CryptographicBuffer.CreateFromByteArray(CollForKeyAndIv);
            key = objAlg.CreateSymmetricKey(keyMaterial);

            // CBC algorithms require an initialization vector. Here, a random number is used for the vector.
            if (strAlgName.Contains("CBC"))
                //iv = CryptographicBuffer.GenerateRandom(objAlg.BlockLength);   // drop it.
                iv = CryptographicBuffer.CreateFromByteArray(CollForKeyAndIv);

            // Encrypt the data and return.
            IBuffer buffEncrypt = CryptographicEngine.Encrypt(key, buffMsg, iv);
            return buffEncrypt;
        }


        public static string CipherDecryption(
            string strAlgName,
            IBuffer buffEncrypt,
            IBuffer iv,
            BinaryStringEncoding encoding,
            CryptographicKey key) {
            // Declare a buffer to contain the decrypted data.
            IBuffer buffDecrypted;

            // Open an symmetric algorithm provider for the specified algorithm. 
            SymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(strAlgName);

            // The input key must be securely shared between the sender of the encrypted message
            // and the recipient. The initialization vector must also be shared but does not
            // need to be shared in a secure manner. If the sender encodes a message string 
            // to a buffer, the binary encoding method must also be shared with the recipient.
            buffDecrypted = CryptographicEngine.Decrypt(key, buffEncrypt, iv);

            // Convert the decrypted buffer to a string (for display). If the sender created the
            // original message buffer from a string, the sender must tell the recipient what 
            // BinaryStringEncoding value was used. Here, BinaryStringEncoding.Utf8 is used to
            // convert the message to a buffer before encryption and to convert the decrypted
            // buffer back to the original plaintext.
            return CryptographicBuffer.ConvertBinaryToString(encoding, buffDecrypted);
        }

        #endregion

    }
}
