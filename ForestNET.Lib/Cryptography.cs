using System.Security.Cryptography;

namespace ForestNET.Lib
{
    /// <summary>
    /// Cryptography class to de-/encrypt data with a secret key instance.
    /// </summary>
    public class Cryptography
    {

        /* Constants */

        public const int KEY128BIT = 0;
        public const int KEY256BIT = 1;

        /* Fields */

        private readonly byte[] a_ivLocal;
        private readonly byte[] o_secretKeyLocal;

        /* Properties */

        /* Methods */

        /// <summary>
        /// Constructor to create cryptography instance for encoding and decoding byte arrays with AES/GCM, one secret key during cryptography instance
        /// for higher security static encoding and decoding methods are available which are always generating a new secret key, thus need higher performance.
        /// </summary>
        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
        /// <param name="p_i_keyLengthOption">Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']</param>
        /// <exception cref="ArgumentException">invalid key length option [Cryptography.KEY128BIT | Cryptography.KEY256BIT]</exception>
        public Cryptography(string p_s_commonSecretPassphrase, int p_i_keyLengthOption)
        {
            /* check length of common secret passphrase */
            if (p_s_commonSecretPassphrase.Length < 36)
            {
                throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + p_s_commonSecretPassphrase.Length + "' characters");
            }

            /* check key length option */
            if ((p_i_keyLengthOption < 0) || (p_i_keyLengthOption > 1))
            {
                throw new ArgumentException("Unknown key length option[" + p_i_keyLengthOption + "]. Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']");
            }

            /* get 12 bytes for salt from common secret passphrase */
            byte[] by_utf8 = System.Text.Encoding.UTF8.GetBytes(p_s_commonSecretPassphrase);
            string s_utf8 = System.Text.Encoding.UTF8.GetString(by_utf8);
            this.a_ivLocal = System.Text.Encoding.UTF8.GetBytes(s_utf8.Substring(0, 12));

            /* generate secret key for cryptography instance */
            this.o_secretKeyLocal = Cryptography.GenerateSecretKey(this.a_ivLocal, p_s_commonSecretPassphrase, p_i_keyLengthOption);
        }

        /// <summary>
        /// Encrypt data with cryptography instance configuration.
        /// </summary>
        /// <param name="p_a_data">data in byte array which will be encrypted</param>
        /// <returns>encrypted byte array</returns>
        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
        public byte[] Encrypt(byte[] p_a_data)
        {
            return this.Encrypt(p_a_data, true);
        }

        /// <summary>
        /// Encrypt data with cryptography instance configuration.
        /// </summary>
        /// <param name="p_a_data">data in byte array which will be encrypted</param>
        /// <param name="p_b_skipIV">true - do not add iv part to encrypted data, false - add iv part as first part to encrypted data</param>
        /// <returns>encrypted byte array</returns>
        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
        public byte[] Encrypt(byte[] p_a_data, bool p_b_skipIV)
        {
            /* measure time */
            System.Diagnostics.Stopwatch o_stopwatch = new();
            o_stopwatch.Start();

            /* check input data parameter */
            if ((p_a_data == null) || (p_a_data.Length == 0))
            {
                throw new ArgumentException("Byte array for encryption is empty");
            }

            /* set salt, encrypted data and authentication tag in one byte data */
            System.IO.MemoryStream o_encryptedData = new();

            /* encrypt data with cryptographic engine */
            using (AesGcm o_aesGcm = new(this.o_secretKeyLocal, 16))
            {
                byte[] a_encryptedBytes = new byte[p_a_data.Length];
                byte[] a_tag = new byte[16];

                /* doing encryption */
                o_aesGcm.Encrypt(this.a_ivLocal, p_a_data, a_encryptedBytes, a_tag);

                /* write return value with order IV + encrypted data + TAG */
                using System.IO.BinaryWriter o_binaryWriter = new(o_encryptedData);

                if (!p_b_skipIV)
                {
                    o_binaryWriter.Write(this.a_ivLocal);
                }

                o_binaryWriter.Write(a_encryptedBytes);
                o_binaryWriter.Write(a_tag);
            }

            /* log amount if ms needed for encryption */
            o_stopwatch.Stop();
            ForestNET.Lib.Global.ILogFiner("encrypt: " + (o_stopwatch.ElapsedMilliseconds) + " ms");

            /* return encrypted data */
            return o_encryptedData.ToArray();
        }

        /// <summary>
        /// Decrypt data with cryptography instance configuration.
        /// </summary>
        /// <param name="p_a_data">data in byte array which will be decrypted</param>
        /// <returns>decrypted byte array</returns>
        /// <exception cref="ArgumentException">byte array for decryption is empty</exception>
        public byte[] Decrypt(byte[] p_a_data)
        {
            return this.Decrypt(p_a_data, true);
        }

