using System;
using System.Globalization;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x0200004D RID: 77
	public interface IPlayer
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060002B8 RID: 696
		object Object { get; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060002B9 RID: 697
		// (set) Token: 0x060002BA RID: 698
		CommandType LastCommand { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002BB RID: 699
		// (set) Token: 0x060002BC RID: 700
		string Name { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002BD RID: 701
		string Id { get; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002BE RID: 702
		string Address { get; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002BF RID: 703
		int Ping { get; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002C0 RID: 704
		CultureInfo Language { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002C1 RID: 705
		bool IsConnected { get; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002C2 RID: 706
		bool IsSleeping { get; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002C3 RID: 707
		bool IsServer { get; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002C4 RID: 708
		bool IsAdmin { get; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002C5 RID: 709
		bool IsBanned { get; }

		// Token: 0x060002C6 RID: 710
		void Ban(string reason, TimeSpan duration = default(TimeSpan));

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060002C7 RID: 711
		TimeSpan BanTimeRemaining { get; }

		// Token: 0x060002C8 RID: 712
		void Heal(float amount);

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002C9 RID: 713
		// (set) Token: 0x060002CA RID: 714
		float Health { get; set; }

		// Token: 0x060002CB RID: 715
		void Hurt(float amount);

		// Token: 0x060002CC RID: 716
		void Kick(string reason);

		// Token: 0x060002CD RID: 717
		void Kill();

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060002CE RID: 718
		// (set) Token: 0x060002CF RID: 719
		float MaxHealth { get; set; }

		// Token: 0x060002D0 RID: 720
		void Rename(string name);

		// Token: 0x060002D1 RID: 721
		void Teleport(float x, float y, float z);

		// Token: 0x060002D2 RID: 722
		void Teleport(GenericPosition pos);

		// Token: 0x060002D3 RID: 723
		void Unban();

		// Token: 0x060002D4 RID: 724
		void Position(out float x, out float y, out float z);

		// Token: 0x060002D5 RID: 725
		GenericPosition Position();

		// Token: 0x060002D6 RID: 726
		void Message(string message, string prefix, params object[] args);

		// Token: 0x060002D7 RID: 727
		void Message(string message);

		// Token: 0x060002D8 RID: 728
		void Reply(string message, string prefix, params object[] args);

		// Token: 0x060002D9 RID: 729
		void Reply(string message);

		// Token: 0x060002DA RID: 730
		void Command(string command, params object[] args);

		// Token: 0x060002DB RID: 731
		bool HasPermission(string perm);

		// Token: 0x060002DC RID: 732
		void GrantPermission(string perm);

		// Token: 0x060002DD RID: 733
		void RevokePermission(string perm);

		// Token: 0x060002DE RID: 734
		bool BelongsToGroup(string group);

		// Token: 0x060002DF RID: 735
		void AddToGroup(string group);

		// Token: 0x060002E0 RID: 736
		void RemoveFromGroup(string group);
	}
}
