using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;
using Org.BouncyCastle.Utilities;
using System.Security.Cryptography;

namespace Project1.Utils
{
    public class PasswordEncryptor
    {
        public string HashPassword(string password)
        {
            // Gera o salt (sequência aleatória de bytes) para ser utilizado na encriptação
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Converte a senha para um array de bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Concatena o salt com a senha
            byte[] saltedPasswordBytes = new byte[salt.Length + passwordBytes.Length];
            Array.Copy(salt, 0, saltedPasswordBytes, 0, salt.Length);
            Array.Copy(passwordBytes, 0, saltedPasswordBytes, salt.Length, passwordBytes.Length);

            // Calcula o hash usando a função de hash PBKDF2 com SHA-256
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            gen.Init(saltedPasswordBytes, salt, 10000);
            KeyParameter keyParam = (KeyParameter)gen.GenerateDerivedMacParameters(256);

            // Converte o hash e o salt para strings e retorna o hash concatenado com o salt
            string hashedPassword = Convert.ToBase64String(keyParam.GetKey());
            string saltString = Convert.ToBase64String(salt);
            return hashedPassword + "." + saltString;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Separa o hash e o salt
            string[] hashAndSalt = hashedPassword.Split('.');
            byte[] hash = Convert.FromBase64String(hashAndSalt[0]);
            byte[] salt = Convert.FromBase64String(hashAndSalt[1]);

            // Converte a senha para um array de bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Concatena o salt com a senha
            byte[] saltedPasswordBytes = new byte[salt.Length + passwordBytes.Length];
            Array.Copy(salt, 0, saltedPasswordBytes, 0, salt.Length);
            Array.Copy(passwordBytes, 0, saltedPasswordBytes, salt.Length, passwordBytes.Length);

            // Calcula o hash usando a função de hash PBKDF2 com SHA-256
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            gen.Init(saltedPasswordBytes, salt, 10000);
            KeyParameter keyParam = (KeyParameter)gen.GenerateDerivedMacParameters(256);

            // Verifica se o hash gerado é igual ao hash salvo
            byte[] generatedHash = keyParam.GetKey();
            return Arrays.ConstantTimeAreEqual(hash, generatedHash);
        }
    }
}
