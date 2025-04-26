using System;
using System.Collections.Generic;
using UnityEngine;

namespace TriLibCore.Samples
{
    /// <summary>
    /// Represents an AnimationClip where Curves and Properties are decoupled in lists.
    /// </summary>
    public class AnimationClipWithCurves
    {
        private readonly AnimationClip _animationClip;
        private readonly IList<AnimationCurve> _animationCurves = new List<AnimationCurve>();
        private readonly IList<string> _gameObjectPaths = new List<string>();
        private readonly IList<Type> _types = new List<Type>();
        private readonly IList<string> _properties = new List<string>();
        
        public string Name => _animationClip?.name;
        public float FrameRate => _animationClip == null ? 0f : _animationClip.frameRate;

        public AnimationClipWithCurves(string name, float frameRate)
        {
            _animationClip = new AnimationClip();
            _animationClip.name = name;
            _animationClip.legacy = true;
            _animationClip.frameRate = frameRate;
        }
        
        public AnimationCurve GetCurve(string gameObjectPath, Type type, string property)
        {
            for (var i = 0; i < _animationCurves.Count; i++)
            {
                var existingAnimationCurve = _animationCurves[i];
                var existingGameObjectPath = _gameObjectPaths[i];
                var existingType = _types[i];
                var existingProperty = _properties[i];
                if (existingGameObjectPath == gameObjectPath && existingType == type && existingProperty == property)
                {
                    return existingAnimationCurve;
                }
            }
            var newAnimationCurve = new AnimationCurve();
            _animationCurves.Add(newAnimationCurve);
            _gameObjectPaths.Add(gameObjectPath);
            _types.Add(type);
            _properties.Add(property);
            return newAnimationCurve;
        }

        public AnimationClip BuildAnimationClip()
        {
            for (var i = 0; i < _animationCurves.Count; i++)
            {
                var existingAnimationCurve = _animationCurves[i];
                var existingGameObjectPath = _gameObjectPaths[i];
                var existingType = _types[i];
                var existingProperty = _properties[i];
                _animationClip.SetCurve(existingGameObjectPath, existingType, existingProperty, existingAnimationCurve);
            }
            return _animationClip;
        }
    }
}