#pragma warning disable 649

using TriLibCore.General;
using TriLibCore.Mappers;
using TriLibCore.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TriLibCore.Samples
{
    /// <summary>
    /// This script will load the selected Model and create new Animations by merging the Animations with names as the pairs in "AnimationNamesA" and "AnimationNamesB".
    /// Use the "ModelAsset" field for testing inside Unity Editor, and "ModelFilename" if using the script on a built Player.
    /// </summary>
    public class MergeAnimations : MonoBehaviour
    {
        /// <summary>
        /// The Model filename (used when the sample is executed outside Unity Editor).
        /// </summary>
        public string ModelFilename;

#if UNITY_EDITOR
        /// <summary>
        /// The Model asset used to locate the filename when running in Unity Editor.
        /// </summary>
        [SerializeField]
        private Object ModelAsset;
#endif

        /// <summary>
        /// Returns the path to the "TriLibSample.obj" Model.
        /// </summary>
        private string ModelPath
        {
            get
            {
#if UNITY_EDITOR
                return AssetDatabase.GetAssetPath(ModelAsset);
#else
                return ModelFilename;
#endif
            }
        }

        /// <summary>
        /// Contains the primary Animation names to merge.
        /// </summary>
        public string[] AnimationNamesA;

        /// <summary>
        /// Contains the secondary Animation names to merge.
        /// </summary>
        public string[] AnimationNamesB;

        /// <summary>
        /// The AnimationClipMapper used to merge the Animations.
        /// </summary>
        private MergeAnimationClipsMapper _mergeAnimationClipsMapper;

        /// <summary>
        /// Loads the model from "ModelFilename" and merges Animations from pairs in "AnimationNamesA" and "AnimationNamesB" using a custom AnimationClipMapper.
        /// </summary>
        private void Start()
        {
            if (ModelFilename == null)
            {
                return;
            }
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            assetLoaderOptions.AnimationType = AnimationType.Legacy;
            _mergeAnimationClipsMapper = ScriptableObject.CreateInstance<MergeAnimationClipsMapper>();
            _mergeAnimationClipsMapper.AnimationNamesA = AnimationNamesA;
            _mergeAnimationClipsMapper.AnimationNamesB = AnimationNamesB;
            assetLoaderOptions.AnimationClipMappers = new AnimationClipMapper[] { _mergeAnimationClipsMapper };
            AssetLoader.LoadModelFromFile(ModelPath, null, OnMaterialsLoad, null, null, null, assetLoaderOptions);
        }

        /// <summary>
        /// Plays the merged Animations.
        /// The merged Animations are stored in the MergeAnimationClipsMapper MergedAnimationClips field.
        /// </summary>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.RootGameObject.TryGetComponent<Animation>(out var animation) && _mergeAnimationClipsMapper.MergedAnimationClips != null)
            {
                foreach (var mergedAnimationClip in _mergeAnimationClipsMapper.MergedAnimationClips)
                {
                    Debug.Log($"Playing merged animation: {mergedAnimationClip.Name}");
                    animation.Play(mergedAnimationClip.Name);
                }
            }
        }
    }
}