/**
 *      _   _           _         __  __           _ 
 *     | \ | | __ _ ___| |_ _   _|  \/  | ___   __| |
 *     |  \| |/ _` / __| __| | | | |\/| |/ _ \ / _` |
 *     | |\  | (_| \__ \ |_| |_| | |  | | (_) | (_| |
 *     |_| \_|\__,_|___/\__|\__, |_|  |_|\___/ \__,_|
 *                          |___/                    
 *     
 *     a MelonLoader mod for the game Schedule I
 */
using System;
using System.Collections.Generic;

using UnityEngine;
using MelonLoader;
using HarmonyLib;

using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Employees;

using JetBrains.Annotations;
using Il2CppScheduleOne.NPCs;

public class NastyModClass : MelonMod
{
    public static NastyModClass Instance { get; private set; }

    private bool _isOpen = false;

    private int buttonWidth = 90;
    private int buttonHeight = 30;
    private int buttonSpacing = 10;

    private int menuWidth = 800;
    private int menuHeight = 600;
    private int menuSpacing = 15;

    private int tabWidth = 0;
    private int tabHeight = 0;

    private int menuTab = 0;
    private readonly List<string> menuTabs = new List<string> { "Player", "World", "Spawner", "Misc", "Employees", "Teleport", "Credits" };

    private Dictionary<string, List<string>> itemTree;
    private List<string> propertys;

    private int _moddedStackSize = 100;
    private int _moddedCash = 1000;
    private int _moddedBalance = 1000;

    private HarmonyLib.Harmony harmony;

    private bool _godMode = false;
    private bool _infiniteEnergy = false;
    private bool _infiniteStamina = false;
    private bool _neverWanted = false;
    private bool _ESP = false;
    private float _espDistance = 25f;
    private float _moveSpeedMultiplier = 1f;
    private float _crouchSpeedMultiplier = 0.6f;
    private float _jumpMultiplier = 1f;

    private string _selectedCategory = "Product";
    private Vector2 _itemSpawnerCategoryScrollPosition;
    private int _itemAmount = 1;

    private string _selectedProperty = "barn";
    private Vector2 _employeeSpawnerPropertyScrollPosition;

    private Vector2 _playerTabScrollPosition;
    private Vector2 _worldTabScrollPosition;
    private Vector2 _miscTabScrollPosition;
    private Vector2 _creditsTabScrollPosition;
    private Vector2 _npcTeleportScrollPosition;

