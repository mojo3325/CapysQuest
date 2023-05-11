using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

public class CapyController : MonoBehaviour
{
    public static event Action CapyDiedThreeTimes;
    private const string RUN_ANIMATION = "IsRunning";
    private const string JETPACK_ANIMATION = "Jetpack";
    private const string HELMET_ANIMATION = "Helmet";
    private const string SELECTED_SKIN = "SelectedSkin";

    [Header("Эффекты")]
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject waterPrefab;

    [Header("Capy")]
    [SerializeField] private CapyCharacter capy;
    [SerializeField] private List<SkinData> _ownedSkins;
    [SerializeField] private List<SkinData> AllCapysSkins;
    
    [Header("CapyParts")]
    [SerializeField] private SpriteRenderer skin_body;
    [SerializeField] private SpriteRenderer skin_back_ear;
    [SerializeField] private SpriteRenderer skin_forw_ear;
    [SerializeField] private SpriteRenderer skin_tear;
    [SerializeField] private SpriteRenderer skin_mustache;
    [SerializeField] private SpriteRenderer skin_left_back_paw;
    [SerializeField] private SpriteRenderer skin_right_back_paw;
    [SerializeField] private SpriteRenderer skin_left_forw_paw;
    [SerializeField] private SpriteRenderer skin_right_forw_paw;
    
    [Header("Лэеры уровня")]
    [SerializeField] private LayerMask levelLayer;
    [SerializeField] private LayerMask iceLevelLayer;

    [Header("Показатели Кэпи")]
    [SerializeField] private bool _isActiveJetpack = false;
    [SerializeField] private bool _isActiveHelmet = false;
    [SerializeField] private int _capyDieCount;
    
    public LayerMask LevelLayer => levelLayer;
    public LayerMask IceLevelLayer => iceLevelLayer;
    public List<SkinData> OwnedSkins => _ownedSkins;
    
    public SkinData SelectedSkin
    {
        get => _selectedSkin;
        set
        {
            _selectedSkin = value;
            SaveSelectedSkin();
        }
    }

    public bool IsActiveJetpack => _isActiveJetpack;
    public bool IsActiveHelmet => _isActiveHelmet;

    [Header("Коорутины")]
    private Coroutine _jetpackCoroutine;
    private SkinData _selectedSkin; 

    private void OnEnable()
    {
        // Capy CHARACTER Died
        CapyCharacter.OnCapyDied += OnCapyDied;

        // Capy Enabled
        CapyCharacter.CapyEnabled += SetupCapy;

        //Ad controller
        IntAdController.OnAdClosed += ResetDieCount;

        //Capy Behaviour Events
        CapyCharacter.CapyHelmetEnemyTouched += OnTouchWithHelmet;
        CapyCharacter.JetpackClaimed += OnJetpackClaimed;
        CapyCharacter.HelmetClaimed += OnHelmetClaimed;
        
        CustomizationScreen.BackButtonClicked += SetupCurrentSkin;
        MenuBar.PlayButtonClicked += SetupCurrentSkin;

        FireBaseRepo.SignInUserSkinsFetched += SyncUserSkins;
        AccountScreenController.UserSignedOut += () => SyncUserSkins();
        FireBaseRepo.SuccessSignUp += () => SyncUserSkins();
    }

    private void OnDisable()
    {
        // Capy CHARACTER Died
        CapyCharacter.OnCapyDied -= OnCapyDied;

        // Capy Enabled
        CapyCharacter.CapyEnabled -= SetupCapy;

        //Ad controller
        IntAdController.OnAdClosed -= ResetDieCount;

        //Capy Behaviour Events
        CapyCharacter.CapyHelmetEnemyTouched -= OnTouchWithHelmet;
        CapyCharacter.JetpackClaimed -= OnJetpackClaimed;
        CapyCharacter.HelmetClaimed -= OnHelmetClaimed;
        
        CustomizationScreen.BackButtonClicked -= SetupCurrentSkin;
        MenuBar.PlayButtonClicked -= SetupCurrentSkin;
        
        FireBaseRepo.SignInUserSkinsFetched -= SyncUserSkins;
        AccountScreenController.UserSignedOut -= () => SyncUserSkins();
        FireBaseRepo.SuccessSignUp -= () => SyncUserSkins();
    }

    private void OnCapyDied(DieType dieType, Vector3 position)
    {
        if (_jetpackCoroutine != null)
            StopCoroutine(_jetpackCoroutine);

        ParticleSpawn(dieType, position);
        CountCapyDies();
    }

    private void CountCapyDies()
    {
        var isAdRemoved = PlayerPrefs.GetInt("RemoveAds", 0);

        if (_capyDieCount >= 2 && isAdRemoved == 0)
        {
            CapyDiedThreeTimes?.Invoke();
        }
        else
        {
            _capyDieCount += 1;
        }
    }

