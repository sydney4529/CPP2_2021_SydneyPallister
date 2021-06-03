using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;


public class LoadSaveManager : MonoBehaviour {

	// Save game data
    [XmlRoot("GameData")]
	public class GameStateData
	{
		public struct DataTransform
		{
			public float posX;
			public float posY;
			public float posZ;
			public float rotX;
			public float rotY;
			public float rotZ;
			public float scaleX;
			public float scaleY;
			public float scaleZ;
		}
			
		// Data for enemy
		public class DataEnemy
		{
			//Enemy Transform Data
			public DataTransform posRotScale;

			//Enemy ID
			public string enemyID;

			//Health
			public int health;

			//alive
			public bool saveAlive;
		}
			
		// Data for player
		public class DataPlayer
		{
			//Transform Data
			public DataTransform posRotScale;

			//camera transfror
			public DataTransform camPosRotScale;

			//Collected cash
			public float collectedScore;

			//Has Collected Gun 01?
			//public bool collectedWeapon;

			//Health
			public int health;

		}

		// Instance variables
		public List<DataEnemy> enemies = new List<DataEnemy>();
		public List<DataEnemy> enemies1 = new List<DataEnemy>();

		public DataPlayer player = new DataPlayer();
	}

	// Game data to save/load
	public GameStateData gameState = new GameStateData();

	// Saves game data to XML file
	public void SaveOld(string fileName = "GameData.xml")
	{
		// Clear existing enemy data
		//gameState.enemies.Clear();

        // Save game data
        XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
		FileStream stream = new FileStream(fileName, FileMode.Create);
		serializer.Serialize(stream, gameState);
		stream.Flush();
		stream.Dispose();
		stream.Close();
	}

	// Load game data from XML file
	public void LoadOld(string fileName = "GameData.xml")
	{

		//aes.Mode = CipherMode.CBC;
		//aes.Padding = PaddingMode.PKCS7;

		XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
		FileStream stream = new FileStream(fileName, FileMode.Open);
		gameState = serializer.Deserialize(stream) as GameStateData;
		stream.Flush();
		stream.Dispose();
		stream.Close();
	}

	public void Save(string fileName = "GameData.xml")
	{
		// Clear existing enemy data
		//gameState.enemies.Clear();
		//gameState.enemies1.Clear();

		//Clear existing player data
		//gameState.player = null;


		XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
		FileStream stream = new FileStream(fileName, FileMode.Create);

		using (Aes aes = Aes.Create())
		{

			byte[] key =
			{
				0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
				0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
			};
			aes.Key = key;

			byte[] iv = aes.IV;
			stream.Write(iv, 0, iv.Length);

			// Save game data
			using (CryptoStream cryptoStream = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
			{
				using (StreamWriter encryptWriter = new StreamWriter(cryptoStream))
				{
					//encryptWriter.WriteLine(stream, gameState);
					serializer.Serialize(cryptoStream, gameState);

				}
				
			}

		}
		//stream.Flush();
		//stream.Dispose();
		//stream.Close();
	}

	// Load game data from XML file
	public void Load(string fileName = "GameData.xml")
	{

		//aes.Mode = CipherMode.CBC;
		//aes.Padding = PaddingMode.PKCS7;
		XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
		FileStream stream = new FileStream(fileName, FileMode.Open);

		using (Aes aes = Aes.Create())
		{
			byte[] iv = new byte[aes.IV.Length];
			int numBytesToRead = aes.IV.Length;
			int numBytesRead = 0;
			while (numBytesToRead > 0)
			{
				int n = stream.Read(iv, numBytesRead, numBytesToRead);
				if (n == 0) break;

				numBytesRead += n;
				numBytesToRead -= n;
			}

			byte[] key =
			{
				0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
				0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
			};

			using (CryptoStream cryptoStream = new CryptoStream(stream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
			{
				using (StreamReader decryptReader = new StreamReader(cryptoStream))
				{
					gameState = serializer.Deserialize(cryptoStream) as GameStateData;
				}
				
			}
		}

		//stream.Flush();
		//stream.Dispose();
		//stream.Close();
	}
}