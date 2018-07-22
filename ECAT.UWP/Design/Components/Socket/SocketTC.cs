﻿using ECAT.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ECAT.UWP
{
	/// <summary>
	/// Templated control for sockets
	/// </summary>
	public sealed class SocketTC : Control
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public SocketTC()
        {
			this.DefaultStyleKey = typeof(SocketTC);
        }

		#endregion	

		#region PartialNode Dependency Property

		/// <summary>
		/// ITerminal assigned to this <see cref="SocketTC"/>
		/// </summary>
		public ITerminal Terminal
		{
			get => (ITerminal)GetValue(TerminalProperty);
			set => SetValue(TerminalProperty, value);
		}

		/// <summary>
		/// Backing store for <see cref="Terminal"/>
		/// </summary>
		public static readonly DependencyProperty TerminalProperty =
			DependencyProperty.Register(nameof(Terminal), typeof(ITerminal),
			typeof(SocketTC), new PropertyMetadata(default(ITerminal)));

		#endregion
	}
}