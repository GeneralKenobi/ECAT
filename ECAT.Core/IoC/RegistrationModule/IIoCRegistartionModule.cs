using Autofac;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for classes that wish to perform a custom registration with <see cref="IoC"/>. At start-up an instance of all
	/// classes that implement this interface is created using an <see cref="System.Activator"/> and
	/// <see cref="Register(ContainerBuilder)"/> method is called.
	/// </summary>
	public interface IIoCRegistartionModule
	{
		#region Public methods

		/// <summary>
		/// Method called on start-up, implementing classes should perform their registration with <paramref name="builder"/> inside it.
		/// </summary>
		/// <param name="builder"></param>
		void Register(ContainerBuilder builder);

		#endregion
	}
}