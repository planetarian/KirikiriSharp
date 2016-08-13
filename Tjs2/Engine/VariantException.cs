/*
 * TJS2 CSharp
 */

namespace Tjs2.Engine
{
	[System.Serializable]
	public class VariantException : TJSException
	{
		private const long serialVersionUID = -3605064460238917638L;

		public VariantException()
		{
		}

		public VariantException(string msg) : base(msg)
		{
		}
	}
}
