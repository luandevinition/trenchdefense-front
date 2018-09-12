using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Scripts.Settings.Sound
{
    public enum CommonSoundType
    {
        CommonSE1,
        CommonSE2,
        CommonSE3,
        SpecialSE1,
        SpecialSE2,
        SpecialSE3,
        SpecialSE4,
        SpecialSE5,
        SpecialSE6,
        SpecialSE7,
        SpecialSE8,
        SpecialSE9,
        SpecialSE10,
        SpecialSE11,
        SpecialSE12,
        SpecialSE13,
        SpecialSE14,
        SpecialSE15,
        SpecialSE16,
        SpecialSE17,
        SpecialSE18,
        SpecialSE19,
        SpecialSE20,
        SpecialSE21,
        SpecialSE22,
        None,
    }


    public enum CommonSeKind
    {
        Common,
        Special,
    };
    
    /// <summary>
    /// 汎用サウンドの一覧
    /// </summary>
    public class CommonSoundList : ScriptableObject
    {
        private const int COMMONSE_NUM = 3;
        /// <summary>
        /// CommonSEの種類を取得
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CommonSeKind GetAssetBundleKind(CommonSoundType type)
        {
            if (type >= CommonSoundType.SpecialSE1)
            {
                return CommonSeKind.Special;
            }

            return CommonSeKind.Common;
        }

        
        /// <summary>
        /// CommonSEのファイル番号を取得
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetAssetBundleName(CommonSoundType type)
        {
            if (type >= CommonSoundType.SpecialSE1)
            {
                return (int)type - COMMONSE_NUM + 1;
            }

            return (int) type + 1;
        }
    }
}