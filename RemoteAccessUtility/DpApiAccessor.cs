using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace RemoteAccessUtility
{
    public static class DpApiAccessor
    {
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptProtectData(
            DATA_BLOB pDataIn,
            string szDataDescr,
            DATA_BLOB pOptionalEntropy,
            IntPtr pvReserved,
            CRYPTPROTECT_PROMPTSTRUCT pPromptStruct,
            int dwFlags,
            DATA_BLOB pDataOut);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptUnprotectData(
            DATA_BLOB pDataIn,
            ref string ppszDataDescr,
            DATA_BLOB pOptionalEntropy,
            IntPtr pvReserved,
            CRYPTPROTECT_PROMPTSTRUCT pPromptStruct,
            int dwFlags, DATA_BLOB pDataOut);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;

            public DATA_BLOB() { }
            public DATA_BLOB(byte[] data)
            {
                if (data == null)
                    data = new byte[0];

                pbData = Marshal.AllocHGlobal(data.Length);

                if (pbData == IntPtr.Zero)
                    throw new Exception("Unable to allocate data buffer for BLOB structure.");

                cbData = data.Length;

                Marshal.Copy(data, 0, pbData, data.Length);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class CRYPTPROTECT_PROMPTSTRUCT
        {
            public int cbSize;
            public int dwPromptFlags;
            public IntPtr hwndApp;
            public string szPrompt;

            public CRYPTPROTECT_PROMPTSTRUCT()
            {
                cbSize = Marshal.SizeOf(typeof(CRYPTPROTECT_PROMPTSTRUCT));
                dwPromptFlags = 0;
                hwndApp = IntPtr.Zero;
                szPrompt = null;
            }
        }

        private const int CRYPTPROTECT_UI_FORBIDDEN = 0x1;
        private const int CRYPTPROTECT_LOCAL_MACHINE = 0x4;

        public enum KeyType { UserKey = 1, MachineKey };
        private static KeyType defaultKeyType = KeyType.UserKey;

        public static byte[] Encrypt(string plainText)
        {
            return Encrypt(defaultKeyType, plainText);
        }

        public static byte[] Encrypt(KeyType keyType, string plainText)
        {
            return Encrypt(keyType, plainText, String.Empty);
        }

        public static byte[] Encrypt(KeyType keyType, string plainText, string entropy)
        {
            return Encrypt(keyType, plainText, entropy, String.Empty);
        }

        public static byte[] Encrypt(KeyType keyType, string plainText, string entropy, string description)
        {
            return Encrypt(keyType, Encoding.Unicode.GetBytes(plainText), Encoding.Unicode.GetBytes(entropy), description);
        }

        public static byte[] Encrypt(KeyType keyType, byte[] plainTextBytes, byte[] entropyBytes, string description)
        {
            DATA_BLOB plainTextBlob = new DATA_BLOB(plainTextBytes);
            DATA_BLOB cipherTextBlob = new DATA_BLOB();
            DATA_BLOB entropyBlob = new DATA_BLOB(entropyBytes);

            CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();

            try
            {
                int flags = CRYPTPROTECT_UI_FORBIDDEN;

                if (keyType == KeyType.MachineKey)
                    flags |= CRYPTPROTECT_LOCAL_MACHINE;

                bool success = CryptProtectData(plainTextBlob, description, entropyBlob, IntPtr.Zero, prompt, flags, cipherTextBlob);

                if (!success)
                {
                    int errCode = Marshal.GetLastWin32Error();

                    throw new Exception("CryptProtectData failed.", new Win32Exception(errCode));
                }

                byte[] cipherTextBytes = new byte[cipherTextBlob.cbData];

                Marshal.Copy(cipherTextBlob.pbData, cipherTextBytes, 0, cipherTextBlob.cbData);

                return cipherTextBytes;
            }
            catch (Exception e)
            {
                throw new Exception("DPAPI was unable to encrypt data.", e);
            }
            finally
            {
                if (plainTextBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(plainTextBlob.pbData);

                if (cipherTextBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(cipherTextBlob.pbData);

                if (entropyBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(entropyBlob.pbData);
            }
        }

        public static string Decrypt(byte[] cipherTextBytes)
        {
            return Decrypt(cipherTextBytes, out var description);
        }

        public static string Decrypt(byte[] cipherTextBytes, out string description)
        {
            return Decrypt(cipherTextBytes, String.Empty, out description);
        }

        public static string Decrypt(byte[] cipherTextBytes, string entropy, out string description)
        {
            return Encoding.Unicode.GetString(Decrypt(cipherTextBytes, Encoding.Unicode.GetBytes(entropy), out description));
        }

        public static byte[] Decrypt(byte[] cipherTextBytes, byte[] entropyBytes, out string description)
        {
            DATA_BLOB plainTextBlob = new DATA_BLOB();
            DATA_BLOB cipherTextBlob = new DATA_BLOB(cipherTextBytes);
            DATA_BLOB entropyBlob = new DATA_BLOB(entropyBytes);

            CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();

            description = String.Empty;

            try
            {
                int flags = CRYPTPROTECT_UI_FORBIDDEN;

                bool success = CryptUnprotectData(cipherTextBlob, ref description, entropyBlob, IntPtr.Zero, prompt, flags, plainTextBlob);

                if (!success)
                {
                    int errCode = Marshal.GetLastWin32Error();

                    throw new Exception("CryptUnprotectData failed.", new Win32Exception(errCode));
                }

                byte[] plainTextBytes = new byte[plainTextBlob.cbData];

                Marshal.Copy(plainTextBlob.pbData, plainTextBytes, 0, plainTextBlob.cbData);

                return plainTextBytes;
            }
            catch (Exception e)
            {
                throw new Exception("DPAPI was unable to decrypt data.", e);
            }
            finally
            {
                if (plainTextBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(plainTextBlob.pbData);

                if (cipherTextBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(cipherTextBlob.pbData);

                if (entropyBlob.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(entropyBlob.pbData);
            }
        }
    }
}
