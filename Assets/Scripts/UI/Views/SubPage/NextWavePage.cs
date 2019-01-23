using UI.Views.Parts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.SubPage
{
    public class NextWavePage : MonoBehaviour
    {
        private readonly ReactiveProperty<int> _skillPoint = new IntReactiveProperty(0);

        [SerializeField]
        private Text _skillPointText;

        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private SkillLevelView[] skills;
        
        public IObservable<Unit> OnClickButtonNextWave()
        {
            return _nextButton.OnClickAsObservable();
        }
        
        // Start is called before the first frame update
        void Start()
        {
            _skillPoint.Subscribe(val =>
            {
                _skillPointText.text = val.ToString();
            }).AddTo(this);
        }

        public void SetSkillPoint(int skillPoint)
        {
            _skillPoint.Value += skillPoint;

            foreach (var skillView in skills)
            {
                skillView.ResetPointAddArray();
            }
        }
        
        public int GetSkillPoint()
        {
            return _skillPoint.Value;
        }
        
        public void SetSkillPointMinusOne()
        {
            _skillPoint.Value--;
        }
        
        public void SetSkillPointPlusOne()
        {
            _skillPoint.Value++;
        }
    }
}
