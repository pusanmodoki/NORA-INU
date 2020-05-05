using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using System.Security.Cryptography;

public static class ByteEncryptor
{
	static readonly int m_cKeySize = 256;
	static readonly int m_cBlockSize = 128;

	static readonly string m_cEncryptionKey = "9m5cEUtWkBJLas9hWhRmiBL5g6tdDFJa";
	static readonly string m_cEncryptionIV = "a6xwdDinQVGrAVGG";

	public static byte[] EncryptAuto(byte[] encryptedData)
	{
		return EncryptManual(encryptedData, m_cEncryptionKey, m_cEncryptionIV);
	}

	public static byte[] EncryptManual(byte[] encryptedData, string encryptionKey, string iv)
	{
		byte[] result = null;

		using (AesManaged aesManaged = new AesManaged())
		{
			InitAesParameters(aesManaged, encryptionKey, iv);

			ICryptoTransform encryptor = aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV);

			using (MemoryStream encryptedStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write))
					cryptoStream.Write(encryptedData, 0, encryptedData.Length);
				result = encryptedStream.ToArray();
			}
		}

		return result;
	}


	public static byte[] UnencryptAuto(byte[] encryptedData)
	{
		return UnencryptManual(encryptedData, m_cEncryptionKey, m_cEncryptionIV);
	}

	public static byte[] UnencryptManual(byte[] encryptedData, string encryptionKey, string iv)
	{
		byte[] result = null;

		using (AesManaged aesManaged = new AesManaged())
		{
			InitAesParameters(aesManaged, encryptionKey, iv);

			ICryptoTransform decryptor = aesManaged.CreateDecryptor(aesManaged.Key, aesManaged.IV);

			using (MemoryStream encryptedStream = new MemoryStream(encryptedData))
			{
				using (MemoryStream unencryptedStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, decryptor, CryptoStreamMode.Read))
						cryptoStream.CopyTo(unencryptedStream);
					result = unencryptedStream.ToArray();
				}
			}
		}

		return result;
	}


	static void InitAesParameters(AesManaged aes, string key, string iv)
	{
		aes.KeySize = m_cKeySize;
		aes.BlockSize = m_cBlockSize;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;

		aes.Key = Encoding.UTF8.GetBytes(PaddingString(key, m_cKeySize / 8));
		aes.IV = Encoding.UTF8.GetBytes(PaddingString(iv, m_cBlockSize / 8));
	}

	static string PaddingString(string str, int length)
	{
		const char cPaddingCharacter = '.';

		if (str.Length < length)
		{
			string result = str;
			for (int i = 0, count = length - str.Length; i < count; ++i)
				result += cPaddingCharacter;
			return result;
		}
		else if (str.Length > length)
			return str.Substring(0, length);
		else
			return str;
	}
}
