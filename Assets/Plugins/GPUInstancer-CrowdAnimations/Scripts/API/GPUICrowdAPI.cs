﻿#if GPU_INSTANCER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GPUInstancer.CrowdAnimations
{
    public static class GPUICrowdAPI
    {
        #region Crowd Animator

        /// <summary>
        /// Start the given Animation Clip on the given instance
        /// </summary>
        /// <param name="crowdInstance">Character that will play the animation clip</param>
        /// <param name="animationClip">Animation Clip</param>
        /// <param name="startTime">(Optional) Start time of the animation clip</param>
        /// <param name="speed">(Optional) Speed of the playing clip</param>
        /// <param name="transitionTime">(Optional) Time in seconds for smooth transition from the previous animation clips</param>
        public static void StartAnimation(GPUICrowdPrefab crowdInstance, AnimationClip animationClip, float startTime = -1.0f, float speed = 1.0f, float transitionTime = 0)
        {
            crowdInstance.StartAnimation(animationClip, startTime, speed, transitionTime);
        }

        /// <summary>
        /// Blend the given Animation Clips and start playing for the given instance
        /// </summary>
        /// <param name="crowdInstance">Character that will play the animation clips</param>
        /// <param name="animationWeights">Weights that will define the blending weights of the given animation clips</param>
        /// <param name="animationClip1">Clip 1</param>
        /// <param name="animationClip2">Clip 2</param>
        /// <param name="animationClip3">(Optional) Clip 3</param>
        /// <param name="animationClip4">(Optional) Clip 4</param>
        /// <param name="animationTimes">(Optional) Time of the animation clips</param>
        /// <param name="animationSpeeds">(Optional) Speed of the animation clips</param>
        /// <param name="transitionTime">(Optional) Time in seconds for smooth transition from the previous animation clips</param>
        public static void StartBlend(GPUICrowdPrefab crowdInstance, Vector4 animationWeights, AnimationClip animationClip1, AnimationClip animationClip2, 
            AnimationClip animationClip3 = null, AnimationClip animationClip4 = null, float[] animationTimes = null, float[] animationSpeeds = null, float transitionTime = 0)
        {
            crowdInstance.StartBlend(animationWeights, animationClip1, animationClip2, animationClip3, animationClip4, animationTimes, animationSpeeds, transitionTime);
        }

        /// <summary>
        /// Set the blend weights of the playing animation blends
        /// </summary>
        /// <param name="crowdInstance">Character to set the speed</param>
        /// <param name="animationWeights">Animation Weights</param>
        public static void SetAnimationWeights(GPUICrowdPrefab crowdInstance, Vector4 animationWeights)
        {
            crowdInstance.SetAnimationWeights(animationWeights);
        }
               
        /// <summary>
        /// Set the speed of the playing animation clips
        /// </summary>
        /// <param name="crowdInstance">Character to set the speed</param>
        /// <param name="animationSpeed">Speed value</param>
        public static void SetAnimationSpeed(GPUICrowdPrefab crowdInstance, float animationSpeed)
        {
            crowdInstance.SetAnimationSpeed(animationSpeed);
        }

        /// <summary>
        /// Set the speeds for multiple animation clips
        /// </summary>
        /// <param name="crowdInstance">Character to set the speed</param>
        /// <param name="animationSpeeds">Speed values</param>
        public static void SetAnimationSpeeds(GPUICrowdPrefab crowdInstance, float[] animationSpeeds)
        {
            crowdInstance.SetAnimationSpeeds(animationSpeeds);
        }

        /// <summary>
        /// Returns the current time of the clip
        /// </summary>
        /// <param name="crowdInstance">Character to get the time</param>
        /// <param name="animationClip">The clip to get the time for</param>
        public static float GetAnimationTime(GPUICrowdPrefab crowdInstance, AnimationClip animationClip)
        {
            return crowdInstance.GetAnimationTime(animationClip);
        }

        /// <summary>
        /// Sets the current time of the clip
        /// </summary>
        /// <param name="crowdInstance">Character to set the time</param>
        /// <param name="animationClip">The clip to set the time for</param>
        /// <param name="time">Time in seconds</param>
        public static void SetAnimationTime(GPUICrowdPrefab crowdInstance, AnimationClip animationClip, float time)
        {
            crowdInstance.SetAnimationTime(animationClip, time);
        }

        /// <summary>
        /// Set the animation speed for all instances of the given prototype
        /// </summary>
        /// <param name="crowdManager">Crowd Manager that the prototype is defined on</param>
        /// <param name="crowdPrototype">Crowd Prototype</param>
        /// <param name="animationSpeed">Speed value</param>
        public static void SetAnimationSpeedsForPrototype(GPUICrowdManager crowdManager, GPUICrowdPrototype crowdPrototype, float animationSpeed)
        {
            GPUICrowdRuntimeData runtimeData = (GPUICrowdRuntimeData)crowdManager.GetRuntimeData(crowdPrototype);
            if (runtimeData != null)
            {
                List<GPUInstancerPrefab> instanceList = crowdManager.GetRegisteredPrefabsRuntimeData()[crowdPrototype];
                foreach (GPUICrowdPrefab crowdInstance in instanceList)
                {
                    crowdInstance.SetAnimationSpeed(animationSpeed);
                }
            }
        }

        /// <summary>
        /// Set the animation speeds for multiple clips for all instances of the given prototype
        /// </summary>
        /// <param name="crowdManager">Crowd Manager that the prototype is defined on</param>
        /// <param name="crowdPrototype">Crowd Prototype</param>
        /// <param name="animationSpeeds">Speed values</param>
        public static void SetAnimationSpeedsForPrototype(GPUICrowdManager crowdManager, GPUICrowdPrototype crowdPrototype, float[] animationSpeeds)
        {
            GPUICrowdRuntimeData runtimeData = (GPUICrowdRuntimeData)crowdManager.GetRuntimeData(crowdPrototype);
            if (runtimeData != null)
            {
                int speedCount = 0;
                if (animationSpeeds != null)
                    speedCount = animationSpeeds.Length;
                List<GPUInstancerPrefab> instanceList = crowdManager.GetRegisteredPrefabsRuntimeData()[crowdPrototype];
                foreach (GPUICrowdPrefab crowdInstance in instanceList)
                {
                    int crowdAnimIndex = (crowdInstance.gpuInstancerID - 1) * 4;
                    for (int i = 0; i < crowdInstance.crowdAnimator.activeClipCount; i++)
                    {
                        if (i < speedCount)
                            crowdInstance.crowdAnimator.currentClipSpeeds[i] = animationSpeeds[i];
                        else
                            crowdInstance.crowdAnimator.currentClipSpeeds[i] = 1;
                        runtimeData.crowdAnimatorControllerData[crowdAnimIndex + i].z = crowdInstance.crowdAnimator.currentClipSpeeds[i];
                    }
                }
                runtimeData.crowdAnimatorControllerBuffer.SetData(runtimeData.crowdAnimatorControllerData);
            }
        }

        /// <summary>
        /// Add an event to an animation clip of a prototype
        /// </summary>
        /// <param name="crowdManager">Crowd Manager that the prototype is defined on</param>
        /// <param name="crowdPrototype">Crowd Prototype</param>
        /// <param name="animationClip">The clip to attach the event</param>
        /// <param name="eventFrame">Frame of the animation on which the event will take place</param>
        /// <param name="eventAction">UnityAction that will be invoked at the given frame for the given clip</param>
        /// <param name="floatParam">(Optional) Float parameter value</param>
        /// <param name="intParam">(Optional) Integer parameter value</param>
        /// <param name="stringParam">(Optional) String parameter value</param>
        public static void AddAnimationEvent(GPUICrowdManager crowdManager, GPUICrowdPrototype crowdPrototype, AnimationClip animationClip, int eventFrame,
            UnityAction<GPUICrowdPrefab, float, int, string> eventAction, float floatParam = 0, int intParam = 0, string stringParam = null)
        {
            if (!crowdManager.isInitialized)
                crowdManager.InitializeRuntimeDataAndBuffers();
            GPUICrowdRuntimeData runtimeData = (GPUICrowdRuntimeData)crowdManager.GetRuntimeData(crowdPrototype);
            if (runtimeData != null)
            {
                GPUIAnimationClipData clipData;
                if(runtimeData.animationClipDataDict.TryGetValue(animationClip.GetHashCode(), out clipData))
                {
                    if (runtimeData.eventDict == null)
                        runtimeData.eventDict = new Dictionary<GPUIAnimationClipData, List<GPUIAnimationEvent>>();
                    runtimeData.hasEvents = true;

                    GPUIAnimationEvent animationEvent = new GPUIAnimationEvent(crowdPrototype, animationClip);
                    animationEvent.eventFrame = eventFrame;
                    animationEvent.floatParam = floatParam;
                    animationEvent.intParam = intParam;
                    animationEvent.stringParam = stringParam;

                    List<GPUIAnimationEvent> animationEvents;
                    if (!runtimeData.eventDict.TryGetValue(clipData, out animationEvents))
                    {
                        animationEvents = new List<GPUIAnimationEvent>();

                        runtimeData.eventDict.Add(clipData, animationEvents);
                    }
                    animationEvents.Add(animationEvent);
                    animationEvent.AddListener(eventAction);
                }
            }
        }

        /// <summary>
        /// Add an event to the manager using GPUIAnimationEvent class properties
        /// </summary>
        /// <param name="crowdManager"></param>
        /// <param name="animationEvent"></param>
        public static void AddAnimationEvent(GPUICrowdManager crowdManager, GPUIAnimationEvent animationEvent)
        {
            if (!crowdManager.isInitialized)
                crowdManager.InitializeRuntimeDataAndBuffers();
            GPUICrowdRuntimeData runtimeData = (GPUICrowdRuntimeData)crowdManager.GetRuntimeData(animationEvent.prototype);
            if (runtimeData != null)
            {
                GPUIAnimationClipData clipData;
                if (runtimeData.animationClipDataDict.TryGetValue(animationEvent.clip.GetHashCode(), out clipData))
                {
                    if (runtimeData.eventDict == null)
                        runtimeData.eventDict = new Dictionary<GPUIAnimationClipData, List<GPUIAnimationEvent>>();
                    runtimeData.hasEvents = true;

                    List<GPUIAnimationEvent> animationEvents;
                    if (!runtimeData.eventDict.TryGetValue(clipData, out animationEvents))
                    {
                        animationEvents = new List<GPUIAnimationEvent>();

                        runtimeData.eventDict.Add(clipData, animationEvents);
                    }
                    animationEvents.Add(animationEvent);
                }
            }
        }

        /// <summary>
        /// Removes all animation events for the given prototype
        /// </summary>
        /// <param name="crowdManager">Crowd Manager that the prototype is defined on</param>
        /// <param name="crowdPrototype">Crowd Prototype</param>
        public static void ClearAnimationEvents(GPUICrowdManager crowdManager, GPUICrowdPrototype crowdPrototype)
        {
            if (crowdManager.isInitialized)
            {
                GPUICrowdRuntimeData runtimeData = (GPUICrowdRuntimeData)crowdManager.GetRuntimeData(crowdPrototype);
                if (runtimeData != null && runtimeData.eventDict != null)
                {
                    runtimeData.eventDict.Clear();
                    runtimeData.hasEvents = false;
                }
            }
        }

        #endregion Crowd Animator
    }
}
#endif //GPU_INSTANCER