    private void SetupCurrentSkin()
    {
        skin_body.sprite = _selectedSkin.bodySprite;
        skin_tear.sprite = _selectedSkin.tearSprite;
        skin_mustache.sprite = _selectedSkin.mustacheSprite;
        skin_left_back_paw.sprite = _selectedSkin.backLeftPawSprite;
        skin_right_back_paw.sprite = _selectedSkin.backRightPawSprite;
        skin_left_forw_paw.sprite = _selectedSkin.forwLeftPawSprite;
        skin_right_forw_paw.sprite = _selectedSkin.forwRightPawSprite;
        skin_back_ear.sprite = _selectedSkin.backEar;
        skin_forw_ear.sprite = _selectedSkin.forwEar;
    }

    public void DetermineOwnedSkins()
    {
        _ownedSkins.Clear();

        foreach (var skin in AllCapysSkins.Where(skin => skin.skinName == "DEFAULT_SKIN"))
        {
            _ownedSkins.Add(skin);
        }
        
        foreach (SkinData skin in AllCapysSkins)
        {
            if (PlayerPrefs.HasKey(skin.skinName))
            {
                if (PlayerPrefs.GetInt(skin.skinName) == 1)
                {
                    _ownedSkins.Add(skin);
                }
            }
        }
    }
    
    private void SyncUserSkins(List<string> skins = null)
    {
        foreach (var skin in AllCapysSkins)
        {
            PlayerPrefs.DeleteKey(skin.skinName);
        }

        _selectedSkin = _ownedSkins[0];

        if (skins != null)
        {
            foreach (var skinName in skins.Where(skin => skin != "DEFAULT_SKIN"))
            {
                PlayerPrefs.SetInt(skinName, 1);
            }
        }
        
        DetermineOwnedSkins();
    }
    
    private void Awake()
    {
        DetermineOwnedSkins();
        
        LoadSelectedSkin();
        
        if (_selectedSkin == null)
        {
            _selectedSkin = _ownedSkins[0];
        }
    }

    private void SaveSelectedSkin()
    {
        if (_selectedSkin != null)
        {
            var selectedSkinName = _selectedSkin.name;
            PlayerPrefs.SetString(SELECTED_SKIN, selectedSkinName);
        }
    }

    private void LoadSelectedSkin()
    {
        if (PlayerPrefs.HasKey(SELECTED_SKIN))
        {
            var selectedSkinName = PlayerPrefs.GetString(SELECTED_SKIN);
            _selectedSkin = _ownedSkins.FirstOrDefault(skin => skin.name == selectedSkinName);
        }
    }

    private void ResetCapyState()
    {
        ResetCapyAnimations();
        ResetCapyBoosters();
    }

    private void ResetCapyAnimations()
    {
        capy.Animator.SetBool(RUN_ANIMATION, true);
    }

    private void ResetCapyBoosters()
    {
        _isActiveJetpack = false;
        _isActiveHelmet = false;

        if (_jetpackCoroutine != null)
            StopCoroutine(_jetpackCoroutine);
    }
    
    private void SetupCapy()
    {
        ResetCapyState();
    }

    private void ResetDieCount()
    {
        _capyDieCount = 0;
    }

    private void ParticleSpawn(DieType dieType, Vector3 targetPosition)
    {
        for (var i = 0; i < 15; i++)
        {
            var direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized * 80f;
            var position = new Vector3(targetPosition.x, targetPosition.y + 5, 0);
            var particle = Instantiate(dieType == DieType.Enemy ? bloodPrefab : waterPrefab, position, Quaternion.identity);
            particle.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
        }
    }

    private IEnumerator JetpackOffAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isActiveJetpack = false;
        capy.Animator.SetBool(RUN_ANIMATION, true);
    }

    private void OnTouchWithHelmet()
    {
        _isActiveHelmet = false;
        capy.Animator.SetBool(RUN_ANIMATION, true);
    }

    private void OnJetpackClaimed(float delay)
    {
        if (_jetpackCoroutine != null)
            StopCoroutine(_jetpackCoroutine);

        _isActiveHelmet = false;
        _isActiveJetpack = true;
        capy.Animator.SetBool(RUN_ANIMATION, false);
        capy.Animator.SetTrigger(JETPACK_ANIMATION);
        _jetpackCoroutine = StartCoroutine(JetpackOffAfter(delay));
    }

    private void OnHelmetClaimed()
    {
        if (_jetpackCoroutine != null)
            StopCoroutine(_jetpackCoroutine);

        _isActiveJetpack = false;
        _isActiveHelmet = true;
        capy.Animator.SetBool(RUN_ANIMATION, false);
        capy.Animator.SetTrigger(HELMET_ANIMATION);
    }
}
