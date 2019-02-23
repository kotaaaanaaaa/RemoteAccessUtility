using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace RemoteAccessUtility
{
    public static class SecureStringEx
    {
        public static bool BStrEquals(this SecureString self, SecureString str)
        {
            if (self == null && str == null)
            { return true; }

            if (self == null || str == null)
            { return false; }

            if (self.Length != str.Length)
            { return false; }

            var aPtr = Marshal.SecureStringToBSTR(self);
            var bPtr = Marshal.SecureStringToBSTR(str);
            try
            {
                return Enumerable.Range(0, self.Length)
                    .All(i => Marshal.ReadInt16(aPtr, i) == Marshal.ReadInt16(bPtr, i));
            }
            finally
            {
                Marshal.ZeroFreeBSTR(aPtr);
                Marshal.ZeroFreeBSTR(bPtr);
            }
        }

        public static void CopyFromBSTR(this SecureString self, IntPtr bstr, int count)
        {
            self.Clear();
            var chars = Enumerable.Range(0, count)
                .Select(i => (char)Marshal.ReadInt16(bstr, i * 2));
            foreach (var c in chars)
            {
                self.AppendChar(c);
            }
        }
    }
}