    private readonly Dictionary<string, string> _npcIdMap = new Dictionary<string, string>
    {
        { "kylecooley", "Kyle_Cooley" },
        { "austinsteiner", "Austin_Steiner" },
        { "alberthoover", "Albert_Hoover" },
        { "jessiwaters", "Jessi_Waters" },
        { "kathyhenderson", "Kathy_Henderson" },
        { "micklubbin", "Mick_Lubbin" },
        { "samthompson", "Sam_Thompson" },
        { "peterfile", "Peter_File" },
        { "donnamartin", "Donna_Martin" },
        { "geraldinepoon", "Geraldine_Poon" },
        { "chloebowers", "Chloe_Bowers" },
        { "peggymyers", "Peggy_Myers" },
        { "benjicoleman", "Benji_Coleman" },
        { "ludwigmeyer", "Ludwig_Meyer" },
        { "bethpenn", "Beth_Penn" },
        { "mrs.ming", "Mrs._Ming" },
        { "trentsherman", "Trent_Sherman" },
        { "megcooley", "Meg_Cooley" },
        { "joyceball", "Joyce_Ball" },
        { "keithwagner", "Keith_Wagner" },
        { "shirleywatts", "Shirley_Watts" },
        { "jerrymontero", "Jerry_Montero" },
        { "dorislubbin", "Doris_Lubbin" },
        { "kimdelaney", "Kim_Delaney" },
        { "deanwebster", "Dean_Webster" },
        { "mollypresley", "Molly_Presley" },
        { "charlesrowland", "Charles_Rowland" },
        { "georgegreene", "George_Greene" },
        { "elizabethhomley", "Elizabeth_Homley" },
        { "jenniferrivera", "Jennifer_Rivera" },
        { "kevinoakley", "Kevin_Oakley" },
        { "louisfourier", "Louis_Fourier" },
        { "lucypennington", "Lucy_Pennington" },
        { "randycaulfield", "Randy_Caulfield" },
        { "bradcrosby", "Brad_Crosby" },
        { "eugenebuckley", "Eugene_Buckley" },
        { "gregfiggle", "Greg_Figgle" },
        { "jeffgilmore", "Jeff_Gilmore" },
        { "phillipwentworth", "Phillip_Wentworth" },
        { "annachesterfield", "Anna_Chesterfield" },
        { "lisagardener", "Lisa_Gardener" },
        { "genghisbarn", "Genghis_Barn" },
        { "crankyfrank", "Cranky_Frank" },
        { "javierperez", "Javier_Perez" },
        { "marcobarone", "Marco_Baron" },
        { "melissawood", "Melissa_Wood" },
        { "salvadormoreno", "Salvador_Moreno" },
        { "maccooper", "Mac_Cooper" },
        { "billykramer", "Billy_Kramer" },
        { "janelucero", "Jane_Lucero" },
        { "chrissullivan", "Chris_Sullivan" },
        { "hankstevenson", "Hank_Stevenson" },
        { "karenkennedy", "Karen_Kennedy" },
        { "alisonknight", "Alison_Knight" },
        { "jeremywilkinson", "Jeremy_Wilkinson" },
        { "carlbundy", "Carl_Bundy" },
        { "jackiestevenson", "Jackie_Stevenson" },
        { "jackknight", "Jack_Knight" },
        { "haroldcolt", "Harold_Colt" },
        { "weilong", "Wei_Long" },
        { "denniskennedy", "Dennis_Kennedy" },
        { "fionahancock", "Fiona_Hancock" },
        { "lilyturner", "Lily_Turner" },
        { "rayhoffman", "Ray_Hoffman" },
        { "jenheard", "Jen_Heard" },
        { "waltercussler", "Walter_Cussler" },
        { "leorivers", "Leo_Rivers" },
        { "herbetbleuball", "Herbet_Bleuball" },
        { "michaelboog", "Michael_Boog" },
        { "tobiaswentworth", "Tobias_Wentworth" },
        { "pearlmoore", "Pearl_Moore" }
    };

    private GUIStyle _titleStyle;
    private GUIStyle _headerStyle;
    private GUIStyle _buttonStyle;

    public void SendLoggerMsg(string msg)
    {
        MelonLogger.Msg(msg);
    }

