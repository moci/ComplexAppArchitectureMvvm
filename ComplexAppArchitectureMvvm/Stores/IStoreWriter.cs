namespace ComplexAppArchitectureMvvm.Stores
{
	public interface IStoreWriter<TDataType>
	{
		void Write(TDataType value);
	}
}
