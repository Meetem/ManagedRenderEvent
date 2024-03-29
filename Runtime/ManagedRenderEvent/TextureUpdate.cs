﻿using System;
using System.Runtime.InteropServices;

namespace ManagedRender
{
    public enum UnityRenderingExtEventType : int
    {
        kUnityRenderingExtEventSetStereoTarget =
            0, // issued during SetStereoTarget and carrying the current 'eye' index as parameter
        kUnityRenderingExtEventSetStereoEye, // issued during stereo rendering at the beginning of each eye's rendering loop. It carries the current 'eye' index as parameter
        kUnityRenderingExtEventStereoRenderingDone, // issued after the rendering has finished
        kUnityRenderingExtEventBeforeDrawCall, // issued during BeforeDrawCall and carrying UnityRenderingExtBeforeDrawCallParams as parameter
        kUnityRenderingExtEventAfterDrawCall, // issued during AfterDrawCall. This event doesn't carry any parameters
        kUnityRenderingExtEventCustomGrab, // issued during GrabIntoRenderTexture since we can't simply copy the resources

        //      when custom rendering is used - we need to let plugin handle this. It carries over
        //      a UnityRenderingExtCustomBlitParams params = { X, source, dest, 0, 0 } ( X means it's irrelevant )
        kUnityRenderingExtEventCustomBlit, // issued by plugin to insert custom blits. It carries over UnityRenderingExtCustomBlitParams as param.
        kUnityRenderingExtEventUpdateTextureBegin, // Deprecated.
        kUnityRenderingExtEventUpdateTextureEnd, // Deprecated.

        kUnityRenderingExtEventUpdateTextureBeginV1 =
            kUnityRenderingExtEventUpdateTextureBegin, // Deprecated. Issued to update a texture. It carries over UnityRenderingExtTextureUpdateParamsV1

        kUnityRenderingExtEventUpdateTextureEndV1 =
            kUnityRenderingExtEventUpdateTextureEnd, // Deprecated. Issued to signal the plugin that the texture update has finished. It carries over the same UnityRenderingExtTextureUpdateParamsV1 as kUnityRenderingExtEventUpdateTextureBeginV1
        kUnityRenderingExtEventUpdateTextureBeginV2, // Issued to update a texture. It carries over UnityRenderingExtTextureUpdateParamsV2
        kUnityRenderingExtEventUpdateTextureEndV2, // Issued to signal the plugin that the texture update has finished. It carries over the same UnityRenderingExtTextureUpdateParamsV2 as kUnityRenderingExtEventUpdateTextureBeginV2

        // keep this last
        kUnityRenderingExtEventCount,
        kUnityRenderingExtUserEventsStart = kUnityRenderingExtEventCount
    };

    public enum UnityRenderingExtTextureFormat : int
    {
        kUnityRenderingExtFormatNone = 0,
        kUnityRenderingExtFormatFirst = kUnityRenderingExtFormatNone,

        // sRGB formats
        kUnityRenderingExtFormatR8_SRGB,
        kUnityRenderingExtFormatR8G8_SRGB,
        kUnityRenderingExtFormatR8G8B8_SRGB,
        kUnityRenderingExtFormatR8G8B8A8_SRGB,

        // 8 bit integer formats
        kUnityRenderingExtFormatR8_UNorm,
        kUnityRenderingExtFormatR8G8_UNorm,
        kUnityRenderingExtFormatR8G8B8_UNorm,
        kUnityRenderingExtFormatR8G8B8A8_UNorm,
        kUnityRenderingExtFormatR8_SNorm,
        kUnityRenderingExtFormatR8G8_SNorm,
        kUnityRenderingExtFormatR8G8B8_SNorm,
        kUnityRenderingExtFormatR8G8B8A8_SNorm,
        kUnityRenderingExtFormatR8_UInt,
        kUnityRenderingExtFormatR8G8_UInt,
        kUnityRenderingExtFormatR8G8B8_UInt,
        kUnityRenderingExtFormatR8G8B8A8_UInt,
        kUnityRenderingExtFormatR8_SInt,
        kUnityRenderingExtFormatR8G8_SInt,
        kUnityRenderingExtFormatR8G8B8_SInt,
        kUnityRenderingExtFormatR8G8B8A8_SInt,

