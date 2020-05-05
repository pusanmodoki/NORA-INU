using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;

public static class ByteCompressor
{
	public static byte[] Compress(byte[] data)
	{
		byte[] result = null;

		using (MemoryStream compressed = new MemoryStream())
		{
			using (GZipStream zipStream = new GZipStream(compressed, CompressionMode.Compress))
				zipStream.Write(data, 0, data.Length);
			result = compressed.ToArray();
		}

		return result;
	}
	public static byte[] Uncompress(byte[] data)
	{
		byte[] result = null;

		using (MemoryStream compressed = new MemoryStream(data))
		{
			using (MemoryStream decompressed = new MemoryStream())
			{
				using (GZipStream zipStream = new GZipStream(compressed, CompressionMode.Decompress))
					zipStream.CopyTo(decompressed);
				result = decompressed.ToArray();
			}
		}

		return result;
	}
}
