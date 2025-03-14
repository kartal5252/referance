using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Oxide.Core.Plugins
{
	// Token: 0x02000022 RID: 34
	public class HookMethod
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00008145 File Offset: 0x00006345
		// (set) Token: 0x0600014E RID: 334 RVA: 0x0000814D File Offset: 0x0000634D
		public ParameterInfo[] Parameters { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00008156 File Offset: 0x00006356
		// (set) Token: 0x06000150 RID: 336 RVA: 0x0000815E File Offset: 0x0000635E
		public bool IsBaseHook { get; set; }

		// Token: 0x06000151 RID: 337 RVA: 0x00008168 File Offset: 0x00006368
		public HookMethod(MethodInfo method)
		{
			this.Method = method;
			this.Name = method.Name;
			this.Parameters = this.Method.GetParameters();
			if (this.Parameters.Length != 0)
			{
				this.Name = this.Name + "(" + string.Join(", ", (from x in this.Parameters
				select x.ParameterType.ToString()).ToArray<string>()) + ")";
			}
			this.IsBaseHook = this.Name.StartsWith("base_");
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00008214 File Offset: 0x00006414
		public bool HasMatchingSignature(object[] args, out bool exact)
		{
			exact = true;
			if (this.Parameters.Length == 0 && (args == null || args.Length == 0))
			{
				return true;
			}
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
				{
					if (!this.CanAssignNull(this.Parameters[i].ParameterType))
					{
						return false;
					}
				}
				else
				{
					if (exact && args[i].GetType() != this.Parameters[i].ParameterType && args[i].GetType().MakeByRefType() != this.Parameters[i].ParameterType && !this.CanConvertNumber(args[i], this.Parameters[i].ParameterType))
					{
						exact = false;
					}
					if (!exact && args[i].GetType() != this.Parameters[i].ParameterType && args[i].GetType().MakeByRefType() != this.Parameters[i].ParameterType && !(this.Parameters[i].ParameterType.FullName == "System.Object"))
					{
						if (args[i].GetType().IsValueType)
						{
							if (!TypeDescriptor.GetConverter(this.Parameters[i].ParameterType).CanConvertFrom(args[i].GetType()) && !this.CanConvertNumber(args[i], this.Parameters[i].ParameterType))
							{
								return false;
							}
						}
						else if (!this.Parameters[i].ParameterType.IsInstanceOfType(args[i]))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000837D File Offset: 0x0000657D
		private bool CanAssignNull(Type type)
		{
			return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00008392 File Offset: 0x00006592
		private bool IsNumber(object obj)
		{
			return obj != null && this.IsNumber(Nullable.GetUnderlyingType(obj.GetType()) ?? obj.GetType());
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000083B4 File Offset: 0x000065B4
		private bool IsNumber(Type type)
		{
			if (type.IsPrimitive)
			{
				return type != typeof(bool) && type != typeof(char) && type != typeof(IntPtr) && type != typeof(UIntPtr);
			}
			return type == typeof(decimal);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00008410 File Offset: 0x00006610
		private bool CanConvertNumber(object value, Type type)
		{
			return this.IsNumber(value) && this.IsNumber(type) && TypeDescriptor.GetConverter(type).IsValid(value);
		}

		// Token: 0x0400008D RID: 141
		public string Name;

		// Token: 0x0400008E RID: 142
		public MethodInfo Method;
	}
}
