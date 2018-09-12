using System;
using System.Collections.Generic;
using System.Linq;
using UI.Scripts.PageTransitions;
using UnityEngine;

namespace UI.Scripts.Route
{
    /// <summary>
    /// トランジションのリソースをロードするクラス
    /// </summary>
    public static class TransitionLoader
    {
        /// <summary>
        /// ページトランジションを取得する
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="resourcePath"></param>
        public static T GetPageTransition<T>(List<PageTransition> cache, string resourcePath)
            where T : PageTransition
        {
            return GetTransition<T, PageTransition>(cache, resourcePath);
        }

        /// <summary>
        /// サブページのトランジションを取得する
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="resourcePath"></param>
        public static T GetSubPageTransition<T>(List<PopupTransition> cache, string resourcePath)
            where T : PopupTransition
        {
            return GetTransition<T, PopupTransition>(cache, resourcePath);
        }

        /// <summary>
        /// 共通サブページのトランジションを取得する
        /// </summary>
        /// <param name="transitionName"></param>
        /// <param name="cache"></param>
        /// <param name="resourcePath"></param>
        public static CommonSubPageTransition GetCommonSubPageTransition(
            string transitionName,
            List<PopupTransition> cache,
            string resourcePath
        )
        {
            var path = resourcePath + "/" + transitionName;
            return GetTransition<CommonSubPageTransition, PopupTransition>(
                cache,
                path,
                t => t.name == transitionName
            );
        }

        /// <summary>
        /// 一般ポップアップを取得する
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="cache"></param>
        /// <param name="resourcePath"></param>
        public static CommonPopupTransition GetCommonPopupTransition(
            int ID,
            List<CommonPopupTransition> cache,
            string resourcePath
        )
        {
            var path = resourcePath + "/" + string.Format("PopupId{0:D4}", ID);
            return GetTransition<CommonPopupTransition, CommonPopupTransition>(cache, path, t => t.ID == ID);
        }

        private static T1 GetTransition<T1, T2>(List<T2> cache, string resourcePath)
            where T1 : T2
            where T2 : UnityEngine.Object
        {
            var path = resourcePath + "/" + typeof(T1).ToString().Split('.').Last();
            return GetTransition<T1, T2>(cache, path, t => t.GetType() == typeof(T1));
        }

        private static T1 GetTransition<T1, T2>(List<T2> cache, string resourcePath, Func<T2, Boolean> predicate)
            where T1 : T2
            where T2 : UnityEngine.Object
        {
            var transition = cache == null ? null : (T1)cache.FirstOrDefault(predicate);
            if (transition == null)
            {
                transition = (T1) Resources.Load(resourcePath);
                if (cache != null) cache.Add(transition);
            }
            return (T1) transition;
        }
    }
}
