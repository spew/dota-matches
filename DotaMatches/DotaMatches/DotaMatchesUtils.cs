using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotaMatches
{
    public static class DotaMatchesUtils
    {
        public static byte[] PackInteger(ulong integer)
        {
            List<byte> bytes = new List<byte>();

            if (integer == 0)
            {
                return new byte[] { 0 };
            }

            while (integer != 0)
            {
                byte value = Convert.ToByte(integer & 0x7F);
                integer >>= 7;

                if (integer > 0)
                {
                    value |= (byte)0x80;
                }

                bytes.Add(value);
            }

            return bytes.ToArray();
        }

        public static long ExtractInteger(byte[] bytes, int index)
        {
            byte curValue;
            int curShift = 0;
            long retValue = 0;

            do
            {
                curValue = bytes[index];
                retValue |= (long)(curValue & 0x7F) << curShift;

                curShift += 7;
                index++;
            }
            while ((curValue & 0x80) != 0);

            return retValue;
        }
    }
}
