using System;
using System.Globalization;
using System.Net;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x02000050 RID: 80
	public interface IServer
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060002F3 RID: 755
		// (set) Token: 0x060002F4 RID: 756
		string Name { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060002F5 RID: 757
		IPAddress Address { get; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060002F6 RID: 758
		IPAddress LocalAddress { get; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002F7 RID: 759
		ushort Port { get; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002F8 RID: 760
		string Version { get; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002F9 RID: 761
		string Protocol { get; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060002FA RID: 762
		CultureInfo Language { get; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060002FB RID: 763
		int Players { get; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060002FC RID: 764
		// (set) Token: 0x060002FD RID: 765
		int MaxPlayers { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060002FE RID: 766
		// (set) Token: 0x060002FF RID: 767
		DateTime Time { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000300 RID: 768
		SaveInfo SaveInfo { get; }

		// Token: 0x06000301 RID: 769
		void Ban(string id, string reason, TimeSpan duration = default(TimeSpan));

		// Token: 0x06000302 RID: 770
		TimeSpan BanTimeRemaining(string id);

		// Token: 0x06000303 RID: 771
		bool IsBanned(string id);

		// Token: 0x06000304 RID: 772
		void Save();

		// Token: 0x06000305 RID: 773
		void Unban(string id);

		// Token: 0x06000306 RID: 774
		void Broadcast(string message, string prefix, params object[] args);

		// Token: 0x06000307 RID: 775
		void Broadcast(string message);

		// Token: 0x06000308 RID: 776
		void Command(string command, params object[] args);
	}
}
