using ProtoBuf;
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
		/// <param name="obj">モデル</param>
		public static uint ObjectToTypeId(IExtensible obj)
		{
			System.Type type = obj.GetType();
			switch (type.ToString()) {
				case "App.Proto.Authenticate": return 2493520340; // 94a019d4
				case "App.Proto.AuthenticateParameter": return 3452663711; // cdcb779f
				case "App.Proto.Type": return 1038840918; // 3deb7456
				case "App.Proto.Error": return 2133813781; // 7f2f6a15
				case "App.Proto.ProtobufMessage": return 3854281208; // e5bba9f8
				case "App.Proto.ProtobufMessages": return 4218599789; // fb72b96d

			}
			throw new Exception("unknown object type");
		}

		/// <summary>
		/// TypeIdToObject
		/// </summary>
		/// <returns>モデルのオブジェクト</returns>
		/// <param name="typeId">型ID</param>
		public static IExtensible TypeIdToObject(uint typeId)
		{
			switch (typeId) {
				case 2493520340: return new App.Proto.Authenticate(); //94a019d4
				case 3452663711: return new App.Proto.AuthenticateParameter(); //cdcb779f
				case 1038840918: return new App.Proto.Type(); //3deb7456
				case 2133813781: return new App.Proto.Error(); //7f2f6a15
				case 3854281208: return new App.Proto.ProtobufMessage(); //e5bba9f8
				case 4218599789: return new App.Proto.ProtobufMessages(); //fb72b96d

			}
			throw new Exception("unknown type id");
		}
	}
}

