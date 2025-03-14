using System;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x0200004E RID: 78
	public class GenericPosition
	{
		// Token: 0x060002E1 RID: 737 RVA: 0x0000D16D File Offset: 0x0000B36D
		public GenericPosition()
		{
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000D175 File Offset: 0x0000B375
		public GenericPosition(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000D194 File Offset: 0x0000B394
		public override bool Equals(object obj)
		{
			if (!(obj is GenericPosition))
			{
				return false;
			}
			GenericPosition genericPosition = (GenericPosition)obj;
			return this.X.Equals(genericPosition.X) && this.Y.Equals(genericPosition.Y) && this.Z.Equals(genericPosition.Z);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000D1EC File Offset: 0x0000B3EC
		public static bool operator ==(GenericPosition a, GenericPosition b)
		{
			return a == b || (a != null && b != null && (a.X.Equals(b.X) && a.Y.Equals(b.Y)) && a.Z.Equals(b.Z));
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000D240 File Offset: 0x0000B440
		public static bool operator !=(GenericPosition a, GenericPosition b)
		{
			return !(a == b);
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000D24C File Offset: 0x0000B44C
		public static GenericPosition operator +(GenericPosition a, GenericPosition b)
		{
			return new GenericPosition(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000D27A File Offset: 0x0000B47A
		public static GenericPosition operator -(GenericPosition a, GenericPosition b)
		{
			return new GenericPosition(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000D2A8 File Offset: 0x0000B4A8
		public static GenericPosition operator *(float mult, GenericPosition a)
		{
			return new GenericPosition(a.X * mult, a.Y * mult, a.Z * mult);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000D2C7 File Offset: 0x0000B4C7
		public static GenericPosition operator *(GenericPosition a, float mult)
		{
			return new GenericPosition(a.X * mult, a.Y * mult, a.Z * mult);
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000D2E6 File Offset: 0x0000B4E6
		public static GenericPosition operator /(GenericPosition a, float div)
		{
			return new GenericPosition(a.X / div, a.Y / div, a.Z / div);
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0000D305 File Offset: 0x0000B505
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000D32E File Offset: 0x0000B52E
		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", this.X, this.Y, this.Z);
		}

		// Token: 0x0400011B RID: 283
		public float X;

		// Token: 0x0400011C RID: 284
		public float Y;

		// Token: 0x0400011D RID: 285
		public float Z;
	}
}