        /// <summary>
        /// Decrypt data with cryptography instance configuration.
        /// </summary>
        /// <param name="p_a_data">data in byte array which will be decrypted</param>
        /// <param name="p_b_skipIV">true - do not read first 12 bytes as iv part, false - read first 12 bytes as iv part</param>
        /// <returns>decrypted byte array</returns>
        /// <exception cref="ArgumentException">byte array for decryption is empty</exception>
        public byte[] Decrypt(byte[] p_a_data, bool p_b_skipIV)
        {
            /* measure time */
            System.Diagnostics.Stopwatch o_stopwatch = new();
            o_stopwatch.Start();

            /* check input data parameter */
            if ((p_a_data == null) || (p_a_data.Length == 0))
            {
                throw new ArgumentException("Byte array for decryption is empty");
            }

            /* byte arrays */
            byte[] a_iv;
            byte[] a_data;
            byte[] a_tag;
            byte[] a_return;

            if (p_b_skipIV) /* do not read first 12 bytes as iv part */
            {
                /* get iv part from class instance */
                a_iv = this.a_ivLocal;

                /* create byte array for encrypted data bytes */
                a_data = new byte[p_a_data.Length - 16];
                /* read encrypted bytes */
                for (int i = 0; i < p_a_data.Length - 16; i++)
                {
                    a_data[i] = p_a_data[i];
                }

                /* create byte array for authentication tag bytes */
                a_tag = new byte[16];
                /* read authentication tag bytes */
                for (int i = p_a_data.Length - 16; i < p_a_data.Length; i++)
                {
                    a_tag[i - (p_a_data.Length - 16)] = p_a_data[i];
                }

                /* prepare return value */
                a_return = new byte[p_a_data.Length - 16];
            }
            else /* read first 12 bytes as iv part */
            {
                /* create byte array for iv bytes */
                a_iv = new byte[12];
                /* read encrypted bytes */
                for (int i = 0; i < 12; i++)
                {
                    a_iv[i] = p_a_data[i];
                }

                /* create byte array for encrypted data bytes */
                a_data = new byte[p_a_data.Length - 12 - 16];
                /* read encrypted bytes */
                for (int i = 12; i < p_a_data.Length - 16; i++)
                {
                    a_data[i - 12] = p_a_data[i];
                }

                /* create byte array for authentication tag bytes */
                a_tag = new byte[16];
                /* read authentication tag bytes */
                for (int i = p_a_data.Length - 16; i < p_a_data.Length; i++)
                {
                    a_tag[i - (p_a_data.Length - 16)] = p_a_data[i];
                }

                /* prepare return value */
                a_return = new byte[p_a_data.Length - 12 - 16];
            }

            /* decrypt input data with cryptographic engine */
            using (AesGcm o_aesGcm = new(this.o_secretKeyLocal, 16))
            {
                o_aesGcm.Decrypt(a_iv, a_data, a_tag, a_return);
            }

            /* log amount if ms needed for decryption */
            o_stopwatch.Stop();
            ForestNET.Lib.Global.ILogFiner("decrypt: " + (o_stopwatch.ElapsedMilliseconds) + " ms");

            /* return decrypted data */
            return a_return;
        }

        /// <summary>
        /// Encrypt data with new salt and secret key instance with key length 256 bit.
        /// </summary>
        /// <param name="p_a_data">data in byte array which will be encrypted</param>
        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
        /// <returns>encrypted byte array</returns>
        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
        public static byte[] Encrypt_AES_GCM(byte[] p_a_data, string p_s_commonSecretPassphrase)
        {
            return Cryptography.Encrypt_AES_GCM(p_a_data, p_s_commonSecretPassphrase, Cryptography.KEY256BIT);
        }

        /// <summary>
        /// Encrypt data with new salt and secret key instance.
        /// </summary>
        /// <param name="p_a_data">data in byte array which will be encrypted</param>
        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
        /// <param name="p_i_keyLengthOption">Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']</param>
        /// <returns>encrypted byte array</returns>
        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
        public static byte[] Encrypt_AES_GCM(byte[] p_a_data, string p_s_commonSecretPassphrase, int p_i_keyLengthOption)
        {
            /* measure time */
            System.Diagnostics.Stopwatch o_stopwatch = new();
            o_stopwatch.Start();

            /* check input data parameter */
            if ((p_a_data == null) || (p_a_data.Length == 0))
            {
                throw new ArgumentException("Byte array for decryption is empty");
            }

            /* check length of common secret passphrase */
            if (p_s_commonSecretPassphrase.Length < 36)
            {
                throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + p_s_commonSecretPassphrase.Length + "' characters");
            }

