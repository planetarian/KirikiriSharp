/*
 * TJS2 CSharp
 */

namespace Tjs2.Engine
{
	public interface EnumMembersCallback
	{
		/// <exception cref="VariantException"></exception>
		/// <exception cref="TJSException"></exception>
		bool Callback(string name, int flags, Variant value);
	}
}
