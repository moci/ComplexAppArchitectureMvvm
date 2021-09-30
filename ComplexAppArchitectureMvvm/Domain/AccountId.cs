using System;
using System.Collections.Generic;

namespace ComplexAppArchitectureMvvm.Domain
{
	public class AccountId : IEquatable<AccountId>
	{
		public static class Factory
		{
			public static AccountId Create(string id)
			{
				return new AccountId(id ?? string.Empty);
			}
		}

		public static AccountId Empty { get; } = Factory.Create(string.Empty);
		public static bool IsNullOrEmpty(AccountId other) => other == null || other == Empty;

		private readonly string mId;

		private AccountId(string id)
		{
			mId = id;
		}

		public override string ToString() => mId;

		#region Equals
		public override bool Equals(object obj)
		{
			return Equals(obj as AccountId);
		}
		public bool Equals(AccountId other)
		{
			return other != null && mId == other.mId;
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(mId);
		}
		public static bool operator ==(AccountId left, AccountId right)
		{
			return EqualityComparer<AccountId>.Default.Equals(left, right);
		}
		public static bool operator !=(AccountId left, AccountId right)
		{
			return !(left == right);
		}
		#endregion
	}
}
