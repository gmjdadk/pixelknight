// Decompiled with JetBrains decompiler
// Type: PixelStarships.SpriteManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 80B94462-ED78-4229-8BBA-3D648F342C5D
// Assembly location: C:\Users\guoqiang.zhang66\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PixelKnight.Models;
using PixelStarships;

namespace PixelKnight.Managers
{
    public class SpriteManager
    {
        private Dictionary<string, SpriteDesign> spriteDictionaryByKey = new Dictionary<string, SpriteDesign>();
        private Dictionary<int, SpriteDesign> spriteDictionaryById = new Dictionary<int, SpriteDesign>();
        public static bool spritesDownloaded;
        private DatabaseManager databaseManager;

        protected SpriteManager()
        {
        }

        public Dictionary<string, SpriteDesign> GetSpriteDictionaryByKey()
        {
            return this.spriteDictionaryByKey;
        }

        private void ClearSpriteDictionaryByKey()
        {
            foreach (KeyValuePair<string, SpriteDesign> keyValuePair in this.spriteDictionaryByKey)
                keyValuePair.Value.ClearUnitySpriteData(true);
            this.spriteDictionaryByKey.Clear();
        }

        public Dictionary<int, SpriteDesign> GetSpriteDictionaryById()
        {
            return this.spriteDictionaryById;
        }

        private void ClearSpriteDictionaryById()
        {
            foreach (KeyValuePair<int, SpriteDesign> keyValuePair in this.spriteDictionaryById)
                keyValuePair.Value.ClearUnitySpriteData(true);
            this.spriteDictionaryById.Clear();
        }

        private void Awake()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            // ISSUE: explicit reference operation
            if (!(((Scene)@activeScene).get_name() != "LoadingScene") && Application.get_isPlaying())
                return;
            this.LoadSprites((SimpleManager.DownloadDelegate)null);
        }

