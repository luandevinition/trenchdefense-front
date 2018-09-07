using System.Text;

namespace Utils
{
	/// <summary>
	/// 16進数に関するユーテリティ
	/// </summary>
	public static class HEXUtil
	{

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

