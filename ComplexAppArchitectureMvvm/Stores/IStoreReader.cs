using System;

namespace ComplexAppArchitectureMvvm.Stores
{
	public interface IStoreReader<TDataType>
	{
		event EventHandler ValueChanged;

		TDataType Read();
	}
}
