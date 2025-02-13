using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Support;
//TODO: [ENCRYPTION MAY BE INSECURE] review and implement this code, currently not needed.

#if false
class AesExample
{
    public static void Main()
    {
        try
        {
            string original = "Here is some data to encrypt!";

            // Create a new instance of the Aes class.
            // This generates a new key and initialization vector (IV).
            using (Aes myAes = Aes.Create())
            {
                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes(original, myAes.Key, myAes.IV);

                // Decrypt the bytes to a string.
                string roundtrip = DecryptStringFromBytes(encrypted, myAes.Key, myAes.IV);

                // Display the original data and the decrypted data.
                Console.WriteLine("Original:   {0}", original);
                Console.WriteLine("Round trip: {0}", roundtrip);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message);
        }
    }

    static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException(nameof(plainText));
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException(nameof(Key));
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException(nameof(IV));
        byte[] encrypted;

        // Create a Aes object with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new())
            {
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new(csEncrypt))
                    {
                        // Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }

    static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException(nameof(cipherText));
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException(nameof(Key));
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException(nameof(IV));

        // Declare the string used to hold the decrypted text.
        string plaintext = null;

        // Create a Aes object with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new(cipherText))
            {
                using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}

public class AesGcmEncryption
{
    private const int KeySize = 32; // 256 bits
    private const int NonceSize = 12; // Tamanho recomendado para GCM
    private const int TagSize = 16; // 128 bits

    public static void Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, Span<byte> output)
    {
        // Deriva a chave usando PBKDF2 via KeyDerivation
        Span<byte> key = stackalloc byte[KeySize];
        DeriveKey(password, salt, key);

        Span<byte> nonce = stackalloc byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        // Cria o AES-GCM
        using var aesGcm = new AesGcm(key);

        // Escreve o nonce no início da saída
        nonce.CopyTo(output);

        // Define onde o ciphertext e a tag serão armazenados
        var ciphertext = output.Slice(NonceSize, data.Length);
        var tag = output.Slice(NonceSize + data.Length, TagSize);

        // Criptografa os dados
        aesGcm.Encrypt(nonce, data, ciphertext, tag);
    }

    public static void Decrypt(ReadOnlySpan<byte> encryptedData, ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, Span<byte> output)
    {
        // Deriva a chave usando PBKDF2 via KeyDerivation
        Span<byte> key = stackalloc byte[KeySize];
        DeriveKey(password, salt, key);

        // Lê o nonce
        var nonce = encryptedData.Slice(0, NonceSize);

        // Define onde o ciphertext e a tag estão
        var ciphertext = encryptedData.Slice(NonceSize, encryptedData.Length - NonceSize - TagSize);
        var tag = encryptedData.Slice(encryptedData.Length - TagSize, TagSize);

        // Cria o AES-GCM
        using var aesGcm = new AesGcm(key);

        // Descriptografa os dados
        aesGcm.Decrypt(nonce, ciphertext, tag, output);
    }

    private static void DeriveKey(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, Span<byte> key)
    {
       //derivedKey.CopyTo(key);
    }
}
#endif
public static class Encryption
{
    private const int BLOCK_SIZE = 256;
    private const int KEY_SIZE = 32;
    private const int SALT_SIZE = KEY_SIZE;
    private const int IV_SIZE = KEY_SIZE;
    // This constant determines the number of iterations for the password bytes generation function.
    private const int DERIVATION_ITERATIONS = 1000;
    public static void Encrypt(Stream input, Stream output, string password)
    {
        // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
        // so that they can be used when decrypting.
        var salt = new byte[SALT_SIZE];
        var iv = new byte[IV_SIZE];
#pragma warning disable SYSLIB0023 // Type or member is obsolete
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
            rng.GetBytes(iv);
        }
#pragma warning restore SYSLIB0023 // Type or member is obsolete
        var key = DeriveKey(password, salt);
        using var algorithm = CreateSymmetricAlgorithm();
        using var encryptor = algorithm.CreateEncryptor(key, iv);
        var crypt = new CryptoStream(output, encryptor, CryptoStreamMode.Write);
        output.Write(salt);
        output.Write(iv);
        input.CopyTo(crypt);
        crypt.FlushFinalBlock();
    }
    public static string Encrypt(string message, string password)
    {
        var input = new ImprovedMemoryStream(Encoding.UTF8.GetBytes(message));
        var output = new ImprovedMemoryStream();
        input.Position = 0;
        Encrypt(input, output, password);
        output.Position = 0;
        return Convert.ToBase64String(output.AsReadOnlySpan());
    }
    public static void Decrypt(Stream input, Stream output, string password)
    {
        var salt = new byte[SALT_SIZE];
        var iv = new byte[IV_SIZE];
        _ = input.Read(salt);
        _ = input.Read(iv);
        var key = DeriveKey(password, salt);
        using var algorithm = CreateSymmetricAlgorithm();
        using var decryptor = algorithm.CreateDecryptor(key, iv);
        var crypt = new CryptoStream(input, decryptor, CryptoStreamMode.Read);
        crypt.CopyTo(output);
        output.Flush();
    }
    public static string Decrypt(string message, string password)
    {
        var input = new ImprovedMemoryStream(Convert.FromBase64String(message));
        var output = new ImprovedMemoryStream();
        input.Position = 0;
        Decrypt(input, output, password);
        output.Position = 0;
        return Encoding.UTF8.GetString(output.AsReadOnlySpan());
    }
    private static byte[] DeriveKey(string key, byte[] salt)
    {
#pragma warning disable SYSLIB0041 // Type or member is obsolete
        using var derivated = new Rfc2898DeriveBytes(key, salt, DERIVATION_ITERATIONS);
#pragma warning restore SYSLIB0041 // Type or member is obsolete
        return derivated.GetBytes(KEY_SIZE);
    }
    private static SymmetricAlgorithm CreateSymmetricAlgorithm()
    {
#pragma warning disable SYSLIB0022 // Type or member is obsolete
        var rijndael = new RijndaelManaged { BlockSize = BLOCK_SIZE, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
#pragma warning restore SYSLIB0022 // Type or member is obsolete
        return rijndael;
    }
}