            /* check key length option */
            if ((p_i_keyLengthOption < 0) || (p_i_keyLengthOption > 1))
            {
                throw new ArgumentException("Unknown key length option[" + p_i_keyLengthOption + "]. Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']");
            }

            /* define salt byte array with 12 bytes */
            byte[] a_iv = new byte[12];
            /* get our secure random number generator instance and fill salt byte array randomly */
            using (System.Security.Cryptography.RandomNumberGenerator o_randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                o_randomNumberGenerator.GetBytes(a_iv);
            }

            /* generate secret key for cryptography instance */
            byte[] o_secretKey = Cryptography.GenerateSecretKey(a_iv, p_s_commonSecretPassphrase, p_i_keyLengthOption);

            /* set salt, encrypted data and authentication tag in one byte data */
            System.IO.MemoryStream o_encryptedData = new();

            /* encrypt data with cryptographic engine */
            using (AesGcm o_aesGcm = new(o_secretKey, 16))
            {
                byte[] a_encryptedBytes = new byte[p_a_data.Length];
                byte[] a_tag = new byte[16];

                /* doing encryption */
                o_aesGcm.Encrypt(a_iv, p_a_data, a_encryptedBytes, a_tag);

                /* write return value with order IV + encrypted data + TAG */
                using System.IO.BinaryWriter o_binaryWriter = new(o_encryptedData);
                o_binaryWriter.Write(a_iv);
                o_binaryWriter.Write(a_encryptedBytes);
                o_binaryWriter.Write(a_tag);
            }

            /* log amount if ms needed for encryption */
            o_stopwatch.Stop();
            ForestNET.Lib.Global.ILogFiner("encrypt: " + (o_stopwatch.ElapsedMilliseconds) + " ms");

