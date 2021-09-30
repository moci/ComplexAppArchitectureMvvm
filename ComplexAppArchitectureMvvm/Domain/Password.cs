using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ComplexAppArchitectureMvvm.Domain
{
	public class Password : IEquatable<Password>
	{
		public static class Factory
		{
			public static Password FromClearText(string clearText)
			{
				return new Password(PasswordUtilities.Hash(clearText));
			}
			public static Password FromHash(string hash)
			{
				return new Password(hash);
			}

			public static Password FromSecureString(SecureString secureString)
			{
				var pw = new NetworkCredential(string.Empty, secureString ?? new SecureString()).Password;
				return FromClearText(pw);
			}
		}

		public static Password Empty { get; }
		public static bool IsNullOrEmpty(Password other) => other == null || other == Empty;

		private readonly string mHash;

		private Password(string hash)
		{
			if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentException("A hash must be provided", nameof(hash));
			mHash = hash;
		}

		#region Equals
		public override bool Equals(object obj)
		{
			return Equals(obj as Password);
		}
		public bool Equals(Password other)
		{
			return other != null && mHash == other.mHash;
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(mHash);
		}
		public static bool operator ==(Password left, Password right)
		{
			return EqualityComparer<Password>.Default.Equals(left, right);
		}
		public static bool operator !=(Password left, Password right)
		{
			return !(left == right);
		}
		#endregion
	}
	internal class PasswordUtilities
	{
		public static string Hash(string clearText)
		{
			clearText ??= string.Empty;

			using (var sha256 = SHA256.Create())
			{
				var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(clearText));
				var builder = new StringBuilder();

				for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));

				return builder.ToString();
			}
		}
	}
}
