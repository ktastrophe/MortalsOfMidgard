using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace MyFirstXNAGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    
    public class SoundUpdater : Microsoft.Xna.Framework.GameComponent
    {
        //Game1 myGame;
        public static SoundEffect 
            grassStepFX1, grassStepFX2, grassStepFX3,
            stoneStepFX1, stoneStepFX2, 
      
        slashFX, bonebreakFX, throwFX,
        clickFX, hoverFX,
        
       titleMusic, dungeonBGM, forestBGM = null;

        public enum EffectsIDs { slashFX, bonebreakFX, throwFX, grassStepFX1, grassStepFX2, grassStepFX3, stoneStepFX1, stoneStepFX2, clickFX, hoverFX, forestBGM, dungeonBGM};
        public static SoundEffect[] fxList;
        Dictionary<string, float> fxNameVolumeDict;
        SoundEffectInstance curSoundFx; //no concurrent sound effects for the time being 

        public static SoundEffectInstance BGMInst; //holds the current background music

        public SoundUpdater(Game1 game)
            : base(game)
        {
            // TODO: Construct any child components here
           
        }
        private void CreateAssetsFromFile(Dictionary<string, float>fxVolDict, string nameString)
        {
            StreamReader file = new StreamReader(nameString);
            //List<SoundEffect> tmpList = new List<SoundEffect>(fxLst);
            
            //for (int i = 0; i < fxList.Length; i++)
            // {
            //     //SoundEffect snd2 = fxLst[fxLst.IndexOf(snd2)];
            //     string curAsset = file.ReadLine();
            //     fxList[i] = Game.Content.Load<SoundEffect>(curAsset);
            //     fxList[i].Name = curAsset;
            //     string volStr = file.ReadLine();
            //     float assetVol = 0;
            //     foreach (char c in volStr)
            //     {//the text file uses digital roots to calculate volume (eg. 94 = 9+4 = 13, which becomes 1.3, or 130% of original effect volume)
            //         assetVol += (float)char.GetNumericValue(c);
            //     } assetVol *= 0.1f;
            //     fxVolDict.Add(curAsset, assetVol);
                 
            // }
            //file.Close();
            Type IDsType = typeof(EffectsIDs);
            
            int i = 0;
            while (!file.EndOfStream) //do
            {
               
                string curAsset = file.ReadLine();
                fxList[i] = Game.Content.Load<SoundEffect>(curAsset);
                
                fxList[i].Name = curAsset;
                string volStr = file.ReadLine();
                float assetVol = 0;
                foreach (char c in volStr)
                    //uses digital root to calculate volume (eg. 94 = 9+4 = 13, which becomes 1.3, or 130% of original effect volume)
                    assetVol += (float)char.GetNumericValue(c);
                assetVol *= 0.1f;
                fxVolDict.Add(curAsset, assetVol);
                i++;
            }
            //while (!file.EndOfStream);

            file.Close();


        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            
            Type eType = typeof(EffectsIDs);
            fxList = new SoundEffect[Enum.GetValues(eType).Length];
            
            //Background music
            titleMusic = Game.Content.Load<SoundEffect>("MoM Title");
            forestBGM = Game.Content.Load<SoundEffect>("forest bgm");
            dungeonBGM = Game.Content.Load<SoundEffect>("MoM Dungeon");

            //Sound effects
            
            SoundEffect.MasterVolume = 1f;
            
            fxNameVolumeDict = new Dictionary<string, float>();
            
            
            CreateAssetsFromFile(fxNameVolumeDict, "SoundFXList.dat");
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void PlayTitleMusic(GameTime gameTime, bool newGame)
        {
            
            if (curSoundFx == null)
            {
                //titleBGMInstance = titleMusic.Play();
                //titleMusic.Play();
                curSoundFx = titleMusic.Play();
                return;
            }
            if (newGame)
            {
                if (curSoundFx.State == SoundState.Playing)
                    curSoundFx.Stop();
            }
        }
        //change this function later to take a SoundEffect, assign it to a temp variable?
            //and use ONE generic Soundeffectinstance to play all sound effects?
        public void UpdateSound(SoundEffect snd)
        {
            
            //float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!(curSoundFx.State == SoundState.Playing))
            {
                float vol;
                if(!fxNameVolumeDict.TryGetValue(snd.Name, out vol))
                    vol = SoundEffect.MasterVolume;
                
                    //snd.Play(vol, 0, 0);
                curSoundFx = snd.Play(vol);
            }
                    
        }
        public SoundEffectInstance PlayBGM(SoundEffect bgm)
        {
          
            float vol;
            if (!fxNameVolumeDict.TryGetValue(bgm.Name, out vol))
                vol = SoundEffect.MasterVolume;
            //bgm.Play(vol, 0, 0);
            BGMInst = bgm.Play(vol);
            return BGMInst;
        }
    }
}