            /* return encrypted data */
            return o_encryptedData.ToArray();
        }

        /// <summary>
        /// Decrypt data with new secret key instance by reading salt at the beginning of encrypted data byte array, with key length 256 bit.
        /// </summary>
        /// <param name="p_a_data">data in byte array which will be decrypted</param>
        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
        /// <returns>decrypted byte array</returns>
        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
        public static byte[] Decrypt_AES_GCM(byte[] p_a_data, string p_s_commonSecretPassphrase)
        {
            return Cryptography.Decrypt_AES_GCM(p_a_data, p_s_commonSecretPassphrase, Cryptography.KEY256BIT);
        }

        /// <summary>
        /// Decrypt data with new secret key instance by reading salt at the beginning of encrypted data byte array.
        /// </summary>
        /// <param name="p_a_data">data in byte array which will be decrypted</param>
        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
        /// <param name="p_i_keyLengthOption">Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']</param>
        /// <returns>decrypted byte array</returns>
        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
        public static byte[] Decrypt_AES_GCM(byte[] p_a_data, string p_s_commonSecretPassphrase, int p_i_keyLengthOption)
        {
            /* measure time */
            System.Diagnostics.Stopwatch o_stopwatch = new();
            o_stopwatch.Start();

            /* check input data parameter */
            if ((p_a_data == null) || (p_a_data.Length == 0))
            {
                throw new ArgumentException("Byte array for decryption is empty");
            }

            /* check length of common secret passphrase */
            if (p_s_commonSecretPassphrase.Length < 36)
            {
                throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + p_s_commonSecretPassphrase.Length + "' characters");
            }

            /* check key length option */
            if ((p_i_keyLengthOption < 0) || (p_i_keyLengthOption > 1))
            {
                throw new ArgumentException("Unknown key length option[" + p_i_keyLengthOption + "]. Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']");
            }

            /* create byte array for salt bytes */
            byte[] a_iv = new byte[12];
            /* read salt bytes */
            for (int i = 0; i < 12; i++)
            {
                a_iv[i] = p_a_data[i];
            }

            /* create byte array for encrypted data bytes */
            byte[] a_data = new byte[p_a_data.Length - 12 - 16];
            /* read encrypted bytes */
            for (int i = 12; i < p_a_data.Length - 16; i++)
            {
                a_data[i - 12] = p_a_data[i];
            }

            /* create byte array for authentication tag bytes */
            byte[] a_tag = new byte[16];
            /* read authentication tag bytes */
            for (int i = p_a_data.Length - 16; i < p_a_data.Length; i++)
            {
                a_tag[i - (p_a_data.Length - 16)] = p_a_data[i];
            }

            /* generate secret key for cryptography instance */
            byte[] o_secretKey = Cryptography.GenerateSecretKey(a_iv, p_s_commonSecretPassphrase, p_i_keyLengthOption);

            /* prepare return value */
            byte[] a_return = new byte[p_a_data.Length - 12 - 16];

            /* decrypt input data with cryptographic engine */
            using (AesGcm o_aesGcm = new(o_secretKey, 16))
            {
                o_aesGcm.Decrypt(a_iv, a_data, a_tag, a_return);
            }

            /* log amount if ms needed for decryption */
            o_stopwatch.Stop();
            ForestNET.Lib.Global.ILogFiner("decrypt: " + (o_stopwatch.ElapsedMilliseconds) + " ms");

            /* return decrypted data */
            return a_return;
        }

        /// <summary>
        /// generate secret key material which can be used with a cipher to de-/encrypt data.
        /// </summary>
        /// <param name="p_a_iv">salt byte array, default 16 bytes</param>
        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
        /// <param name="p_i_keyLengthOption">Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']</param>
        /// <returns>key material as byte array</returns>
        /// <exception cref="ArgumentException">invalid key length option [Cryptography.KEY128BIT | Cryptography.KEY256BIT]</exception>
        private static byte[] GenerateSecretKey(byte[] p_a_iv, string p_s_commonSecretPassphrase, int p_i_keyLengthOption)
        {
            /* retrieve iteration count from salt byte array, max. 16 bits, otherwise key generation takes to long  */
            byte[] a_iterationCount = new byte[] { 0x00, 0x00, 0x00, 0x00 };

            a_iterationCount[2] |= (byte)(p_a_iv[0] & 0x80); /* 8th bit of 1st salt byte */
            a_iterationCount[2] |= (byte)(p_a_iv[1] & 0x40); /* 7th bit of 2nd salt byte */
            a_iterationCount[2] |= (byte)(p_a_iv[2] & 0x20); /* 6th bit of 3rd salt byte */
            a_iterationCount[2] |= (byte)(p_a_iv[3] & 0x10); /* 5th bit of 4th salt byte */
            a_iterationCount[2] |= (byte)(p_a_iv[4] & 0x08); /* 4th bit of 5th salt byte */
            a_iterationCount[2] |= (byte)(p_a_iv[5] & 0x04); /* 3rd bit of 6th salt byte */
            a_iterationCount[2] |= (byte)(p_a_iv[6] & 0x02); /* 2nd bit of 7th salt byte */
            a_iterationCount[2] |= (byte)(p_a_iv[7] & 0x01); /* 1st bit of 8th salt byte */

            a_iterationCount[3] |= (byte)(p_a_iv[8] & 0x05); /* 1st + 3rd bit of 9th salt byte */
            a_iterationCount[3] |= (byte)(p_a_iv[9] & 0x0A); /* 2nd + 4th bit of 10th salt byte */
            a_iterationCount[3] |= (byte)(p_a_iv[10] & 0x50); /* 5th + 7th bit of 11th salt byte */
            a_iterationCount[3] |= (byte)(p_a_iv[11] & 0xA0); /* 6th + 8th bit of 12th salt byte */

            /* convert iteration count bytes to integer value */
            int i_iterationCount = ForestNET.Lib.Helper.ByteArrayToInt(a_iterationCount);

            /* ensure that common secret passphrase is handled as utf8-string */
            byte[] by_utf8 = System.Text.Encoding.UTF8.GetBytes(p_s_commonSecretPassphrase);
            string s_utf8 = System.Text.Encoding.UTF8.GetString(by_utf8);

            /* check key length option and set key factory algorithm */
            if (p_i_keyLengthOption == Cryptography.KEY128BIT)
            {
                /* generate key material with RFC-2898 derived bytes and SHA-1, returning 16 bytes */
                using var o_pbkdf2Sha1 = new Rfc2898DeriveBytes(s_utf8, p_a_iv, i_iterationCount, HashAlgorithmName.SHA1);
                return o_pbkdf2Sha1.GetBytes(16);
            }
            else if (p_i_keyLengthOption == Cryptography.KEY256BIT)
            {
                /* generate key material with RFC-2898 derived bytes and SHA-512, returning 32 bytes */
                using var o_pbkdf2Sha256 = new Rfc2898DeriveBytes(s_utf8, p_a_iv, i_iterationCount, HashAlgorithmName.SHA512);
                return o_pbkdf2Sha256.GetBytes(32);
            }
            else
            {
                throw new ArgumentException("Unknown key length option[" + p_i_keyLengthOption + "]. Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']");
            }
        }
    }
}

/* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */
/* +++++++++++++++++++++++++++++ The following code can be used for UWP applications +++++++++++++++++++++++++++++ */
/* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

//using System;
//using Windows.Security.Cryptography;
//using Windows.Security.Cryptography.Core;
//using Windows.Storage.Streams;
//using System.Runtime.InteropServices.WindowsRuntime;

//namespace ForestNETLib.Core
//{
//    /// <summary>
//    /// Cryptography class to de-/encrypt data with a secret key instance.
//    /// </summary>
//    public class Cryptography
//    {

//        /* Constants */

//        public const int KEY128BIT = 0;
//        public const int KEY256BIT = 1;

//        /* Fields */

//        private readonly byte[] a_ivLocal;
//        private readonly CryptographicKey o_secretKeyLocal;

//        /* Properties */

//        /* Methods */

//        /// <summary>
//        /// Constructor to create cryptography instance for encoding and decoding byte arrays with AES/GCM, one secret key during cryptography instance
//        /// for higher security static encoding and decoding methods are available which are always generating a new secret key, thus need higher performance.
//        /// </summary>
//        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
//        /// <param name="p_i_keyLengthOption">Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']</param>
//        /// <exception cref="ArgumentException">invalid key length option [Cryptography.KEY128BIT | Cryptography.KEY256BIT]</exception>
//        public Cryptography(string p_s_commonSecretPassphrase, int p_i_keyLengthOption)
//        {
//            /* check length of common secret passphrase */
//            if (p_s_commonSecretPassphrase.Length < 36)
//            {
//                throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + p_s_commonSecretPassphrase.Length + "' characters");
//            }

//            /* check key length option */
//            if ((p_i_keyLengthOption < 0) || (p_i_keyLengthOption > 1))
//            {
//                throw new ArgumentException("Unknown key length option[" + p_i_keyLengthOption + "]. Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']");
//            }

//            /* get 12 bytes for salt from common secret passphrase */
//            this.a_ivLocal = System.Text.Encoding.UTF8.GetBytes(p_s_commonSecretPassphrase.Substring(0, 12));

//            /* generate secret key for cryptography instance */
//            this.o_secretKeyLocal = Cryptography.GenerateSecretKey(this.a_ivLocal, p_s_commonSecretPassphrase, p_i_keyLengthOption);
//        }

//        /// <summary>
//        /// Encrypt data with cryptography instance configuration.
//        /// </summary>
//        /// <param name="p_a_data">data in byte array which will be encrypted</param>
//        /// <returns>encrypted byte array</returns>
//        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
//        public byte[] Encrypt(byte[] p_a_data)
//        {
//            /* measure time */
//            System.Diagnostics.Stopwatch o_stopwatch = new System.Diagnostics.Stopwatch();
//            o_stopwatch.Start();

//            /* check input data parameter */
//            if ((p_a_data == null) || (p_a_data.Length == 0))
//            {
//                throw new ArgumentException("Byte array for encryption is empty");
//            }

//            /* encrypt data with cryptographic engine */
//            EncryptedAndAuthenticatedData o_encryptedAndAuthenticatedData = CryptographicEngine.EncryptAndAuthenticate(this.o_secretKeyLocal, p_a_data.AsBuffer(), this.a_ivLocal.AsBuffer(), null);

//            /* set salt, encrypted data and authentication tag in one byte data */
//            System.IO.MemoryStream o_encryptedData = new System.IO.MemoryStream();

//            using (System.IO.BinaryWriter o_binaryWriter = new System.IO.BinaryWriter(o_encryptedData))
//            {
//                o_binaryWriter.Write(o_encryptedAndAuthenticatedData.EncryptedData.ToArray());
//                o_binaryWriter.Write(o_encryptedAndAuthenticatedData.AuthenticationTag.ToArray());
//            }

//            /* log amount if ms needed for encryption */
//            o_stopwatch.Stop();
//            ForestNETLib.Core.Global.ILogFiner("encrypt: " + (o_stopwatch.ElapsedMilliseconds) + " ms");

//            /* return encrypted data */
//            return o_encryptedData.ToArray();
//        }

//        /// <summary>
//        /// Decrypt data with cryptography instance configuration.
//        /// </summary>
//        /// <param name="p_a_data">data in byte array which will be decrypted</param>
//        /// <returns>decrypted byte array</returns>
//        /// <exception cref="ArgumentException">byte array for decryption is empty</exception>
//        public byte[] Decrypt(byte[] p_a_data)
//        {
//            /* measure time */
//            System.Diagnostics.Stopwatch o_stopwatch = new System.Diagnostics.Stopwatch();
//            o_stopwatch.Start();

