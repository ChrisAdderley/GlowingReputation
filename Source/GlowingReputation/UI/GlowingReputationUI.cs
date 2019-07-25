using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace GlowingReputation
{
  /// <summary>
  /// This class creates a small UI to track the current vessel's sensitive area status
  /// </summary>
  [KSPAddon(KSPAddon.Startup.EveryScene, false)]
  public class GlowingReputationUI:MonoBehaviour
  {
    private bool showToolbarButton = false;
    private bool showTips = false;

    private float repScale;
    private float fundsScale;
    private float scienceScale;

    // Control Vars
    protected static bool showWindow = false;
    protected int windowID = new System.Random(3256231).Next();
    public Rect windowPos = new Rect(200f, 200f, 700f, 400f);
    private Vector2 scrollPosition = Vector2.zero;
    private float scrollHeight = 0f;
    protected bool initUI = false;

    // Assets
    protected string toolbarUIIconURLOff = "DynamicBatteryStorage/UI/toolbar_off";
    protected string toolbarUIIconURLOn = "DynamicBatteryStorage/UI/toolbar_on";
    protected UIResources resources;

    // Stock toolbar button
    protected static ApplicationLauncherButton stockToolbarButton = null;

    public Rect WindowPosition { get {return windowPos; } set { windowPos = value; } }
    public UIResources GUIResources { get { return resources; } }

    public static GlowingReputationUI Instance { get; private set; }


    /// <summary>
    /// Turn the window on or off
    /// </summary>
    public static void ToggleWindow()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Toggle Window");
      showWindow = !showWindow;
    }

    /// <summary>
    /// Initialize the UI comoponents, do localization, set up styles
    /// </summary>
    protected virtual void InitUI()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Initializing");

      resources = new UIResources();
      initUI = true;
    }

    protected virtual void Awake()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Awake fired");
      GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
      GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);
    }

    protected virtual void Start()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Start fired");

      if (ApplicationLauncher.Ready)
        OnGUIAppLauncherReady();
    }
    protected void Update()
    {

      UpdateToolbar();
      UpdateTips();

    }
    void UpdateToolbar()
    {
      if (showWindow)
      {
        Vector3 pos = stockToolbarButton.GetAnchor();
        if (ApplicationLauncher.Instance.IsPositionedAtTop)
        {
          miniWindowPos = new Rect(Screen.width-280f, 0f, 240f, 42f);
        }
        else
        {
          miniWindowPos = new Rect(Screen.width - 280f, Screen.height-92f, 245f, 42f);
        }

        if (HighLogic.LoadedSceneIsFlight)
        {
          UpdateScalesFlight();
        }

        if (fundsScale > MultiplierWarningLevel)
          currentFundsWarning = warningString;
        else if (fundsScale > MultiplierDangerLevel)
          currentFundsWarning = dangerString;
        else
          currentFundsWarning = okString;


        if (repScale > MultiplierWarningLevel)
          currentReputationWarning = warningString;
        else if (repScale > MultiplierDangerLevel)
          currentReputationWarning = dangerString;
        else
          currentReputationWarning = okString;

        if (scienceScale > MultiplierWarningLevel)
          currentScienceWarning = warningString;
        else if (scienceScale > MultiplierDangerLevel)
          currentScienceWarning = dangerString;
        else
          currentScienceWarning = okString;

        currentFundsQuantity = String.Format("({0:F1}%)", fundsScale*100.0f);
        currentReputationQuantity = String.Format("({0:F1}%)", repScale*100.0f);
        currentScienceQuantity = String.Format("({0:F1}%)", scienceScale*100.0f);
      }
    }

    void UpdateScalesFlight()
    {
      if (FlightGlobals.activeVessl != null)
      {
        repScale = PenaltyHelpers.CalculateReputationLoss(FlightGlobals.activeVessl);
        fundsScale = PenaltyHelpers.CalculateFundsLoss(FlightGlobals.activeVessl);
        scienceScale = PenaltyHelpers.CalculateScienceLoss(FlightGlobals.activeVessl);
      }
    }

    void UpdateTips()
    {
      if (GlowingReputation.Instance.FirstLoad)
      {
        showTips = true;
      }
      else
      {
        showTips = false;
      }
    }

    protected virtual void OnGUI()
    {
      if (Event.current.type == EventType.Repaint || Event.current.isMouse) {}
        Draw();
    }

    /// <summary>
    /// Draw the UI
    /// </summary>
    protected virtual void Draw()
    {
      if (!initUI)
        InitUI();

      if (showWindow)
      {
        GUI.skin = HighLogic.Skin;

        windowPos = GUI.Window(windowID, windowPos, DrawMiniWindow, "", GUIResources.GetStyle("window_toolbar"));
      }
    }

    /// <summary>
    /// Draw the toolbar window
    /// </summary>
    /// <param name="windowId">window ID</param>

    protected virtual void DrawWindow(int windowId)
    {
      Rect titleRect = new Rect(39f, 0f, 160f, 32f);

      Rect reputationRect = new Rect(0f, 32f, 160f, 32f);
      Rect scienceRect = new Rect(0f, 64f, 160f, 32f);
      Rect fundsRect = new Rect(0f, 96f, 160f, 32f);

      Rect groupIconRect = new Rect(0f, 0f, 32f, 32f);
      Rect groupTextRect = new Rect(32f, 0f, 80f, 32f);
      Rect groupQuantityRect = new Rect(140f, 0f, 30f, 32f);

      GUI.Label(titleRect, windowTitle, GUIResources.GetStyle("text_header"));

      GUI.BeginGroup(reputationRect);
      GUI.DrawTextureWithTexCoords(groupIconRect, GUIResources.GetIcon("reputation").iconAtlas, GUIResources.GetIcon("reputation").iconRect);
      GUI.Label(groupTextRect, currentReputationWarning, GUIResources.GetStyle("text_warning"));
      GUI.Label(groupQuantityRect, currentReputationQuantity, GUIResources.GetStyle("text_quantity"));
      GUI.EndGroup();
      GUI.BeginGroup(scienceRect);
      GUI.DrawTextureWithTexCoords(groupIconRect, GUIResources.GetIcon("science").iconAtlas, GUIResources.GetIcon("science").iconRect);
      GUI.Label(groupTextRect, currentScienceWarning, GUIResources.GetStyle("text_basic"));
      GUI.Label(groupQuantityRect, currentScienceQuantity, GUIResources.GetStyle("text_basic"));
      GUI.EndGroup();
      GUI.BeginGroup(fundsRect);
      GUI.DrawTextureWithTexCoords(groupIconRect, GUIResources.GetIcon("funds").iconAtlas, GUIResources.GetIcon("funds").iconRect);
      GUI.Label(groupTextRect, currentFundsWarning, GUIResources.GetStyle("text_basic"));
      GUI.Label(groupQuantityRect, currentFundsQuantity, GUIResources.GetStyle("text_basic"));
      GUI.EndGroup();
    }


    void DrawTips()
    {}

    // Stock toolbar handling methods
    public void OnDestroy()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: OnDestroy Fired");
      // Remove the stock toolbar button
      GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
      if (stockToolbarButton != null)
      {
        ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
      }
    }

    protected void OnToolbarButtonToggle()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Toolbar Button Toggled");
      ToggleWindow();
      stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture(showWindow ? toolbarUIIconURLOn : toolbarUIIconURLOff, false));
    }


    protected void OnGUIAppLauncherReady()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: App Launcher Ready");
      if (ApplicationLauncher.Ready && stockToolbarButton == null)
      {
        stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(
            OnToolbarButtonToggle,
            OnToolbarButtonToggle,
            DummyVoid,
            DummyVoid,
            DummyVoid,
            DummyVoid,
            ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT,
            (Texture)GameDatabase.Instance.GetTexture(toolbarUIIconURLOff, false));
      }
    }

    protected void OnGUIAppLauncherDestroyed()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: App Launcher Destroyed");
      if (stockToolbarButton != null)
      {
        ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
        stockToolbarButton = null;
      }
    }

    protected void onAppLaunchToggleOff()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: App Launcher Toggle Off");
      stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture(toolbarUIIconURLOff, false));
    }

    protected void DummyVoid() { }
  }
}
