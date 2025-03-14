using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x02000048 RID: 72
	public class Formatter
	{
		// Token: 0x0600029E RID: 670 RVA: 0x0000CB30 File Offset: 0x0000AD30
		private static List<Element> Parse(List<Formatter.Token> tokens)
		{
			int i = 0;
			Stack<Formatter.Entry> s = new Stack<Formatter.Entry>();
			s.Push(new Formatter.Entry(null, Element.Tag(ElementType.String)));
			while (i < tokens.Count)
			{
				Formatter.Token t = tokens[i++];
				Action<Element> action = delegate(Element el)
				{
					s.Push(new Formatter.Entry(t.Pattern, el));
				};
				Element element = s.Peek().Element;
				Formatter.TokenType type = t.Type;
				Formatter.TokenType? tokenType = Formatter.closeTags[element.Type];
				if (type == tokenType.GetValueOrDefault() & tokenType != null)
				{
					s.Pop();
					s.Peek().Element.Body.Add(element);
				}
				else
				{
					switch (t.Type)
					{
					case Formatter.TokenType.String:
						element.Body.Add(Element.String(t.Val));
						break;
					case Formatter.TokenType.Bold:
						action(Element.Tag(ElementType.Bold));
						break;
					case Formatter.TokenType.Italic:
						action(Element.Tag(ElementType.Italic));
						break;
					case Formatter.TokenType.Color:
						action(Element.ParamTag(ElementType.Color, t.Val));
						break;
					case Formatter.TokenType.Size:
						action(Element.ParamTag(ElementType.Size, t.Val));
						break;
					default:
						element.Body.Add(Element.String(t.Pattern));
						break;
					}
				}
			}
			while (s.Count > 1)
			{
				Formatter.Entry entry = s.Pop();
				List<Element> body = s.Peek().Element.Body;
				body.Add(Element.String(entry.Pattern));
				body.AddRange(entry.Element.Body);
			}
			return s.Pop().Element.Body;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000CD3E File Offset: 0x0000AF3E
		public static List<Element> Parse(string text)
		{
			return Formatter.Parse(Formatter.Lexer.Lex(text));
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000CD4C File Offset: 0x0000AF4C
		private static Formatter.Tag Translation(Element e, Dictionary<ElementType, Func<object, Formatter.Tag>> translations)
		{
			Func<object, Formatter.Tag> func;
			if (!translations.TryGetValue(e.Type, out func))
			{
				return new Formatter.Tag("", "");
			}
			return func(e.Val);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000CD88 File Offset: 0x0000AF88
		private static string ToTreeFormat(List<Element> tree, Dictionary<ElementType, Func<object, Formatter.Tag>> translations)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Element element in tree)
			{
				if (element.Type == ElementType.String)
				{
					stringBuilder.Append(element.Val);
				}
				else
				{
					Formatter.Tag tag = Formatter.Translation(element, translations);
					stringBuilder.Append(tag.Open);
					stringBuilder.Append(Formatter.ToTreeFormat(element.Body, translations));
					stringBuilder.Append(tag.Close);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000CE28 File Offset: 0x0000B028
		private static string ToTreeFormat(string text, Dictionary<ElementType, Func<object, Formatter.Tag>> translations)
		{
			return Formatter.ToTreeFormat(Formatter.Parse(text), translations);
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000CE36 File Offset: 0x0000B036
		private static string RGBAtoRGB(object rgba)
		{
			return rgba.ToString().Substring(0, 6);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000CE45 File Offset: 0x0000B045
		public static string ToPlaintext(string text)
		{
			return Formatter.ToTreeFormat(text, new Dictionary<ElementType, Func<object, Formatter.Tag>>());
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000CE54 File Offset: 0x0000B054
		public static string ToUnity(string text)
		{
			Dictionary<ElementType, Func<object, Formatter.Tag>> dictionary = new Dictionary<ElementType, Func<object, Formatter.Tag>>();
			dictionary[ElementType.Bold] = ((object _) => new Formatter.Tag("<b>", "</b>"));
			dictionary[ElementType.Italic] = ((object _) => new Formatter.Tag("<i>", "</i>"));
			dictionary[ElementType.Color] = ((object c) => new Formatter.Tag(string.Format("<color=#{0}>", c), "</color>"));
			dictionary[ElementType.Size] = ((object s) => new Formatter.Tag(string.Format("<size={0}>", s), "</size>"));
			return Formatter.ToTreeFormat(text, dictionary);
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000CF04 File Offset: 0x0000B104
		public static string ToRustLegacy(string text)
		{
			Dictionary<ElementType, Func<object, Formatter.Tag>> dictionary = new Dictionary<ElementType, Func<object, Formatter.Tag>>();
			dictionary[ElementType.Color] = ((object c) => new Formatter.Tag("[color #" + Formatter.RGBAtoRGB(c) + "]", "[color #ffffff]"));
			return Formatter.ToTreeFormat(text, dictionary);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000CF37 File Offset: 0x0000B137
		public static string ToRoKAnd7DTD(string text)
		{
			Dictionary<ElementType, Func<object, Formatter.Tag>> dictionary = new Dictionary<ElementType, Func<object, Formatter.Tag>>();
			dictionary[ElementType.Color] = ((object c) => new Formatter.Tag("[" + Formatter.RGBAtoRGB(c) + "]", "[e7e7e7]"));
			return Formatter.ToTreeFormat(text, dictionary);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000CF6A File Offset: 0x0000B16A
		public static string ToTerraria(string text)
		{
			Dictionary<ElementType, Func<object, Formatter.Tag>> dictionary = new Dictionary<ElementType, Func<object, Formatter.Tag>>();
			dictionary[ElementType.Color] = ((object c) => new Formatter.Tag("[c/" + Formatter.RGBAtoRGB(c) + ":", "]"));
			return Formatter.ToTreeFormat(text, dictionary);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000CFA8 File Offset: 0x0000B1A8
		// Note: this type is marked as 'beforefieldinit'.
		static Formatter()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["aqua"] = "00ffff";
			dictionary["black"] = "000000";
			dictionary["blue"] = "0000ff";
			dictionary["brown"] = "a52a2a";
			dictionary["cyan"] = "00ffff";
			dictionary["darkblue"] = "0000a0";
			dictionary["fuchsia"] = "ff00ff";
			dictionary["green"] = "008000";
			dictionary["grey"] = "808080";
			dictionary["lightblue"] = "add8e6";
			dictionary["lime"] = "00ff00";
			dictionary["magenta"] = "ff00ff";
			dictionary["maroon"] = "800000";
			dictionary["navy"] = "000080";
			dictionary["olive"] = "808000";
			dictionary["orange"] = "ffa500";
			dictionary["purple"] = "800080";
			dictionary["red"] = "ff0000";
			dictionary["silver"] = "c0c0c0";
			dictionary["teal"] = "008080";
			dictionary["white"] = "ffffff";
			dictionary["yellow"] = "ffff00";
			Formatter.colorNames = dictionary;
			Dictionary<ElementType, Formatter.TokenType?> dictionary2 = new Dictionary<ElementType, Formatter.TokenType?>();
			dictionary2[ElementType.String] = null;
			dictionary2[ElementType.Bold] = new Formatter.TokenType?(Formatter.TokenType.CloseBold);
			dictionary2[ElementType.Italic] = new Formatter.TokenType?(Formatter.TokenType.CloseItalic);
			dictionary2[ElementType.Color] = new Formatter.TokenType?(Formatter.TokenType.CloseColor);
			dictionary2[ElementType.Size] = new Formatter.TokenType?(Formatter.TokenType.CloseSize);
			Formatter.closeTags = dictionary2;
		}

		// Token: 0x04000116 RID: 278
		private static readonly Dictionary<string, string> colorNames;

		// Token: 0x04000117 RID: 279
		private static readonly Dictionary<ElementType, Formatter.TokenType?> closeTags;

		// Token: 0x02000094 RID: 148
		private class Token
		{
			// Token: 0x040001F7 RID: 503
			public Formatter.TokenType Type;

			// Token: 0x040001F8 RID: 504
			public object Val;

			// Token: 0x040001F9 RID: 505
			public string Pattern;
		}

		// Token: 0x02000095 RID: 149
		private enum TokenType
		{
			// Token: 0x040001FB RID: 507
			String,
			// Token: 0x040001FC RID: 508
			Bold,
			// Token: 0x040001FD RID: 509
			Italic,
			// Token: 0x040001FE RID: 510
			Color,
			// Token: 0x040001FF RID: 511
			Size,
			// Token: 0x04000200 RID: 512
			CloseBold,
			// Token: 0x04000201 RID: 513
			CloseItalic,
			// Token: 0x04000202 RID: 514
			CloseColor,
			// Token: 0x04000203 RID: 515
			CloseSize
		}

		// Token: 0x02000096 RID: 150
		private class Lexer
		{
			// Token: 0x06000461 RID: 1121 RVA: 0x00010FD8 File Offset: 0x0000F1D8
			private char Current()
			{
				return this.text[this.position];
			}

			// Token: 0x06000462 RID: 1122 RVA: 0x00010FEB File Offset: 0x0000F1EB
			private void Next()
			{
				this.position++;
			}

			// Token: 0x06000463 RID: 1123 RVA: 0x00010FFB File Offset: 0x0000F1FB
			private void StartNewToken()
			{
				this.tokenStart = this.position;
			}

			// Token: 0x06000464 RID: 1124 RVA: 0x00011009 File Offset: 0x0000F209
			private void StartNewPattern()
			{
				this.patternStart = this.position;
				this.StartNewToken();
			}

			// Token: 0x06000465 RID: 1125 RVA: 0x0001101D File Offset: 0x0000F21D
			private void Reset()
			{
				this.tokenStart = this.patternStart;
			}

			// Token: 0x06000466 RID: 1126 RVA: 0x0001102B File Offset: 0x0000F22B
			private string Token()
			{
				return this.text.Substring(this.tokenStart, this.position - this.tokenStart);
			}

			// Token: 0x06000467 RID: 1127 RVA: 0x0001104C File Offset: 0x0000F24C
			private void Add(Formatter.TokenType type, object val = null)
			{
				Formatter.Token item = new Formatter.Token
				{
					Type = type,
					Val = val,
					Pattern = this.text.Substring(this.patternStart, this.position - this.patternStart)
				};
				this.tokens.Add(item);
			}

			// Token: 0x06000468 RID: 1128 RVA: 0x000110A0 File Offset: 0x0000F2A0
			private void WritePatternString()
			{
				if (this.patternStart >= this.position)
				{
					return;
				}
				int num = this.tokenStart;
				this.tokenStart = this.patternStart;
				this.Add(Formatter.TokenType.String, this.Token());
				this.tokenStart = num;
			}

			// Token: 0x06000469 RID: 1129 RVA: 0x000110E3 File Offset: 0x0000F2E3
			private static bool IsValidColorCode(string val)
			{
				if (val.Length == 6 || val.Length == 8)
				{
					return val.All((char c) => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
				}
				return false;
			}

			// Token: 0x0600046A RID: 1130 RVA: 0x00011120 File Offset: 0x0000F320
			private static object ParseColor(string val)
			{
				string text;
				if (!Formatter.colorNames.TryGetValue(val.ToLower(), out text) && !Formatter.Lexer.IsValidColorCode(val))
				{
					return null;
				}
				text = (text ?? val);
				if (text.Length == 6)
				{
					text += "ff";
				}
				return text;
			}

			// Token: 0x0600046B RID: 1131 RVA: 0x00011168 File Offset: 0x0000F368
			private static object ParseSize(string val)
			{
				int num;
				if (int.TryParse(val, out num))
				{
					return num;
				}
				return null;
			}

			// Token: 0x0600046C RID: 1132 RVA: 0x00011187 File Offset: 0x0000F387
			private Formatter.Lexer.State EndTag(Formatter.TokenType t)
			{
				this.Next();
				return delegate()
				{
					if (this.Current() == ']')
					{
						this.Next();
						this.Add(t, null);
						this.StartNewPattern();
						return new Formatter.Lexer.State(this.Str);
					}
					this.Reset();
					return new Formatter.Lexer.State(this.Str);
				};
			}

			// Token: 0x0600046D RID: 1133 RVA: 0x000111B0 File Offset: 0x0000F3B0
			private Formatter.Lexer.State ParamTag(Formatter.TokenType t, Func<string, object> parse)
			{
				this.Next();
				this.StartNewToken();
				Formatter.Lexer.State s = null;
				s = delegate()
				{
					if (this.Current() != ']')
					{
						this.Next();
						return s;
					}
					object obj = parse(this.Token());
					if (obj == null)
					{
						this.Reset();
						return new Formatter.Lexer.State(this.Str);
					}
					this.Next();
					this.Add(t, obj);
					this.StartNewPattern();
					return new Formatter.Lexer.State(this.Str);
				};
				return s;
			}

			// Token: 0x0600046E RID: 1134 RVA: 0x00011204 File Offset: 0x0000F404
			private Formatter.Lexer.State CloseTag()
			{
				char c = this.Current();
				if (c <= '+')
				{
					if (c == '#')
					{
						return this.EndTag(Formatter.TokenType.CloseColor);
					}
					if (c == '+')
					{
						return this.EndTag(Formatter.TokenType.CloseSize);
					}
				}
				else
				{
					if (c == 'b')
					{
						return this.EndTag(Formatter.TokenType.CloseBold);
					}
					if (c == 'i')
					{
						return this.EndTag(Formatter.TokenType.CloseItalic);
					}
				}
				this.Reset();
				return new Formatter.Lexer.State(this.Str);
			}

			// Token: 0x0600046F RID: 1135 RVA: 0x00011268 File Offset: 0x0000F468
			private Formatter.Lexer.State Tag()
			{
				char c = this.Current();
				if (c <= '+')
				{
					if (c == '#')
					{
						return this.ParamTag(Formatter.TokenType.Color, new Func<string, object>(Formatter.Lexer.ParseColor));
					}
					if (c == '+')
					{
						return this.ParamTag(Formatter.TokenType.Size, new Func<string, object>(Formatter.Lexer.ParseSize));
					}
				}
				else
				{
					if (c == '/')
					{
						this.Next();
						return new Formatter.Lexer.State(this.CloseTag);
					}
					if (c == 'b')
					{
						return this.EndTag(Formatter.TokenType.Bold);
					}
					if (c == 'i')
					{
						return this.EndTag(Formatter.TokenType.Italic);
					}
				}
				this.Reset();
				return new Formatter.Lexer.State(this.Str);
			}

			// Token: 0x06000470 RID: 1136 RVA: 0x000112FB File Offset: 0x0000F4FB
			private Formatter.Lexer.State Str()
			{
				if (this.Current() == '[')
				{
					this.WritePatternString();
					this.StartNewPattern();
					this.Next();
					return new Formatter.Lexer.State(this.Tag);
				}
				this.Next();
				return new Formatter.Lexer.State(this.Str);
			}

			// Token: 0x06000471 RID: 1137 RVA: 0x00011338 File Offset: 0x0000F538
			public static List<Formatter.Token> Lex(string text)
			{
				Formatter.Lexer lexer = new Formatter.Lexer
				{
					text = text
				};
				Formatter.Lexer.State state = new Formatter.Lexer.State(lexer.Str);
				while (lexer.position < lexer.text.Length)
				{
					state = state();
				}
				lexer.WritePatternString();
				return lexer.tokens;
			}

			// Token: 0x04000204 RID: 516
			private string text;

			// Token: 0x04000205 RID: 517
			private int patternStart;

			// Token: 0x04000206 RID: 518
			private int tokenStart;

			// Token: 0x04000207 RID: 519
			private int position;

			// Token: 0x04000208 RID: 520
			private List<Formatter.Token> tokens = new List<Formatter.Token>();

			// Token: 0x020000B0 RID: 176
			// (Invoke) Token: 0x060004BB RID: 1211
			private delegate Formatter.Lexer.State State();
		}

		// Token: 0x02000097 RID: 151
		private class Entry
		{
			// Token: 0x06000473 RID: 1139 RVA: 0x0001139A File Offset: 0x0000F59A
			public Entry(string pattern, Element e)
			{
				this.Pattern = pattern;
				this.Element = e;
			}

			// Token: 0x04000209 RID: 521
			public string Pattern;

			// Token: 0x0400020A RID: 522
			public Element Element;
		}

		// Token: 0x02000098 RID: 152
		private class Tag
		{
			// Token: 0x06000474 RID: 1140 RVA: 0x000113B0 File Offset: 0x0000F5B0
			public Tag(string open, string close)
			{
				this.Open = open;
				this.Close = close;
			}

			// Token: 0x0400020B RID: 523
			public string Open;

			// Token: 0x0400020C RID: 524
			public string Close;
		}
	}
}
