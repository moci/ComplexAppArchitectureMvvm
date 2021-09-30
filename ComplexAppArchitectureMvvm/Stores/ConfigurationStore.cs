using ComplexAppArchitectureMvvm.Domain;
using System;

namespace ComplexAppArchitectureMvvm.Stores
{
	public interface IConfigurationStore : IStoreReader<Configuration>, IStoreWriter<Configuration> { }

	public class ConfigurationStore : Store<Configuration>, IConfigurationStore
	{
		public ConfigurationStore(Configuration initialValue) : base(initialValue) { }

		public override void Write(Configuration value)
		{
			base.Write(value ?? throw new ArgumentNullException());
		}
	}
}
