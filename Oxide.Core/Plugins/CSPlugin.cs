using System;
using System.Collections.Generic;
using System.Reflection;
using Oxide.Core.Libraries;

namespace Oxide.Core.Plugins
{
	// Token: 0x02000020 RID: 32
	public abstract class CSPlugin : Plugin
	{
		// Token: 0x06000142 RID: 322 RVA: 0x00007AF4 File Offset: 0x00005CF4
		public static T GetLibrary<T>(string name = null) where T : Library
		{
			return Interface.Oxide.GetLibrary<T>(name);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00007B04 File Offset: 0x00005D04
		public CSPlugin()
		{
			Type type = base.GetType();
			List<Type> list = new List<Type>
			{
				type
			};
			while (type != typeof(CSPlugin))
			{
				list.Add(type = type.BaseType);
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				foreach (MethodInfo methodInfo in list[i].GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(HookMethodAttribute), true);
					if (customAttributes.Length >= 1)
					{
						HookMethodAttribute hookMethodAttribute = customAttributes[0] as HookMethodAttribute;
						this.AddHookMethod((hookMethodAttribute != null) ? hookMethodAttribute.Name : null, methodInfo);
					}
				}
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00007BD4 File Offset: 0x00005DD4
		public override void HandleAddedToManager(PluginManager manager)
		{
			base.HandleAddedToManager(manager);
			foreach (string hook in this.Hooks.Keys)
			{
				base.Subscribe(hook);
			}
			try
			{
				this.OnCallHook("Init", null);
			}
			catch (Exception ex)
			{
				Interface.Oxide.LogException(string.Format("Failed to initialize plugin '{0} v{1}'", base.Name, base.Version), ex);
				if (base.Loader != null)
				{
					base.Loader.PluginErrors[base.Name] = ex.Message;
				}
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00007C9C File Offset: 0x00005E9C
		protected void AddHookMethod(string name, MethodInfo method)
		{
			List<HookMethod> list;
			if (!this.Hooks.TryGetValue(name, out list))
			{
				list = new List<HookMethod>();
				this.Hooks[name] = list;
			}
			list.Add(new HookMethod(method));
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00007CD8 File Offset: 0x00005ED8
		protected sealed override object OnCallHook(string name, object[] args)
		{
			object result = null;
			bool flag = false;
			foreach (HookMethod hookMethod in this.FindHooks(name, args))
			{
				int num = (args != null) ? args.Length : 0;
				object[] array;
				if (num != hookMethod.Parameters.Length)
				{
					array = ArrayPool.Get(hookMethod.Parameters.Length);
					flag = true;
					if (num > 0 && array.Length != 0)
					{
						Array.Copy(args, array, Math.Min(num, array.Length));
					}
					if (array.Length > num)
					{
						for (int i = num; i < array.Length; i++)
						{
							ParameterInfo parameterInfo = hookMethod.Parameters[i];
							if (parameterInfo.DefaultValue != null && parameterInfo.DefaultValue != DBNull.Value)
							{
								array[i] = parameterInfo.DefaultValue;
							}
							else if (parameterInfo.ParameterType.IsValueType)
							{
								array[i] = Activator.CreateInstance(parameterInfo.ParameterType);
							}
						}
					}
				}
				else
				{
					array = args;
				}
				try
				{
					result = this.InvokeMethod(hookMethod, array);
				}
				catch (TargetInvocationException ex)
				{
					if (flag)
					{
						ArrayPool.Free(array);
					}
					throw ex.InnerException ?? ex;
				}
				if (num != hookMethod.Parameters.Length)
				{
					for (int j = 0; j < hookMethod.Parameters.Length; j++)
					{
						if (hookMethod.Parameters[j].IsOut || hookMethod.Parameters[j].ParameterType.IsByRef)
						{
							args[j] = array[j];
						}
					}
				}
				if (flag)
				{
					ArrayPool.Free(array);
				}
			}
			return result;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00007E90 File Offset: 0x00006090
		protected List<HookMethod> FindHooks(string name, object[] args)
		{
			HookCache hookCache;
			List<HookMethod> hookMethod = this.HooksCache.GetHookMethod(name, args, out hookCache);
			if (hookMethod != null)
			{
				return hookMethod;
			}
			List<HookMethod> list = new List<HookMethod>();
			if (!this.Hooks.TryGetValue(name, out hookMethod))
			{
				return list;
			}
			HookMethod hookMethod2 = null;
			HookMethod hookMethod3 = null;
			foreach (HookMethod hookMethod4 in hookMethod)
			{
				if (hookMethod4.IsBaseHook)
				{
					list.Add(hookMethod4);
				}
				else
				{
					int num = (args != null) ? args.Length : 0;
					bool flag = false;
					object[] array;
					if (num != hookMethod4.Parameters.Length)
					{
						array = ArrayPool.Get(hookMethod4.Parameters.Length);
						flag = true;
						if (num > 0 && array.Length != 0)
						{
							Array.Copy(args, array, Math.Min(num, array.Length));
						}
						if (array.Length > num)
						{
							for (int i = num; i < array.Length; i++)
							{
								ParameterInfo parameterInfo = hookMethod4.Parameters[i];
								if (parameterInfo.DefaultValue != null && parameterInfo.DefaultValue != DBNull.Value)
								{
									array[i] = parameterInfo.DefaultValue;
								}
								else if (parameterInfo.ParameterType.IsValueType)
								{
									array[i] = Activator.CreateInstance(parameterInfo.ParameterType);
								}
							}
						}
					}
					else
					{
						array = args;
					}
					bool flag2;
					if (hookMethod4.HasMatchingSignature(array, out flag2))
					{
						if (flag2)
						{
							hookMethod2 = hookMethod4;
							break;
						}
						hookMethod3 = hookMethod4;
					}
					if (flag)
					{
						ArrayPool.Free(array);
					}
				}
			}
			if (hookMethod2 != null)
			{
				list.Add(hookMethod2);
			}
			else if (hookMethod3 != null)
			{
				list.Add(hookMethod3);
			}
			hookCache.SetupMethods(list);
			return list;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00008038 File Offset: 0x00006238
		protected virtual object InvokeMethod(HookMethod method, object[] args)
		{
			return method.Method.Invoke(this, args);
		}

		// Token: 0x04000088 RID: 136
		protected Dictionary<string, List<HookMethod>> Hooks = new Dictionary<string, List<HookMethod>>();

		// Token: 0x04000089 RID: 137
		protected HookCache HooksCache = new HookCache();
	}
}