        // 16 bit integer formats
        kUnityRenderingExtFormatR16_UNorm,
        kUnityRenderingExtFormatR16G16_UNorm,
        kUnityRenderingExtFormatR16G16B16_UNorm,
        kUnityRenderingExtFormatR16G16B16A16_UNorm,
        kUnityRenderingExtFormatR16_SNorm,
        kUnityRenderingExtFormatR16G16_SNorm,
        kUnityRenderingExtFormatR16G16B16_SNorm,
        kUnityRenderingExtFormatR16G16B16A16_SNorm,
        kUnityRenderingExtFormatR16_UInt,
        kUnityRenderingExtFormatR16G16_UInt,
        kUnityRenderingExtFormatR16G16B16_UInt,
        kUnityRenderingExtFormatR16G16B16A16_UInt,
        kUnityRenderingExtFormatR16_SInt,
        kUnityRenderingExtFormatR16G16_SInt,
        kUnityRenderingExtFormatR16G16B16_SInt,
        kUnityRenderingExtFormatR16G16B16A16_SInt,

        // 32 bit integer formats
        kUnityRenderingExtFormatR32_UInt,
        kUnityRenderingExtFormatR32G32_UInt,
        kUnityRenderingExtFormatR32G32B32_UInt,
        kUnityRenderingExtFormatR32G32B32A32_UInt,
        kUnityRenderingExtFormatR32_SInt,
        kUnityRenderingExtFormatR32G32_SInt,
        kUnityRenderingExtFormatR32G32B32_SInt,
        kUnityRenderingExtFormatR32G32B32A32_SInt,

        // HDR formats
        kUnityRenderingExtFormatR16_SFloat,
        kUnityRenderingExtFormatR16G16_SFloat,
        kUnityRenderingExtFormatR16G16B16_SFloat,
        kUnityRenderingExtFormatR16G16B16A16_SFloat,
        kUnityRenderingExtFormatR32_SFloat,
        kUnityRenderingExtFormatR32G32_SFloat,
        kUnityRenderingExtFormatR32G32B32_SFloat,
        kUnityRenderingExtFormatR32G32B32A32_SFloat,

        // Luminance and Alpha format
        kUnityRenderingExtFormatL8_UNorm,
        kUnityRenderingExtFormatA8_UNorm,
        kUnityRenderingExtFormatA16_UNorm,

        // BGR formats
        kUnityRenderingExtFormatB8G8R8_SRGB,
        kUnityRenderingExtFormatB8G8R8A8_SRGB,
        kUnityRenderingExtFormatB8G8R8_UNorm,
        kUnityRenderingExtFormatB8G8R8A8_UNorm,
        kUnityRenderingExtFormatB8G8R8_SNorm,
        kUnityRenderingExtFormatB8G8R8A8_SNorm,
        kUnityRenderingExtFormatB8G8R8_UInt,
        kUnityRenderingExtFormatB8G8R8A8_UInt,
        kUnityRenderingExtFormatB8G8R8_SInt,
        kUnityRenderingExtFormatB8G8R8A8_SInt,

        // 16 bit packed formats
        kUnityRenderingExtFormatR4G4B4A4_UNormPack16,
        kUnityRenderingExtFormatB4G4R4A4_UNormPack16,
        kUnityRenderingExtFormatR5G6B5_UNormPack16,
        kUnityRenderingExtFormatB5G6R5_UNormPack16,
        kUnityRenderingExtFormatR5G5B5A1_UNormPack16,
        kUnityRenderingExtFormatB5G5R5A1_UNormPack16,
        kUnityRenderingExtFormatA1R5G5B5_UNormPack16,

        // Packed formats
        kUnityRenderingExtFormatE5B9G9R9_UFloatPack32,
        kUnityRenderingExtFormatB10G11R11_UFloatPack32,

        kUnityRenderingExtFormatA2B10G10R10_UNormPack32,
        kUnityRenderingExtFormatA2B10G10R10_UIntPack32,
        kUnityRenderingExtFormatA2B10G10R10_SIntPack32,
        kUnityRenderingExtFormatA2R10G10B10_UNormPack32,
        kUnityRenderingExtFormatA2R10G10B10_UIntPack32,
        kUnityRenderingExtFormatA2R10G10B10_SIntPack32,
        kUnityRenderingExtFormatA2R10G10B10_XRSRGBPack32,
        kUnityRenderingExtFormatA2R10G10B10_XRUNormPack32,
        kUnityRenderingExtFormatR10G10B10_XRSRGBPack32,
        kUnityRenderingExtFormatR10G10B10_XRUNormPack32,
        kUnityRenderingExtFormatA10R10G10B10_XRSRGBPack32,
        kUnityRenderingExtFormatA10R10G10B10_XRUNormPack32,

        // ARGB formats... TextureFormat legacy
        kUnityRenderingExtFormatA8R8G8B8_SRGB,
        kUnityRenderingExtFormatA8R8G8B8_UNorm,
        kUnityRenderingExtFormatA32R32G32B32_SFloat,