    [Obsolete]
    public override void OnApplicationStart()
    {
        harmony = new HarmonyLib.Harmony("com.nastymod");

        MelonLogger.Msg("Initializing...");

        UnlockDebugMode();
    }
    private static void UnlockDebugMode()
    {
        try
        {
            MelonLogger.Msg("Attempting to unlock Debug Mode...");
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(DebugPatch), (string)null);
            MelonLogger.Msg("Debug Mode unlocked!");
        }
        catch (Exception value)
        {
            MelonLogger.Error($"Error unlocking Debug Mode: {value}");
        }
    }

    public override void OnInitializeMelon()
    {
        Instance = this;

        loadAllItems();
        loadAllPropertys();
    }

    private void loadAllItems()
    {
        NastyMod.JsonLoader itemLoader = new NastyMod.JsonLoader();
        itemTree = itemLoader.LoadItems();

        MelonLogger.Msg($"Loaded {itemTree.Count} item categories");
    }

    private void loadAllPropertys()
    {
        NastyMod.JsonLoader propertyLoader = new NastyMod.JsonLoader();
        propertys = propertyLoader.LoadPropertys();

        MelonLogger.Msg($"Loaded {propertys.Count} propertys");
    }

    public override void OnUpdate()
    {
        if (_godMode)
        {
            Player.Local.Health.RecoverHealth(100);
        }

        if (_infiniteEnergy)
        {
            Player.Local.Energy.RestoreEnergy();
        }

        if (_infiniteStamina)
        {
            PlayerMovement.Instance.ChangeStamina(100);
        }

        if (_neverWanted)
        {
            Player.Local.CrimeData.SetPursuitLevel(PlayerCrimeData.EPursuitLevel.None);
        }

        // SetStackSize();

        if (Input.GetKeyDown(KeyCode.F11))
            _isOpen = !_isOpen;
    }

    public override void OnGUI()
    {
        // **ESP** (Could be cleaned up and possibly distance sliders / box sliders. And not putting variables in loop but thats TODO)
        if (_ESP)
        {
            foreach (NPC npcs in NPCManager.NPCRegistry)
            {
                if (npcs.FirstName == String.Empty) continue;
                float dist = Vector3.Distance(Player.Local.transform.position, npcs.transform.position);
                if (dist > _espDistance) continue;
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(npcs.transform.position);
                if (screenPosition.z < 0) continue; //hides boxes for entities behind you.
                Vector2 start = new Vector2(Screen.width / 2, Screen.height);
                Vector2 end = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
                float boxwidth = 100f * (dist / 100f);
                if (boxwidth < 80) boxwidth = 80f;
                float boxheight = 100f * (dist / 100f);
                if (boxheight < 100) boxheight = 100f;
                GUI.Label(new Rect(end.x - (boxwidth / 2) - 20f, end.y - boxheight - 20f, boxwidth + 50f, boxheight + 50f), npcs.FirstName + " " + npcs.LastName);
                DrawBox(new Vector2(end.x - (boxwidth / 2), end.y - boxheight), boxwidth, boxheight, Color.blue, 1f);
            }
        }

        if (!_isOpen) return;

        // **Styles**
        _titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        _headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        _buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 14,
            fixedWidth = 80,
            fixedHeight = 24
        };
        // *********

        GUILayout.BeginArea(new Rect(50, 50, menuWidth, menuHeight), GUI.skin.box);

        // **Title**
        GUILayout.BeginArea(new Rect(15, 15, menuWidth - (menuSpacing * 2), 40));
        GUILayout.Label("NastyMod v1", _titleStyle);
        GUILayout.EndArea();
        // *********

        // **Dynamic Tab Buttons**
        GUILayout.BeginArea(new Rect(15, 55, menuWidth - (menuSpacing * 2), 40));
        GUILayout.BeginHorizontal();
        for (int i = 0; i < menuTabs.Count; i++)
        {
            if (menuTabs[i] == "Player" && !PlayerMovement.InstanceExists)
                continue;

            if (GUILayout.Button(menuTabs[i], GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                menuTab = i;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        // ***********************
        
        // **Tabs**
        GUILayout.BeginArea(new Rect(15, 95, menuWidth - (menuSpacing * 2), menuHeight - 110), GUI.skin.box);
        switch (menuTab)
        {
            case 0:
                if (PlayerMovement.InstanceExists) { 
                    RenderPlayerTab();
                    break;
                } else
                {
                    menuTab = 1;
                    RenderWorldTab();
                    break;
                }
            case 1:
                RenderWorldTab();
                break;
            case 2:
                RenderSpawnerTab();
                break;
            case 3:
                RenderMiscTab();
                break;
            case 4:
                RenderEmployeesTab();
                break;
            case 5:
                RenderTeleportTab();
                break;
            case 6:
                RenderCreditsTab();
                break;
        }
        GUILayout.EndArea();
        // ********

        GUILayout.EndArea();
    }

    private void RenderPlayerTab()
    {
        GUILayout.BeginArea(new Rect(15, 15, menuWidth - (menuSpacing * 4), menuHeight - 110 - (menuSpacing * 2)));

        // Tab Title
        GUILayout.Label("Player", _headerStyle);

        // Spacer
        GUILayout.Space(10);

        // ** Scrollable content
        _playerTabScrollPosition = GUILayout.BeginScrollView(_playerTabScrollPosition, GUILayout.Width(menuWidth - (menuSpacing * 4)), GUILayout.Height(menuHeight - 110 - (menuSpacing * 2) - 60));

        // ** God Mode toggle
        GUILayout.BeginHorizontal();
        GUILayout.Label("God Mode");
        string status_godMode = _godMode ? "On" : "Off";
        if (GUILayout.Button(status_godMode, _buttonStyle))
        {
            _godMode = !_godMode;
            MelonLogger.Msg("God Mode toggled!");
        }
        GUILayout.EndHorizontal();

        // ** Infinite Energy toggle
        GUILayout.BeginHorizontal();
        GUILayout.Label("Infinite Energy");
        string status_infiniteEnergy = _infiniteEnergy ? "On" : "Off";
        if (GUILayout.Button(status_infiniteEnergy, _buttonStyle))
        {
            _infiniteEnergy = !_infiniteEnergy;
            MelonLogger.Msg("Infinite Energy toggled!");
        }
        GUILayout.EndHorizontal();

        // ** Infinite Stamina toggle
        GUILayout.BeginHorizontal();
        GUILayout.Label("Infinite Stamina");
        string status_infiniteStamina = _infiniteStamina ? "On" : "Off";
        if (GUILayout.Button(status_infiniteStamina, _buttonStyle))
        {
            _infiniteStamina = !_infiniteStamina;
            MelonLogger.Msg("Infinite Stamina toggled!");
        }
        GUILayout.EndHorizontal();

        // ** Never wanted toggle
        GUILayout.BeginHorizontal();
        GUILayout.Label("Never Wanted");
        string status_neverWanted = _neverWanted ? "On" : "Off";
        if (GUILayout.Button(status_neverWanted, _buttonStyle))
        {
            _neverWanted = !_neverWanted;
            MelonLogger.Msg("Never Wanted toggled!");
        }
        GUILayout.EndHorizontal();

        // Spacer
        GUILayout.Space(20);

        // ** Move speed slider
        GUILayout.BeginHorizontal();
        GUILayout.Label("Move speed multiplier: " + _moveSpeedMultiplier);
        _moveSpeedMultiplier = GUILayout.HorizontalSlider(_moveSpeedMultiplier, 1f, 10f, GUILayout.Width(180), GUILayout.Height(10));
        PlayerMovement.Instance.MoveSpeedMultiplier = _moveSpeedMultiplier;
        GUILayout.EndHorizontal();

        // ** Jump Multiplier slider
        GUILayout.BeginHorizontal();
        GUILayout.Label("Jump Multiplier: " + _jumpMultiplier);
        _jumpMultiplier = GUILayout.HorizontalSlider(_jumpMultiplier, 1f, 10f, GUILayout.Width(180), GUILayout.Height(10));
        PlayerMovement.JumpMultiplier = _jumpMultiplier;
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void RenderWorldTab()
    {
        GUILayout.BeginArea(new Rect(15, 15, menuWidth - (menuSpacing * 4), menuHeight - 110 - (menuSpacing * 2)));

        // Tab Title
        GUILayout.Label("World", _headerStyle);

        // Spacer
        GUILayout.Space(10);

        // ** Scrollable content
        _worldTabScrollPosition = GUILayout.BeginScrollView(_worldTabScrollPosition, GUILayout.Width(menuWidth - (menuSpacing * 4)), GUILayout.Height(menuHeight - 110 - (menuSpacing * 2) - 60));

        // ** ESP toggle
        GUILayout.BeginHorizontal();
        GUILayout.Label("ESP (Box Based)");
        string status_ESP = _ESP ? "On" : "Off";
        if (GUILayout.Button(status_ESP, _buttonStyle))
        {
            _ESP = !_ESP;
            MelonLogger.Msg("ESP toggled!");
        }
        GUILayout.EndHorizontal();

        // Spacer
        GUILayout.Space(10);

        // ** ESP Distance slider
        GUILayout.BeginHorizontal();
        GUILayout.Label("ESP Distance: " + _espDistance.ToString("F0") + "m");
        _espDistance = GUILayout.HorizontalSlider(_espDistance, 10f, 200f, GUILayout.Width(180), GUILayout.Height(10));
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void RenderSpawnerTab()
    {
        GUILayout.BeginArea(new Rect(15, 15, menuWidth - (menuSpacing * 4), menuHeight - 110 - (menuSpacing * 2)));

        // Tab Title
        GUILayout.Label("Spawner", _headerStyle);

        #region Left Side
        GUILayout.BeginArea(new Rect(0, 40, (menuWidth - (menuSpacing * 4) - menuSpacing) / 4, menuHeight - 110 - (menuSpacing * 2) - 40));
        
        // ** Item Category dropdown
        _itemSpawnerCategoryScrollPosition = GUILayout.BeginScrollView(_itemSpawnerCategoryScrollPosition, GUILayout.Width((menuWidth - (menuSpacing * 4) - menuSpacing) / 4), GUILayout.Height(menuHeight - 110 - (menuSpacing * 2) - 80 - menuSpacing));
        foreach (var category in itemTree)
        {
            if (GUILayout.Button(category.Key))
            {
                _selectedCategory = category.Key;
            }
        }
        GUILayout.EndScrollView();

        // Spacer
        GUILayout.Space(10);

        // ** Item Amount slider
        GUILayout.Label($"Item Amount: {_itemAmount}");
        _itemAmount = (int)GUILayout.HorizontalSlider(_itemAmount, 1, _moddedStackSize);

        GUILayout.EndArea();
        #endregion

        #region Right side
        GUILayout.BeginArea(new Rect(((menuWidth - (menuSpacing * 4) - menuSpacing) / 4) + menuSpacing, 40, ((menuWidth - (menuSpacing * 4) - menuSpacing) / 4) * 3, menuHeight - 110 - (menuSpacing * 2) - 40));
        
        // ** Item buttons
        int columns = 3;
        int currentColumn = 0;
        GUILayout.BeginHorizontal();
        foreach (var category in itemTree)
        {
            if (category.Key == _selectedCategory)
            {
                foreach (var item in category.Value)
                {
                    if (currentColumn == columns)
                    {
                        currentColumn = 0;

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }

                    int availableWidth = ((menuWidth - (menuSpacing * 4) - menuSpacing) / 4) * 3;
                    if (GUILayout.Button(item, GUILayout.Width((availableWidth - (5 * columns-1)) / columns)))
                    {
                        Il2CppScheduleOne.Console.AddItemToInventoryCommand command = new Il2CppScheduleOne.Console.AddItemToInventoryCommand();
                        Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();
                        args.Add(item);
                        args.Add(_itemAmount.ToString());
                        command.Execute(args);
                    }

                    currentColumn++;
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        #endregion

        GUILayout.EndArea();
    }

    private void RenderMiscTab()
    {
        GUILayout.BeginArea(new Rect(15, 15, menuWidth - (menuSpacing * 4), menuHeight - 110 - (menuSpacing * 2)));

        // Tab Title
        GUILayout.Label("Misc", _headerStyle);

        // Spacer
        GUILayout.Space(10);

        // ** Scrollable content
        _miscTabScrollPosition = GUILayout.BeginScrollView(_miscTabScrollPosition, GUILayout.Width(menuWidth - (menuSpacing * 4)), GUILayout.Height(menuHeight - 110 - (menuSpacing * 2) - 60));

        // ** Default Stack Limit slider
        GUILayout.Label("Default Stack Limit: " + _moddedStackSize);
        _moddedStackSize = (int)GUILayout.HorizontalSlider(_moddedStackSize, 10, 250);

        // Spacer
        GUILayout.Space(10);

        // Note
        GUILayout.Label("Change equipped quality type");
        GUILayout.Label("NOTE: You need to have the item equipped for this feature to work.");

        // Spacer
        GUILayout.Space(10);

        // ** Quality type buttons
        GUILayout.BeginHorizontal();
        foreach (var qualityType in Enum.GetValues(typeof(EQuality)))
        {
            if (GUILayout.Button(qualityType.ToString(), _buttonStyle))
            {
                Il2CppScheduleOne.Console.SetQuality command = new Il2CppScheduleOne.Console.SetQuality();
                Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();
                args.Add(qualityType.ToString());
                command.Execute(args);
            }
        }
        GUILayout.EndHorizontal();

        // Spacer
        GUILayout.Space(20);

        // ** Package Product section
        GUILayout.Label("Package Equipped Product");
        GUILayout.Label("NOTE: You need to have a product equipped for this feature to work.");
        
        // Spacer
        GUILayout.Space(10);

        // ** Package type buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Baggie", _buttonStyle))
        {
            Il2CppScheduleOne.Console.PackageProduct command = new Il2CppScheduleOne.Console.PackageProduct();
            Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();
            args.Add("Baggie");
            command.Execute(args);
            MelonLogger.Msg("Packaged product into Baggie!");
        }
        if (GUILayout.Button("Jar", _buttonStyle))
        {
            Il2CppScheduleOne.Console.PackageProduct command = new Il2CppScheduleOne.Console.PackageProduct();
            Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();
            args.Add("Jar");
            command.Execute(args);
            MelonLogger.Msg("Packaged product into Jar!");
        }
        if (GUILayout.Button("Brick", _buttonStyle))
        {
            Il2CppScheduleOne.Console.PackageProduct command = new Il2CppScheduleOne.Console.PackageProduct();
            Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();
            args.Add("Brick");
            command.Execute(args);
            MelonLogger.Msg("Packaged product into Brick!");
        }
        GUILayout.EndHorizontal();

        // Spacer
        GUILayout.Space(20);

        // ** Cash Modifier
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Cash: " + _moddedCash);
        _moddedCash = (int) GUILayout.HorizontalSlider(_moddedCash, 0, 100000, GUILayout.Width((menuWidth - (30 * 2)) - 170), GUILayout.Height(20));
        GUILayout.EndVertical();
        if (GUILayout.Button("Add", _buttonStyle))
        {
            Il2CppScheduleOne.Console.ChangeCashCommand command = new Il2CppScheduleOne.Console.ChangeCashCommand();
            Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();

            args.Add(_moddedCash.ToString());

            command.Execute(args);
        }
        if (GUILayout.Button("Remove", _buttonStyle))
        {
            Il2CppScheduleOne.Console.ChangeCashCommand command = new Il2CppScheduleOne.Console.ChangeCashCommand();
            Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();

            args.Add($"-{_moddedCash.ToString()}");

            command.Execute(args);
        }
        GUILayout.EndHorizontal();

        // Spacer
        GUILayout.Space(10);

        // ** Balance Modifier
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Balance: " + _moddedBalance);
        _moddedBalance = (int)GUILayout.HorizontalSlider(_moddedBalance, 0, 100000, GUILayout.Width((menuWidth - (30 * 2)) - 170), GUILayout.Height(20));
        GUILayout.EndVertical();
        if (GUILayout.Button("Add", _buttonStyle))
        {
            Il2CppScheduleOne.Console.ChangeOnlineBalanceCommand command = new Il2CppScheduleOne.Console.ChangeOnlineBalanceCommand();
            Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();

            args.Add(_moddedBalance.ToString());

            command.Execute(args);
        }
        if (GUILayout.Button("Remove", _buttonStyle))
        {
            Il2CppScheduleOne.Console.ChangeOnlineBalanceCommand command = new Il2CppScheduleOne.Console.ChangeOnlineBalanceCommand();
            Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();

            args.Add($"-{_moddedBalance.ToString()}");

            command.Execute(args);
        }
        GUILayout.EndHorizontal();

        // Spacer
        GUILayout.Space(20);

        // ** Unlock all achievements button
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Unlock all achievements", GUILayout.Width(menuWidth - (30 * 2) - 20), GUILayout.Height(24)))
        {
            foreach (var achievement in Enum.GetValues(typeof(AchievementManager.EAchievement)))
            {
                AchievementManager.Instance.UnlockAchievement((AchievementManager.EAchievement)achievement);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void RenderEmployeesTab()
    {
        GUILayout.BeginArea(new Rect(15, 15, menuWidth - (menuSpacing * 4), menuHeight - 110 - (menuSpacing * 2)));

        // Tab Title
        GUILayout.Label($"Employees on Property \"{_selectedProperty}\"", _headerStyle);

        // Spacer
        GUILayout.Space(10);

        #region Left Side
        GUILayout.BeginArea(new Rect(0, 40, (menuWidth - (menuSpacing * 4) - menuSpacing) / 4, menuHeight - 110 - (menuSpacing * 2) - 40));
        // ** Property buttons
        _employeeSpawnerPropertyScrollPosition = GUILayout.BeginScrollView(_employeeSpawnerPropertyScrollPosition, GUILayout.Width((menuWidth - (menuSpacing * 4) - menuSpacing) / 4), GUILayout.Height(menuHeight - 110 - (menuSpacing * 2) - 80 - menuSpacing));
        foreach (var property in propertys)
        {
            if (GUILayout.Button(property))
            {
                _selectedProperty = property;
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        #endregion

        #region Right side
        GUILayout.BeginArea(new Rect(((menuWidth - (menuSpacing * 4) - menuSpacing) / 4) + menuSpacing, 40, ((menuWidth - (menuSpacing * 4) - menuSpacing) / 4) * 3, menuHeight - 110 - (menuSpacing * 2) - 40));
        
        // ** Employee buttons
        Dictionary<string, Dictionary<string, List<Employee>>> employees = new Dictionary<string, Dictionary<string, List<Employee>>>();
        employees = GetAlLEmployees();

        if (!employees.ContainsKey(_selectedProperty))
        {
            GUILayout.Label($"No Employees on Property \"{_selectedProperty}\"");
        } else
        {
            foreach (var employeeType in employees[_selectedProperty])
            {
                GUILayout.Label(employeeType.Key);
                foreach (var employee in employeeType.Value)
                {
                    string currentEmployeeType = employee.EmployeeType.ToString();
                    string currentEmployeeName = employee.FirstName;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{currentEmployeeType} {currentEmployeeName}");

                    if (GUILayout.Button("TP", GUILayout.Width(60)))
                    {
                        employee.gameObject.transform.position = Player.Local.transform.position;
                    }
                    if (GUILayout.Button("Fire", GUILayout.Width(60)))
                    {
                        employee.SendFire();
                        employee.Movement.MoveSpeedMultiplier = 2.5f;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(10);
            }
        }
        
        // Spacer
        GUILayout.Space(10);

        // ** Add Employee buttons
        GUILayout.BeginHorizontal();
        foreach (var employeeType in Enum.GetValues(typeof(EEmployeeType)))
        {
            if (GUILayout.Button($"Add {employeeType.ToString()}"))
            {
                Il2CppScheduleOne.Console.AddEmployeeCommand command = new Il2CppScheduleOne.Console.AddEmployeeCommand();
                Il2CppSystem.Collections.Generic.List<string> args = new Il2CppSystem.Collections.Generic.List<string>();

                args.Add(employeeType.ToString());
                args.Add(_selectedProperty);

                command.Execute(args);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
        #endregion

        GUILayout.EndArea();
    }

    private void RenderTeleportTab()
    {
        GUILayout.BeginArea(new Rect(15, 15, menuWidth - (menuSpacing * 4), menuHeight - 110 - (menuSpacing * 2)));

        // Tab Title
        GUILayout.Label("Teleport", _headerStyle);

        // Spacer
        GUILayout.Space(10);

        // ** Scrollable content area
        _npcTeleportScrollPosition = GUILayout.BeginScrollView(_npcTeleportScrollPosition, GUILayout.Width(menuWidth - (menuSpacing * 4)), GUILayout.Height(menuHeight - 110 - (menuSpacing * 2) - 60));

        // ** Property Teleport Section
        GUILayout.Label("Teleport to Properties:");
        GUILayout.Space(5);
        
        int propertyButtonCount = 0;
        GUILayout.BeginHorizontal();
        foreach (var property in propertys)
        {
            if (propertyButtonCount > 0 && propertyButtonCount % 4 == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
            
            if (GUILayout.Button(property, GUILayout.Width(140), GUILayout.Height(24)))
            {
                try
                {
                    GameObject propertyObject = GameObject.Find(property);
                    if (propertyObject != null)
                    {
                        Player.Local.transform.position = propertyObject.transform.position + new Vector3(0, 2, 0);
                        MelonLogger.Msg($"Teleported to property: {property}");
                    }
                    else
                    {
                        MelonLogger.Warning($"Property not found: {property}");
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Error teleporting to property: {ex.Message}");
                }
            }
            propertyButtonCount++;
        }
        GUILayout.EndHorizontal();

        // Spacer
        GUILayout.Space(20);

        // ** NPC Teleport Section
        GUILayout.Label("Teleport to NPCs:");
        
        // Spacer
        GUILayout.Space(5);

        // ** NPC Teleport buttons
        int columns = 3;
        int currentColumn = 0;
        GUILayout.BeginHorizontal();
        
        foreach (var npcEntry in _npcIdMap)
        {
            if (currentColumn == columns)
            {
                currentColumn = 0;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }

            string displayName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(npcEntry.Key.Replace(".", " "));
            int npcButtonWidth = ((menuWidth - (menuSpacing * 4) - 20) - (10 * (columns - 1))) / columns;
            
            if (GUILayout.Button(displayName, GUILayout.Width(npcButtonWidth), GUILayout.Height(30)))
            {
                bool npcFound = false;
                foreach (NPC npc in NPCManager.NPCRegistry)
                {
                    string npcFullName = npc.FirstName + "_" + npc.LastName;
                    if (npcFullName.Equals(npcEntry.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        Player.Local.transform.position = npc.transform.position + new Vector3(0, 0, 2);
                        MelonLogger.Msg($"Teleported to {displayName}");
                        npcFound = true;
                        break;
                    }
                }
                if (!npcFound)
                {
                    MelonLogger.Warning($"NPC not found: {displayName}");
                }
            }

            currentColumn++;
        }
        
        GUILayout.EndHorizontal();
        
        // End scrollview
        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void RenderCreditsTab()
    {
        GUILayout.BeginArea(new Rect(15, 15, menuWidth - (menuSpacing * 4), menuHeight - 110 - (menuSpacing * 2)));

        // **Tab Title**
        GUILayout.Label("Credits", _headerStyle);

        // **Spacer**
        GUILayout.Space(10);

        // ** Scrollable content
        _creditsTabScrollPosition = GUILayout.BeginScrollView(_creditsTabScrollPosition, GUILayout.Width(menuWidth - (menuSpacing * 4)), GUILayout.Height(menuHeight - 110 - (menuSpacing * 2) - 60));

        // **Creator**
        GUILayout.Label("This mod was created by nasty.codes");
        GUILayout.Label("Discord: nasty.codes");

        // **Spacer**
        GUILayout.Space(20);

        // **Thanks**
        GUILayout.Label("Thanks to");
        GUILayout.Label("GitHub Copilot");
        GUILayout.Label("MelonLoader");
        GUILayout.Label("Jumpman");

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    public Dictionary<string, Dictionary<string, List<Employee>>> GetAlLEmployees()
    {
        Dictionary<string, Dictionary<string, List<Employee>>> employees = new Dictionary<string, Dictionary<string, List<Employee>>>();

        foreach (var employee in EmployeeManager.Instance.AllEmployees.ToArray())
        {
            // MelonLogger.Msg("Employee: " + employee.FirstName + " " + employee.LastName + " " + employee.EmployeeType + " " + employee.AssignedProperty.propertyCode);

            if (employees.ContainsKey(employee.AssignedProperty.propertyCode))
            {
                if (employees[employee.AssignedProperty.propertyCode].ContainsKey(employee.EmployeeType.ToString()))
                {
                    employees[employee.AssignedProperty.propertyCode][employee.EmployeeType.ToString()].Add(employee);
                }
                else
                {
                    employees[employee.AssignedProperty.propertyCode].Add(employee.EmployeeType.ToString(), new List<Employee> { employee });
                }
            }
            else
            {
                employees.Add(employee.AssignedProperty.propertyCode, new Dictionary<string, List<Employee>> { { employee.EmployeeType.ToString(), new List<Employee> { employee } } });
            }
        }
        return employees;
    }

    public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        Color oldColour = GUI.color;

        var rad2deg = 360 / (Math.PI * 2);

        Vector2 d = end - start;

        float a = (float)rad2deg * Mathf.Atan(d.y / d.x);

        if (d.x < 0)
            a += 180;

        int width2 = (int)Mathf.Ceil(width / 2);

        GUIUtility.RotateAroundPivot(a, start);

        GUI.color = color;

        GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), Texture2D.whiteTexture, ScaleMode.StretchToFill);

        GUIUtility.RotateAroundPivot(-a, start);

        GUI.color = oldColour;
    }

    public static void DrawBox(Vector2 topLeft, float width, float height, Color color, float lineWidth)
    {
        // Define the four corners of the box
        Vector2 topRight = new Vector2(topLeft.x + width, topLeft.y);
        Vector2 bottomLeft = new Vector2(topLeft.x, topLeft.y + height);
        Vector2 bottomRight = new Vector2(topLeft.x + width, topLeft.y + height);

        // Draw the four sides of the box (top, right, bottom, left)
        DrawLine(topLeft, topRight, color, lineWidth);      // Top
        DrawLine(topRight, bottomRight, color, lineWidth);  // Right
        DrawLine(bottomRight, bottomLeft, color, lineWidth); // Bottom
        DrawLine(bottomLeft, topLeft, color, lineWidth);    // Left
    }

    [HarmonyPatch]
    public static class DebugPatch
    {
        [HarmonyPatch(typeof(Debug), "get_isDebugBuild")]
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }

    [HarmonyPatch]
    public class StackLimitPatch
    {
        [HarmonyPatch(typeof(ItemInstance), "StackLimit", MethodType.Getter)]
        [HarmonyPostfix]
        public static void Postfix(ref int __result)
        {
            if (NastyModClass.Instance != null)
            {
                __result = (int)NastyModClass.Instance._moddedStackSize;
            }
        }
    }
}
