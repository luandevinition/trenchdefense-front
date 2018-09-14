using UnityEngine;
using System.Linq;
using System;
using UnityObject = UnityEngine.Object;
using System.Collections.Generic;
using BattleStage.Domain;

namespace Utils
{
    public static class ListExtension
    {
        public static Zombie GetRandomZombie(this List<Zombie> list)
        {
            if (list == null || list.Count <=0)
                return null;
            
            return list.OrderBy(a => Guid.NewGuid()).ToList().FirstOrDefault();
        }
    }
}