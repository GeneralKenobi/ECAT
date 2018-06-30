using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAT.Core
{
	public static class IoC
	{
		#region Private Properties

		/// <summary>
		/// The standard IoC container
		/// </summary>
		private static ContainerBuilder Container { get; set; } = new ContainerBuilder();

		/// <summary>
		/// The IoC container for 1 object of each type
		/// </summary>
		private static ContainerBuilder SingletonContainer { get; set; } = new ContainerBuilder();

		#endregion

		#region Singleton Container methods

		/// <summary>
		/// Adds a singleton object to the IoC container	
		/// </summary>
		/// <typeparam name="T">Type of the item to add</typeparam>
		/// <param name="item">Object to add</param>
		public static void AddSingleton<T>(T item) where T : class =>
			// Add either the given item, or a new T if item wasn't passed
			SingletonContainer.Properties.Add(typeof(T).FullName, item);

		/// <summary>
		/// Retrieves the singleton object stored in the IoC container. Returns true and assings the item to result
		/// if it was found, otherwise returns false and assigns default(T) to result
		/// </summary>
		/// <typeparam name="T">Type of the item to retrieve</typeparam>
		/// <param name="result">Variable to assign the result to</param>
		/// <returns></returns>
		public static bool GetSingleton<T>(out T result) where T : class => GetHelper<T>(SingletonContainer, typeof(T).FullName, out result);

		/// <summary>
		/// Retrieves the singleton object stored in the IoC container and returns it.
		/// If the item wasn't found, throws an exception
		/// </summary>
		/// <typeparam name="T">Type of the item to retrieve</typeparam>
		/// <param name="result">Variable to assign the result to</param>
		/// <returns></returns>
		public static T GetSingleton<T>() where T : class
		{
			if(GetHelper(SingletonContainer, typeof(T).FullName, out T result))
			{
				return result;
			}
			else
			{
				throw new Exception("Couldn't find item for the given type");
			}
		}

		/// <summary>
		/// Removes the singleton object stored in the IoC container
		/// Returns true on success, false otherwise
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool RemoveSingleton<T>() where T : class => SingletonContainer.Properties.Remove(typeof(T).FullName);

		#endregion

		#region Container methods

		/// <summary>
		/// Adds an object to the IoC container, throws exception if the operation couldn't succeed
		/// </summary>
		/// <typeparam name="T">Type of the item to add</typeparam>
		/// <param name="item">Object to add</param>
		/// <param name="key">Key to assign to this item</param>
		public static void Add<T>(T item, string key) where T : class =>
			// Add either the gi^ven item, or a new T if item wasn't passed
			Container.Properties.Add(key, item);

		/// <summary>
		/// Retrieves the object stored in the IoC container with the given key. Returns true on success and assigns to result.
		/// Returns false if no item was found and assigns default(T) to result.
		/// Throws an exception if the item was found but it wasn't of type T
		/// </summary>
		/// <typeparam name="T">Type of the item to retrieve</typeparam>
		/// <param name="key">Key assigned to the item</param>
		/// <param name="result">Variable to assign the result to</param>
		/// <returns></returns>
		public static bool Get<T>(string key, out T result) where T : class => GetHelper<T>(Container, key, out result);

		/// <summary>
		/// Retrieves the object stored in the IoC container with the given key and returns it.
		/// Throws an exception if the item wasn't found or it was found but it wasn't of type T
		/// </summary>
		/// <typeparam name="T">Type of the item to retrieve</typeparam>
		/// <param name="key">Key assigned to the item</param>
		/// <param name="result">Variable to assign the result to</param>
		/// <returns></returns>
		public static T Get<T>(string key) where T : class
		{
			if(GetHelper(Container, key, out T result))
			{
				return result;
			}
			else
			{
				throw new Exception("Couldn't find item with the given key: " + key);
			}
		}

		/// <summary>
		/// Retrieves the object stored in the IoC container with the given key without casting it, returns true
		/// and assigns the item to the result on success. Assigns null to result and returns false if the item wasn't found
		/// </summary>
		/// <param name="key">Key assigned to the item</param>
		/// <param name="result">Variable to assign the result to</param>
		/// <returns></returns>
		public static bool Get(string key, out object result) => GetHelper<object>(Container, key, out result);

		/// <summary>
		/// Retrieves the object stored in the IoC container with the given key without casting it and returns it.
		/// If the item wasn't found, throws an exception
		/// </summary>
		/// <param name="key">Key assigned to the item</param>
		/// <param name="result">Variable to assign the result to</param>
		/// <returns></returns>
		public static object Get(string key)
		{
			if (GetHelper<object>(Container, key, out object result))
			{
				return result;
			}
			else
			{
				throw new Exception("Couldn't find item with the given key: " + key);
			}
		}

		/// <summary>
		/// Removes the object stored in the IoC container under the given key
		/// Returns true on success, false otherwise
		/// </summary>
		/// <param name="key">Key of the object to remove</param>
		/// <returns></returns>
		public static bool Remove(string key) => Container.Properties.Remove(key);

		#endregion

		#region Private Helpers

		/// <summary>
		/// Retrieves item with the given key from the given IoC container.
		/// If the item couldn't be found, throws an error
		/// </summary>
		/// <typeparam name="T">Type of the item to retrieve</typeparam>
		/// <param name="container">Container to retrievei it from</param>
		/// <param name="key">Key of the item</param>
		/// <returns></returns>
		private static bool GetHelper<T>(ContainerBuilder container, string key, out T result)
			where T : class
		{
			// Try to get the object out of the Container
			object item = null;
			container.Properties.TryGetValue(key, out item);

			// If we didn't find it
			if (item == null)
			{
				result = default(T);
				return false;
			}

			// Cast it to the deisred type
			T castedItem = item as T;

			// If it's different type
			result = castedItem ?? throw new Exception("The specified type didn't match the type of the item" +
					" stored in the container with the given key");

			// Return true
			return true;
		}

		#endregion
	}
}
