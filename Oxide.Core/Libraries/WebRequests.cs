using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;

namespace Oxide.Core.Libraries
{
	// Token: 0x02000041 RID: 65
	public class WebRequests : Library
	{
		// Token: 0x06000277 RID: 631 RVA: 0x0000C0BD File Offset: 0x0000A2BD
		public static string FormatWebException(Exception exception, string response)
		{
			if (!string.IsNullOrEmpty(response))
			{
				response += Environment.NewLine;
			}
			response += exception.Message;
			if (exception.InnerException != null)
			{
				response = WebRequests.FormatWebException(exception.InnerException, response);
			}
			return response;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000C0FC File Offset: 0x0000A2FC
		public WebRequests()
		{
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.ServerCertificateValidationCallback = ((object <p0>, X509Certificate <p1>, X509Chain <p2>, SslPolicyErrors <p3>) => true);
			ServicePointManager.DefaultConnectionLimit = 200;
			ThreadPool.GetMaxThreads(out this.maxWorkerThreads, out this.maxCompletionPortThreads);
			this.maxCompletionPortThreads = (int)((double)this.maxCompletionPortThreads * 0.6);
			this.maxWorkerThreads = (int)((double)this.maxWorkerThreads * 0.75);
			this.workerthread = new Thread(new ThreadStart(this.Worker));
			this.workerthread.Start();
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000C1C8 File Offset: 0x0000A3C8
		public override void Shutdown()
		{
			if (this.shutdown)
			{
				return;
			}
			this.shutdown = true;
			this.workevent.Set();
			Thread.Sleep(250);
			this.workerthread.Abort();
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000C1FC File Offset: 0x0000A3FC
		private void Worker()
		{
			try
			{
				while (!this.shutdown)
				{
					int num;
					int num2;
					ThreadPool.GetAvailableThreads(out num, out num2);
					if (num <= this.maxWorkerThreads || num2 <= this.maxCompletionPortThreads)
					{
						Thread.Sleep(100);
					}
					else
					{
						WebRequests.WebRequest webRequest = null;
						object obj = this.syncroot;
						lock (obj)
						{
							if (this.queue.Count > 0)
							{
								webRequest = this.queue.Dequeue();
							}
						}
						if (webRequest != null)
						{
							webRequest.Start();
						}
						else
						{
							this.workevent.WaitOne();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Interface.Oxide.LogException("WebRequests worker: ", ex);
			}
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000C2B4 File Offset: 0x0000A4B4
		[LibraryFunction("EnqueueGet")]
		[Obsolete("EnqueueGet is deprecated, use Enqueue instead")]
		public void EnqueueGet(string url, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			this.Enqueue(url, null, callback, owner, RequestMethod.GET, headers, timeout);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000C2C5 File Offset: 0x0000A4C5
		[LibraryFunction("EnqueuePost")]
		[Obsolete("EnqueuePost is deprecated, use Enqueue instead")]
		public void EnqueuePost(string url, string body, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			this.Enqueue(url, body, callback, owner, RequestMethod.POST, headers, timeout);
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000C2D7 File Offset: 0x0000A4D7
		[LibraryFunction("EnqueuePut")]
		[Obsolete("EnqueuePut is deprecated, use Enqueue instead")]
		public void EnqueuePut(string url, string body, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			this.Enqueue(url, body, callback, owner, RequestMethod.PUT, headers, timeout);
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000C2EC File Offset: 0x0000A4EC
		[LibraryFunction("Enqueue")]
		public void Enqueue(string url, string body, Action<int, string> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			WebRequests.WebRequest item = new WebRequests.WebRequest(url, callback, owner)
			{
				Method = method.ToString(),
				RequestHeaders = headers,
				Timeout = timeout,
				Body = body
			};
			object obj = this.syncroot;
			lock (obj)
			{
				this.queue.Enqueue(item);
			}
			this.workevent.Set();
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000C36C File Offset: 0x0000A56C
		[LibraryFunction("GetQueueLength")]
		public int GetQueueLength()
		{
			return this.queue.Count;
		}

		// Token: 0x040000FA RID: 250
		private static readonly Covalence covalence = Interface.Oxide.GetLibrary<Covalence>(null);

		// Token: 0x040000FB RID: 251
		public static float Timeout = 30f;

		// Token: 0x040000FC RID: 252
		public static bool AllowDecompression = false;

		// Token: 0x040000FD RID: 253
		private readonly Queue<WebRequests.WebRequest> queue = new Queue<WebRequests.WebRequest>();

		// Token: 0x040000FE RID: 254
		private readonly object syncroot = new object();

		// Token: 0x040000FF RID: 255
		private readonly Thread workerthread;

		// Token: 0x04000100 RID: 256
		private readonly AutoResetEvent workevent = new AutoResetEvent(false);

		// Token: 0x04000101 RID: 257
		private bool shutdown;

		// Token: 0x04000102 RID: 258
		private readonly int maxWorkerThreads;

		// Token: 0x04000103 RID: 259
		private readonly int maxCompletionPortThreads;

		// Token: 0x02000091 RID: 145
		public class WebRequest
		{
			// Token: 0x170000AD RID: 173
			// (get) Token: 0x06000442 RID: 1090 RVA: 0x00010851 File Offset: 0x0000EA51
			public Action<int, string> Callback { get; }

			// Token: 0x170000AE RID: 174
			// (get) Token: 0x06000443 RID: 1091 RVA: 0x00010859 File Offset: 0x0000EA59
			// (set) Token: 0x06000444 RID: 1092 RVA: 0x00010861 File Offset: 0x0000EA61
			public float Timeout { get; set; }

			// Token: 0x170000AF RID: 175
			// (get) Token: 0x06000445 RID: 1093 RVA: 0x0001086A File Offset: 0x0000EA6A
			// (set) Token: 0x06000446 RID: 1094 RVA: 0x00010872 File Offset: 0x0000EA72
			public string Method { get; set; }

			// Token: 0x170000B0 RID: 176
			// (get) Token: 0x06000447 RID: 1095 RVA: 0x0001087B File Offset: 0x0000EA7B
			public string Url { get; }

			// Token: 0x170000B1 RID: 177
			// (get) Token: 0x06000448 RID: 1096 RVA: 0x00010883 File Offset: 0x0000EA83
			// (set) Token: 0x06000449 RID: 1097 RVA: 0x0001088B File Offset: 0x0000EA8B
			public string Body { get; set; }

			// Token: 0x170000B2 RID: 178
			// (get) Token: 0x0600044A RID: 1098 RVA: 0x00010894 File Offset: 0x0000EA94
			// (set) Token: 0x0600044B RID: 1099 RVA: 0x0001089C File Offset: 0x0000EA9C
			public int ResponseCode { get; protected set; }

			// Token: 0x170000B3 RID: 179
			// (get) Token: 0x0600044C RID: 1100 RVA: 0x000108A5 File Offset: 0x0000EAA5
			// (set) Token: 0x0600044D RID: 1101 RVA: 0x000108AD File Offset: 0x0000EAAD
			public string ResponseText { get; protected set; }

			// Token: 0x170000B4 RID: 180
			// (get) Token: 0x0600044E RID: 1102 RVA: 0x000108B6 File Offset: 0x0000EAB6
			// (set) Token: 0x0600044F RID: 1103 RVA: 0x000108BE File Offset: 0x0000EABE
			public Plugin Owner { get; protected set; }

			// Token: 0x170000B5 RID: 181
			// (get) Token: 0x06000450 RID: 1104 RVA: 0x000108C7 File Offset: 0x0000EAC7
			// (set) Token: 0x06000451 RID: 1105 RVA: 0x000108CF File Offset: 0x0000EACF
			public Dictionary<string, string> RequestHeaders { get; set; }

			// Token: 0x06000452 RID: 1106 RVA: 0x000108D8 File Offset: 0x0000EAD8
			public WebRequest(string url, Action<int, string> callback, Plugin owner)
			{
				this.Url = url;
				this.Callback = callback;
				this.Owner = owner;
				Plugin owner2 = this.Owner;
				this.removedFromManager = ((owner2 != null) ? owner2.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.owner_OnRemovedFromManager)) : null);
			}

			// Token: 0x06000453 RID: 1107 RVA: 0x0001092C File Offset: 0x0000EB2C
			public void Start()
			{
				try
				{
					this.request = (HttpWebRequest)System.Net.WebRequest.Create(this.Url);
					this.request.Method = this.Method;
					this.request.Credentials = CredentialCache.DefaultCredentials;
					this.request.Proxy = null;
					this.request.KeepAlive = false;
					this.request.Timeout = (int)Math.Round((double)((this.Timeout.Equals(0f) ? WebRequests.Timeout : this.Timeout) * 1000f));
					this.request.AutomaticDecompression = (WebRequests.AllowDecompression ? (DecompressionMethods.GZip | DecompressionMethods.Deflate) : DecompressionMethods.None);
					this.request.ServicePoint.MaxIdleTime = this.request.Timeout;
					this.request.ServicePoint.Expect100Continue = ServicePointManager.Expect100Continue;
					this.request.ServicePoint.ConnectionLimit = ServicePointManager.DefaultConnectionLimit;
					if (!this.request.RequestUri.IsLoopback && Environment.OSVersion.Platform != PlatformID.Unix)
					{
						this.request.ServicePoint.BindIPEndPointDelegate = ((ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount) => new IPEndPoint(WebRequests.covalence.Server.LocalAddress ?? WebRequests.covalence.Server.Address, 0));
					}
					byte[] data = new byte[0];
					if (this.Body != null)
					{
						data = Encoding.UTF8.GetBytes(this.Body);
						this.request.ContentLength = (long)data.Length;
						this.request.ContentType = "application/x-www-form-urlencoded";
					}
					if (this.RequestHeaders != null)
					{
						this.request.SetRawHeaders(this.RequestHeaders);
					}
					if (data.Length != 0)
					{
						this.request.BeginGetRequestStream(delegate(IAsyncResult result)
						{
							if (this.request == null)
							{
								return;
							}
							try
							{
								using (Stream stream = this.request.EndGetRequestStream(result))
								{
									stream.Write(data, 0, data.Length);
								}
							}
							catch (Exception exception)
							{
								this.ResponseText = WebRequests.FormatWebException(exception, this.ResponseText ?? string.Empty);
								HttpWebRequest httpWebRequest2 = this.request;
								if (httpWebRequest2 != null)
								{
									httpWebRequest2.Abort();
								}
								this.OnComplete();
								return;
							}
							this.WaitForResponse();
						}, null);
					}
					else
					{
						this.WaitForResponse();
					}
				}
				catch (Exception ex)
				{
					this.ResponseText = WebRequests.FormatWebException(ex, this.ResponseText ?? string.Empty);
					string text = "Web request produced exception (Url: " + this.Url + ")";
					if (this.Owner)
					{
						text += string.Format(" in '{0} v{1}' plugin", this.Owner.Name, this.Owner.Version);
					}
					Interface.Oxide.LogException(text, ex);
					HttpWebRequest httpWebRequest = this.request;
					if (httpWebRequest != null)
					{
						httpWebRequest.Abort();
					}
					this.OnComplete();
				}
			}

			// Token: 0x06000454 RID: 1108 RVA: 0x00010BBC File Offset: 0x0000EDBC
			private void WaitForResponse()
			{
				IAsyncResult asyncResult = this.request.BeginGetResponse(delegate(IAsyncResult res)
				{
					try
					{
						using (HttpWebResponse httpWebResponse = (HttpWebResponse)this.request.EndGetResponse(res))
						{
							using (Stream responseStream = httpWebResponse.GetResponseStream())
							{
								using (StreamReader streamReader = new StreamReader(responseStream))
								{
									this.ResponseText = streamReader.ReadToEnd();
								}
							}
							this.ResponseCode = (int)httpWebResponse.StatusCode;
						}
					}
					catch (WebException ex)
					{
						this.ResponseText = WebRequests.FormatWebException(ex, this.ResponseText ?? string.Empty);
						HttpWebResponse httpWebResponse2 = ex.Response as HttpWebResponse;
						if (httpWebResponse2 != null)
						{
							try
							{
								using (Stream responseStream2 = httpWebResponse2.GetResponseStream())
								{
									using (StreamReader streamReader2 = new StreamReader(responseStream2))
									{
										this.ResponseText = streamReader2.ReadToEnd();
									}
								}
							}
							catch (Exception)
							{
							}
							this.ResponseCode = (int)httpWebResponse2.StatusCode;
						}
					}
					catch (Exception ex2)
					{
						this.ResponseText = WebRequests.FormatWebException(ex2, this.ResponseText ?? string.Empty);
						string text = "Web request produced exception (Url: " + this.Url + ")";
						if (this.Owner)
						{
							text += string.Format(" in '{0} v{1}' plugin", this.Owner.Name, this.Owner.Version);
						}
						Interface.Oxide.LogException(text, ex2);
					}
					if (this.request == null)
					{
						return;
					}
					this.request.Abort();
					this.OnComplete();
				}, null);
				this.waitHandle = asyncResult.AsyncWaitHandle;
				this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.waitHandle, new WaitOrTimerCallback(this.OnTimeout), null, this.request.Timeout, true);
			}

			// Token: 0x06000455 RID: 1109 RVA: 0x00010C18 File Offset: 0x0000EE18
			private void OnTimeout(object state, bool timedOut)
			{
				if (timedOut)
				{
					HttpWebRequest httpWebRequest = this.request;
					if (httpWebRequest != null)
					{
						httpWebRequest.Abort();
					}
				}
				if (this.Owner == null)
				{
					return;
				}
				Event.Remove<Plugin, PluginManager>(ref this.removedFromManager);
				this.Owner = null;
			}

			// Token: 0x06000456 RID: 1110 RVA: 0x00010C49 File Offset: 0x0000EE49
			private void OnComplete()
			{
				Event.Remove<Plugin, PluginManager>(ref this.removedFromManager);
				RegisteredWaitHandle registeredWaitHandle = this.registeredWaitHandle;
				if (registeredWaitHandle != null)
				{
					registeredWaitHandle.Unregister(this.waitHandle);
				}
				Interface.Oxide.NextTick(delegate
				{
					if (this.request == null)
					{
						return;
					}
					this.request = null;
					Plugin owner = this.Owner;
					if (owner != null)
					{
						owner.TrackStart();
					}
					try
					{
						this.Callback(this.ResponseCode, this.ResponseText);
					}
					catch (Exception ex)
					{
						string text = "Web request callback raised an exception";
						if (this.Owner && this.Owner != null)
						{
							text += string.Format(" in '{0} v{1}' plugin", this.Owner.Name, this.Owner.Version);
						}
						Interface.Oxide.LogException(text, ex);
					}
					Plugin owner2 = this.Owner;
					if (owner2 != null)
					{
						owner2.TrackEnd();
					}
					this.Owner = null;
				});
			}

			// Token: 0x06000457 RID: 1111 RVA: 0x00010C84 File Offset: 0x0000EE84
			private void owner_OnRemovedFromManager(Plugin sender, PluginManager manager)
			{
				if (this.request == null)
				{
					return;
				}
				System.Net.WebRequest webRequest = this.request;
				this.request = null;
				webRequest.Abort();
			}

			// Token: 0x040001EF RID: 495
			private HttpWebRequest request;

			// Token: 0x040001F0 RID: 496
			private WaitHandle waitHandle;

			// Token: 0x040001F1 RID: 497
			private RegisteredWaitHandle registeredWaitHandle;

			// Token: 0x040001F2 RID: 498
			private Event.Callback<Plugin, PluginManager> removedFromManager;
		}
	}
}