        protected override void OnExist()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            // ISSUE: explicit reference operation
            if (!(((Scene)@activeScene).get_name() != "LoadingScene"))
                return;
            this.LoadSprites((SimpleManager.DownloadDelegate)null);
        }

        public void ClearUnitySpriteData()
        {
            foreach (KeyValuePair<string, SpriteDesign> keyValuePair in this.spriteDictionaryByKey)
                keyValuePair.Value.ClearUnitySpriteData(true);
            foreach (KeyValuePair<int, SpriteDesign> keyValuePair in this.spriteDictionaryById)
                keyValuePair.Value.ClearUnitySpriteData(true);
        }

        public void ClearUnitySpriteData(ShipDesign[] shipDesigns)
        {
            HashSet<int> spriteIds = new HashSet<int>();
            foreach (ShipDesign shipDesign in shipDesigns)
            {
                spriteIds.Add(shipDesign.DoorFrameLeftSprite.SpriteId);
                spriteIds.Add(shipDesign.DoorFrameRightSprite.SpriteId);
                spriteIds.Add(shipDesign.ExteriorSprite.SpriteId);
                spriteIds.Add(shipDesign.InteriorSprite.SpriteId);
                spriteIds.Add(shipDesign.LiftSprite.SpriteId);
                if (shipDesign.LogoSprite != null)
                    spriteIds.Add(shipDesign.LogoSprite.SpriteId);
                spriteIds.Add(shipDesign.RoomFrameSprite.SpriteId);
                spriteIds.Add(shipDesign.ThrustParticleSprite.SpriteId);
            }
            this.ClearUnitySpriteDataForSpriteIds(spriteIds);
        }

        public void ClearUnitySpriteData(RoomDesign[] roomDesigns)
        {
            HashSet<int> spriteIds = new HashSet<int>();
            foreach (RoomDesign roomDesign in roomDesigns)
            {
                spriteIds.Add(roomDesign.ImageSpriteId);
                spriteIds.Add(roomDesign.LogoSpriteId);
                spriteIds.Add(roomDesign.ConstructionSpriteId);
                if (SingletonManager<RoomManager>.Instance.roomDesignSpriteDictionary.ContainsKey(roomDesign.RoomDesignId))
                {
                    foreach (RoomDesignSprite roomDesignSprite in SingletonManager<RoomManager>.Instance.roomDesignSpriteDictionary[roomDesign.RoomDesignId].ToArray())
                    {
                        spriteIds.Add(roomDesignSprite.SpriteId);
                        if (roomDesignSprite.AnimationId != 0)
                        {
                            PSAnimation animationById = SingletonManager<AnimationManager>.Instance.GetAnimationByID(roomDesignSprite.AnimationId);
                            if (animationById != null)
                            {
                                foreach (SpriteDesign spriteDesign in animationById.AnimationSprites.ToArray())
                                    spriteIds.Add(spriteDesign.SpriteId);
                            }
                        }
                    }
                }
                if (roomDesign.MissileDesignId != 0)
                {
                    PSAnimation launchPsAnimation = roomDesign.MissileDesign.LaunchPsAnimation;
                    if (launchPsAnimation != null)
                    {
                        foreach (SpriteDesign spriteDesign in launchPsAnimation.AnimationSprites.ToArray())
                            spriteIds.Add(spriteDesign.SpriteId);
                    }
                    PSAnimation psAnimation = roomDesign.MissileDesign.PsAnimation;
                    if (psAnimation != null)
                    {
                        foreach (SpriteDesign spriteDesign in psAnimation.AnimationSprites.ToArray())
                            spriteIds.Add(spriteDesign.SpriteId);
                    }
                    PSAnimation hitPsAnimation = roomDesign.MissileDesign.HitPsAnimation;
                    if (hitPsAnimation != null)
                    {
                        foreach (SpriteDesign spriteDesign in hitPsAnimation.AnimationSprites.ToArray())
                            spriteIds.Add(spriteDesign.SpriteId);
                    }
                }
            }
            this.ClearUnitySpriteDataForSpriteIds(spriteIds);
        }

        public void ClearUnitySpriteData(CharacterDesign[] characterDesigns)
        {
            HashSet<int> spriteIds = new HashSet<int>();
            foreach (CharacterDesign characterDesign in characterDesigns)
            {
                if (characterDesign.ProfileSprite != null)
                    spriteIds.Add(characterDesign.ProfileSprite.SpriteId);
                foreach (PSCharacterPart characterPart in characterDesign.CharacterParts)
                {
                    spriteIds.Add(characterPart.ActionBorderSpriteId);
                    spriteIds.Add(characterPart.ActionSpriteId);
                    spriteIds.Add(characterPart.StandardBorderSpriteId);
                    spriteIds.Add(characterPart.StandardSpriteId);
                }
            }
            this.ClearUnitySpriteDataForSpriteIds(spriteIds);
        }

        public void ClearUnitySpriteData(ItemDesign[] itemDesigns)
        {
            HashSet<int> spriteIds = new HashSet<int>();
            foreach (ItemDesign itemDesign in itemDesigns)
            {
                spriteIds.Add(itemDesign.ImageSprite.SpriteId);
                spriteIds.Add(itemDesign.LogoSprite.SpriteId);
                if (itemDesign.CharacterPart != null)
                {
                    spriteIds.Add(itemDesign.CharacterPart.ActionBorderSprite.SpriteId);
                    spriteIds.Add(itemDesign.CharacterPart.ActionSprite.SpriteId);
                    spriteIds.Add(itemDesign.CharacterPart.StandardBorderSprite.SpriteId);
                    spriteIds.Add(itemDesign.CharacterPart.StandardSprite.SpriteId);
                }
                if (itemDesign.CharacterDesign != null)
                {
                    spriteIds.Add(itemDesign.CharacterDesign.ProfileSprite.SpriteId);
                    foreach (PSCharacterPart psCharacterPart in itemDesign.CharacterDesign.CharacterParts.ToArray())
                    {
                        spriteIds.Add(psCharacterPart.ActionBorderSprite.SpriteId);
                        spriteIds.Add(psCharacterPart.ActionSprite.SpriteId);
                        spriteIds.Add(psCharacterPart.StandardBorderSprite.SpriteId);
                        spriteIds.Add(psCharacterPart.StandardSprite.SpriteId);
                    }
                }
            }
            this.ClearUnitySpriteDataForSpriteIds(spriteIds);
        }

        private void ClearUnitySpriteDataForSpriteIds(HashSet<int> spriteIds)
        {
            foreach (int spriteId in spriteIds)
            {
                SpriteDesign spriteDesign = this.spriteDictionaryById[spriteId];
                if (spriteDesign != null)
                    spriteDesign.ClearUnitySpriteData(true);
            }
        }

        public void DownloadSprites(SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadSprites(SingletonManager<Configuration>.Instance.ServerUrl + "/FileService/ListSprites2?languageKey=" + SingletonManager<LocalizationManager>.Instance.CurrentServerLanguage, del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadSprites(string url, SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new SpriteManager.\u003CDownloadSprites\u003Ec__Iterator0() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        private void ParseSprite(string xml)
        {
            SpriteDesign spriteDesign = PSObject.Deserialize<SpriteDesign>(xml);
            if (!this.spriteDictionaryByKey.ContainsKey(spriteDesign.SpriteKey))
                this.spriteDictionaryByKey.Add(spriteDesign.SpriteKey, spriteDesign);
            if (this.spriteDictionaryById.ContainsKey(spriteDesign.SpriteId))
                return;
            this.spriteDictionaryById.Add(spriteDesign.SpriteId, spriteDesign);
        }

        private void ParseSprites(string xml)
        {
            SpriteContainer spriteContainer = SpriteContainer.LoadFromText(xml);
            int num = 0;
            foreach (SpriteDesign sprite in spriteContainer.sprites)
            {
                ++num;
                if (!this.spriteDictionaryByKey.ContainsKey(sprite.SpriteKey))
                    this.spriteDictionaryByKey.Add(sprite.SpriteKey, sprite);
                if (!this.spriteDictionaryById.ContainsKey(sprite.SpriteId))
                    this.spriteDictionaryById.Add(sprite.SpriteId, sprite);
            }
            Singleton<SharedManager>.Instance.SendTaskToMainThread((Action)(() => PlayerPrefs.SetFloat("SpriteVersion", (float)SingletonManager<SystemManager>.Instance.settings.SpriteVersion)), false);
        }

        public void LoadSprites(SimpleManager.DownloadDelegate del = null)
        {
            this.ClearSpriteDictionaryByKey();
            this.ClearSpriteDictionaryById();
            this.LoadXML("Sprites.txt", (Action<string>)(xmlText => this.ReadXML(SingletonManager<Configuration>.Instance.ServerUrl + "/FileService/ListSprites2?languageKey=" + SingletonManager<LocalizationManager>.Instance.CurrentServerLanguage, del, xmlText, "Sprites", new SimpleManager.ParseDelegate(this.ParseSprites), (Action)null)));
            SpriteManager.spritesDownloaded = true;
        }

        public SpriteDesign GetSpriteById(int id)
        {
            if (id != 0 && this.spriteDictionaryById.ContainsKey(id))
                return this.spriteDictionaryById[id];
            return (SpriteDesign)null;
        }

        public SpriteDesign GetSpriteByKey(string key)
        {
            if (!string.IsNullOrWhiteSpace(key) && this.spriteDictionaryByKey.ContainsKey(key))
                return this.spriteDictionaryByKey[key];
            return (SpriteDesign)null;
        }
    }
}
