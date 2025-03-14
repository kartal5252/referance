using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Oxide.Core.Libraries
{
	// Token: 0x02000042 RID: 66
	public static class HttpWebRequestExtensions
	{
		// Token: 0x06000281 RID: 641 RVA: 0x0000C39C File Offset: 0x0000A59C
		static HttpWebRequestExtensions()
		{
			Type typeFromHandle = typeof(HttpWebRequest);
			foreach (string text in HttpWebRequestExtensions.RestrictedHeaders)
			{
				HttpWebRequestExtensions.HeaderProperties[text] = typeFromHandle.GetProperty(text.Replace("-", ""));
			}
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000C480 File Offset: 0x0000A680
		public static void SetRawHeaders(this WebRequest request, Dictionary<string, string> headers)
		{
			foreach (KeyValuePair<string, string> keyValuePair in headers)
			{
				request.SetRawHeader(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000C4DC File Offset: 0x0000A6DC
		public static void SetRawHeader(this WebRequest request, string name, string value)
		{
			if (!HttpWebRequestExtensions.HeaderProperties.ContainsKey(name))
			{
				request.Headers[name] = value;
				return;
			}
			PropertyInfo propertyInfo = HttpWebRequestExtensions.HeaderProperties[name];
			if (propertyInfo.PropertyType == typeof(DateTime))
			{
				propertyInfo.SetValue(request, DateTime.Parse(value), null);
				return;
			}
			if (propertyInfo.PropertyType == typeof(bool))
			{
				propertyInfo.SetValue(request, bool.Parse(value), null);
				return;
			}
			if (propertyInfo.PropertyType == typeof(long))
			{
				propertyInfo.SetValue(request, long.Parse(value), null);
				return;
			}
			propertyInfo.SetValue(request, value, null);
		}

		// Token: 0x04000104 RID: 260
		private static readonly string[] RestrictedHeaders = new string[]
		{
			"Accept",
			"Connection",
			"Content-Length",
			"Content-Type",
			"Date",
			"Expect",
			"Host",
			"If-Modified-Since",
			"Keep-Alive",
			"Proxy-Connection",
			"Range",
			"Referer",
			"Transfer-Encoding",
			"User-Agent"
		};

		// Token: 0x04000105 RID: 261
		private static readonly Dictionary<string, PropertyInfo> HeaderProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
	}
}