//            /* check input data parameter */
//            if ((p_a_data == null) || (p_a_data.Length == 0))
//            {
//                throw new ArgumentException("Byte array for decryption is empty");
//            }

//            /* create byte array for encrypted data bytes */
//            byte[] a_data = new byte[p_a_data.Length - 16];
//            /* read encrypted bytes */
//            for (int i = 0; i < p_a_data.Length - 16; i++)
//            {
//                a_data[i] = p_a_data[i];
//            }

//            /* create byte array for authentication tag bytes */
//            byte[] a_tag = new byte[16];
//            /* read authentication tag bytes */
//            for (int i = p_a_data.Length - 16; i < p_a_data.Length; i++)
//            {
//                a_tag[i - (p_a_data.Length - 16)] = p_a_data[i];
//            }

//            /* decrypt input data with cryptographic engine */
//            IBuffer a_return = CryptographicEngine.DecryptAndAuthenticate(this.o_secretKeyLocal, a_data.AsBuffer(), this.a_ivLocal.AsBuffer(), a_tag.AsBuffer(), null);

//            /* log amount if ms needed for decryption */
//            o_stopwatch.Stop();
//            ForestNETLib.Core.Global.ILogFiner("decrypt: " + (o_stopwatch.ElapsedMilliseconds) + " ms");

//            /* return decrypted data */
//            return a_return.ToArray();
//        }

//        /// <summary>
//        /// Encrypt data with new salt and secret key instance with key length 256 bit.
//        /// </summary>
//        /// <param name="p_a_data">data in byte array which will be encrypted</param>
//        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
//        /// <returns>encrypted byte array</returns>
//        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
//        public static byte[] Encrypt_AES_GCM(byte[] p_a_data, string p_s_commonSecretPassphrase)
//        {
//            return Cryptography.Encrypt_AES_GCM(p_a_data, p_s_commonSecretPassphrase, Cryptography.KEY256BIT);
//        }

//        /// <summary>
//        /// Encrypt data with new salt and secret key instance.
//        /// </summary>
//        /// <param name="p_a_data">data in byte array which will be encrypted</param>
//        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
//        /// <param name="p_i_keyLengthOption">Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']</param>
//        /// <returns>encrypted byte array</returns>
//        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
//        public static byte[] Encrypt_AES_GCM(byte[] p_a_data, string p_s_commonSecretPassphrase, int p_i_keyLengthOption)
//        {
//            /* measure time */
//            System.Diagnostics.Stopwatch o_stopwatch = new System.Diagnostics.Stopwatch();
//            o_stopwatch.Start();

//            /* check input data parameter */
//            if ((p_a_data == null) || (p_a_data.Length == 0))
//            {
//                throw new ArgumentException("Byte array for decryption is empty");
//            }

//            /* check length of common secret passphrase */
//            if (p_s_commonSecretPassphrase.Length < 36)
//            {
//                throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + p_s_commonSecretPassphrase.Length + "' characters");
//            }

//            /* check key length option */
//            if ((p_i_keyLengthOption < 0) || (p_i_keyLengthOption > 1))
//            {
//                throw new ArgumentException("Unknown key length option[" + p_i_keyLengthOption + "]. Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']");
//            }

//            /* define salt byte array with 12 bytes */
//            byte[] a_iv = new byte[12];
//            /* get our secure random number generator instance and fill salt byte array randomly */
//            using (System.Security.Cryptography.RandomNumberGenerator o_randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create())
//            {
//                o_randomNumberGenerator.GetBytes(a_iv);
//            }

//            /* generate secret key for cryptography instance */
//            CryptographicKey o_secretKey = Cryptography.GenerateSecretKey(a_iv, p_s_commonSecretPassphrase, p_i_keyLengthOption);

//            /* encrypt data with cryptographic engine */
//            EncryptedAndAuthenticatedData o_encryptedAndAuthenticatedData = CryptographicEngine.EncryptAndAuthenticate(o_secretKey, p_a_data.AsBuffer(), a_iv.AsBuffer(), null);

//            /* set salt, encrypted data and authentication tag in one byte data */
//            System.IO.MemoryStream o_encryptedData = new System.IO.MemoryStream();

//            using (System.IO.BinaryWriter o_binaryWriter = new System.IO.BinaryWriter(o_encryptedData))
//            {
//                o_binaryWriter.Write(a_iv);
//                o_binaryWriter.Write(o_encryptedAndAuthenticatedData.EncryptedData.ToArray());
//                o_binaryWriter.Write(o_encryptedAndAuthenticatedData.AuthenticationTag.ToArray());
//            }

