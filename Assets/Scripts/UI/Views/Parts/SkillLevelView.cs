using System;
using System.Collections.Generic;
using UI.Views.SubPage;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using CharacterController = BattleStage.Controller.Character.CharacterController;

namespace UI.Views.Parts
{
    public class SkillLevelView : MonoBehaviour
    {
        [SerializeField] private Button _buttonPlus;

        [SerializeField] private Button _buttonMinus;

        [SerializeField] private Text _skillImagePlus;

        [SerializeField] private CharacterController _characterController;

        [SerializeField] private SkillType _skillType;
        
        [SerializeField] private NextWavePage _nextWavePage;

        private int _pointLevel = 1;
        // Start is called before the first frame update
        
        private int pointAdded = 0;

        public void ResetPointAddArray()
        {
            pointAdded = 0;
        }
        
        void Start()
        {
            switch (_skillType)
            {
                case SkillType.ATTACK:
                    _pointLevel = _characterController.Character.UnitStatus.GetttackLevel();
                    break;
                case SkillType.HP:
                    _pointLevel = _characterController.Character.UnitStatus.GetHPLevel();
                    break;
                case SkillType.SPEED:
                    _pointLevel = _characterController.Character.UnitStatus.GetSpeedLevel();
                    break;
            }

            _skillImagePlus.text = (10 * _pointLevel) + "%";

            _buttonPlus.OnClickAsObservable().Subscribe(_ =>
            {
                if (_nextWavePage.GetSkillPoint() <= 0)
                {
                    return;
                }
                
                _pointLevel ++;
                _nextWavePage.SetSkillPointMinusOne();

                switch (_skillType)
                {
                    case SkillType.ATTACK:
                        _characterController.Character.UnitStatus.IncreaseAttackLevel(_pointLevel);
                        break;
                    case SkillType.HP:
                        _characterController.Character.UnitStatus.IncreaseHPLevel(_pointLevel);
                        break;
                    case SkillType.SPEED:
                        _characterController.Character.UnitStatus.IncreaseSpeedLevel(_pointLevel);
                        break;
                }
                
                _skillImagePlus.text = (10 * _pointLevel) + "%";
                pointAdded++;
                
            }).AddTo(this);
            
            _buttonMinus.OnClickAsObservable().Subscribe(_ =>
            {
                if (_pointLevel <= 0 || pointAdded <= 0)
                {
                    return;
                }
                
                _nextWavePage.SetSkillPointPlusOne();
                _pointLevel--;
                pointAdded--;
                
                switch (_skillType)
                {
                    case SkillType.ATTACK:
                        _characterController.Character.UnitStatus.IncreaseAttackLevel(_pointLevel);
                        break;
                    case SkillType.HP:
                        _characterController.Character.UnitStatus.IncreaseHPLevel(_pointLevel);
                        break;
                    case SkillType.SPEED:
                        _characterController.Character.UnitStatus.IncreaseSpeedLevel(_pointLevel);
                        break;
                }
                
                _skillImagePlus.text = (10 * _pointLevel) + "%";
                
            }).AddTo(this);
            

        }

       
    }

    public enum SkillType
    {
        ATTACK,
        SPEED,
        HP
    }
}