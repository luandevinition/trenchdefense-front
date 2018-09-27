using UnityEngine;

namespace StaticAssets.Configs
{
	[CreateAssetMenu(fileName="SettingData" ,menuName="Setting Data")]
	public class SettingData : ScriptableObject {

		#region PUBLIC_VERIABLE

		public string url;

		public bool keepUsingOldToken;
		
		public string imei;
		
		#endregion
	}
}