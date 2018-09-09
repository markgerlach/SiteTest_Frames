using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mws1199.Model
{
    public class SiteMaster
    {
		private static readonly string AesIV256 = @"!X#EDC4RQAZ2WSFV";
		private static readonly string AesKey256 = @"5T<5TGB&YHN77UJM(IUJGB&YHNKM(IK<";

		private static string Encrypt256(string text)
		{
			// AesCryptoServiceProvider
			AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
			aes.BlockSize = 128;
			aes.KeySize = 256;
			aes.IV = Encoding.UTF8.GetBytes(AesIV256);
			aes.Key = Encoding.UTF8.GetBytes(AesKey256);
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			// Convert string to byte array
			byte[] src = Encoding.Unicode.GetBytes(text);

			// encryption
			using (ICryptoTransform encrypt = aes.CreateEncryptor())
			{
				byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

				// Convert byte array to Base64 strings
				return Convert.ToBase64String(dest);
			}
		}

		/// <summary>
		/// AES decryption
		/// </summary>
		private static string Decrypt256(string text)
		{
			// AesCryptoServiceProvider
			AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
			aes.BlockSize = 128;
			aes.KeySize = 256;
			aes.IV = Encoding.UTF8.GetBytes(AesIV256);
			aes.Key = Encoding.UTF8.GetBytes(AesKey256);
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			// Convert Base64 strings to byte array
			byte[] src = System.Convert.FromBase64String(text);

			// decryption
			using (ICryptoTransform decrypt = aes.CreateDecryptor())
			{
				byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
				return Encoding.Unicode.GetString(dest);
			}
		}

		/// <summary>
		/// Get the site HH name for the day
		/// </summary>
		/// <returns>The HH name</returns>
		public static string GetSiteHHName()
		{
			string hhName = DateTime.Now.ToString("yyyyMMdd");
			hhName = Encrypt256(hhName);
			if (hhName.EndsWith("=")) { hhName = hhName.Substring(0, hhName.Length - 1); }
			return hhName;
		}

		/// <summary>
		/// Get whether or not the site HH name is valid
		/// </summary>
		/// <param name="hhName">The Host Header name to check</param>
		/// <returns>True if valid, otherwise, false</returns>
		public static bool SiteHHNameValid(string hhName)
		{
			bool rtv = false;

			string targetName = GetSiteHHName();
			rtv = (targetName == hhName);

			//string targetName = GetSiteHHName();
			//string passedName = string.Empty;
			//try
			//{
			//	if (!hhName.EndsWith("=")) { hhName += "="; }
			//	passedName = Decrypt256(hhName);
			//}
			//catch (Exception ex)
			//{
			//}
			//rtv = (targetName == passedName);

			return rtv;			// Return the value
		}
    }
}
