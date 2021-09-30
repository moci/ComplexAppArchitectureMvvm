using System;

namespace ComplexAppArchitectureMvvm.Stores
{
	public abstract class Store<TDataType> : IStoreReader<TDataType>, IStoreWriter<TDataType>
	{
		private TDataType mValue;

		protected Store(TDataType initialValue)
		{
			Write(initialValue);
		}

		public event EventHandler ValueChanged = delegate { };

		public TDataType Read()
		{
			return mValue;
		}

		public virtual void Write(TDataType value)
		{
			mValue = value;
			ValueChanged(this, EventArgs.Empty);
		}
	}
}
