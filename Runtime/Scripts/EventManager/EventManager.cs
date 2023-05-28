// © 2023 Marcello De Bonis. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGamesToolkit.Runtime
{
    /// <summary>
    /// Static class for managing events in the game.
    /// </summary>
    public static class EventManager
    {
        #region Variables & Properties

        #region AudioAction

        /// <summary>
        /// Event action invoked when an audio channel's volume is upgraded.
        /// </summary>
        public static Action<S_AudioChannel> OnUpgradeVolume;

        /// <summary>
        /// Event action invoked when an audio channel is muted.
        /// </summary>
        public static Action<S_AudioChannel> OnMuteChannel;

        /// <summary>
        /// Event action invoked when an audio channel is demuted.
        /// </summary>
        public static Action<S_AudioChannel> OnDemuteChannel;

        /// <summary>
        /// Event action invoked when an audio starts playing.
        /// </summary>
        public static Action<S_Audio> OnPlayAudio;

        /// <summary>
        /// Event action invoked when an audio starts playing with an action to be executed at the end.
        /// </summary>
        public static Action<S_Audio, Action> OnPlayAudioWithActionAtEnd;

        /// <summary>
        /// Event action invoked when an audio stops playing.
        /// </summary>
        public static Action<S_Audio> OnStopAudio;

        /// <summary>
        /// Event action invoked when an audio repeats its loop.
        /// </summary>
        public static Action<S_Audio> OnRepeatLoopAudio;

        /// <summary>
        /// Event action invoked when an audio cluster starts playing.
        /// </summary>
        public static Action<S_AudioCluster> OnPlayAudioCluster;

        /// <summary>
        /// Event action invoked when the next audio cluster song starts playing.
        /// </summary>
        public static Action<S_AudioCluster> OnNextAudioCluster;

        /// <summary>
        /// Event action invoked when an audio cluster stops playing.
        /// </summary>
        public static Action<S_AudioCluster> OnStopAudioCluster;

        /// <summary>
        /// List of currently reproducing audio clusters.
        /// </summary>
        public static List<S_AudioCluster> reproducingCluster;

        #endregion

        #endregion

        #region Initialization/Deleting

        /// <summary>
        /// Initializes the EventManager.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Application.quitting += Shutdown;
            OnPlayAudioCluster += PlayAudioCluster;
            OnNextAudioCluster += NextAudioCluster;
            OnStopAudioCluster += StopAudioCluster;

            InitVariables();
        }

        /// <summary>
        /// Initializes the variables used by the EventManager.
        /// </summary>
        private static void InitVariables()
        {
            reproducingCluster = new List<S_AudioCluster>();
        }

        /// <summary>
        /// Shuts down the EventManager.
        /// </summary>
        private static void Shutdown()
        {
            OnPlayAudioCluster -= PlayAudioCluster;
            OnNextAudioCluster -= NextAudioCluster;
            OnStopAudioCluster -= StopAudioCluster;

            ClearVariables();
        }

        /// <summary>
        /// Clears the variables used by the EventManager.
        /// </summary>
        private static void ClearVariables()
        {
            reproducingCluster.Clear();
        }

        #endregion

        #region AudioClusterMethods

        /// <summary>
        /// Starts playing an audio cluster.
        /// </summary>
        private static void PlayAudioCluster(S_AudioCluster audioCluster)
        {
            audioCluster.ResetIndex();
            OnPlayAudioWithActionAtEnd?.Invoke(audioCluster.CurrentSong(), () => { OnNextAudioCluster?.Invoke(audioCluster); });
            reproducingCluster.Add(audioCluster);
        }

        /// <summary>
        /// Plays the next song in the audio cluster.
        /// </summary>
        private static void NextAudioCluster(S_AudioCluster audioCluster)
        {
            if (reproducingCluster.Contains(audioCluster))
            {
                if (audioCluster.CurrentSong().content.loop)
                {
                    OnPlayAudioWithActionAtEnd?.Invoke(audioCluster.CurrentSong(),
                        () => { OnNextAudioCluster?.Invoke(audioCluster); });
                }
                else
                {
                    OnStopAudio?.Invoke(audioCluster.CurrentSong());
                    audioCluster.IncreaseSongIndex();
                    if (audioCluster.ExistCurrentSong())
                    {
                        OnPlayAudioWithActionAtEnd?.Invoke(audioCluster.CurrentSong(),
                            () => { OnNextAudioCluster?.Invoke(audioCluster); });
                    }
                    else
                    {
                        OnStopAudioCluster?.Invoke(audioCluster);
                    }
                }
            }
        }

        /// <summary>
        /// Stops playing an audio cluster.
        /// </summary>
        private static void StopAudioCluster(S_AudioCluster audioCluster)
        {
            reproducingCluster.Remove(audioCluster);
            OnStopAudio?.Invoke(audioCluster.CurrentSong());
            audioCluster.ResetIndex();
        }

        #endregion
    }
}