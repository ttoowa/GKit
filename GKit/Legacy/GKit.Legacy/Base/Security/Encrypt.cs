﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
			public static string ComputeMD5(string text) {
				MD5 cryptor = MD5.Create();
				byte[] hash = cryptor.ComputeHash(Encoding.UTF8.GetBytes(text));
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hash.Length; ++i) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString();
			}
			public static string SHA256(string text) {
				SHA256Managed cryptor = new SHA256Managed();
				StringBuilder sb = new StringBuilder();
				byte[] textData = Encoding.UTF8.GetBytes(text);
				byte[] hash = cryptor.ComputeHash(textData, 0, textData.Length);
				for (int i = 0; i < hash.Length; ++i) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString();
			}
		}
		//양방향 암호화
		public static class DuplexHash {
			public static string AES256Encrypt(string text, string password) {
				RijndaelManaged rijndaelCipher = new RijndaelManaged();
				byte[] plainText = Encoding.UTF8.GetBytes(text);
				byte[] salt = Encoding.UTF8.GetBytes(password.Length.ToString());
				PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
				ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
				byte[] encryptedData;
				using (MemoryStream memoryStream = new MemoryStream()) {
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
						cryptoStream.Write(plainText, 0, plainText.Length);
						cryptoStream.FlushFinalBlock();
						encryptedData = memoryStream.ToArray();
					}
				}
				return Convert.ToBase64String(encryptedData);
			}
			public static string AES256Decrypt(string encryptedText, string password) {
				RijndaelManaged rijndaelCipher = new RijndaelManaged();
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
		}
	}
}