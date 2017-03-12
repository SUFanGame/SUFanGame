using SUGame.Factions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util;
using SUGame.Util.Common;
using System.Linq;
using SUGame.Characters;
using SUGame.StrategyMap.Characters.Actions.UIStates;
using SUGame.StrategyMap.UI.CombatPanelUI;

namespace SUGame.StrategyMap.Players
{
    /// <summary>
    /// An example AI Player
    /// </summary>
    public class AIPlayer : StrategyPlayer
    {

        public override IEnumerator Tick()
        {
            //Cache the Grid instance
            Grid grid = Grid.Instance;

            //Perform AI actions for each unit
            foreach (MapCharacter unit in Units)
            {
                //Get all of the hostile characters
                MapCharacter[] hostileMapCharacters = GetHostileMapCharacters();

                //Build a list of possible paths to take
                Dictionary<MapCharacter, List<Node>> paths = new Dictionary<MapCharacter, List<Node>>();
                
                //Find all valid paths to each hostile character
                foreach (MapCharacter hostileMapCharacter in hostileMapCharacters)
                {
                    //Create modifiers to be used for finding the spaces surrounding the hostile character
                    IntVector3[] directionalModifiers = new IntVector3[] { new IntVector3(-1, 0, 0), new IntVector3(1, 0, 0), new IntVector3(0, -1, 0), new IntVector3(0, 1, 0), };

                    //Get paths to all spaces surrounding the hostile character
                    List<List<Node>> potentialPaths = new List<List<Node>>();
                    foreach (IntVector3 directionalModifier in directionalModifiers)
                    {
                        //Calculate the potential destination by offsetting the hostile character's position by the current modifier
                        IntVector3 potentialDestination = hostileMapCharacter.GridPosition + directionalModifier;

                        //Check if the potential destination exists in the grid
                        bool potentialDestinationNodeExists = grid.GetNode(potentialDestination) != null;

                        //Only add the path if the destination Node exists
                        if ( potentialDestinationNodeExists )
                        {
                            //Create the buffer to be populated with the found path
                            List<Node> potentialPath = new List<Node>();
                            //TODO use the actual movement type of the specific unit, once units have movement types assigned to them
                            grid.GetPath(unit.GridPosition, potentialDestination, potentialPath, MovementType.GROUNDED);

                            potentialPaths.Add(potentialPath);
                        }
                    }

                    //Get the paths that don't end on an occupied square
                    List<List<Node>> viablePaths = new List<List<Node>>();
                    foreach (List<Node> potentialPath in potentialPaths)
                    {
                        //Get the position of the final destination of the current potential path
                        IntVector3 potentialDestination = potentialPath[potentialPath.Count - 1].Pos_;

                        //Get all the MapCharacters at the potential destination
                        List<MapCharacter> mapCharactersAtPotentialDestination = new List<MapCharacter>();
                        grid.GetObjects<MapCharacter>(potentialDestination, mapCharactersAtPotentialDestination);

                        //Check if there are any MapCharacters occupying the destination
                        int mapCharactersAtPotentialDestinationCount = mapCharactersAtPotentialDestination.Count;

                        //Don't count this unit as occupying the destination
                        if (mapCharactersAtPotentialDestination.Contains(unit))
                        {
                            mapCharactersAtPotentialDestinationCount--;
                        }

                        //If there are no MapCharacters occupying the destination, this path is viable
                        if (mapCharactersAtPotentialDestinationCount == 0)
                        {
                            viablePaths.Add(potentialPath);
                        }
                    }

                    //Sort the viable paths by the shortest lengths
                    viablePaths = viablePaths.OrderBy(p => p.Count).ToList();

                    //Add the shortest path to this target to the greater list of all paths
                    if (viablePaths.Count > 0)
                    {
                        paths.Add(hostileMapCharacter, viablePaths[0]);
                    }
                }

                //Sort all of the paths by the shortest lengths
                paths = paths.OrderBy(kvp => kvp.Value.Count).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                //Find the number of completable paths (paths within range of the unit)
                bool uncompletablePathFound = false;
                int numberOfCompletablePaths = 0;
                KeyValuePair<MapCharacter, List<Node>>[] pathsArray = paths.ToArray();
                while (!uncompletablePathFound && numberOfCompletablePaths < pathsArray.Length)
                {
                    KeyValuePair<MapCharacter, List<Node>> currentPath = pathsArray[numberOfCompletablePaths];

                    if (currentPath.Value.Count <= unit.moveRange_)
                    {
                        numberOfCompletablePaths++;
                    }
                    else
                    {
                        uncompletablePathFound = true;
                    }
                }

                //If there are any completable paths, prioritize these over all other paths
                if (numberOfCompletablePaths > 0)
                {
                    Dictionary<MapCharacter, List<Node>> completablePaths = new Dictionary<MapCharacter, List<Node>>();
                    //Get the completable paths
                    for (int i = 0; i < numberOfCompletablePaths; i++)
                    {
                        KeyValuePair<MapCharacter, List<Node>> completablePath = pathsArray[i];
                        completablePaths.Add(completablePath.Key, completablePath.Value);
                    }

                    //Replace the existing paths dictionary with the completable paths dictionary
                    paths = completablePaths;

                    //TODO prioritize by lowest health / matching weapon / other logic for determining best target. Right now it prioritizes by name just so all enemies will attack the same unit when possible
                    paths = paths.OrderBy(kvp => kvp.Key.name).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }

                //Get the shortest path
                KeyValuePair<MapCharacter, List<Node>> bestPath = paths.ToList()[0];
                
                //Get the target character and path from the best path
                MapCharacter targetCharacter = bestPath.Key;
                List<Node> targetPath = bestPath.Value;

                bool pathCompleted = true;

                //Shorten the path if it's longer than the unit's range
                if (targetPath.Count > unit.moveRange_)
                {
                    targetPath = targetPath.GetRange(0, unit.moveRange_);
                    pathCompleted = false;
                }

                //Get the destination (the final location of the path)
                IntVector3 destination = targetPath[targetPath.Count - 1].Pos_;

                //Move the unit
                yield return CharacterUtility.MoveTo(unit, destination);

                //If the path is completable, attack the hostile character
                if (pathCompleted)
                {
                    Debug.LogFormat("{0} would attack {1}", unit.name, targetCharacter.name);
                    CombatPanel.Initialize(unit, targetCharacter);

                    var state = new CombatUIState(unit, targetCharacter);
                    HumanPlayer.Instance.StateMachine_.Push(state);



                    yield return state.WaitForAnimations();
                }

                //Pause the unit
                unit.Paused_ = true;
            }

            // AI would perform it's actions here.
            yield return null;
        }

        //Get all of the MapCharacters that are on a Faction that is aligned as Hostile to this AI Player's Faction
        private MapCharacter[] GetHostileMapCharacters()
        {
            List<Faction> hostileFactions = new List<Faction>(Faction_.GetFactionsWithStanding(Standing.HOSTILE));
            List<MapCharacter> hostileMapCharacters = new List<MapCharacter>();

            MapCharacter[] allMapCharacters = GameObject.FindObjectsOfType<MapCharacter>();
            foreach (MapCharacter mapCharacter in allMapCharacters)
            {
                Faction mapCharacterFaction = mapCharacter.OwningPlayer.Faction_;
                if (hostileFactions.Contains(mapCharacterFaction))
                {
                    hostileMapCharacters.Add(mapCharacter);
                }
            }

            return hostileMapCharacters.ToArray();
        }
    }
}