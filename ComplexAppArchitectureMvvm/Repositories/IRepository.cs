using System.Collections.Generic;

namespace ComplexAppArchitectureMvvm.Repositories
{
	public interface IRepository<TDataType, TIdType>
	{
		IEnumerable<TDataType> GetAll();
		TDataType Get(TIdType id);
		void Remove(TIdType id);
		void Set(TDataType value);
	}
}
