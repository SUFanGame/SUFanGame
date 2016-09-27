using UnityEngine;

namespace StevenUniverse.FanGame.Util
{
    public static class ScenesExtensions
    {
        private const string OVERWORLD = "Overworld";
        private const string BRENDANS_HOUSE_1F = "Brendan's House 1F";
        private const string BRENDANS_HOUSE_2F = "Brendan's House 2F";
        private const string PETALBURG_WOODS = "Petalburg Woods";
        private const string RUSTURF_TUNNEL = "Rusturf Tunnel";
        private const string TEST = "Test";
        private const string MAYS_HOUSE_1F = "May's House 1F";
        private const string MAYS_HOUSE_2F = "May's House 2F";
        private const string DINACOUTA_KEY = "Dinacouta Key";
        private const string DINATURF_TUNNEL = "Dinaturf Tunnel";
        private const string SS_ANNE = "SS Anne";
        private const string SS_ANNE_HALL = "SS Anne Hall";
        private const string SS_ANNE_ROOM = "SS Anne Room";

        public static string GetSceneString(this Scenes scene)
        {
            switch (scene)
            {
                case Scenes.Overworld:
                    return OVERWORLD;
                case Scenes.PetalburgWoods:
                    return PETALBURG_WOODS;
                case Scenes.BrendansHouse1F:
                    return BRENDANS_HOUSE_1F;
                case Scenes.BrendansHouse2F:
                    return BRENDANS_HOUSE_2F;
                case Scenes.Test:
                    return TEST;
                case Scenes.RusturfTunnel:
                    return RUSTURF_TUNNEL;
                case Scenes.MaysHouse1F:
                    return MAYS_HOUSE_1F;
                case Scenes.MaysHouse2F:
                    return MAYS_HOUSE_2F;
                case Scenes.DinacoutaKey:
                    return DINACOUTA_KEY;
                case Scenes.DinaturfTunnel:
                    return DINATURF_TUNNEL;
                case Scenes.SSAnne:
                    return SS_ANNE;
                case Scenes.SSAnneHall:
                    return SS_ANNE_HALL;
                case Scenes.SSAnneRoom:
                    return SS_ANNE_ROOM;
                default:
                    throw new UnityException("Scene not found");
            }
        }

        public static Scenes GetSceneFromSceneString(string sceneString)
        {
            switch (sceneString)
            {
                case OVERWORLD:
                    return Scenes.Overworld;
                case PETALBURG_WOODS:
                    return Scenes.PetalburgWoods;
                case BRENDANS_HOUSE_1F:
                    return Scenes.BrendansHouse1F;
                case TEST:
                    return Scenes.Test;
                case RUSTURF_TUNNEL:
                    return Scenes.RusturfTunnel;
                case MAYS_HOUSE_1F:
                    return Scenes.MaysHouse1F;
                case MAYS_HOUSE_2F:
                    return Scenes.MaysHouse2F;
                case DINACOUTA_KEY:
                    return Scenes.DinacoutaKey;
                case DINATURF_TUNNEL:
                    return Scenes.DinaturfTunnel;
                case SS_ANNE:
                    return Scenes.SSAnne;
                case SS_ANNE_HALL:
                    return Scenes.SSAnneHall;
                case SS_ANNE_ROOM:
                    return Scenes.SSAnneRoom;
                default:
                    throw new UnityException("Scene string not found");
            }
        }
    }
}