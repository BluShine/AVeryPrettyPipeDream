using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    public static class ImageEffectHelper
    {
        public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
        {
            if (s == null || !s.isSupported)
            {
                Debug.Log("shader is: " + s);
                Debug.LogWarningFormat("Missing shader for image effect {0}", effect);
                return false;
            }

            if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
                return false;

            if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
                return false;

            if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                return false;

            return true;
        }

        public static Material CheckShaderAndCreateMaterial(Shader s)
        {
            if (s == null || !s.isSupported)
                return null;

            var material = new Material(s);
            material.hideFlags = HideFlags.DontSave;
            return material;
        }
    }
}
