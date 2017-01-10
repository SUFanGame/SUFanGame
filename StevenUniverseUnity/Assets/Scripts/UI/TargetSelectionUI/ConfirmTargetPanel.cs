using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SUGame.StrategyMap.UI.ConfirmTargetUI
{
    public class ConfirmTargetPanel : MonoBehaviour
    {
        Animator animator_;
        [SerializeField]
        TargetPanel attackerPanel_;
        [SerializeField]
        TargetPanel defenderPanel_;

        static ConfirmTargetPanel instance_;

        void Awake()
        {
            animator_ = GetComponent<Animator>();
            instance_ = this;
            //ShowUI();
        }


        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        ShowUI();
        //    }
        //}

        public static void Toggle()
        {
            instance_.animator_.SetTrigger("Toggle");
        }

        public static void SetAttacker(MapCharacter character)
        {
            instance_.attackerPanel_.SetTarget(character);
        }

        public static void SetDefender( MapCharacter character )
        {
            instance_.defenderPanel_.SetTarget(character);
        }

        
    }
}