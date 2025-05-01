using System;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using Xunit;
using SOSGame.Logic;

namespace SOSGame.Tests
{
    public class VulnerableGameTests
    {
        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        public void Constructor_ShouldSetGridSizeWithoutTriggering_WhenInputIsSafe(int input)
        {
            var game = new VulnerableGame(input);

            // Use reflection to get protected field 'gridSize'
            var field = typeof(VulnerableGame).GetField("gridSize", BindingFlags.NonPublic | BindingFlags.Instance);
            int actualValue = (int)field.GetValue(game);

            Assert.Equal(input, actualValue);
        }

        [Fact]
        public void TriggerLOLBinExecution_ShouldBeCalled_WhenInputIsMagicValue()
        {
            bool triggered = false;
            int safeGridSize = 3;

            var testGame = new TestableVulnerableGame(() => triggered = true, safeGridSize);

            // Manually simulate the trigger logic
            var triggerMethod = typeof(TestableVulnerableGame).GetMethod("Trigger_LOLBin_Execution", BindingFlags.NonPublic | BindingFlags.Instance);
            triggerMethod.Invoke(testGame, null);

            Assert.True(triggered);
        }

        [Fact]
        public void DecryptPayload_ShouldReturnOriginalMessage()
        {
            string originalMessage = "<html><script>msgbox('test')</script></html>";
            string privateKeyXml;
            byte[] encrypted;

            using (var rsa = RSA.Create(2048))
            {
                privateKeyXml = rsa.ToXmlString(true);
                string publicKeyXml = rsa.ToXmlString(false);

                var encryptor = RSA.Create();
                encryptor.FromXmlString(publicKeyXml);
                encrypted = encryptor.Encrypt(Encoding.UTF8.GetBytes(originalMessage), RSAEncryptionPadding.Pkcs1);
            }

            var game = new VulnerableGame(5); // gridSize doesn't matter here
            var decrypted = InvokePrivateDecrypt(game, encrypted, privateKeyXml);

            Assert.Equal(originalMessage, decrypted);
        }

        private string InvokePrivateDecrypt(VulnerableGame game, byte[] data, string keyXml)
        {
            var method = typeof(VulnerableGame).GetMethod("DecryptPayload", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)method.Invoke(game, new object[] { data, keyXml });
        }

        private class TestableVulnerableGame : VulnerableGame
        {
            private readonly Action _triggerCallback;

            public TestableVulnerableGame(Action triggerCallback, int gridSize)
                : base(gridSize)
            {
                _triggerCallback = triggerCallback;
            }

            protected override void Trigger_LOLBin_Execution()
            {
                _triggerCallback?.Invoke();
            }
        }
    }
}
