using System;
using System.Collections.Generic;
using TriLibCore.Interfaces;
using TriLibCore.Mappers;
using UnityEngine;

namespace TriLibCore.Samples
{
    /// <summary>
    /// Represents an Animation Clip Mapper that merges Animation Clip Tuples based on Animation names.
    /// Each pair on AnimationNamesA and AnimationNamesB will be merged.
    /// </summary>
    public class MergeAnimationClipsMapper : AnimationClipMapper
    {
        /// <summary>
        /// The Animations to be merged (Primary Animations).
        /// </summary>
        public string[] AnimationNamesA;

        /// <summary>
        /// The Animations to be merged (Secondary Animations).
        /// </summary>
        public string[] AnimationNamesB;

        /// <summary>
        /// The merged AnimationClips.
        /// </summary>
        public IList<AnimationClipWithCurves> MergedAnimationClips;

        public override AnimationClip[] MapArray(AssetLoaderContext assetLoaderContext, AnimationClip[] sourceAnimationClips)
        {
            void AddAllCurvesToClip(IAnimationCurveBinding animationCurveBinding, AnimationClipWithCurves animationClipWithCurves, float lastKeyTime, ref float lastProcessedTime)
            {
                foreach (var animationCurve in animationCurveBinding.AnimationCurves)
                {
                    var gameObject = assetLoaderContext.GameObjects[animationCurveBinding.Model];
                    var gameObjectPath = assetLoaderContext.GameObjectPaths[gameObject];
                    var mergedAnimationCurve = animationClipWithCurves.GetCurve(gameObjectPath, animationCurve.AnimatedType, animationCurve.Property);
                    foreach (var key in animationCurve.AnimationCurve.keys)
                    {
                        mergedAnimationCurve.AddKey(new Keyframe(lastKeyTime + key.time, key.value, key.inTangent, key.outTangent, key.inWeight, key.outWeight));
                        lastProcessedTime = Mathf.Max(lastProcessedTime, key.time);
                    }
                }
            }
            if (assetLoaderContext.RootGameObject == null || !assetLoaderContext.RootGameObject.TryGetComponent<Animation>(out var animationComponent) || AnimationNamesA == null || AnimationNamesB == null || AnimationNamesA.Length != AnimationNamesB.Length || assetLoaderContext.RootModel?.AllAnimations == null)
            {
                return sourceAnimationClips;
            }
            MergedAnimationClips = new List<AnimationClipWithCurves>();
            for (var i = 0; i < AnimationNamesA.Length; i++)
            {
                var animationNameA = AnimationNamesA[i];
                var animationNameB = AnimationNamesB[i];
                IAnimation animationA = null;
                IAnimation animationB = null;
                foreach (var animation in assetLoaderContext.RootModel.AllAnimations)
                {
                    if (animationA != null && animationB != null)
                    {
                        break;
                    }
                    if (string.Compare(animation.Name, animationNameA, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        animationA = animation;
                    }
                    else if (string.Compare(animation.Name, animationNameB, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        animationB = animation;
                    }
                }
                if (animationA == null || animationB == null || animationA == animationB)
                {
                    continue;
                }
                var mergedAnimationClipName = $"{animationA.Name}|{animationB.Name}|merged";
                var mergedAnimationClipFrameRate = (animationA.FrameRate + animationB.FrameRate) * 0.5f;
                var newAnimationClip = new AnimationClipWithCurves(mergedAnimationClipName, mergedAnimationClipFrameRate);
                MergedAnimationClips.Add(newAnimationClip);
                var lastKeyTime = 0f;
                var lastProcessedTime = 0f;
                foreach (var animationCurveBinding in animationA.AnimationCurveBindings)
                {
                    AddAllCurvesToClip(animationCurveBinding, newAnimationClip, lastKeyTime, ref lastProcessedTime);
                }
                lastProcessedTime += 1.0f / newAnimationClip.FrameRate;
                foreach (var animationCurveBinding in animationB.AnimationCurveBindings)
                {
                    AddAllCurvesToClip(animationCurveBinding, newAnimationClip, lastProcessedTime, ref lastKeyTime);
                }
            }
            var resultingAnimationClips = new List<AnimationClip>(sourceAnimationClips);
            foreach (var mergedAnimationClip in MergedAnimationClips)
            {
                var builtAnimationClip = mergedAnimationClip.BuildAnimationClip();
                if (assetLoaderContext.Options.EnsureQuaternionContinuity)
                {
                    builtAnimationClip.EnsureQuaternionContinuity();
                }
                resultingAnimationClips.Add(builtAnimationClip);
                animationComponent.AddClip(builtAnimationClip, builtAnimationClip.name);
            }
            return resultingAnimationClips.ToArray();
        }
    }
}