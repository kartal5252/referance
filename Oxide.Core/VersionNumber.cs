using System;

namespace Oxide.Core
{
	// Token: 0x02000018 RID: 24
	public struct VersionNumber
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x00006602 File Offset: 0x00004802
		public VersionNumber(int major, int minor, int patch)
		{
			this.Major = major;
			this.Minor = minor;
			this.Patch = patch;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00006619 File Offset: 0x00004819
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Patch);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006646 File Offset: 0x00004846
		public static bool operator ==(VersionNumber a, VersionNumber b)
		{
			return a.Major == b.Major && a.Minor == b.Minor && a.Patch == b.Patch;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00006674 File Offset: 0x00004874
		public static bool operator !=(VersionNumber a, VersionNumber b)
		{
			return a.Major != b.Major || a.Minor != b.Minor || a.Patch != b.Patch;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000066A8 File Offset: 0x000048A8
		public static bool operator >(VersionNumber a, VersionNumber b)
		{
			return a.Major >= b.Major && (a.Major > b.Major || (a.Minor >= b.Minor && (a.Minor > b.Minor || a.Patch > b.Patch)));
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00006704 File Offset: 0x00004904
		public static bool operator >=(VersionNumber a, VersionNumber b)
		{
			return a.Major >= b.Major && (a.Major > b.Major || (a.Minor >= b.Minor && (a.Minor > b.Minor || a.Patch >= b.Patch)));
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00006764 File Offset: 0x00004964
		public static bool operator <(VersionNumber a, VersionNumber b)
		{
			return a.Major <= b.Major && (a.Major < b.Major || (a.Minor <= b.Minor && (a.Minor < b.Minor || a.Patch < b.Patch)));
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x000067C0 File Offset: 0x000049C0
		public static bool operator <=(VersionNumber a, VersionNumber b)
		{
			return a.Major <= b.Major && (a.Major < b.Major || (a.Minor <= b.Minor && (a.Minor < b.Minor || a.Patch <= b.Patch)));
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00006820 File Offset: 0x00004A20
		public override bool Equals(object obj)
		{
			if (!(obj is VersionNumber))
			{
				return false;
			}
			VersionNumber b = (VersionNumber)obj;
			return this == b;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000684A File Offset: 0x00004A4A
		public override int GetHashCode()
		{
			return ((17 * 23 + this.Major.GetHashCode()) * 23 + this.Minor.GetHashCode()) * 23 + this.Patch.GetHashCode();
		}

		// Token: 0x04000061 RID: 97
		public int Major;

		// Token: 0x04000062 RID: 98
		public int Minor;

		// Token: 0x04000063 RID: 99
		public int Patch;
	}
}
