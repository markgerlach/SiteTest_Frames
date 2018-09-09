using System; 
using System.IO; 
using System.Security.Cryptography; 

namespace mws1199.Model.Crypto
{
	public class EncryptDecrypt
	{
		/// <summary>
		/// Encrypt the byte array coming in.
		/// </summary>
		/// <param name="clearData">Byte Array</param>
		/// <param name="Key">Secret Key for the Symmetric Algorithm (32 bytes)</param>
		/// <param name="IV">Initialization Vector (16 bytes)</param>
		/// <returns>encrypted byte array</returns>
		public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV) 
		{ 
			// Create a MemoryStream to accept the encrypted bytes 
			MemoryStream ms = new MemoryStream(); 

			// Create a symmetric algorithm. 
			// We are going to use Rijndael because it is strong and available on all platforms. 	
			Rijndael alg = Rijndael.Create(); 

			// Now set the key and the IV.
			alg.Key = Key; 
			alg.IV = IV; 

			// Create a CryptoStream through which we are going to be pumping our data. 
			CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write); 

			// Write the data and make it do the encryption 
			cs.Write(clearData, 0, clearData.Length); 

			// Close the crypto stream
			cs.Close(); 

			byte[] encryptedData = ms.ToArray();
			return encryptedData; 
		} 

		/// <summary>
		/// Encrypt the incoming string using the password as salt.
		/// </summary>
		/// <param name="clearText">Unicode text to encrypt</param>
		/// <param name="Password">Password to salt the encryption with</param>
		/// <returns>encrypted string</returns>
		public static string Encrypt(string clearText, string Password) 
		{ 
			// First we need to turn the input string into a byte array. 
			byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText); 

			// Then, we need to turn the password into Key and IV 
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, 
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 

			byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));

			// Now we need to turn the resulting byte array into a string.
			return Convert.ToBase64String(encryptedData); 
		}
    
		/// <summary>
		/// Encrypt the byte array using the supplied password as Salt
		/// </summary>
		/// <param name="clearData">byte array of the encrypted text.</param>
		/// <param name="Password">the password used to salt the encryption</param>
		/// <returns>encrypted byte array</returns>
		public static byte[] Encrypt(byte[] clearData, string Password) 
		{ 
			// We need to turn the password into Key and IV. 
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, 
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 

			return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16)); 

		}

		/// <summary>
		/// Encrypt the supplied file paths using the password as salt.
		/// </summary>
		/// <param name="fileIn">Full filepath to encrypt</param>
		/// <param name="fileOut">Full filepath for the resulting encryption</param>
		/// <param name="Password">A password to salt the encryption with</param>
		public static void Encrypt(string fileIn, string fileOut, string Password) 
		{ 

			// First we are going to open the file streams 
			FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read); 
			FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write); 

			// Then we are going to derive a Key and an IV from the
			// Password and create an algorithm 
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, 
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 

			Rijndael alg = Rijndael.Create(); 
			alg.Key = pdb.GetBytes(32); 
			alg.IV = pdb.GetBytes(16); 

			// Now create a crypto stream through which we are going to be pumping data. 
			// Our fileOut is going to be receiving the encrypted bytes. 
			CryptoStream cs = new CryptoStream(fsOut, alg.CreateEncryptor(), CryptoStreamMode.Write); 

			// Now will will initialize a buffer and will be processing the input file in chunks. 
			// This is done to avoid reading the whole file (which can be huge) into memory. 
			int bufferLen = 4096; 
			byte[] buffer = new byte[bufferLen]; 
			int bytesRead; 

			do 
			{ 
				// read a chunk of data from the input file 
				bytesRead = fsIn.Read(buffer, 0, bufferLen);
				// encrypt it 
				cs.Write(buffer, 0, bytesRead); 
			} while(bytesRead != 0); 

			// close everything 
			// this will also close the unrelying fsOut stream
			cs.Close(); 
			fsIn.Close();     
		}

		/// <summary>
		/// Encrypt the supplied file paths with the supplied secret key and initalization vector
		/// </summary>
		/// <param name="fileIn">Full filepath to encrypt</param>
		/// <param name="fileOut">Full filepath for the resulting encryption</param>
		/// <param name="Key">byte array (32)</param>
		/// <param name="IV">byte array (16)</param>
		public static void Encrypt(string fileIn, string fileOut, byte[] Key, byte[] IV) 
		{ 

			// First we are going to open the file streams 
			FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read); 
			FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write); 		

			Rijndael alg = Rijndael.Create(); 
			alg.Key = Key;
			alg.IV = IV;

			// Now create a crypto stream through which we are going to be pumping data. 
			// Our fileOut is going to be receiving the encrypted bytes. 
			CryptoStream cs = new CryptoStream(fsOut, alg.CreateEncryptor(), CryptoStreamMode.Write); 

			// Now will will initialize a buffer and will be processing the input file in chunks. 
			// This is done to avoid reading the whole file (which can be huge) into memory. 
			int bufferLen = 4096; 
			byte[] buffer = new byte[bufferLen]; 
			int bytesRead; 

			do 
			{ 
				// read a chunk of data from the input file 
				bytesRead = fsIn.Read(buffer, 0, bufferLen); 

				// encrypt it
				cs.Write(buffer, 0, bytesRead); 
			} while(bytesRead != 0); 

			// close everything 

			// this will also close the unrelying fsOut stream
			cs.FlushFinalBlock();
			cs.Close(); 
			fsIn.Close();     
		}

		/// <summary>
		/// Decrypt the supplied byte array using the secret key and initalization vector.
		/// </summary>
		/// <param name="cipherData">Encrypted byte array</param>
		/// <param name="Key">Secret key (32 bytes)</param>
		/// <param name="IV">Initalization vector (16 bytes)</param>
		/// <returns>decrypted byte array</returns>
		public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV) 
		{ 
			// Create a MemoryStream that is going to accept the decrypted bytes 
			MemoryStream ms = new MemoryStream(); 

			// Create a symmetric algorithm.
			Rijndael alg = Rijndael.Create(); 

			// Now set the key and the IV. 
			alg.Key = Key; 
			alg.IV = IV; 

			// Create a CryptoStream through which we are going to be pumping our data. 
			CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write); 

			// Write the data and make it do the decryption 
			cs.Write(cipherData, 0, cipherData.Length); 

			// Close the crypto stream (or do FlushFinalBlock)
			cs.FlushFinalBlock();
			cs.Close(); 

			// Now get the decrypted data from the MemoryStream.
			byte[] decryptedData = ms.ToArray(); 
			return decryptedData; 
		}

		/// <summary>
		/// Decrypt the supplied string using the password as salt for the decryption.
		/// </summary>
		/// <param name="cipherText">Base64 Encrypted text</param>
		/// <param name="Password">Password used to encrypt the text</param>
		/// <returns>decrypted string</returns>
		public static string Decrypt(string cipherText, string Password) 
		{ 
			// First we need to turn the input string into a byte array. 
			// We presume that Base64 encoding was used 
			if (cipherText.Length % 4 == 0)
			{
				byte[] cipherBytes = Convert.FromBase64String(cipherText); 

				// Then, we need to turn the password into Key and IV 
				PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, 
					new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 

				// Now get the key/IV and do the decryption using the function that accepts byte arrays. 
				byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16)); 

				return System.Text.Encoding.Unicode.GetString(decryptedData); 
			}
			else
			{
				return cipherText;
			}
		}

		/// <summary>
		/// Decrypt the supplied byte array using the password as salt for the decryption.
		/// </summary>
		/// <param name="cipherData">Encrypted byte array</param>
		/// <param name="Password">Password used to encrypt the byte array</param>
		/// <returns>decrypted byte array</returns>
		public static byte[] Decrypt(byte[] cipherData, string Password) 
		{ 
			// We need to turn the password into Key and IV. 
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, 
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 

			// Now get the key/IV and do the Decryption using the function that accepts byte arrays.
			return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16)); 
		}

		/// <summary>
		/// Decrypts the supplied file to the specified file.
		/// </summary>
		/// <param name="fileIn">Encrypted file path</param>
		/// <param name="fileOut">Destination for the decryption.</param>
		/// <param name="Password">The password used to encrypt the file.</param>
		public static void Decrypt(string fileIn, string fileOut, string Password) 
		{ 
    
			// First we are going to open the file streams 
			FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read); 
			FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write); 
  
			// Then we are going to derive a Key and an IV from the Password and create an algorithm 
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, 
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 
		
			Rijndael alg = Rijndael.Create(); 

			alg.Key = pdb.GetBytes(32); 
			alg.IV = pdb.GetBytes(16); 

			// Now create a crypto stream through which we are going to be pumping data.
			// Our fileOut is going to be receiving the Decrypted bytes.
			CryptoStream cs = new CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write); 
  
			// Now will will initialize a buffer and will be processing the input file in chunks. 
			// This is done to avoid reading the whole file (which can be huge) into memory. 
			int bufferLen = 4096; 
			byte[] buffer = new byte[bufferLen]; 
			int bytesRead; 

			do 
			{ 
				// read a chunk of data from the input file 
				bytesRead = fsIn.Read(buffer, 0, bufferLen); 

				// Decrypt it 
				cs.Write(buffer, 0, bytesRead); 

			} while(bytesRead != 0); 

			// close everything 
			cs.FlushFinalBlock();
			cs.Close();
			fsIn.Close();     
		}

		/// <summary>
		/// Decrypts the supplied file to the specified file.
		/// </summary>
		/// <param name="fileIn">Encrypted file path</param>
		/// <param name="Key">Secret Key (32 bytes)</param>
		/// <param name="IV">Initialization Vector (16 bytes)</param>
		public static void Decrypt(string fileIn, string fileOut, byte[] Key, byte[] IV) 
		{     
			// First we are going to open the file streams 
			FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read); 
			FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write); 	
		
			Rijndael alg = Rijndael.Create(); 

			alg.Key = Key;
			alg.IV = IV;

			// Now create a crypto stream through which we are going to be pumping data.
			// Our fileOut is going to be receiving the Decrypted bytes.
			CryptoStream cs = new CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write); 
  
			// Now will will initialize a buffer and will be processing the input file in chunks. 
			// This is done to avoid reading the whole file (which can be huge) into memory. 
			int bufferLen = 4096; 
			byte[] buffer = new byte[bufferLen]; 
			int bytesRead; 

			do 
			{ 
				// read a chunk of data from the input file 
				bytesRead = fsIn.Read(buffer, 0, bufferLen); 

				// Decrypt it 
				cs.Write(buffer, 0, bytesRead); 

			} while(bytesRead != 0); 

			// close everything 
			cs.Close();
			fsIn.Close();     
		}
	}
}
