using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Oxide.Core.Plugins;

namespace Oxide.Core.Libraries
{
	// Token: 0x0200003C RID: 60
	public class Permission : Library
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600022C RID: 556 RVA: 0x0000AAA5 File Offset: 0x00008CA5
		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600022D RID: 557 RVA: 0x0000AAA8 File Offset: 0x00008CA8
		// (set) Token: 0x0600022E RID: 558 RVA: 0x0000AAB0 File Offset: 0x00008CB0
		public bool IsLoaded { get; private set; }

		// Token: 0x0600022F RID: 559 RVA: 0x0000AAB9 File Offset: 0x00008CB9
		public Permission()
		{
			this.permset = new Dictionary<Plugin, HashSet<string>>();
			this.LoadFromDatafile();
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000AAD4 File Offset: 0x00008CD4
		private void LoadFromDatafile()
		{
			Utility.DatafileToProto<Dictionary<string, UserData>>("oxide.users", true);
			Utility.DatafileToProto<Dictionary<string, GroupData>>("oxide.groups", true);
			this.userdata = (ProtoStorage.Load<Dictionary<string, UserData>>(new string[]
			{
				"oxide.users"
			}) ?? new Dictionary<string, UserData>());
			this.groupdata = (ProtoStorage.Load<Dictionary<string, GroupData>>(new string[]
			{
				"oxide.groups"
			}) ?? new Dictionary<string, GroupData>());
			foreach (KeyValuePair<string, GroupData> keyValuePair in this.groupdata)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Value.ParentGroup) && this.HasCircularParent(keyValuePair.Key, keyValuePair.Value.ParentGroup))
				{
					Interface.Oxide.LogWarning("Detected circular parent group for '{0}'! Removing parent '{1}'", new object[]
					{
						keyValuePair.Key,
						keyValuePair.Value.ParentGroup
					});
					keyValuePair.Value.ParentGroup = null;
				}
			}
			this.IsLoaded = true;
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000ABEC File Offset: 0x00008DEC
		[LibraryFunction("Export")]
		public void Export(string prefix = "auth")
		{
			if (!this.IsLoaded)
			{
				return;
			}
			Interface.Oxide.DataFileSystem.WriteObject<Dictionary<string, GroupData>>(prefix + ".groups", this.groupdata, false);
			Interface.Oxide.DataFileSystem.WriteObject<Dictionary<string, UserData>>(prefix + ".users", this.userdata, false);
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000AC44 File Offset: 0x00008E44
		public void SaveData()
		{
			this.SaveUsers();
			this.SaveGroups();
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000AC52 File Offset: 0x00008E52
		public void SaveUsers()
		{
			ProtoStorage.Save<Dictionary<string, UserData>>(this.userdata, new string[]
			{
				"oxide.users"
			});
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000AC6D File Offset: 0x00008E6D
		public void SaveGroups()
		{
			ProtoStorage.Save<Dictionary<string, GroupData>>(this.groupdata, new string[]
			{
				"oxide.groups"
			});
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000AC88 File Offset: 0x00008E88
		public void RegisterValidate(Func<string, bool> val)
		{
			this.validate = val;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000AC94 File Offset: 0x00008E94
		public void CleanUp()
		{
			if (!this.IsLoaded || this.validate == null)
			{
				return;
			}
			string[] array = (from k in this.userdata.Keys
			where !this.validate(k)
			select k).ToArray<string>();
			if (array.Length == 0)
			{
				return;
			}
			foreach (string key in array)
			{
				this.userdata.Remove(key);
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000ACFC File Offset: 0x00008EFC
		public void MigrateGroup(string oldGroup, string newGroup)
		{
			if (!this.IsLoaded)
			{
				return;
			}
			if (this.GroupExists(oldGroup))
			{
				string fileDataPath = ProtoStorage.GetFileDataPath("oxide.groups.data");
				File.Copy(fileDataPath, fileDataPath + ".old", true);
				foreach (string perm in this.GetGroupPermissions(oldGroup, false))
				{
					this.GrantGroupPermission(newGroup, perm, null);
				}
				if (this.GetUsersInGroup(oldGroup).Length == 0)
				{
					this.RemoveGroup(oldGroup);
				}
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000AD6C File Offset: 0x00008F6C
		[LibraryFunction("RegisterPermission")]
		public void RegisterPermission(string name, Plugin owner)
		{
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			name = name.ToLower();
			if (this.PermissionExists(name, null))
			{
				Interface.Oxide.LogWarning("Duplicate permission registered '{0}' (by plugin '{1}')", new object[]
				{
					name,
					owner.Title
				});
				return;
			}
			HashSet<string> hashSet;
			if (!this.permset.TryGetValue(owner, out hashSet))
			{
				hashSet = new HashSet<string>();
				this.permset.Add(owner, hashSet);
				owner.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.owner_OnRemovedFromManager));
			}
			hashSet.Add(name);
			Interface.CallHook("OnPermissionRegistered", name, owner);
			string text = owner.Name.ToLower() + ".";
			if (!name.StartsWith(text) && !owner.IsCorePlugin)
			{
				Interface.Oxide.LogWarning("Missing plugin name prefix '{0}' for permission '{1}' (by plugin '{2}')", new object[]
				{
					text,
					name,
					owner.Title
				});
			}
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000AE54 File Offset: 0x00009054
		[LibraryFunction("PermissionExists")]
		public bool PermissionExists(string name, Plugin owner = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			name = name.ToLower();
			if (owner == null)
			{
				if (this.permset.Count > 0)
				{
					if (name.Equals("*"))
					{
						return true;
					}
					if (name.EndsWith("*"))
					{
						name = name.TrimEnd(new char[]
						{
							'*'
						});
						return this.permset.Values.SelectMany((HashSet<string> v) => v).Any((string p) => p.StartsWith(name));
					}
				}
				return this.permset.Values.Any((HashSet<string> v) => v.Contains(name));
			}
			HashSet<string> hashSet;
			if (!this.permset.TryGetValue(owner, out hashSet))
			{
				return false;
			}
			if (hashSet.Count > 0)
			{
				if (name.Equals("*"))
				{
					return true;
				}
				if (name.EndsWith("*"))
				{
					name = name.TrimEnd(new char[]
					{
						'*'
					});
					return hashSet.Any((string p) => p.StartsWith(name));
				}
			}
			return hashSet.Contains(name);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000AFC1 File Offset: 0x000091C1
		private void owner_OnRemovedFromManager(Plugin sender, PluginManager manager)
		{
			this.permset.Remove(sender);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000AFD0 File Offset: 0x000091D0
		[LibraryFunction("UserIdValid")]
		public bool UserIdValid(string id)
		{
			return this.validate == null || this.validate(id);
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000AFE8 File Offset: 0x000091E8
		[LibraryFunction("UserExists")]
		public bool UserExists(string id)
		{
			return this.userdata.ContainsKey(id);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000AFF8 File Offset: 0x000091F8
		public UserData GetUserData(string id)
		{
			UserData result;
			if (!this.userdata.TryGetValue(id, out result))
			{
				this.userdata.Add(id, result = new UserData());
			}
			return result;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000B02C File Offset: 0x0000922C
		[LibraryFunction("UpdateNickname")]
		public void UpdateNickname(string id, string nickname)
		{
			if (this.UserExists(id))
			{
				UserData userData = this.GetUserData(id);
				string lastSeenNickname = userData.LastSeenNickname;
				string obj = nickname.Sanitize();
				userData.LastSeenNickname = nickname.Sanitize();
				Interface.CallHook("OnUserNameUpdated", id, lastSeenNickname, obj);
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000B070 File Offset: 0x00009270
		[LibraryFunction("UserHasAnyGroup")]
		public bool UserHasAnyGroup(string id)
		{
			return this.UserExists(id) && this.GetUserData(id).Groups.Count > 0;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000B094 File Offset: 0x00009294
		[LibraryFunction("GroupsHavePermission")]
		public bool GroupsHavePermission(HashSet<string> groups, string perm)
		{
			return groups.Any((string group) => this.GroupHasPermission(group, perm));
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000B0C8 File Offset: 0x000092C8
		[LibraryFunction("GroupHasPermission")]
		public bool GroupHasPermission(string name, string perm)
		{
			GroupData groupData;
			return this.GroupExists(name) && !string.IsNullOrEmpty(perm) && this.groupdata.TryGetValue(name.ToLower(), out groupData) && (groupData.Perms.Contains(perm.ToLower()) || this.GroupHasPermission(groupData.ParentGroup, perm));
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000B124 File Offset: 0x00009324
		[LibraryFunction("UserHasPermission")]
		public bool UserHasPermission(string id, string perm)
		{
			if (string.IsNullOrEmpty(perm))
			{
				return false;
			}
			perm = perm.ToLower();
			UserData userData = this.GetUserData(id);
			return userData.Perms.Contains(perm) || this.GroupsHavePermission(userData.Groups, perm);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000B168 File Offset: 0x00009368
		[LibraryFunction("GetUserGroups")]
		public string[] GetUserGroups(string id)
		{
			return this.GetUserData(id).Groups.ToArray<string>();
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000B17C File Offset: 0x0000937C
		[LibraryFunction("GetUserPermissions")]
		public string[] GetUserPermissions(string id)
		{
			UserData userData = this.GetUserData(id);
			List<string> list = userData.Perms.ToList<string>();
			foreach (string name in userData.Groups)
			{
				list.AddRange(this.GetGroupPermissions(name, false));
			}
			return new HashSet<string>(list).ToArray<string>();
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000B1F4 File Offset: 0x000093F4
		[LibraryFunction("GetGroupPermissions")]
		public string[] GetGroupPermissions(string name, bool parents = false)
		{
			if (!this.GroupExists(name))
			{
				return new string[0];
			}
			GroupData groupData;
			if (!this.groupdata.TryGetValue(name.ToLower(), out groupData))
			{
				return new string[0];
			}
			List<string> list = groupData.Perms.ToList<string>();
			if (parents)
			{
				list.AddRange(this.GetGroupPermissions(groupData.ParentGroup, false));
			}
			return new HashSet<string>(list).ToArray<string>();
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000B25A File Offset: 0x0000945A
		[LibraryFunction("GetPermissions")]
		public string[] GetPermissions()
		{
			return new HashSet<string>(this.permset.Values.SelectMany((HashSet<string> v) => v)).ToArray<string>();
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000B298 File Offset: 0x00009498
		[LibraryFunction("GetPermissionUsers")]
		public string[] GetPermissionUsers(string perm)
		{
			if (string.IsNullOrEmpty(perm))
			{
				return new string[0];
			}
			perm = perm.ToLower();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<string, UserData> keyValuePair in this.userdata)
			{
				if (keyValuePair.Value.Perms.Contains(perm))
				{
					hashSet.Add(keyValuePair.Key + "(" + keyValuePair.Value.LastSeenNickname + ")");
				}
			}
			return hashSet.ToArray<string>();
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000B344 File Offset: 0x00009544
		[LibraryFunction("GetPermissionGroups")]
		public string[] GetPermissionGroups(string perm)
		{
			if (string.IsNullOrEmpty(perm))
			{
				return new string[0];
			}
			perm = perm.ToLower();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<string, GroupData> keyValuePair in this.groupdata)
			{
				if (keyValuePair.Value.Perms.Contains(perm))
				{
					hashSet.Add(keyValuePair.Key);
				}
			}
			return hashSet.ToArray<string>();
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000B3D8 File Offset: 0x000095D8
		[LibraryFunction("AddUserGroup")]
		public void AddUserGroup(string id, string name)
		{
			if (!this.GroupExists(name))
			{
				return;
			}
			if (!this.GetUserData(id).Groups.Add(name.ToLower()))
			{
				return;
			}
			Interface.Call("OnUserGroupAdded", new object[]
			{
				id,
				name
			});
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000B418 File Offset: 0x00009618
		[LibraryFunction("RemoveUserGroup")]
		public void RemoveUserGroup(string id, string name)
		{
			if (!this.GroupExists(name))
			{
				return;
			}
			UserData userData = this.GetUserData(id);
			if (name.Equals("*"))
			{
				if (userData.Groups.Count <= 0)
				{
					return;
				}
				userData.Groups.Clear();
				return;
			}
			else
			{
				if (!userData.Groups.Remove(name.ToLower()))
				{
					return;
				}
				Interface.Call("OnUserGroupRemoved", new object[]
				{
					id,
					name
				});
				return;
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000B48C File Offset: 0x0000968C
		[LibraryFunction("UserHasGroup")]
		public bool UserHasGroup(string id, string name)
		{
			return this.GroupExists(name) && this.GetUserData(id).Groups.Contains(name.ToLower());
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000B4B0 File Offset: 0x000096B0
		[LibraryFunction("GroupExists")]
		public bool GroupExists(string group)
		{
			return !string.IsNullOrEmpty(group) && (group.Equals("*") || this.groupdata.ContainsKey(group.ToLower()));
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000B4DC File Offset: 0x000096DC
		[LibraryFunction("GetGroups")]
		public string[] GetGroups()
		{
			return this.groupdata.Keys.ToArray<string>();
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000B4F0 File Offset: 0x000096F0
		[LibraryFunction("GetUsersInGroup")]
		public string[] GetUsersInGroup(string group)
		{
			if (!this.GroupExists(group))
			{
				return new string[0];
			}
			group = group.ToLower();
			return (from u in this.userdata
			where u.Value.Groups.Contains(@group)
			select u.Key + " (" + u.Value.LastSeenNickname + ")").ToArray<string>();
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000B570 File Offset: 0x00009770
		[LibraryFunction("GetGroupTitle")]
		public string GetGroupTitle(string group)
		{
			if (!this.GroupExists(group))
			{
				return string.Empty;
			}
			GroupData groupData;
			if (!this.groupdata.TryGetValue(group.ToLower(), out groupData))
			{
				return string.Empty;
			}
			return groupData.Title;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000B5B0 File Offset: 0x000097B0
		[LibraryFunction("GetGroupRank")]
		public int GetGroupRank(string group)
		{
			if (!this.GroupExists(group))
			{
				return 0;
			}
			GroupData groupData;
			if (!this.groupdata.TryGetValue(group.ToLower(), out groupData))
			{
				return 0;
			}
			return groupData.Rank;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000B5E8 File Offset: 0x000097E8
		[LibraryFunction("GrantUserPermission")]
		public void GrantUserPermission(string id, string perm, Plugin owner)
		{
			if (!this.PermissionExists(perm, owner))
			{
				return;
			}
			UserData data = this.GetUserData(id);
			perm = perm.ToLower();
			if (perm.EndsWith("*"))
			{
				HashSet<string> source;
				if (owner == null)
				{
					source = new HashSet<string>(this.permset.Values.SelectMany((HashSet<string> v) => v));
				}
				else if (!this.permset.TryGetValue(owner, out source))
				{
					return;
				}
				if (perm.Equals("*"))
				{
					source.Aggregate(false, (bool c, string s) => c | data.Perms.Add(s));
					return;
				}
				perm = perm.TrimEnd(new char[]
				{
					'*'
				});
				(from s in source
				where s.StartsWith(perm)
				select s).Aggregate(false, (bool c, string s) => c | data.Perms.Add(s));
				return;
			}
			else
			{
				if (!data.Perms.Add(perm))
				{
					return;
				}
				Interface.Call("OnUserPermissionGranted", new object[]
				{
					id,
					perm
				});
				return;
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000B730 File Offset: 0x00009930
		[LibraryFunction("RevokeUserPermission")]
		public void RevokeUserPermission(string id, string perm)
		{
			if (string.IsNullOrEmpty(perm))
			{
				return;
			}
			UserData userData = this.GetUserData(id);
			perm = perm.ToLower();
			if (perm.EndsWith("*"))
			{
				if (!perm.Equals("*"))
				{
					perm = perm.TrimEnd(new char[]
					{
						'*'
					});
					userData.Perms.RemoveWhere((string s) => s.StartsWith(perm));
					return;
				}
				if (userData.Perms.Count <= 0)
				{
					return;
				}
				userData.Perms.Clear();
				return;
			}
			else
			{
				if (!userData.Perms.Remove(perm))
				{
					return;
				}
				Interface.Call("OnUserPermissionRevoked", new object[]
				{
					id,
					perm
				});
				return;
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000B81C File Offset: 0x00009A1C
		[LibraryFunction("GrantGroupPermission")]
		public void GrantGroupPermission(string name, string perm, Plugin owner)
		{
			if (!this.PermissionExists(perm, owner) || !this.GroupExists(name))
			{
				return;
			}
			GroupData data;
			if (!this.groupdata.TryGetValue(name.ToLower(), out data))
			{
				return;
			}
			perm = perm.ToLower();
			if (perm.EndsWith("*"))
			{
				HashSet<string> source;
				if (owner == null)
				{
					source = new HashSet<string>(this.permset.Values.SelectMany((HashSet<string> v) => v));
				}
				else if (!this.permset.TryGetValue(owner, out source))
				{
					return;
				}
				if (perm.Equals("*"))
				{
					source.Aggregate(false, (bool c, string s) => c | data.Perms.Add(s));
					return;
				}
				perm = perm.TrimEnd(new char[]
				{
					'*'
				}).ToLower();
				(from s in source
				where s.StartsWith(perm)
				select s).Aggregate(false, (bool c, string s) => c | data.Perms.Add(s));
				return;
			}
			else
			{
				if (!data.Perms.Add(perm))
				{
					return;
				}
				Interface.Call("OnGroupPermissionGranted", new object[]
				{
					name,
					perm
				});
				return;
			}
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000B980 File Offset: 0x00009B80
		[LibraryFunction("RevokeGroupPermission")]
		public void RevokeGroupPermission(string name, string perm)
		{
			if (!this.GroupExists(name) || string.IsNullOrEmpty(perm))
			{
				return;
			}
			GroupData groupData;
			if (!this.groupdata.TryGetValue(name.ToLower(), out groupData))
			{
				return;
			}
			perm = perm.ToLower();
			if (perm.EndsWith("*"))
			{
				if (!perm.Equals("*"))
				{
					perm = perm.TrimEnd(new char[]
					{
						'*'
					}).ToLower();
					groupData.Perms.RemoveWhere((string s) => s.StartsWith(perm));
					return;
				}
				if (groupData.Perms.Count <= 0)
				{
					return;
				}
				groupData.Perms.Clear();
				return;
			}
			else
			{
				if (!groupData.Perms.Remove(perm))
				{
					return;
				}
				Interface.Call("OnGroupPermissionRevoked", new object[]
				{
					name,
					perm
				});
				return;
			}
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000BA88 File Offset: 0x00009C88
		[LibraryFunction("CreateGroup")]
		public bool CreateGroup(string group, string title, int rank)
		{
			if (this.GroupExists(group) || string.IsNullOrEmpty(group))
			{
				return false;
			}
			GroupData value = new GroupData
			{
				Title = title,
				Rank = rank
			};
			group = group.ToLower();
			this.groupdata.Add(group, value);
			Interface.CallHook("OnGroupCreated", group, title, rank);
			return true;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000BAE8 File Offset: 0x00009CE8
		[LibraryFunction("RemoveGroup")]
		public bool RemoveGroup(string group)
		{
			if (!this.GroupExists(group))
			{
				return false;
			}
			group = group.ToLower();
			bool flag = this.groupdata.Remove(group);
			if (this.userdata.Values.Aggregate(false, (bool current, UserData userData) => current | userData.Groups.Remove(group)))
			{
				this.SaveUsers();
			}
			if (flag)
			{
				Interface.CallHook("OnGroupDeleted", group);
			}
			return true;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000BB70 File Offset: 0x00009D70
		[LibraryFunction("SetGroupTitle")]
		public bool SetGroupTitle(string group, string title)
		{
			if (!this.GroupExists(group))
			{
				return false;
			}
			group = group.ToLower();
			GroupData groupData;
			if (!this.groupdata.TryGetValue(group, out groupData))
			{
				return false;
			}
			if (groupData.Title == title)
			{
				return true;
			}
			groupData.Title = title;
			Interface.CallHook("OnGroupTitleSet", group, title);
			return true;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000BBC8 File Offset: 0x00009DC8
		[LibraryFunction("SetGroupRank")]
		public bool SetGroupRank(string group, int rank)
		{
			if (!this.GroupExists(group))
			{
				return false;
			}
			group = group.ToLower();
			GroupData groupData;
			if (!this.groupdata.TryGetValue(group, out groupData))
			{
				return false;
			}
			if (groupData.Rank == rank)
			{
				return true;
			}
			groupData.Rank = rank;
			Interface.CallHook("OnGroupRankSet", group, rank);
			return true;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000BC20 File Offset: 0x00009E20
		[LibraryFunction("GetGroupParent")]
		public string GetGroupParent(string group)
		{
			if (!this.GroupExists(group))
			{
				return string.Empty;
			}
			group = group.ToLower();
			GroupData groupData;
			if (this.groupdata.TryGetValue(group, out groupData))
			{
				return groupData.ParentGroup;
			}
			return string.Empty;
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000BC60 File Offset: 0x00009E60
		[LibraryFunction("SetGroupParent")]
		public bool SetGroupParent(string group, string parent)
		{
			if (!this.GroupExists(group))
			{
				return false;
			}
			group = group.ToLower();
			GroupData groupData;
			if (!this.groupdata.TryGetValue(group, out groupData))
			{
				return false;
			}
			if (string.IsNullOrEmpty(parent))
			{
				groupData.ParentGroup = null;
				return true;
			}
			if (!this.GroupExists(parent) || group.Equals(parent.ToLower()))
			{
				return false;
			}
			parent = parent.ToLower();
			if (!string.IsNullOrEmpty(groupData.ParentGroup) && groupData.ParentGroup.Equals(parent))
			{
				return true;
			}
			if (this.HasCircularParent(group, parent))
			{
				return false;
			}
			groupData.ParentGroup = parent;
			Interface.CallHook("OnGroupParentSet", group, parent);
			return true;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000BD04 File Offset: 0x00009F04
		private bool HasCircularParent(string group, string parent)
		{
			GroupData groupData;
			if (!this.groupdata.TryGetValue(parent, out groupData))
			{
				return false;
			}
			HashSet<string> hashSet = new HashSet<string>
			{
				group,
				parent
			};
			while (!string.IsNullOrEmpty(groupData.ParentGroup))
			{
				if (!hashSet.Add(groupData.ParentGroup))
				{
					return true;
				}
				if (!this.groupdata.TryGetValue(groupData.ParentGroup, out groupData))
				{
					return false;
				}
			}
			return false;
		}

		// Token: 0x040000E3 RID: 227
		private readonly Dictionary<Plugin, HashSet<string>> permset;

		// Token: 0x040000E4 RID: 228
		private Dictionary<string, UserData> userdata;

		// Token: 0x040000E5 RID: 229
		private Dictionary<string, GroupData> groupdata;

		// Token: 0x040000E6 RID: 230
		private Func<string, bool> validate;
	}
}
