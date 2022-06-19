using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
#if OnUnity
using GKitForUnity.Security.Algorithm;
#elif OnWPF
using GKitForWPF.Security.Algorithm;
#else
using GKit.Security.Algorithm;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Security {
	public static class Encrypt {
		static System.Random random = new System.Random();

		//해쉬생성
		public static string GenerateHash(int length) {
			const string chars = "abcdefghijklmnopqrstuvwxyz0123456789_";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		//단방향 암호화
		public static class SimplexHash {
			public static string ComputeMD4(string text) {
				byte[] hash = ComputeMD4Binary(Encoding.UTF8.GetBytes(text));
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hash.Length; ++i) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString();
			}
			public static byte[] ComputeMD4Binary(byte[] data) {
				using (HashAlgorithm cryptor = MD4.Create()) {
					return cryptor.ComputeHash(data);
				}
			}

			public static string ComputeMD5(string text) {
				byte[] hash = ComputeMD5Binary(Encoding.UTF8.GetBytes(text));
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hash.Length; ++i) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString();
			}
			public static byte[] ComputeMD5Binary(byte[] data) {
				using (MD5 cryptor = MD5.Create()) {
					return cryptor.ComputeHash(data);
				}
			}
			public static string SHA256(string text) {
				StringBuilder sb = new StringBuilder();
				byte[] hash = SHA256Binary(Encoding.UTF8.GetBytes(text));
				for (int i = 0; i < hash.Length; ++i) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString();
			}
			public static byte[] SHA256Binary(byte[] data) {
				using (SHA256 cryptor = System.Security.Cryptography.SHA256.Create()) {
					return cryptor.ComputeHash(data);
				}
			}
		}
		//양방향 암호화
		public static class DuplexHash {
			public static string AES256Encrypt(string text, string password) {
				return Convert.ToBase64String(AES256EncryptBinary(Encoding.UTF8.GetBytes(text), password));
			}
			public static byte[] AES256EncryptBinary(byte[] data, string password) {
				RijndaelManaged rijndaelCipher = new RijndaelManaged();
				byte[] salt = Encoding.UTF8.GetBytes(password.Length.ToString());
				PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
				ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
				byte[] encryptedData;
				using (MemoryStream memoryStream = new MemoryStream()) {
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
						cryptoStream.Write(data, 0, data.Length);
						cryptoStream.FlushFinalBlock();
						encryptedData = memoryStream.ToArray();
					}
				}
				return encryptedData;
			}

			public static string AES256Decrypt(string encryptedText, string password) {
				Aes rijndaelCipher = Aes.Create();
				byte[] encryptedData = Convert.FromBase64String(encryptedText);
				byte[] salt = Encoding.UTF8.GetBytes(password.Length.ToString());
				PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
				ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
				byte[] originData;
				int decryptedCount;
				using (MemoryStream memoryStream = new MemoryStream(encryptedData)) {
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
						originData = new byte[encryptedData.Length];
						decryptedCount = cryptoStream.Read(originData, 0, originData.Length);
					}
				}
				return Encoding.UTF8.GetString(originData, 0, decryptedCount);
			}
			public static byte[] AES256DecryptBinary(byte[] encryptedData, string password) {
				Aes aes = Aes.Create();
				byte[] salt = Encoding.UTF8.GetBytes(password.Length.ToString());
				PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
				ICryptoTransform decryptor = aes.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
				byte[] originData;
				int decryptedCount;
				using (MemoryStream memoryStream = new MemoryStream(encryptedData)) {
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
						originData = new byte[encryptedData.Length];
						decryptedCount = cryptoStream.Read(originData, 0, originData.Length);
					}
				}
				return originData;
			}
		}
	}
}