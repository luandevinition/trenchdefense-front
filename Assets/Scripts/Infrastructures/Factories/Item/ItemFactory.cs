using System;
using BattleStage.Domain;

namespace Infrastructures.Factories.Item
{
    public class ItemFactory
    {
        public static BattleStage.Domain.Item Make(App.Proto.Item dto)
        {
            return new BattleStage.Domain.Item((int) dto.id, dto.name, (ItemType) Enum.Parse(typeof(ItemType), dto.type, true), (int) dto.count , dto.resourceID);
        }
        
        public static BattleStage.Domain.DropItem Make(App.Proto.DropItem dto)
        {
            return new BattleStage.Domain.DropItem(Make(dto.item),dto.dropRate);
        }
    }
}