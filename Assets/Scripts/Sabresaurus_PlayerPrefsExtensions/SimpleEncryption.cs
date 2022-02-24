using System;
using System.Security.Cryptography;
using System.Text;

namespace Sabresaurus.PlayerPrefsExtensions
{
	public static class SimpleEncryption
	{
		private static void SetupProvider()
		{
			SimpleEncryption.provider = new RijndaelManaged();
			SimpleEncryption.provider.Key = Encoding.ASCII.GetBytes(SimpleEncryption.key);
			SimpleEncryption.provider.Mode = CipherMode.ECB;
		}

		public static string EncryptString(string sourceString)
		{
			if (SimpleEncryption.provider == null)
			{
				SimpleEncryption.SetupProvider();
			}
			ICryptoTransform cryptoTransform = SimpleEncryption.provider.CreateEncryptor();
			byte[] bytes = Encoding.UTF8.GetBytes(sourceString);
			return Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length));
		}

		public static string DecryptString(string sourceString)
		{
			if (SimpleEncryption.provider == null)
			{
				SimpleEncryption.SetupProvider();
			}
			ICryptoTransform cryptoTransform = SimpleEncryption.provider.CreateDecryptor();
			byte[] array = Convert.FromBase64String(sourceString);
			byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
			return Encoding.UTF8.GetString(bytes);
		}

		public static string EncryptFloat(float value)
		{
			return SimpleEncryption.EncryptString(Convert.ToBase64String(BitConverter.GetBytes(value)));
		}

		public static string EncryptInt(int value)
		{
			return SimpleEncryption.EncryptString(Convert.ToBase64String(BitConverter.GetBytes(value)));
		}

		public static float DecryptFloat(string sourceString)
		{
			return BitConverter.ToSingle(Convert.FromBase64String(SimpleEncryption.DecryptString(sourceString)), 0);
		}

		public static int DecryptInt(string sourceString)
		{
			return BitConverter.ToInt32(Convert.FromBase64String(SimpleEncryption.DecryptString(sourceString)), 0);
		}

		private static string key = ":{j%6j?E:t#}G10mM%9hp5S=%}2,Y26C";

		private static RijndaelManaged provider = null;
	}
}
