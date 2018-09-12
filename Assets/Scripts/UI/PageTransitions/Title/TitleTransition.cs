using System.Collections;
using UI.Scripts.PageTransitions;

namespace UI.PageTransitions.Title
{
    /// <summary>
    /// マイページへ遷移するTransition
    /// </summary>
    public class TitleTransition : PageTransition
    {
        
        public override IEnumerator LoadAsync()
        {
            _loading.Value = true;

            yield return null;

            _loading.Value = false;
        }

        public override void BindLoadedModels()
        {
        }
    }
}
