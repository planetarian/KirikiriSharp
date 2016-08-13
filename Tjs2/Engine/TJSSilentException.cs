/*
 * TJS2 CSharp
 */

using System;

namespace Tjs2.Engine
{
	[System.Serializable]
	public class TjsSilentException : Exception
	{
		private const long serialVersionUID = 51839351639123183L;

		public TjsSilentException()
		{
		}

		public TjsSilentException(string msg) : base(msg)
		{
		}
	}
}
