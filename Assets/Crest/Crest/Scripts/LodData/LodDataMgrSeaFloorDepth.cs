// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

using UnityEngine;
using UnityEngine.Rendering;

namespace Crest
{
    /// <summary>
    /// Renders depth of the ocean (height of sea level above ocean floor), by rendering the relative height of tagged objects from top down.
    /// </summary>
    public class LodDataMgrSeaFloorDepth : LodDataMgr
    {
        public override string SimName { get { return "SeaFloorDepth"; } }
        public override RenderTextureFormat TextureFormat { get { return RenderTextureFormat.RHalf; } }
        protected override bool NeedToReadWriteTextureData { get { return false; } }

        public override SimSettingsBase CreateDefaultSettings() { return null; }
        public override void UseSettings(SimSettingsBase settings) { }

        bool _targetsClear = false;

        public const string ShaderName = "Crest/Inputs/Depth/Cached Depths";

        public override void BuildCommandBuffer(OceanRenderer ocean, CommandBuffer buf)
        {
            base.BuildCommandBuffer(ocean, buf);

            // If there is nothing in the scene tagged up for depth rendering, and we have cleared the RTs, then we can early out
            var drawList = RegisterLodDataInputBase.GetRegistrar(GetType());
            if (drawList.Count == 0 && _targetsClear)
            {
                return;
            }

            for (int lodIdx = OceanRenderer.Instance.CurrentLodCount - 1; lodIdx >= 0; lodIdx--)
            {
                buf.SetRenderTarget(_targets, 0, CubemapFace.Unknown, lodIdx);
                buf.ClearRenderTarget(false, true, Color.red * 1000f);
                buf.SetGlobalInt(sp_LD_SliceIndex, lodIdx);
                SubmitDraws(lodIdx, buf);
            }

            // Targets are only clear if nothing was drawn
            _targetsClear = drawList.Count == 0;
        }

        readonly static string s_textureArrayName = "_LD_TexArray_SeaFloorDepth";
        private static TextureArrayParamIds s_textureArrayParamIds = new TextureArrayParamIds(s_textureArrayName);
        public static int ParamIdSampler(bool sourceLod = false) { return s_textureArrayParamIds.GetId(sourceLod); }
        protected override int GetParamIdSampler(bool sourceLod = false)
        {
            return ParamIdSampler(sourceLod);
        }
        public static void BindNull(IPropertyWrapper properties, bool sourceLod = false)
        {
            properties.SetTexture(ParamIdSampler(sourceLod), TextureArrayHelpers.BlackTextureArray);
        }

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        static void InitStatics()
        {
            // Init here from 2019.3 onwards
            s_textureArrayParamIds = new TextureArrayParamIds(s_textureArrayName);
        }
    }
}
