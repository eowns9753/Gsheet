using Localize.Elements;
using SheetData.Localize;
using SheetData.Localize.Elements;
using TMPro;
using UnityEngine;

namespace SheetData.SheetData
{
    public class TextMeshProLocalizeUGUI : TextMeshProUGUI, ILocalizeListener
    {
        [SerializeField] private LocalizeString _localizeKey = new LocalizeString();
        
        public LocalizeString text_local
        {
            get
            {
                return _localizeKey;
            }
            set
            {
                _localizeKey = value;
                text = LocalizeManager.Instance.Localize(_localizeKey.LocalizeKey);
            }
        }
        public string text_localizeKey
        {
            get
            {
                return _localizeKey.LocalizeKey;
            }
            set
            {
                _localizeKey ??= new LocalizeString();
                _localizeKey.LocalizeKey = value;
                text = LocalizeManager.Instance.Localize(_localizeKey.LocalizeKey);
            }
        }
        
        public LocalizeString LocalizeKey
        {
            get => _localizeKey;
            set => _localizeKey = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(LocalizeManager.HasUse)
                LocalizeManager.Instance.AddListener(this);
        }

        protected override void OnDisable()
        {
            if(LocalizeManager.HasUse)
                LocalizeManager.Instance.RemoveListener(this);
            base.OnDisable();
        }

        public void RefreshLocalize()
        {
            if (LocalizeManager.Instance is { })
                RefreshLocalize(LocalizeManager.Instance, RefreshMode.All);
        }
        
        public void RefreshLocalize(LocalizeManager manager, RefreshMode mode)
        {
            if (_localizeKey == null)
                return;
            
            if (manager == null)
            {
                text = _localizeKey.GetString();
            }
            else
            {
                text = LocalizeManager.Instance.Localize(_localizeKey.LocalizeKey);
            }
        }
    }
}