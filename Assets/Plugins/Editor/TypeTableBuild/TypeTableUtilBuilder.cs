using System;
using System.Security.Cryptography;
using System.Text;

namespace Plugins.Editor.TypeTableBuild
{
	/// <summary>
	/// Protobufの型テーブルを作成するスクリプトのソース
	/// mcs Editor/TypeTableBuild/TypeTableUtilBuilder.cs でコンパイルしてから
	/// sh build-proto.sh /path/to/fencer-protocol をしてください
	/// </summary>
	public class TypeTableUtilBuilder
	{
		public static void Main(string[] args)
		{
			//fixme うまい書き方求む
			string tpl = @"using ProtoBuf;
using System;

namespace Utils
{
    /// Please fix Plugins.Editor.TypeTableBuild/TypeTableUtilBuilder.cs
	/// <summary>
	/// protobufの型テーブルに関するユーテリティ
	/// </summary>
	public static class TypeTableUtil
	{
		/// <summary>
		/// ObjectToTypeId
		/// </summary>
		/// <returns>型ID</returns>
		/// <param name=""obj"">モデル</param>
		public static uint ObjectToTypeId(IExtensible obj)
		{
			System.Type type = obj.GetType();
			switch (type.ToString()) {
";
			foreach(string modelName in args){
				string hex = TypeIdFromModelName (modelName);
				tpl += string.Format("\t\t\t\tcase \"{0}\": return {1}; // {2}\n", "App.Proto."+modelName, Convert.ToUInt32(hex,16), hex);
			}
			tpl += @"
			}
			throw new Exception(""unknown object type"");
		}

		/// <summary>
		/// TypeIdToObject
		/// </summary>
		/// <returns>モデルのオブジェクト</returns>
		/// <param name=""typeId"">型ID</param>
		public static IExtensible TypeIdToObject(uint typeId)
		{
			switch (typeId) {
";
			foreach(string modelName in args){
				string hex = TypeIdFromModelName (modelName);
				tpl += string.Format("\t\t\t\tcase {0}: return new App.Proto.{1}(); //{2}\n", Convert.ToUInt32(hex, 16), modelName, hex);
			}
			tpl += @"
			}
			throw new Exception(""unknown type id"");
		}
	}
}
";
			Console.WriteLine (tpl);
		}

		public static string TypeIdFromModelName(string modelName)
		{
			string hash = HashStringForUTF8String (modelName);
			return hash.Substring (0,8);
		}

		/// <summary>
		/// UTF8文字列をSHA1アルゴリズムでハッシュする
		/// </summary>
		/// <returns>16進数の文字列</returns>
		/// <param name="str">対象文字列</param>
		public static string HashStringForUTF8String(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);

			var sha1 = SHA1.Create();
			byte[] hashBytes = sha1.ComputeHash(bytes);

			return HexStringFromBytes (hashBytes);
		}

		/// <summary>
		/// バイトを16進数の文字列に変換する
		/// </summary>
		/// <returns>The string from bytes.</returns>
		/// <param name="bytes">Bytes.</param>
		public static string HexStringFromBytes(byte[] bytes)
		{
			var sb = new StringBuilder();
			foreach (byte b in bytes)
			{
				var hex = b.ToString("x2");
				sb.Append(hex);
			}
			return sb.ToString();
		}
	}
}
