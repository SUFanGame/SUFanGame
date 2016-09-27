using UnityEngine;
using StevenUniverse.FanGame.Entities;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class NewBattle : Activity
    {
        [SerializeField] private Character.CharacterInstance enemyCharacter;

        public NewBattle(int id, bool allowsControl) : base(id, allowsControl)
        {
        }
    }
}