//            /* log amount if ms needed for encryption */
//            o_stopwatch.Stop();
//            ForestNETLib.Core.Global.ILogFiner("encrypt: " + (o_stopwatch.ElapsedMilliseconds) + " ms");

//            /* return encrypted data */
//            return o_encryptedData.ToArray();
//        }

//        /// <summary>
//        /// Decrypt data with new secret key instance by reading salt at the beginning of encrypted data byte array, with key length 256 bit.
//        /// </summary>
//        /// <param name="p_a_data">data in byte array which will be decrypted</param>
//        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
//        /// <returns>decrypted byte array</returns>
//        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
//        public static byte[] Decrypt_AES_GCM(byte[] p_a_data, string p_s_commonSecretPassphrase)
//        {
//            return Cryptography.Decrypt_AES_GCM(p_a_data, p_s_commonSecretPassphrase, Cryptography.KEY256BIT);
//        }

//        /// <summary>
//        /// Decrypt data with new secret key instance by reading salt at the beginning of encrypted data byte array.
//        /// </summary>
//        /// <param name="p_a_data">data in byte array which will be decrypted</param>
//        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
//        /// <param name="p_i_keyLengthOption">Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']</param>
//        /// <returns>decrypted byte array</returns>
//        /// <exception cref="ArgumentException">byte array for encryption is empty</exception>
//        public static byte[] Decrypt_AES_GCM(byte[] p_a_data, string p_s_commonSecretPassphrase, int p_i_keyLengthOption)
//        {
//            /* measure time */
//            System.Diagnostics.Stopwatch o_stopwatch = new System.Diagnostics.Stopwatch();
//            o_stopwatch.Start();

//            /* check input data parameter */
//            if ((p_a_data == null) || (p_a_data.Length == 0))
//            {
//                throw new ArgumentException("Byte array for decryption is empty");
//            }

//            /* check length of common secret passphrase */
//            if (p_s_commonSecretPassphrase.Length < 36)
//            {
//                throw new ArgumentException("Common secret passphrase must have at least '36' characters, but has '" + p_s_commonSecretPassphrase.Length + "' characters");
//            }

//            /* check key length option */
//            if ((p_i_keyLengthOption < 0) || (p_i_keyLengthOption > 1))
//            {
//                throw new ArgumentException("Unknown key length option[" + p_i_keyLengthOption + "]. Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']");
//            }

//            /* create byte array for salt bytes */
//            byte[] a_iv = new byte[12];
//            /* read salt bytes */
//            for (int i = 0; i < 12; i++)
//            {
//                a_iv[i] = p_a_data[i];
//            }

//            /* create byte array for encrypted data bytes */
//            byte[] a_data = new byte[p_a_data.Length - 12 - 16];
//            /* read encrypted bytes */
//            for (int i = 12; i < p_a_data.Length - 16; i++)
//            {
//                a_data[i - 12] = p_a_data[i];
//            }

//            /* create byte array for authentication tag bytes */
//            byte[] a_tag = new byte[16];
//            /* read authentication tag bytes */
//            for (int i = p_a_data.Length - 16; i < p_a_data.Length; i++)
//            {
//                a_tag[i - (p_a_data.Length - 16)] = p_a_data[i];
//            }

//            /* generate secret key for cryptography instance */
//            CryptographicKey o_secretKey = Cryptography.GenerateSecretKey(a_iv, p_s_commonSecretPassphrase, p_i_keyLengthOption);

//            /* decrypt input data with cryptographic engine */
//            IBuffer a_return = CryptographicEngine.DecryptAndAuthenticate(o_secretKey, a_data.AsBuffer(), a_iv.AsBuffer(), a_tag.AsBuffer(), null);

//            /* log amount if ms needed for decryption */
//            o_stopwatch.Stop();
//            ForestNETLib.Core.Global.ILogFiner("decrypt: " + (o_stopwatch.ElapsedMilliseconds) + " ms");

//            /* return decrypted data */
//            return a_return.ToArray();
//        }

//        /// <summary>
//        /// generate secret key instance which can be used with a cipher to de-/encrypt data.
//        /// </summary>
//        /// <param name="p_a_iv">salt byte array, default 16 bytes</param>
//        /// <param name="p_s_commonSecretPassphrase">common secret passphrase string which must be at least 36 characters long</param>
//        /// <param name="p_i_keyLengthOption">Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']</param>
//        /// <returns>CryptographicKey instance</returns>
//        /// <exception cref="ArgumentException">invalid key length option [Cryptography.KEY128BIT | Cryptography.KEY256BIT]</exception>
//        private static CryptographicKey GenerateSecretKey(byte[] p_a_iv, string p_s_commonSecretPassphrase, int p_i_keyLengthOption)
//        {
//            /* retrieve iteration count from salt byte array, max. 16 bits, otherwise key generation takes to long  */
//            byte[] a_iterationCount = new byte[] { 0x00, 0x00, 0x00, 0x00 };

