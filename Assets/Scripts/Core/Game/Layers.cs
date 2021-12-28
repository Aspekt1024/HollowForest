using UnityEngine;

namespace HollowForest
{
    public static class Layers
    {
        public const string Interactive = "Interactive";
        public const string World = "World";
        public const string Character = "Character";

        public static int GetLayerFromName(string layer)
        {
            return LayerMask.NameToLayer(layer);
        }

        public static int GetLayerMask(params string[] layers)
        {
            var layerMask = 0;
            foreach (var layer in layers)
            {
                layerMask |= 1 << LayerMask.NameToLayer(layer);
            }
            return layerMask;
        }

        public static bool IsLayerMatch(int layer, string layerName)
        {
            return GetLayerFromName(layerName) == layer;
        }
    }
}