using System;
using System.Mvvm;

namespace ComplexAppArchitectureMvvm.ViewModels
{
	public class ViewModelBase : BindableBase, IDisposable
	{
		public virtual void Dispose() { }
	}
}
