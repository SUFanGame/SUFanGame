using System;
using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Data.DataTypes;
using StevenUniverse.FanGame.Entities.Customization;
using StevenUniverse.FanGame.Util;

//TODO rename this namespace

namespace StevenUniverse.FanGame.Entities
{
    [System.Serializable]
    public class Player : Entity
    {
        public static Player GetPlayer(string playerAppDataPath)
        {
            return Get<Player>(playerAppDataPath);
        }

        [SerializeField] private string savedSceneName;
        [SerializeField] private int savedX;
        [SerializeField] private int savedY;
        [SerializeField] private int savedElevation;
        [SerializeField] private SaveData savedData;

        public string SavedSceneName
        {
            get { return savedSceneName; }
            set { savedSceneName = value; }
        }

        public int SavedX
        {
            get { return savedX; }
            set { savedX = value; }
        }

        public int SavedY
        {
            get { return savedY; }
            set { savedY = value; }
        }

        public int SavedElevation
        {
            get { return savedElevation; }
            set { savedElevation = value; }
        }

        public SaveData SavedData
        {
            get { return savedData; }
            set { savedData = value; }
        }

        public WarpPoint SavedWarpPoint
        {
            get { return new WarpPoint("", SavedSceneName, new Vector3(SavedX, SavedY, 0f), SavedElevation); }
        }

        //TODO pokemon box

        public Player(string characterName, string directionName, string stateName, Outfit startingOutfit,
            string sceneName, int x, int y, int elevation, SaveData saveData)
            : base(characterName, directionName, stateName, startingOutfit)
        {
            SavedSceneName = sceneName;
            SavedX = x;
            SavedY = y;
            SavedElevation = elevation;
            SavedData = saveData;
        }

        public static Player CreateDefaultPlayer()
        {
            Outfit playerOutfit = new Outfit("Light Body", "Red Hat", "Calm Eyes", "Red Shirt");
            SaveData playerSaveData = new SaveData
            (
                new DataGroup[]
                {
                    new DataGroup
                    (
                        "Intro",
                        new DataBool[]
                        {
                            new DataBool("CharacterCustomizationComplete", false),
                            new DataBool("RubyIntro", false),
                            new DataBool("SapphireIntro", false),
                            new DataBool("EmeraldIntro", false),
                            new DataBool("RubyDefeated", false),
                            new DataBool("SapphireDefeated", false),
                            new DataBool("EmeraldDefeated", false),
                        },
                        new DataInt[]
                        {
                            new DataInt("ten", 10),
                            new DataInt("five", 5)
                        }
                    ),
                }
            );
            Player defaultPlayer = new Player("Brendan", "Down", "Standing", playerOutfit, "Overworld", 54, 8, 0, playerSaveData);
            defaultPlayer.AppDataPath = "Saves/Player";

            return defaultPlayer;
        }

        public override void Save()
        {
            SavedSceneName = GameController.Instance.CurrentScene;
            SavedX = Mathf.FloorToInt(EntityDriver.ChunkRenderer.transform.position.x);
            SavedY = Mathf.FloorToInt(EntityDriver.ChunkRenderer.transform.position.y);
            SavedElevation = EntityDriver.ChunkRenderer.SourceChunk.CurrentElevation;

            base.Save();
        }

        
    }
}