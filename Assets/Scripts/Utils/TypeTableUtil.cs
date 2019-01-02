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
		/// <param name="obj">Obj to convert</param>
		public static uint ObjectToTypeId(IExtensible obj)
		{
			System.Type type = obj.GetType();
			switch (type.ToString()) {
				case "App.Proto.AccessCode": return 3954737699; // ebb88223
				case "App.Proto.RequestAccessTokenParameter": return 1479466557; // 582ede3d
				case "App.Proto.Character": return 4003022536; // ee9946c8
				case "App.Proto.CharacterStatus": return 1708509719; // 65d5ca17
				case "App.Proto.HavingCharacter": return 3970969170; // ecb02e52
				case "App.Proto.Error": return 2133813781; // 7f2f6a15
				case "App.Proto.Item": return 3973948826; // ecdda59a
				case "App.Proto.BeginMatchParameter": return 3623522443; // d7fa908b
				case "App.Proto.EndMatchParameter": return 2294637224; // 88c562a8
				case "App.Proto.LeaderBoard": return 2860697096; // aa82c608
				case "App.Proto.ProtobufMessage": return 3854281208; // e5bba9f8
				case "App.Proto.ProtobufMessages": return 4218599789; // fb72b96d
				case "App.Proto.UpdateSettingParameter": return 1287612623; // 4cbf68cf
				case "App.Proto.User": return 2676630409; // 9f8a2389
				case "App.Proto.Wave": return 2722979857; // a24d6011
				case "App.Proto.WaveItem": return 316749983; // 12e1389f
				case "App.Proto.WaveListResult": return 3029536267; // b4930e0b
				case "App.Proto.WaveZombie": return 48367679; // 02e2083f
				case "App.Proto.ZombiePosition": return 2594804636; // 9aa9939c
				case "App.Proto.Weapon": return 3939840872; // ead53368
				case "App.Proto.WeaponGroup": return 2152343043; // 804a2603
				case "App.Proto.DropItem": return 105888409; // 064fba99
				case "App.Proto.Zombie": return 1225470534; // 490b3246

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
				case 3954737699: return new App.Proto.AccessCode(); //ebb88223
				case 1479466557: return new App.Proto.RequestAccessTokenParameter(); //582ede3d
				case 4003022536: return new App.Proto.Character(); //ee9946c8
				case 1708509719: return new App.Proto.CharacterStatus(); //65d5ca17
				case 3970969170: return new App.Proto.HavingCharacter(); //ecb02e52
				case 2133813781: return new App.Proto.Error(); //7f2f6a15
				case 3973948826: return new App.Proto.Item(); //ecdda59a
				case 3623522443: return new App.Proto.BeginMatchParameter(); //d7fa908b
				case 2294637224: return new App.Proto.EndMatchParameter(); //88c562a8
				case 2860697096: return new App.Proto.LeaderBoard(); //aa82c608
				case 3854281208: return new App.Proto.ProtobufMessage(); //e5bba9f8
				case 4218599789: return new App.Proto.ProtobufMessages(); //fb72b96d
				case 1287612623: return new App.Proto.UpdateSettingParameter(); //4cbf68cf
				case 2676630409: return new App.Proto.User(); //9f8a2389
				case 2722979857: return new App.Proto.Wave(); //a24d6011
				case 316749983: return new App.Proto.WaveItem(); //12e1389f
				case 3029536267: return new App.Proto.WaveListResult(); //b4930e0b
				case 48367679: return new App.Proto.WaveZombie(); //02e2083f
				case 2594804636: return new App.Proto.ZombiePosition(); //9aa9939c
				case 3939840872: return new App.Proto.Weapon(); //ead53368
				case 2152343043: return new App.Proto.WeaponGroup(); //804a2603
				case 105888409: return new App.Proto.DropItem(); //064fba99
				case 1225470534: return new App.Proto.Zombie(); //490b3246

			}
			throw new Exception("unknown type id : " + typeId);
		}
	}
}

