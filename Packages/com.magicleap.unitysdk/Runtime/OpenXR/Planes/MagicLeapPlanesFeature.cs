// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System.Collections.Generic;
using MagicLeap.OpenXR.Subsystems;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.Planes
{
    /// <summary>
    /// Enables the Magic Leap OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with ML2.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Planes Subsystem",
        Desc="Necessary to deploy a Magic Leap 2 compatible application with Planes detection",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -2,
        FeatureId = FeatureId,
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        OpenxrExtensionStrings = PlaneExtensionName
    )]
#endif
    public class MagicLeapPlanesFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_planes";
        private const string PlaneExtensionName = "XR_EXT_plane_detection";
        
        private readonly List<XRPlaneSubsystemDescriptor> planeSubsystemDescriptors = new();
        
        internal PlanesNativeFunctions PlanesNativeFunctions;
        private MLXrPlaneSubsystem planeSubsystem;

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(PlaneExtensionName))
            {
                var instanceCreateResult = base.OnInstanceCreate(xrInstance);
                if (instanceCreateResult)
                {
                    MLXrPlaneSubsystem.RegisterDescriptor();
                }

                PlanesNativeFunctions = CreateNativeFunctions<PlanesNativeFunctions>();
                return instanceCreateResult;
            }
            Debug.LogError($"{PlaneExtensionName} is not enabled. Disabling {nameof(MagicLeapPlanesFeature)}");
            return false;
        }

        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(planeSubsystemDescriptors, MagicLeapXrProvider.PlanesSubsystemId);
        }

        protected override void OnSubsystemStart()
        {
            base.OnSubsystemStart();
            StartSubsystem<XRPlaneSubsystem>();
        }

        protected override void OnSubsystemStop()
        {
            base.OnSubsystemStop();
            StopSubsystem<XRPlaneSubsystem>();
        }

        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();
            DestroySubsystem<XRPlaneSubsystem>();
        }

        public void InvalidateCurrentPlanes()
        {
            if(planeSubsystem == null)
            {
                var activeLoader = XRGeneralSettings.Instance.Manager.activeLoader;
                planeSubsystem = activeLoader.GetLoadedSubsystem<XRPlaneSubsystem>() as MLXrPlaneSubsystem;
                if (planeSubsystem == null)
                {
                    return;
                }
            }
            planeSubsystem.InvalidateCurrentPlanes();
        }
    }
}
