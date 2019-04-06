using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RemoteAccessUtility.Test
{
    [TestClass]
    public class RdpOptionTest
    {
        [TestMethod]
        public void EncryptDecrypt1()
        {
            var expected = "password";

            var encryptData = DpApiAccessor.Encrypt(expected);
            var actual = DpApiAccessor.Decrypt(encryptData);
            Assert.AreEqual(expected, actual);

            encryptData = DpApiAccessor.Encrypt(DpApiAccessor.KeyType.UserKey, expected);
            actual = DpApiAccessor.Decrypt(encryptData);
            Assert.AreEqual(expected, actual);

            encryptData = DpApiAccessor.Encrypt(DpApiAccessor.KeyType.MachineKey, expected, string.Empty, expected);
            actual = DpApiAccessor.Decrypt(encryptData, out var description);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, description);

            encryptData = DpApiAccessor.Encrypt(DpApiAccessor.KeyType.MachineKey, expected, expected, expected);
            actual = DpApiAccessor.Decrypt(encryptData, expected, out description);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, description);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void EncryptDecrypt2()
        {
            var expected = "password";

            var encryptData = DpApiAccessor.Encrypt(DpApiAccessor.KeyType.MachineKey, expected, expected, expected);
            DpApiAccessor.Decrypt(encryptData, out var description);
        }
    }
}
