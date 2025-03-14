using System;
using System.IO;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x02000051 RID: 81
	public class SaveInfo
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0000D35B File Offset: 0x0000B55B
		// (set) Token: 0x0600030A RID: 778 RVA: 0x0000D363 File Offset: 0x0000B563
		public string SaveName { get; private set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0000D36C File Offset: 0x0000B56C
		// (set) Token: 0x0600030C RID: 780 RVA: 0x0000D374 File Offset: 0x0000B574
		public DateTime CreationTime { get; private set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0000D37D File Offset: 0x0000B57D
		// (set) Token: 0x0600030E RID: 782 RVA: 0x0000D385 File Offset: 0x0000B585
		public uint CreationTimeUnix { get; private set; }

		// Token: 0x0600030F RID: 783 RVA: 0x0000D38E File Offset: 0x0000B58E
		public void Refresh()
		{
			if (!File.Exists(this.FullPath))
			{
				return;
			}
			this.CreationTime = File.GetCreationTime(this.FullPath);
			this.CreationTimeUnix = this.time.GetUnixFromDateTime(this.CreationTime);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000D3C6 File Offset: 0x0000B5C6
		private SaveInfo(string filepath)
		{
			this.FullPath = filepath;
			this.SaveName = Utility.GetFileNameWithoutExtension(filepath);
			this.Refresh();
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0000D3F8 File Offset: 0x0000B5F8
		public static SaveInfo Create(string filepath)
		{
			if (!File.Exists(filepath))
			{
				return null;
			}
			return new SaveInfo(filepath);
		}

		// Token: 0x0400011E RID: 286
		private readonly Time time = Interface.Oxide.GetLibrary<Time>(null);

		// Token: 0x0400011F RID: 287
		private readonly string FullPath;
	}
}
