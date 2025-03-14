using System;
using System.Runtime.Serialization;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x02000045 RID: 69
	[Serializable]
	public class CommandAlreadyExistsException : Exception
	{
		// Token: 0x06000296 RID: 662 RVA: 0x0000CAC0 File Offset: 0x0000ACC0
		public CommandAlreadyExistsException()
		{
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000CAC8 File Offset: 0x0000ACC8
		public CommandAlreadyExistsException(string cmd) : base("Command " + cmd + " already exists")
		{
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000CAE0 File Offset: 0x0000ACE0
		public CommandAlreadyExistsException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000CAEA File Offset: 0x0000ACEA
		protected CommandAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
