using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Extensions
{
    public static  class IO
    {
        public static void ReadBytes(this Stream stream, byte[] buf)
        {
            int pos = 0;
            int r;
            while ((r = stream.Read(buf, pos, buf.Length - pos)) > 0)
            {
                pos += r;
            }
            if (pos != buf.Length)
            {
                throw new IOException($"Stream did not contain enough bytes ({pos}) < ({buf.Length})");
            }
        }
        public static byte[] ReadBytes(this Stream stream, int len)
        {
            return stream.ReadBytes((long)len);
        }
        public static byte[] ReadBytes(this Stream stream, long len)
        {
            byte[] buf = new byte[len];
            stream.ReadBytes(buf);
            return buf;
        }
    }
}
