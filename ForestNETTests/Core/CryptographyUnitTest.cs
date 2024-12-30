namespace ForestNETTests.Core
{
    public class CryptographyUnitTest
    {
        [Test]
        public void TestCryptography()
        {
            try
            {
                string s_commonSecretPassphrase = "123456789012345678901234567890123456";

                /* some test messages */
                System.Collections.Generic.List<string> a_messages = [
                    /* 001 length */	"1",	
					/* 008 length */	"12345678",
					/* 016 length */	"1234567890123456",
					/* 032 length */	"12345678901234567890123456789012",
					/* 063 length */	"123456789012345678901234567890123456789012345678901234567890123",
					/* 064 length */	"1234567890123456789012345678901234567890123456789012345678901234",
					/* 108 length */	"123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678",
					/* 1500 length */	"123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
					/* 1468 length */	"1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678"
                ];

                foreach (string s_message in a_messages)
                {
                    /* static encrypt and decrypt each message with 128-bit key */
                    byte[] a_encrypted = ForestNETLib.Core.Cryptography.Encrypt_AES_GCM(System.Text.Encoding.UTF8.GetBytes(s_message), s_commonSecretPassphrase, ForestNETLib.Core.Cryptography.KEY128BIT);
                    byte[] a_decrypted = ForestNETLib.Core.Cryptography.Decrypt_AES_GCM(a_encrypted, s_commonSecretPassphrase, ForestNETLib.Core.Cryptography.KEY128BIT);

                    Assert.That(
                        a_encrypted, Has.Length.EqualTo((s_message.Length + 28)),
                        a_encrypted.Length + " != " + (s_message.Length + 28)
                    );
                    Assert.That(
                        a_decrypted, Has.Length.EqualTo(s_message.Length),
                        a_decrypted.Length + " != " + s_message.Length
                    );
                    Assert.That(
                        System.Text.Encoding.UTF8.GetBytes(s_message).SequenceEqual(a_decrypted),
                        Is.True,
                        "difference in byte arrays"
                    );

                    /* static encrypt and decrypt each message with 256-bit key */
                    a_encrypted = ForestNETLib.Core.Cryptography.Encrypt_AES_GCM(System.Text.Encoding.UTF8.GetBytes(s_message), s_commonSecretPassphrase, ForestNETLib.Core.Cryptography.KEY256BIT);
                    a_decrypted = ForestNETLib.Core.Cryptography.Decrypt_AES_GCM(a_encrypted, s_commonSecretPassphrase);

                    Assert.That(
                        a_encrypted, Has.Length.EqualTo((s_message.Length + 28)),
                        a_encrypted.Length + " != " + (s_message.Length + 28)
                    );
                    Assert.That(
                        a_decrypted, Has.Length.EqualTo(s_message.Length),
                        a_decrypted.Length + " != " + s_message.Length
                    );
                    Assert.That(
                        System.Text.Encoding.UTF8.GetBytes(s_message).SequenceEqual(a_decrypted),
                        Is.True,
                        "difference in byte arrays"
                    );
                }

                ForestNETLib.Core.Cryptography o_cryptography128 = new(s_commonSecretPassphrase, ForestNETLib.Core.Cryptography.KEY128BIT);

                foreach (string s_message in a_messages)
                {
                    /* encrypt and decrypt each message with 128-bit key and cryptography instance */
                    byte[] a_encrypted = o_cryptography128.Encrypt(System.Text.Encoding.UTF8.GetBytes(s_message));
                    byte[] a_decrypted = o_cryptography128.Decrypt(a_encrypted);

                    Assert.That(
                        a_encrypted, Has.Length.EqualTo((s_message.Length + 16)),
                        a_encrypted.Length + " != " + (s_message.Length + 16)
                    );
                    Assert.That(
                        a_decrypted, Has.Length.EqualTo(s_message.Length),
                        a_decrypted.Length + " != " + s_message.Length
                    );
                    Assert.That(
                        System.Text.Encoding.UTF8.GetBytes(s_message).SequenceEqual(a_decrypted),
                        Is.True,
                        "difference in byte arrays"
                    );
                }

                ForestNETLib.Core.Cryptography o_cryptography256 = new(s_commonSecretPassphrase, ForestNETLib.Core.Cryptography.KEY256BIT);

                foreach (string s_message in a_messages)
                {
                    /* encrypt and decrypt each message with 256-bit key and cryptography instance */
                    byte[] a_encrypted = o_cryptography256.Encrypt(System.Text.Encoding.UTF8.GetBytes(s_message));
                    byte[] a_decrypted = o_cryptography256.Decrypt(a_encrypted);

                    Assert.That(
                        a_encrypted, Has.Length.EqualTo((s_message.Length + 16)),
                        a_encrypted.Length + " != " + (s_message.Length + 16)
                    );
                    Assert.That(
                        a_decrypted, Has.Length.EqualTo(s_message.Length),
                        a_decrypted.Length + " != " + s_message.Length
                    );
                    Assert.That(
                        System.Text.Encoding.UTF8.GetBytes(s_message).SequenceEqual(a_decrypted),
                        Is.True,
                        "difference in byte arrays"
                    );
                }
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
