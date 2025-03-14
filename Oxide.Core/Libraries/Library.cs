using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Oxide.Core.Libraries
{
	// Token: 0x02000039 RID: 57
	public abstract class Library
	{
		// Token: 0x06000211 RID: 529 RVA: 0x0000A7CC File Offset: 0x000089CC
		public static implicit operator bool(Library library)
		{
			return library != null;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000A7D2 File Offset: 0x000089D2
		public static bool operator !(Library library)
		{
			return !library;
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000A7DA File Offset: 0x000089DA
		public virtual bool IsGlobal { get; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000A7E2 File Offset: 0x000089E2
		// (set) Token: 0x06000215 RID: 533 RVA: 0x0000A7EA File Offset: 0x000089EA
		public Exception LastException { get; protected set; }

		// Token: 0x06000216 RID: 534 RVA: 0x0000A7F4 File Offset: 0x000089F4
		public Library()
		{
			this.functions = new Dictionary<string, MethodInfo>();
			this.properties = new Dictionary<string, PropertyInfo>();
			Type type = base.GetType();
			MethodInfo[] methods = type.GetMethods();
			int i = 0;
			while (i < methods.Length)
			{
				MethodInfo methodInfo = methods[i];
				LibraryFunction libraryFunction;
				try
				{
					libraryFunction = (methodInfo.GetCustomAttributes(typeof(LibraryFunction), true).SingleOrDefault<object>() as LibraryFunction);
					if (libraryFunction == null)
					{
						goto IL_B1;
					}
				}
				catch (TypeLoadException)
				{
					goto IL_B1;
				}
				goto IL_5D;
				IL_B1:
				i++;
				continue;
				IL_5D:
				string text = libraryFunction.Name ?? methodInfo.Name;
				if (this.functions.ContainsKey(text))
				{
					Interface.Oxide.LogError(type.FullName + " library tried to register an already registered function: " + text, new object[0]);
					goto IL_B1;
				}
				this.functions[text] = methodInfo;
				goto IL_B1;
			}
			PropertyInfo[] array = type.GetProperties();
			i = 0;
			while (i < array.Length)
			{
				PropertyInfo propertyInfo = array[i];
				LibraryProperty libraryProperty;
				try
				{
					libraryProperty = (propertyInfo.GetCustomAttributes(typeof(LibraryProperty), true).SingleOrDefault<object>() as LibraryProperty);
					if (libraryProperty == null)
					{
						goto IL_153;
					}
				}
				catch (TypeLoadException)
				{
					goto IL_153;
				}
				goto IL_FC;
				IL_153:
				i++;
				continue;
				IL_FC:
				string text2 = libraryProperty.Name ?? propertyInfo.Name;
				if (this.properties.ContainsKey(text2))
				{
					Interface.Oxide.LogError("{0} library tried to register an already registered property: {1}", new object[]
					{
						type.FullName,
						text2
					});
					goto IL_153;
				}
				this.properties[text2] = propertyInfo;
				goto IL_153;
			}
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000A980 File Offset: 0x00008B80
		public virtual void Shutdown()
		{
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000A982 File Offset: 0x00008B82
		public IEnumerable<string> GetFunctionNames()
		{
			return this.functions.Keys;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000A98F File Offset: 0x00008B8F
		public IEnumerable<string> GetPropertyNames()
		{
			return this.properties.Keys;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000A99C File Offset: 0x00008B9C
		public MethodInfo GetFunction(string name)
		{
			MethodInfo result;
			if (!this.functions.TryGetValue(name, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000A9BC File Offset: 0x00008BBC
		public PropertyInfo GetProperty(string name)
		{
			PropertyInfo result;
			if (!this.properties.TryGetValue(name, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x040000D8 RID: 216
		private IDictionary<string, MethodInfo> functions;

		// Token: 0x040000D9 RID: 217
		private IDictionary<string, PropertyInfo> properties;
	}
}
