/*
 * TJS2 CSharp
 */

namespace Tjs2.Engine
{
	public interface StorageInterface
	{
		/// <exception cref="TJSException"></exception>
		string ReadText(string name, string modestr);

		/// <exception cref="TJSException"></exception>
		TextWriteStreamInterface CreateTextWriteStream(string name, string modestr);

		/// <exception cref="TJSException"></exception>
		BinaryStream CreateBinaryWriteStream(string name);
	}
}
