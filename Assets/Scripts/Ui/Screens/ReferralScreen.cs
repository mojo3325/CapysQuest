using Assets.SimpleLocalization;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ReferralScreen : MenuScreen
{
    private Label _emptyLabel;
    private Button _backButton;

    private static string _backButtonName = "ReferralBackButton";
    private static string _emptyLabelName = "ReferralLabel";
    private Tools _tools = new();
    
      protected override void SetVisualElements()
      {
       base.SetVisualElements();
       _showMenuBar = false;

       _backButton = _root.Q<Button>(_backButtonName);
       _emptyLabel = _root.Q<Label>(_emptyLabelName);
       SetupSizes();
      }

      private void OnEnable()
      {
          SettingsController.LanguageChanged += SetupScreenInfo;
          
          LocalizationManager.Read();
          
          SetupScreenInfo();
      }

      private void OnDisable()
      {
          SettingsController.LanguageChanged -= SetupScreenInfo;
      }

      private void SetupScreenInfo()
      {
          _emptyLabel.text = LocalizationManager.Localize("empty_label");
      }
      
      private void SetupSizes()
      {
          var devicetype = _tools.GetDeviceType();

          if (devicetype == DeviceType.Phone)
          {
              _emptyLabel.style.fontSize = new StyleLength(50);
          }
          else
          {
              _emptyLabel.style.fontSize = new StyleLength(35);
          }
      }
    
      protected override void RegisterButtonCallbacks()
      {
          base.RegisterButtonCallbacks();
          _backButton.clicked += _mainMenuUIManager.HideReferralScreen;
      }
}
