/*
 * TJS2 CSharp
 */

using System;

namespace Tjs2.Engine
{
	[System.Serializable]
	public class TjsException : Exception
	{
		private const long serialVersionUID = 1942890050230766470L;

		public TjsException()
		{
		}

		public TjsException(string msg) : base(msg)
		{
		}
	}
}
