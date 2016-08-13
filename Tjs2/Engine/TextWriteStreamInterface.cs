/*
 * TJS2 CSharp
 */

namespace Tjs2.Engine
{
	public interface TextWriteStreamInterface
	{
		/// <exception cref="TJSException"></exception>
		void Write(string val);

		/// <exception cref="TJSException"></exception>
		void Destruct();
	}
}
