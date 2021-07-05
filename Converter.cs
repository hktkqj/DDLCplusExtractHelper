using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using AssetStudio;

namespace DDLC
{
    class Program
    {
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        static void Main(string[] args)
        {
	    // Drag .cy file on compiled .exe program
            string file = args[0];
            XorFileStream fileStream = new XorFileStream(file, FileMode.Open, FileAccess.Read, 40);
            byte[] m_bytes = ReadToEnd(fileStream);
            File.WriteAllBytes(file + ".tmp", m_bytes);
	}

	// XorFileStream comes from reversed dll file
	public class XorFileStream : Stream
	{
		public XorFileStream(string path, FileMode mode, FileAccess access, byte privateKey = 40)
		{
			this.m_Key = privateKey;
			this.m_FileStream = new FileStream(path, mode, access);
		}
		
		private void PerformXor(ref byte[] array, int offset, int count)
		{
			for (int i = offset; i < offset + count; i++)
			{
				array[i] ^= this.m_Key;
			}
		}
		
		public override void Flush()
		{
			this.m_FileStream.Flush();
		}
		
		public override int Read(byte[] array, int offset, int count)
		{
			int result = this.m_FileStream.Read(array, offset, count);
			this.PerformXor(ref array, offset, count);
			return result;
		}
		
		public override void Write(byte[] array, int offset, int count)
		{
			this.PerformXor(ref array, offset, count);
			this.m_FileStream.Write(array, offset, count);
		}
		
		public override int ReadByte()
		{
			return (int)((byte)(this.m_FileStream.ReadByte() ^ (int)this.m_Key));
		}
		
		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.m_FileStream.Seek(offset, origin);
		}
		
		public override void SetLength(long value)
		{
			this.m_FileStream.SetLength(value);
		}
		
		public override void WriteByte(byte value)
		{
			this.m_FileStream.WriteByte((byte)(value ^ this.m_Key));
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000030C8 File Offset: 0x000012C8
		public override bool CanRead
		{
			get
			{
				return this.m_FileStream.CanRead;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000097 RID: 151 RVA: 0x000030D5 File Offset: 0x000012D5
		public override bool CanSeek
		{
			get
			{
				return this.m_FileStream.CanSeek;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000030E2 File Offset: 0x000012E2
		public override bool CanWrite
		{
			get
			{
				return this.m_FileStream.CanWrite;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000030EF File Offset: 0x000012EF
		public override long Length
		{
			get
			{
				return this.m_FileStream.Length;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600009A RID: 154 RVA: 0x000030FC File Offset: 0x000012FC
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00003109 File Offset: 0x00001309
		public override long Position
		{
			get
			{
				return this.m_FileStream.Position;
			}
			set
			{
				this.m_FileStream.Position = value;
			}
		}

		// Token: 0x0400002C RID: 44
		private FileStream m_FileStream;

		// Token: 0x0400002D RID: 45
		private byte m_Key;
	}
}
