/*
 * TJS2 CSharp
 */

namespace Tjs2.Engine
{
	[System.Serializable]
	public class TJSScriptException : TJSScriptError
	{
		private const long serialVersionUID = 5461681283822047431L;

		private Variant mValue;

		public TJSScriptException(string Msg, ScriptBlock block, int pos, Variant val) : 
			base(Msg, block, pos)
		{
			mValue = val;
		}

		public TJSScriptException(TJSScriptException @ref) : base(@ref)
		{
			mValue = @ref.mValue;
		}

		public virtual Variant GetValue()
		{
			return mValue;
		}
	}
}
