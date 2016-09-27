using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Overworld
{
    [System.Serializable]
    public class Chunk : JsonBase<Chunk>
    {
        bool currentElevationInit = false;

        //Class
        private const int PIXELS_PER_TILE = 16;
        private static volatile int tempOutputCounter = 0;

        public static Chunk GetChunk(string chunkAppDataPath)
        {
            Chunk gottenChunk = Get<Chunk>(chunkAppDataPath);
            return gottenChunk;
        }

        //Instance
        //Tile & Group Instances
        [SerializeField] private List<TileInstance> tileInstances = new List<TileInstance>();
        [SerializeField] private List<GroupInstance> groupInstances = new List<GroupInstance>();
        //TODO trace this variable and kill it
        [SerializeField] private bool changed = false;

        //Elevation
        private int startingElevation = 0;
        private int currentElevation = 0;

        //Chunk Layers
        private List<ChunkLayer> chunkLayers;

        //Instance caches
        private Instance[] allInstances;
        private TileInstance[] allInstancesFlattened;
        private CoordinatedList<TileInstance> allInstancesFlattenedCoordinated;

        //Bounds
        private int minX;
        private int minY;
        private int maxX;
        private int maxY;

        //Constructor
        public Chunk(TileInstance[] tileInstances, GroupInstance[] groupInstances)
        {
            TileInstances = tileInstances;
            GroupInstances = groupInstances;
        }

        private void UpdateMinMax()
        {
            if (AllInstances.Length > 0)
            {
                //Find the min and max values
                //TODO use this method for finding min max instead of OrderBy in other places
                MinX = AllInstancesFlattened.Min(ix => ix.X);
                MaxX = AllInstancesFlattened.Max(ix => ix.X);
                MinY = AllInstancesFlattened.Min(ix => ix.Y);
                MaxY = AllInstancesFlattened.Max(ix => ix.Y);
            }
        }

        public int StartingElevation
        {
            get { return startingElevation; }
        }

        public int CurrentElevation
        {
            get
            {
                if (!currentElevationInit)
                {
                    CurrentElevation = StartingElevation;
                    currentElevationInit = true;
                }
                return currentElevation;
            }
            set { currentElevation = value; }
        }

        //Instances
        public TileInstance[] TileInstances
        {
            get { return tileInstances.ToArray(); }
            private set { tileInstances = new List<TileInstance>(value); }
        }

        public GroupInstance[] GroupInstances
        {
            get { return groupInstances.ToArray(); }
            private set { groupInstances = new List<GroupInstance>(value); }
        }

        public bool Changed
        {
            get { return changed; }
            set { changed = value; }
        }

        //Instance Caches
        public Instance[] AllInstances
        {
            get
            {
                if (allInstances == null)
                {
                    //Set the all instances array
                    List<Instance> allInstancesList = new List<Instance>();
                    allInstancesList.AddRange(TileInstances);
                    allInstancesList.AddRange(GroupInstances);
                    allInstances = allInstancesList.ToArray();
                }
                return allInstances;
            }
        }

        public TileInstance[] AllInstancesFlattened
        {
            get
            {
                if (allInstancesFlattened == null)
                {
                    //Flatten groups into tiles
                    List<TileInstance> instancesAsTilesList = new List<TileInstance>();
                    foreach (Instance instance in AllInstances)
                    {
                        if (instance is TileInstance)
                        {
                            instancesAsTilesList.Add(instance as TileInstance);
                        }
                        else if (instance is GroupInstance)
                        {
                            instancesAsTilesList.AddRange((instance as GroupInstance).IndependantTileInstances);
                        }
                    }
                    allInstancesFlattened = instancesAsTilesList.ToArray();
                }

                return allInstancesFlattened;
            }
            private set { allInstancesFlattened = value; }
        }

        public CoordinatedList<TileInstance> AllInstancesFlattenedCoordinated
        {
            get
            {
                if (allInstancesFlattenedCoordinated == null)
                {
                    //Set the flattened coordinated list
                    allInstancesFlattenedCoordinated = new CoordinatedList<TileInstance>(AllInstancesFlattened);
                }

                return allInstancesFlattenedCoordinated;
            }
        }

        //Bounds
        //TODO instead of calling UpdateMinMax everytime, create a variable to keep track of when a change requiring it to be updated has been made
        public int MinX
        {
            get
            {
                UpdateMinMax();
                return minX;
            }
            private set { minX = value; }
        }

        public int MinY
        {
            get
            {
                UpdateMinMax();
                return minY;
            }
            private set { minY = value; }
        }

        public int MaxX
        {
            get
            {
                UpdateMinMax();
                return maxX;
            }
            private set { maxX = value; }
        }

        public int MaxY
        {
            get
            {
                UpdateMinMax();
                return maxY;
            }
            private set { maxY = value; }
        }

        public Vector3 Position
        {
            get { return new Vector3(MinX, MinY, 0); }
        }

        public override string ToString()
        {
            string toString = "";

            foreach (Instance instance in AllInstances)
            {
                toString += instance.ToString() + " / ";
            }

            return toString;
        }

        public bool Rendered
        {
            get { return chunkLayers != null; }
        }

        public void RenderChunkLayers()
        {
            //Create a List to hold the rendered ChunkLayers
            List<ChunkLayer> renderedChunkLayers = new List<ChunkLayer>();

            Dictionary<ChunkLayer.ChunkLayerSignature, List<TileInstance>> allInstancesFlattenedToTilesSorted =
                new Dictionary<ChunkLayer.ChunkLayerSignature, List<TileInstance>>();

            //Create a dictionary with a key for every elevation corresponding to a key for every sortingOrder at that elevation cooresponding to a list of the TileInstances at that elevation-SortingOrder combination
            //Sort the tiles into the dictionary
            foreach (TileInstance tileInstance in AllInstancesFlattened)
            {

                ChunkLayer.ChunkLayerSignature tileInstanceSignature =
                    new ChunkLayer.ChunkLayerSignature(tileInstance.Elevation, tileInstance.TemplateSortingOrder,
                        tileInstance.TileTemplate.AnimationSprites.Length, tileInstance.TileTemplate.SyncAnimation,
                        tileInstance.TileTemplate.SecondsPerFrame);
                if (!allInstancesFlattenedToTilesSorted.ContainsKey(tileInstanceSignature))
                {
                    allInstancesFlattenedToTilesSorted.Add(tileInstanceSignature, new List<TileInstance>());
                }
                allInstancesFlattenedToTilesSorted[tileInstanceSignature].Add(tileInstance);
            }

            int chunkLayerCounter = 0;
            //Create a ChunkLayer from the TileInstances at each signature
            foreach (ChunkLayer.ChunkLayerSignature signature in allInstancesFlattenedToTilesSorted.Keys)
            {
                TileInstance[] tileInstancesAtSignature = allInstancesFlattenedToTilesSorted[signature].ToArray();

                int width = 0;
                int height = 0;
                List<string> chunkLayerTexturePaths = new List<string>();
                for (int currentFrame = 0; currentFrame < signature.Frames; currentFrame++)
                {
                    string outputDirectoryAbsolute;
                    string outputName;

                    //If the chunk has a path, use it
                    if (!string.IsNullOrEmpty(AppDataPath))
                    {
                        outputDirectoryAbsolute =
                            Utilities.ConvertPathToParentDirectoryPath(Utilities.ExternalDataPath + "/" +
                                                                       AppDataPath);
                        outputName = Utilities.ConvertFilePathToFileName(AppDataPath);
                    }
                    //Otherwise, it's temporary so put it in the temporary folder
                    else
                    {
                        outputDirectoryAbsolute = Utilities.ExternalDataPath + "/ChunksTemp";
                        outputName = tempOutputCounter++.ToString();
                    }

                    string outputPathAbsolute = string.Format("{0}/{1}_{2}_{3}.png", outputDirectoryAbsolute, outputName,
                        chunkLayerCounter, currentFrame);

                    Texture2D generatedTexture = null;
                    if (!File.Exists(outputPathAbsolute))
                    {
                        //Generate the Texture
                        generatedTexture = GenerateTextureFromTileInstances(tileInstancesAtSignature, currentFrame);

                        //Save the Output
                        File.WriteAllBytes(outputPathAbsolute, generatedTexture.EncodeToPNG());
                    }

                    //Get the width and height on the first frame
                    if (currentFrame == 0)
                    {
                        System.Drawing.Bitmap sizeBitmap =
                            (System.Drawing.Bitmap) System.Drawing.Image.FromFile(outputPathAbsolute);
                        width = sizeBitmap.Width;
                        height = sizeBitmap.Height;
                    }

                    chunkLayerTexturePaths.Add(outputPathAbsolute);
                }

                renderedChunkLayers.Add(new ChunkLayer(chunkLayerTexturePaths.ToArray(), width, height, signature));
                chunkLayerCounter++;
            }

            ChunkLayer[] stuff = renderedChunkLayers.ToArray();

            //Set the ChunkLayers
            ChunkLayers = stuff;
        }

        public void ResetAsynchronousChunkLayers()
        {
            foreach (ChunkLayer chunkLayer in ChunkLayers)
            {
                if (!chunkLayer.Signature.SyncAnimation)
                {
                    chunkLayer.ResetStartTime();
                }
            }
        }

        public Texture2D GenerateTextureFromTileInstances(TileInstance[] tileInstances, int animationFrame)
        {
            TileInstance[] tilesInDrawingOrder =
                tileInstances.OrderBy(tile => tile.Elevation)
                    .ThenBy(tile => (tile.TileTemplate).TileLayer.SortingValue)
                    .ToArray();

            int textureWidth = (Mathf.Abs(MaxX - MinX) + 1)*PIXELS_PER_TILE;
            int textureHeight = (Mathf.Abs(MaxY - MinY) + 1)*PIXELS_PER_TILE;

            //Create the texture to be drawn onto
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            texture.filterMode = FilterMode.Point;
            
            //TODO this doesn't work in builds, but it also doesn't seem to be neccesary anymore. Do more research.
            //texture.alphaIsTransparency = true;

            //Make the texture transparent
            Color[] baseColors = texture.GetPixels(0, 0, texture.width, texture.height);
            for (int i = 0; i < baseColors.Length; i++)
            {
                baseColors[i].a = 0;
            }
            texture.SetPixels(0, 0, texture.width, texture.height, baseColors);

            //Draw onto the texture
            foreach (TileInstance tile in tilesInDrawingOrder)
            {
                Texture2D tileTexture = tile.TileTemplate.GetCachedSprite(animationFrame).texture;

                Vector3 tixPos = tile.Position;
                int startingX = ((int) tixPos.x - minX)*PIXELS_PER_TILE;
                int startingY = ((int) tixPos.y - minY)*PIXELS_PER_TILE;

                //Get the colors
                Color[] colors = texture.GetPixels(startingX, startingY, PIXELS_PER_TILE, PIXELS_PER_TILE);
                Color[] newColors = tileTexture.GetPixels(0, 0, PIXELS_PER_TILE, PIXELS_PER_TILE);

                for (int i = 0; i < colors.Length; i++)
                {
                    //Skip transparent additions
                    if (newColors[i].a != 0)
                    {
                        //If there is not currently a color, replace it with the new color
                        if (colors[i].a == 0)
                        {
                            colors[i] = newColors[i];
                        }
                        //Otherwise, blend the current color with the new color
                        else
                        {
                            Color blendedColor = Color.Lerp(colors[i], newColors[i], newColors[i].a);
                            colors[i] = blendedColor;
                        }
                    }
                }

                texture.SetPixels(startingX, startingY, PIXELS_PER_TILE, PIXELS_PER_TILE, colors);
            }

            texture.Apply();

            return texture;
        }

        public Texture2D GenerateFlattenedTexture()
        {
            return GenerateTextureFromTileInstances(AllInstancesFlattened, 0);
        }

        //Chunk Layers
        public ChunkLayer[] ChunkLayers
        {
            get
            {
                if (chunkLayers != null)
                {
                    return chunkLayers.ToArray();
                }
                else
                {
                    return new ChunkLayer[] {};
                }
            }
            set { chunkLayers = new List<ChunkLayer>(value); }
        }

        //ChunkLayer subclass
        public class ChunkLayer
        {
            //TODO use this pattern in more places as Dictionary keys
            public struct ChunkLayerSignature : System.IEquatable<ChunkLayerSignature>
            {
                public ChunkLayerSignature(int elevation, int sortingOrder, int frames, bool syncAnimation,
                    float secondsPerFrame)
                {
                    Elevation = elevation;
                    SortingOrder = sortingOrder;
                    Frames = frames;
                    SyncAnimation = syncAnimation;
                    SecondsPerFrame = secondsPerFrame;
                }

                public int Elevation { get; private set; }
                public int SortingOrder { get; private set; }
                public int Frames { get; private set; }
                public bool SyncAnimation { get; private set; }
                public float SecondsPerFrame { get; private set; }

                public bool Equals(ChunkLayerSignature other)
                {
                    return Elevation == other.Elevation
                           && SortingOrder == other.SortingOrder
                           && Frames == other.Frames
                           && SyncAnimation == other.SyncAnimation
                           && SecondsPerFrame == other.SecondsPerFrame;
                }
            }

            //Instance
            private string[] imagePaths;
            private int width;
            private int height;
            private ChunkLayerSignature signature;
            private SpriteRenderer spriteRenderer;

            private Texture2D[] textures;
            private Sprite[] sprites;
            private float startTime = 0f;
            private int currentFrame = 0;

            public ChunkLayer(string[] imagePaths, int width, int height, ChunkLayerSignature signature)
            {
                this.imagePaths = imagePaths;
                this.width = width;
                this.height = height;
                this.signature = signature;
            }

            public string[] ImagePaths
            {
                get { return imagePaths; }
            }

            public ChunkLayerSignature Signature
            {
                get { return signature; }
            }

            public SpriteRenderer SpriteRenderer
            {
                get { return spriteRenderer; }
                set { spriteRenderer = value; }
            }

            public void UpdateSpriteRenderer()
            {
                //Don't update destroyed SpriteRenderers
                //TODO better workaround?
                if (SpriteRenderer == null)
                {
                    return;
                }

                //If the animation is not supposed to sync, cache the start time
                if (!Signature.SyncAnimation && startTime == 0f)
                {
                    ResetStartTime();
                }

                //Cache the last frame for future reference
                int lastFrame = currentFrame;

                //Determine the time to use in the frame calculation
                float time = Time.time;
                if (!Signature.SyncAnimation)
                {
                    time -= startTime;
                }

                //Calculate the current frame
                currentFrame = Mathf.FloorToInt(time/Signature.SecondsPerFrame)%Signature.Frames;

                //If a sprite has not yet been assigned or the frame has changed, update the Sprite
                if (SpriteRenderer.sprite == null || lastFrame != currentFrame)
                {
                    Sprite newSprite = Sprites[currentFrame];
                    SpriteRenderer.sprite = newSprite;
                }
            }

            public Texture2D[] Textures
            {
                get
                {
                    if (textures == null)
                    {
                        List<Texture2D> loadedBitmaps = new List<Texture2D>();
                        foreach (string bitmapPath in ImagePaths)
                        {
                            Texture2D loadedBitmap = new Texture2D(width, height);
                            loadedBitmap.LoadImage(File.ReadAllBytes(bitmapPath));
                            loadedBitmap.filterMode = FilterMode.Point;
                            loadedBitmaps.Add(loadedBitmap);
                        }
                        textures = loadedBitmaps.ToArray();
                    }

                    return textures;
                }
                private set { textures = value; }
            }

            public Sprite[] Sprites
            {
                get
                {
                    if (sprites == null)
                    {
                        List<Sprite> loadedSprites = new List<Sprite>();
                        foreach (Texture2D texture in Textures)
                        {
                            loadedSprites.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                Vector2.zero, 16f));
                        }
                        sprites = loadedSprites.ToArray();
                    }

                    return sprites;
                }
            }

            public void ResetStartTime()
            {
                startTime = Time.time;
            }
        }
    }
}