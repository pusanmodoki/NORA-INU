using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class DatFileEditor
{
	static readonly string m_cExtension = "dat";

	public static bool IsExistsFile(string filePath, string fileName)
	{
		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";

		return File.Exists(fullFilePath);
	}

	public static void SaveString(string filePath, string fileName, List<List<string>> data)
	{
		if (fileName == null || fileName.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		if (filePath == null || filePath.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		if (data == null)
			throw new System.ArgumentNullException("data", "Invalid data. (null)");

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		string linkageData = "";
		byte[] saveData = null;

		for(int i = 0, count = data.Count; i < count; ++i)
		{
			for (int k = 0, count1 = data[i].Count; k < count1; ++k)
				linkageData += data[i][k] + ",";
			if (i < count - 1)
				linkageData += "\n";
		}

		saveData = Encoding.UTF8.GetBytes(linkageData);
		saveData = ByteCompressor.Compress(saveData);
		saveData = ByteEncryptor.EncryptAuto(saveData);

		if (!Directory.Exists(filePath))
			Directory.CreateDirectory(filePath);

		using (FileStream fileStream = File.Create(fullFilePath))
			fileStream.Write(saveData, 0, saveData.Length);
	}

	public static void SaveString(string filePath, string fileName, List<SerializePackageString> data)
	{
		if (fileName == null || fileName.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		if (filePath == null || filePath.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		if (data == null)
			throw new System.ArgumentNullException("data", "Invalid data. (null)");

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		string linkageData = "";
		byte[] saveData = null;

		for (int i = 0, count = data.Count; i < count; ++i)
		{
			for (int k = 0, count1 = data[i].list.Count; k < count1; ++k)
				linkageData += data[i][k] + ",";
			if (i < count - 1)
				linkageData += "\n";
		}

		saveData = Encoding.UTF8.GetBytes(linkageData);
		saveData = ByteCompressor.Compress(saveData);
		saveData = ByteEncryptor.EncryptAuto(saveData);

		if (!Directory.Exists(filePath))
			Directory.CreateDirectory(filePath);

		using (FileStream fileStream = File.Create(fullFilePath))
			fileStream.Write(saveData, 0, saveData.Length);
	}
	public static void SaveString(string filePath, string fileName, List<string> data)
	{
		if (fileName == null || fileName.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		if (filePath == null || filePath.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		if (data == null)
			throw new System.ArgumentNullException("data", "Invalid data. (null)");

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		string linkageData = "";
		byte[] saveData = null;

		for (int i = 0, count = data.Count; i < count; ++i)
			linkageData += data[i] + ",";

		saveData = Encoding.UTF8.GetBytes(linkageData);
		saveData = ByteCompressor.Compress(saveData);
		saveData = ByteEncryptor.EncryptAuto(saveData);

		if (!Directory.Exists(filePath))
			Directory.CreateDirectory(filePath);

		using (FileStream fileStream = File.Create(fullFilePath))
			fileStream.Write(saveData, 0, saveData.Length);
	}
	public static void SaveString(string filePath, string fileName, string data)
	{
		if (fileName == null || fileName.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		if (filePath == null || filePath.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		if (data == null)
			throw new System.ArgumentNullException("data", "Invalid data. (null)");

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		byte[] saveData = null;

		saveData = Encoding.UTF8.GetBytes(data);
		saveData = ByteCompressor.Compress(saveData);
		saveData = ByteEncryptor.EncryptAuto(saveData);

		if (!Directory.Exists(filePath))
			Directory.CreateDirectory(filePath);

		using (FileStream fileStream = File.Create(fullFilePath))
			fileStream.Write(saveData, 0, saveData.Length);
	}


	public static void LoadString(string filePath, string fileName, out List<List<string>> data)
	{
		if (fileName == null || fileName.Length == 0)
		{
			data =null;
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		}
		if (filePath == null || filePath.Length == 0)
		{
			data = null;
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		}

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		string convertData = "";
		byte[] readData = null;

		if (!File.Exists(fullFilePath))
		{
			data =null;
			throw new System.SystemException("File not found. file path: " + fullFilePath);
		}
		using (FileStream fileStream = File.OpenRead(fullFilePath))
		{
			readData = new byte[fileStream.Length];
			fileStream.Read(readData, 0, readData.Length);
		}

		readData = ByteEncryptor.UnencryptAuto(readData);
		readData = ByteCompressor.Uncompress(readData);

		convertData = Encoding.UTF8.GetString(readData);

		SplitData(convertData, out data);
	}
	public static void LoadString(string filePath, string fileName, out List<SerializePackageString> data)
	{
		if (fileName == null || fileName.Length == 0)
		{
			data = null;
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		}
		if (filePath == null || filePath.Length == 0)
		{
			data = null;
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		}

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		string convertData = "";
		byte[] readData = null;

		if (!File.Exists(fullFilePath))
		{
			data = null;
			throw new System.SystemException("File not found. file path: " + fullFilePath);
		}
		using (FileStream fileStream = File.OpenRead(fullFilePath))
		{
			readData = new byte[fileStream.Length];
			fileStream.Read(readData, 0, readData.Length);
		}

		readData = ByteEncryptor.UnencryptAuto(readData);
		readData = ByteCompressor.Uncompress(readData);

		convertData = Encoding.UTF8.GetString(readData);

		SplitData(convertData, out data);
	}
	public static void LoadString(string filePath, string fileName, out List<string> data)
	{
		if (fileName == null || fileName.Length == 0)
		{
			data = null;
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		}
		if (filePath == null || filePath.Length == 0)
		{
			data = null;
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		}

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		string convertData = "";
		byte[] readData = null;

		if (!File.Exists(fullFilePath))
		{
			data = null;
			throw new System.SystemException("File not found. file path: " + fullFilePath);
		}
		using (FileStream fileStream = File.OpenRead(fullFilePath))
		{
			readData = new byte[fileStream.Length];
			fileStream.Read(readData, 0, readData.Length);
		}

		readData = ByteEncryptor.UnencryptAuto(readData);
		readData = ByteCompressor.Uncompress(readData);

		convertData = Encoding.UTF8.GetString(readData);

		SplitData(convertData, out data);
	}
	public static void LoadString(string filePath, string fileName, out string data)
	{
		if (fileName == null || fileName.Length == 0)
		{
			data = null;
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		}
		if (filePath == null || filePath.Length == 0)
		{
			data = null;
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		}

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		byte[] readData = null;

		if (!File.Exists(fullFilePath))
		{
			data = null;
			throw new System.SystemException("File not found. file path: " + fullFilePath);
		}
		using (FileStream fileStream = File.OpenRead(fullFilePath))
		{
			readData = new byte[fileStream.Length];
			fileStream.Read(readData, 0, readData.Length);
		}

		readData = ByteEncryptor.UnencryptAuto(readData);
		readData = ByteCompressor.Uncompress(readData);

		data = Encoding.UTF8.GetString(readData);
	}





	public static void SaveObject<DataType>(string filePath, string fileName, ref DataType data)
	{
		if (fileName == null || fileName.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		if (filePath == null || filePath.Length == 0)
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		if (data == null)
			throw new System.ArgumentNullException("data", "Invalid data. (null)");

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		string convertData = "";
		byte[] saveData = null;

		convertData = JsonUtility.ToJson(data);

		saveData = Encoding.UTF8.GetBytes(convertData);
		saveData = ByteCompressor.Compress(saveData);
		saveData = ByteEncryptor.EncryptAuto(saveData);

		if (!Directory.Exists(filePath))
			Directory.CreateDirectory(filePath);

		using (FileStream fileStream = File.Create(fullFilePath))
			fileStream.Write(saveData, 0, saveData.Length);
	}
	public static void LoadObject<DataType>(string filePath, string fileName, out DataType data)
	{
		if (fileName == null || fileName.Length == 0)
		{
			data = default;
			throw new System.ArgumentNullException("fileName", "Invalid fileName. (null)");
		}
		if (filePath == null || filePath.Length == 0)
		{
			data = default;
			throw new System.ArgumentNullException("fileName", "Invalid filePath. (null)");
		}

		string fullFilePath = $"{filePath}/{fileName}.{m_cExtension}";
		string convertData = "";
		byte[] readData = null;

		if (!File.Exists(fullFilePath))
		{
			data = default;
			throw new System.SystemException("File not found. file path: " + fullFilePath);
		}
		using (FileStream fileStream = File.OpenRead(fullFilePath))
		{
			readData = new byte[fileStream.Length];
			fileStream.Read(readData, 0, readData.Length);
		}

		readData = ByteEncryptor.UnencryptAuto(readData);
		readData = ByteCompressor.Uncompress(readData);

		convertData = Encoding.UTF8.GetString(readData);
		data = JsonUtility.FromJson<DataType>(convertData);
	}

	static void SplitData(string data, out List<List<string>> read)
	{
		if (data == "")
		{
			read = new List<List<string>>();
			return;
		}

		// StringSplitOption
		System.StringSplitOptions option = System.StringSplitOptions.None;
		// 行に分ける
		string[] lines = data.Split(new char[] { '\r', '\n' }, option);
		// 区分けする文字
		char[] spliter = new char[1] { ',' };


		// 行データを切り分けて,2次元配列へ変換する
		read = new List<List<string>>();
		for (int i = 0, length = lines.Length; i < length; ++i)
		{
			string[] lineToSplit = lines[i].Split(spliter, option);

			read.Add(new List<string>());

			for (int k = 0, length1 = lineToSplit.Length; k < length1; ++k)
				read[i].Add(lineToSplit[k]);

			if (read[i].Count > 0)
				read[i].RemoveAt(read[i].Count - 1);
		}
	}
	static void SplitData(string data, out List<SerializePackageString> read)
	{
		if (data == "")
		{
			read = new List<SerializePackageString>();
			return;
		}

		// StringSplitOption
		System.StringSplitOptions option = System.StringSplitOptions.None;
		// 行に分ける
		string[] lines = data.Split(new char[] { '\r', '\n' }, option);
		// 区分けする文字
		char[] spliter = new char[1] { ',' };


		// 行データを切り分けて,2次元配列へ変換する
		read = new List<SerializePackageString>();
		for (int i = 0, length = lines.Length; i < length; ++i)
		{
			string[] lineToSplit = lines[i].Split(spliter, option);
			read.Add(new SerializePackageString());

			for (int k = 0, length1 = lineToSplit.Length; k < length1; ++k)
				read[i].list.Add(lineToSplit[k]);

			if (read[i].list.Count > 0)
				read[i].list.RemoveAt(read[i].list.Count - 1);
		}
	}
	static void SplitData(string data, out List<string> read)
	{
		if (data == "")
		{
			read = new List<string>();
			return;
		}

		// StringSplitOption
		System.StringSplitOptions option = System.StringSplitOptions.None;
		// 区分けする文字
		char[] spliter = new char[1] { ',' };

		string[] lineToSplit = data.Split(spliter, option);

		// 行データを切り分けて,2次元配列へ変換する
		read = new List<string>();
		for (int k = 0, length = lineToSplit.Length; k < length; ++k)
			read.Add(lineToSplit[k]);
		
		read.RemoveAt(read.Count - 1);
	}
}
