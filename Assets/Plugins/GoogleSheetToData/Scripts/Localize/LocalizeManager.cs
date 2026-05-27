using System;
using System.Collections.Generic;
using System.Linq;
using Localize.Elements;
using SheetData.Localize.Elements;
using TMPro;
using UnityEngine;

namespace SheetData.Localize
{
    public class LocalizeManager : GameObjectSingleton<LocalizeManager>
    {
        public override bool DonDestroyedObject => true;
        [SerializeField] private List<LocalizeFontSet> _fontSets = new();
        private Dictionary<LangCode, TMP_FontAsset> _fontDic;
        private HashSet<ILocalizeListener> _listeners;
        private LangCode _lang = LangCode.KR;
        public LangCode Language => _lang;

        protected override void OnRegisterInstance()
        {
            base.OnRegisterInstance();
            _fontDic = new Dictionary<LangCode, TMP_FontAsset>();
            _listeners = new();
            foreach (var set in _fontSets)
                _fontDic.Add(set.Mode, set.Font);
        }

        public void AddListener(ILocalizeListener target)
        {
            
            _listeners.Add(target);
            target.RefreshLocalize(this, RefreshMode.All);
        }

        public void RemoveListener(ILocalizeListener target)
        {
            _listeners.Remove(target);
        }

        public void SetLanguage(LangCode mode)
        {
            bool dirty = _lang != mode;
            _lang = mode;
            if(dirty)
                RefreshListener(RefreshMode.Lang);    
        }

        public string Localize(string localizeKey)
        {
            return LocalizeSheetBinder.GetLocalizeString(_lang, localizeKey);
        }
        
        public TMP_FontAsset GetFontByLanguage(LangCode mode)
        {
            if (!_fontDic.TryGetValue(mode, out TMP_FontAsset font))
            {
                Debug.LogError("Undefined TMP_FontAsset");
                return _fontDic.First().Value;
            }
            return font;
        }
        
        public void RefreshListener(RefreshMode mode)
        {
            foreach (var l in _listeners)
                l.RefreshLocalize(this, mode);
        }
        
        [Serializable]
        struct LocalizeFontSet
        {
            public TMP_FontAsset Font;
            public LangCode Mode;
        }
    }
}