        // Depth Stencil for formats
        kUnityRenderingExtFormatD16_UNorm,
        kUnityRenderingExtFormatD24_UNorm,
        kUnityRenderingExtFormatD24_UNorm_S8_UInt,
        kUnityRenderingExtFormatD32_SFloat,
        kUnityRenderingExtFormatD32_SFloat_S8_Uint,
        kUnityRenderingExtFormatS8_Uint,

        // Compression formats
        kUnityRenderingExtFormatRGBA_DXT1_SRGB,
        kUnityRenderingExtFormatRGBA_DXT1_UNorm,
        kUnityRenderingExtFormatRGBA_DXT3_SRGB,
        kUnityRenderingExtFormatRGBA_DXT3_UNorm,
        kUnityRenderingExtFormatRGBA_DXT5_SRGB,
        kUnityRenderingExtFormatRGBA_DXT5_UNorm,
        kUnityRenderingExtFormatR_BC4_UNorm,
        kUnityRenderingExtFormatR_BC4_SNorm,
        kUnityRenderingExtFormatRG_BC5_UNorm,
        kUnityRenderingExtFormatRG_BC5_SNorm,
        kUnityRenderingExtFormatRGB_BC6H_UFloat,
        kUnityRenderingExtFormatRGB_BC6H_SFloat,
        kUnityRenderingExtFormatRGBA_BC7_SRGB,
        kUnityRenderingExtFormatRGBA_BC7_UNorm,

        kUnityRenderingExtFormatRGB_PVRTC_2Bpp_SRGB,
        kUnityRenderingExtFormatRGB_PVRTC_2Bpp_UNorm,
        kUnityRenderingExtFormatRGB_PVRTC_4Bpp_SRGB,
        kUnityRenderingExtFormatRGB_PVRTC_4Bpp_UNorm,
        kUnityRenderingExtFormatRGBA_PVRTC_2Bpp_SRGB,
        kUnityRenderingExtFormatRGBA_PVRTC_2Bpp_UNorm,
        kUnityRenderingExtFormatRGBA_PVRTC_4Bpp_SRGB,
        kUnityRenderingExtFormatRGBA_PVRTC_4Bpp_UNorm,

        kUnityRenderingExtFormatRGB_ETC_UNorm,
        kUnityRenderingExtFormatRGB_ETC2_SRGB,
        kUnityRenderingExtFormatRGB_ETC2_UNorm,
        kUnityRenderingExtFormatRGB_A1_ETC2_SRGB,
        kUnityRenderingExtFormatRGB_A1_ETC2_UNorm,
        kUnityRenderingExtFormatRGBA_ETC2_SRGB,
        kUnityRenderingExtFormatRGBA_ETC2_UNorm,

        kUnityRenderingExtFormatR_EAC_UNorm,
        kUnityRenderingExtFormatR_EAC_SNorm,
        kUnityRenderingExtFormatRG_EAC_UNorm,
        kUnityRenderingExtFormatRG_EAC_SNorm,

        kUnityRenderingExtFormatRGBA_ASTC4X4_SRGB,
        kUnityRenderingExtFormatRGBA_ASTC4X4_UNorm,
        kUnityRenderingExtFormatRGBA_ASTC5X5_SRGB,
        kUnityRenderingExtFormatRGBA_ASTC5X5_UNorm,
        kUnityRenderingExtFormatRGBA_ASTC6X6_SRGB,
        kUnityRenderingExtFormatRGBA_ASTC6X6_UNorm,
        kUnityRenderingExtFormatRGBA_ASTC8X8_SRGB,
        kUnityRenderingExtFormatRGBA_ASTC8X8_UNorm,
        kUnityRenderingExtFormatRGBA_ASTC10X10_SRGB,
        kUnityRenderingExtFormatRGBA_ASTC10X10_UNorm,
        kUnityRenderingExtFormatRGBA_ASTC12X12_SRGB,
        kUnityRenderingExtFormatRGBA_ASTC12X12_UNorm,

        // Video formats
        kUnityRenderingExtFormatYUV2,

        // Automatic formats, back-end decides
        kUnityRenderingExtFormatLDRAuto,
        kUnityRenderingExtFormatHDRAuto,
        kUnityRenderingExtFormatDepthAuto,
        kUnityRenderingExtFormatShadowAuto,
        kUnityRenderingExtFormatVideoAuto,
        kUnityRenderingExtFormatLast = kUnityRenderingExtFormatVideoAuto, // Remove?
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct UnityRenderingExtTextureUpdateParamsV2
    {
        public IntPtr uploadData;
        public IntPtr textureNativePtr;
        public uint userData;
        public UnityRenderingExtTextureFormat format;
        public uint width;
        public uint height;
        public uint bpp;
    }

    public delegate void CustomTextureUpdateV2(UnityRenderingExtEventType eventId, ref UnityRenderingExtTextureUpdateParamsV2 data);
}