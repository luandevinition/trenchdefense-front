using BattleStage.Domain;
using UnityEngine;

namespace UI.Views.Parts
{
	public class DropItemView : MonoBehaviour
	{
		public Item DropItemData;

		public void Bind(Item dropItem)
		{
			DropItemData = dropItem;
		}
	}
}
