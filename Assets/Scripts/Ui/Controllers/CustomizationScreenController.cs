using UnityEngine;
using UnityEngine.UIElements;

public class CustomizationScreenController : MonoBehaviour
{
    [Header("Анимация")]
    [SerializeField] private GameObject _animationCapy;

    [Header("Controller")]
    [SerializeField] private CapyController _capyController;
    
    [Header("Parts")]
    [SerializeField] private SpriteRenderer _anim_body;
    [SerializeField] private SpriteRenderer _anim_back_ear;
    [SerializeField] private SpriteRenderer _anim_forw_ear;
    [SerializeField] private SpriteRenderer _anim_tear;
    [SerializeField] private SpriteRenderer _anim_mustache;
    [SerializeField] private SpriteRenderer _anim_left_back_paw;
    [SerializeField] private SpriteRenderer _anim_right_back_paw;
    [SerializeField] private SpriteRenderer _anim_left_forw_paw;
    [SerializeField] private SpriteRenderer _anim_right_forw_paw;

    [Header("Screens")]
    [SerializeField] private CustomizationScreen _customizationScreen;
    
    [Header("UI")]
    [SerializeField] private Sprite _emptySkin;
    
    private const string RUN_ANIMATION = "IsRunning";
    private Animator _animatorComponent;
    private int _selectedSkinIndex;

    private void OnEnable()
    {
        MenuBar.CustomizationButtonClicked += OnCustomizationOpen;
        CustomizationScreen.BackButtonClicked += StopShowAnimation;
        CustomizationScreen.NextButtonClicked += OnNextButtonClicked;
        CustomizationScreen.PreviousButtonClicked += OnPreviousButtonClicked;
    }

    private void OnDisable()
    {
        MenuBar.CustomizationButtonClicked -= OnCustomizationOpen;
        CustomizationScreen.BackButtonClicked -= StopShowAnimation;
        CustomizationScreen.NextButtonClicked -= OnNextButtonClicked;
        CustomizationScreen.PreviousButtonClicked -= OnPreviousButtonClicked;
    }

    private void OnCustomizationOpen()
    {
        ShowCapyAnimation();
        UpdateSkinSprites();
        UpdatePreviousNextSkinSprites();
    }
    
    private void ShowCapyAnimation()
    {
        _animationCapy.SetActive(true);
        _animatorComponent.SetBool(RUN_ANIMATION, true);
    }
    
    private void StopShowAnimation()
    {
        _animatorComponent.SetBool(RUN_ANIMATION, false);
        _animationCapy.SetActive(false);
    }
    
    private void OnNextButtonClicked()
    {
        if (_capyController.OwnedSkins.Count > 1)
        {
            _selectedSkinIndex = GetSelectedSkinIndex();
            _selectedSkinIndex = (_selectedSkinIndex + 1) % _capyController.OwnedSkins.Count;
            _capyController.SelectedSkin = _capyController.OwnedSkins[_selectedSkinIndex];
            UpdateSkinSprites();    
            UpdatePreviousNextSkinSprites();
        }
    }

    private void OnPreviousButtonClicked()
    {
        if (_capyController.OwnedSkins.Count > 1)
        {
            _selectedSkinIndex = GetSelectedSkinIndex();
            _selectedSkinIndex = (_selectedSkinIndex - 1 + _capyController.OwnedSkins.Count) % _capyController.OwnedSkins.Count;
            _capyController.SelectedSkin = _capyController.OwnedSkins[_selectedSkinIndex];
            UpdateSkinSprites();    
            UpdatePreviousNextSkinSprites();
        }
    }
    
    private void UpdateSkinSprites()
    {
        var selectedSkin = _capyController.SelectedSkin;

        _anim_body.sprite = selectedSkin.bodySprite;
        _anim_tear.sprite = selectedSkin.tearSprite;
        _anim_mustache.sprite = selectedSkin.mustacheSprite;
        _anim_left_back_paw.sprite = selectedSkin.backLeftPawSprite;
        _anim_right_back_paw.sprite = selectedSkin.backRightPawSprite;
        _anim_left_forw_paw.sprite = selectedSkin.forwLeftPawSprite;
        _anim_right_forw_paw.sprite = selectedSkin.forwRightPawSprite;
        _anim_back_ear.sprite = selectedSkin.backEar;
        _anim_forw_ear.sprite = selectedSkin.forwEar;
    }
    
    private void UpdatePreviousNextSkinSprites()
    {
        var ownedSkinsCount = _capyController.OwnedSkins.Count;
        
        _selectedSkinIndex = GetSelectedSkinIndex();

        if (ownedSkinsCount > 2)
        {
            var previousIndex = (_selectedSkinIndex - 1 + ownedSkinsCount) % ownedSkinsCount;
            var nextIndex = (_selectedSkinIndex + 1) % ownedSkinsCount;

            var previousSkin = _capyController.OwnedSkins[previousIndex];
            var nextSkin = _capyController.OwnedSkins[nextIndex];

            SetSkinIconSprite(_customizationScreen.PreviousSkin, previousSkin.skinIcon);
            SetSkinIconSprite(_customizationScreen.NextSkin, nextSkin.skinIcon);
        }
        else if (ownedSkinsCount == 2)
        {
            if (_selectedSkinIndex != 0)
            {
                var _previousSkin = _capyController.OwnedSkins[_selectedSkinIndex - 1].skinIcon;
                if (_previousSkin != null)
                {
                    SetSkinIconSprite(_customizationScreen.PreviousSkin, _previousSkin);
                }
                else
                {
                    SetSkinIconSprite(_customizationScreen.PreviousSkin, _emptySkin);    
                }
            }
            else
            {
                SetSkinIconSprite(_customizationScreen.PreviousSkin, _emptySkin);
            }

            if (_selectedSkinIndex + 1 < _capyController.OwnedSkins.Count)
            {
                var _nextSkin = _capyController.OwnedSkins[_selectedSkinIndex + 1].skinIcon;
                if (_nextSkin != null)
                {
                    SetSkinIconSprite(_customizationScreen.NextSkin, _nextSkin);
                }
                else
                {
                    SetSkinIconSprite(_customizationScreen.NextSkin, _emptySkin);
                }
            }
            else
            {
                SetSkinIconSprite(_customizationScreen.NextSkin, _emptySkin);
            }
        }
        else
        {
            SetSkinIconSprite(_customizationScreen.PreviousSkin, _emptySkin);
            SetSkinIconSprite(_customizationScreen.NextSkin, _emptySkin);
        }
    }
    
    private static void SetSkinIconSprite(VisualElement skinElement, Sprite icon)
    {
        if (skinElement != null && icon != null)
        {
            skinElement.style.backgroundImage = new StyleBackground(icon);
        }
    }
    
    private int GetSelectedSkinIndex()
    {
        for (var i = 0; i < _capyController.OwnedSkins.Count; i++)
        {
            if (_capyController.OwnedSkins[i] == _capyController.SelectedSkin)
            {
                return i;
            }
        }
        return 0;
    }
    
    private void Awake()
    {
        _animatorComponent = _animationCapy.GetComponent<Animator>();
    }
}

