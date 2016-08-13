/*
 * TJS2 CSharp
 */

using System.Reflection;

namespace Tjs2.Engine.NativeApi.Java
{
	public class NativeJavaClassConstructor : NativeJavaClassMethod
	{
		/// <exception cref="TJSException"></exception>
		public NativeJavaClassConstructor(MethodInfo m, int classID) : base(m, classID)
		{
		}
	}
}
