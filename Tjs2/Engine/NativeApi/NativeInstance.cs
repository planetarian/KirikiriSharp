/*
 * TJS2 CSharp
 */

namespace Tjs2.Engine.NativeApi
{
	public interface NativeInstance
	{
		// TJS constructor
		/// <exception cref="VariantException"></exception>
		/// <exception cref="TJSException"></exception>
		int Construct(Variant[] param, Dispatch2 tjsObj);

		// called before destruction
		/// <exception cref="VariantException"></exception>
		/// <exception cref="TJSException"></exception>
		void Invalidate();

		// must destruct itself
		void Destruct();
	}
}
