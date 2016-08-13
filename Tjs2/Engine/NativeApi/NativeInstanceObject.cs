/*
 * TJS2 CSharp
 */

namespace Tjs2.Engine.NativeApi
{
	public class NativeInstanceObject : NativeInstance
	{
		// TJS constructor
		/// <exception cref="VariantException"></exception>
		/// <exception cref="TJSException"></exception>
		public virtual int Construct(Variant[] param, Dispatch2 tjsObj)
		{
			return Error.S_OK;
		}

		// called before destruction
		/// <exception cref="VariantException"></exception>
		/// <exception cref="TJSException"></exception>
		public virtual void Invalidate()
		{
		}

		// must destruct itself
		public virtual void Destruct()
		{
		}
	}
}
