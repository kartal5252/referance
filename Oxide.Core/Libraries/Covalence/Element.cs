using System;
using System.Collections.Generic;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x02000046 RID: 70
	public class Element
	{
		// Token: 0x0600029A RID: 666 RVA: 0x0000CAF4 File Offset: 0x0000ACF4
		private Element(ElementType type, object val)
		{
			this.Type = type;
			this.Val = val;
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000CB15 File Offset: 0x0000AD15
		public static Element String(object s)
		{
			return new Element(ElementType.String, s);
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000CB1E File Offset: 0x0000AD1E
		public static Element Tag(ElementType type)
		{
			return new Element(type, null);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000CB27 File Offset: 0x0000AD27
		public static Element ParamTag(ElementType type, object val)
		{
			return new Element(type, val);
		}

		// Token: 0x0400010D RID: 269
		public ElementType Type;

		// Token: 0x0400010E RID: 270
		public object Val;

		// Token: 0x0400010F RID: 271
		public List<Element> Body = new List<Element>();
	}
}