//            a_iterationCount[2] |= (byte)(p_a_iv[0] & 0x80); /* 8th bit of 1st salt byte */
//            a_iterationCount[2] |= (byte)(p_a_iv[1] & 0x40); /* 7th bit of 2nd salt byte */
//            a_iterationCount[2] |= (byte)(p_a_iv[2] & 0x20); /* 6th bit of 3rd salt byte */
//            a_iterationCount[2] |= (byte)(p_a_iv[3] & 0x10); /* 5th bit of 4th salt byte */
//            a_iterationCount[2] |= (byte)(p_a_iv[4] & 0x08); /* 4th bit of 5th salt byte */
//            a_iterationCount[2] |= (byte)(p_a_iv[5] & 0x04); /* 3rd bit of 6th salt byte */
//            a_iterationCount[2] |= (byte)(p_a_iv[6] & 0x02); /* 2nd bit of 7th salt byte */
//            a_iterationCount[2] |= (byte)(p_a_iv[7] & 0x01); /* 1st bit of 8th salt byte */

//            a_iterationCount[3] |= (byte)(p_a_iv[8] & 0x05); /* 1st + 3rd bit of 9th salt byte */
//            a_iterationCount[3] |= (byte)(p_a_iv[9] & 0x0A); /* 2nd + 4th bit of 10th salt byte */
//            a_iterationCount[3] |= (byte)(p_a_iv[10] & 0x50); /* 5th + 7th bit of 11th salt byte */
//            a_iterationCount[3] |= (byte)(p_a_iv[11] & 0xA0); /* 6th + 8th bit of 12th salt byte */

//            /* convert iteration count bytes to integer value */
//            int i_iterationCount = ForestNETLib.Core.Helper.ByteArrayToInt(a_iterationCount);
//            /* key material variable */
//            IBuffer o_keyMaterial;

//            /* check key length option and set key factory algorithm */
//            if (p_i_keyLengthOption == Cryptography.KEY128BIT)
//            {
//                /* open secret key algorithm PBKDF2 Sha1 */
//                KeyDerivationAlgorithmProvider o_secretKeyAlgorithm = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha1);
//                /* convert common secret password to utf8 password buffer */
//                IBuffer o_passwordBuffer = CryptographicBuffer.ConvertStringToBinary(p_s_commonSecretPassphrase, BinaryStringEncoding.Utf8);
//                /* create first version of our secret key */
//                CryptographicKey o_secretKey = o_secretKeyAlgorithm.CreateKey(o_passwordBuffer);
//                /* create secret key specifications */
//                KeyDerivationParameters o_parameters = KeyDerivationParameters.BuildForPbkdf2(p_a_iv.AsBuffer(), (uint)i_iterationCount);
//                /* create secret key material with our new specifications and key length of 128-bit */
//                o_keyMaterial = CryptographicEngine.DeriveKeyMaterial(o_secretKey, o_parameters, 16);
//            }
//            else if (p_i_keyLengthOption == Cryptography.KEY256BIT)
//            {
//                /* open secret key algorithm PBKDF2 Sha256 */
//                KeyDerivationAlgorithmProvider o_secretKeyAlgorithm = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha256);
//                /* convert common secret password to utf8 password buffer */
//                IBuffer o_passwordBuffer = CryptographicBuffer.ConvertStringToBinary(p_s_commonSecretPassphrase, BinaryStringEncoding.Utf8);
//                /* create first version of our secret key */
//                CryptographicKey o_secretKey = o_secretKeyAlgorithm.CreateKey(o_passwordBuffer);
//                /* create secret key specifications */
//                KeyDerivationParameters o_parameters = KeyDerivationParameters.BuildForPbkdf2(p_a_iv.AsBuffer(), (uint)i_iterationCount);
//                /* create secret key material with our new specifications and key length of 256-bit */
//                o_keyMaterial = CryptographicEngine.DeriveKeyMaterial(o_secretKey, o_parameters, 32);
//            }
//            else
//            {
//                throw new ArgumentException("Unknown key length option[" + p_i_keyLengthOption + "]. Please use ['Cryptography.KEY128BIT'] or ['Cryptography.KEY256BIT']");
//            }

//            /* return crypthographic key with open symmetric key algorithm AES GCM and our key material */
//            return SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesGcm).CreateSymmetricKey(o_keyMaterial);
//        }
//    }
//}
