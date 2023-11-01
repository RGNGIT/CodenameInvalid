using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WispKeyBoardEventSystem : MonoBehaviour
{
    [SerializeField] private bool printKeyToConsole = false;
    
    // KeyIsEvent use onGUI() to detect key presses.
    // CheckForKey use Update() to detect key presses.
    public enum WispEventSystemCheckMode { KeyIsEvent, CheckForKey }
    
    public enum WispEventSystemResultType 
    { 
        Unique,
        NoWinner,
        WinnerHasNoAction,
        NullOwner, 
        FocusedVisualComponent, 
        RecursiveParentOfFocusedVisualComponent,
        CurrentSelectedGameObject, 
        ParentOfCurrentSelectedGameObject, 
        RecursiveParentOfCurrentSelectedGameObject,
        VisualComponentLowInHierarchy,
        DefaultToCurrentIterration,
        WinnerDoesNotRequireFocus,
        FocusedVisualComponentIsParent,
        Modal
    }

    public class WispKeyActionInfo
    {
        private int eventID;
        private KeyCode key;
        private UnityAction action;
        private WispVisualComponent owner;
        private bool requireFocus = true;

        public WispKeyActionInfo(KeyCode ParamKey, UnityAction ParamAction, WispVisualComponent ParamEventOwner, int ParamID)
        {
            key = ParamKey;
            action = ParamAction;
            owner = ParamEventOwner;
            eventID = ParamID;
        }

        public KeyCode Key { get => key; }
        public WispVisualComponent Owner { get => owner; }
        public int EventID { get => eventID; set => eventID = value; }
        public UnityAction Action { get => action; set => action = value; }
        public bool RequireFocus { get => requireFocus; set => requireFocus = value; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Key : " + key.ToString());
            sb.AppendLine("Owner : " + owner.ToString());
            sb.AppendLine("EventID : " + eventID.ToString());
            sb.AppendLine("Action : " + action.Method.Name);
            sb.AppendLine("FocusRequired : " + requireFocus.ToString());

            return sb.ToString();
        }
    }

    private static WispKeyBoardEventSystem singleton;
    private Dictionary<KeyCode, List<WispKeyActionInfo>> actions = new Dictionary<KeyCode, List<WispKeyActionInfo>>();
    private Dictionary<int, WispKeyActionInfo> indexedActions = new Dictionary<int, WispKeyActionInfo>();
    private List<KeyCode> keysDown = new List<KeyCode>();
    private List<WispKeyActionInfo> garbage = new List<WispKeyActionInfo>();
    private WispEventSystemCheckMode keyCheckMode = WispEventSystemCheckMode.KeyIsEvent;
    private int currentEventID = 0;
    private string debugInfoText = "";
    private WispKeyActionInfo lastExecutedAction = null;
    private WispEventSystemResultType lastResultType;
    private string lastResultDepth = "0";
    
    public WispEventSystemCheckMode KeyCheckMode { get => keyCheckMode; set => keyCheckMode = value; }
    public int CurrentEventID { get => currentEventID; }
    public string DebugInfoText { get => debugInfoText; }
    public WispKeyActionInfo LastExecutedAction { get => lastExecutedAction; }
    public WispEventSystemResultType LastresultType { get => lastResultType; }
    public string LastResultDepth { get => lastResultDepth; }

    //...
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != null && singleton != this)
        {
            // There can only be one...
            WispVisualComponent.LogError("More then one instance of WispEventSystem found, Destroying extra instance.");
            Destroy(this); // Sorry bro... nothing personal.
        }

    }

    // Better then update+iterate.
    void OnGUI()
    {
        if (keyCheckMode != WispEventSystemCheckMode.KeyIsEvent)
            return;

        Event e = Event.current;
        if (e.isKey)
        {
            if (printKeyToConsole)
                print(e.keyCode);
            
            if (e.rawType == EventType.KeyDown)
            {
                if (keysDown.Contains(e.keyCode))
                    return;
            }
            else if (e.rawType == EventType.KeyUp)
            {
                if (keysDown.Contains(e.keyCode))
                {
                    keysDown.Remove(e.keyCode);
                    return;
                }
            }

            if (actions.ContainsKey(e.keyCode))
            {
                keysDown.Add(e.keyCode); // Prevents same key events from being handled multiple times.

                WispKeyActionInfo winner = GetAppropriateActionToExecute(actions[e.keyCode]);

                if (winner != null)
                {
                    if (winner.Action != null)
                    {
                        winner.Action.Invoke();
                        lastExecutedAction = winner;
                    }
                    else
                    {
                        lastResultType = WispEventSystemResultType.WinnerHasNoAction;
                    }
                }
                else
                {
                    lastResultType = WispEventSystemResultType.NoWinner;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (keyCheckMode != WispEventSystemCheckMode.CheckForKey)
            return;

        if (!Input.anyKeyDown)
            return;

        foreach (KeyValuePair<KeyCode, List<WispKeyActionInfo>> kv in actions)
        {
            if (Input.GetKeyDown(kv.Key))
            {
                if(kv.Value.Count == 1)
                {
                    if (!kv.Value[0].Owner.IsAvailableForInput)
                        continue;

                    kv.Value[0].Action.Invoke();

                    lastResultType = WispEventSystemResultType.Unique;
                }
                else
                {
                    WispKeyActionInfo winner = GetAppropriateActionToExecute(kv.Value);

                    if (winner != null)
                    {
                        if (winner.Action != null)
                        {
                            winner.Action.Invoke();
                            lastExecutedAction = winner;
                        }
                        else
                        {
                            lastResultType = WispEventSystemResultType.WinnerHasNoAction;
                        }
                    }
                    else
                    {
                        lastResultType = WispEventSystemResultType.NoWinner;
                    }
                }

                break; // What if multiple keys ?
            }
        }
    }

    private WispKeyActionInfo GetAppropriateActionToExecute(List<WispKeyActionInfo> ParamActions)
    {
        WispKeyActionInfo result = null;
        lastResultDepth = "0";

        // Highest priority is for the focused visual component.
        foreach (WispKeyActionInfo info in ParamActions)
        {
            // Remove actions if owner is NULL.
            if (info.Owner == null)
            {
                garbage.Add(info);
                continue;
            }

            // Check if the owner is available for input.
            if (!info.Owner.IsAvailableForInput || !info.Owner.gameObject.activeInHierarchy)
                continue; // Skip if the owner is not available for input or the gameObject is inactive.

            // Check if the owner is NULL.
            if (info.Owner == null || info.Owner.gameObject == null)
            {
                continue; // Skip if NULL.
            }

            // Check if focused visual component is the owner.
            if (info.Owner == WispVisualComponent.FocusedComponent)
            {
                // result = info;
                lastResultDepth = "1.1";
                lastResultType = WispEventSystemResultType.FocusedVisualComponent;
                // break;
                return info;
            }
        }

        // Remove garbage
        foreach (WispKeyActionInfo info in garbage)
        {
            ParamActions.Remove(info);
        }

        garbage.Clear();

        WispKeyActionInfo best = null;
        int score = 0;

        // Use score system to decide...
        foreach (WispKeyActionInfo info in ParamActions)
        {
            int myScore = 1;
            
            // First let's check the focused visual component...
            if (WispVisualComponent.FocusedComponent != null)
            {
                // If the owner is a parent or ancestor of the focused visual component
                if (WispVisualComponent.FocusedComponent.IsMyParent_Recursive(info.Owner))
                {
                    result = info;
                    lastResultDepth = "1.2";
                    lastResultType = WispEventSystemResultType.RecursiveParentOfFocusedVisualComponent;
                    myScore *= 2;
                    // break;
                }
                // Then if the focused visual component is a parent or ancestor to the owner
                else if (info.Owner.IsMyParent_Recursive(WispVisualComponent.FocusedComponent))
                {
                    result = info;
                    lastResultDepth = "1.3";
                    lastResultType = WispEventSystemResultType.FocusedVisualComponentIsParent;
                    myScore *= 2;
                    // break;
                }
            }

            // Then modal popups
            if (info.Owner == WispWindow.CurrentlyOpen)
            {
                result = info;
                lastResultDepth = "2";
                lastResultType = WispEventSystemResultType.Modal;
                myScore *= 10;
                // break;
            }
            // Then children of modal popup
            else if (WispWindow.CurrentlyOpen != null)
            {
                // Is the popup my parent ?
                if (info.Owner.IsMyParent_Recursive(WispWindow.CurrentlyOpen))
                {
                    result = info;
                    lastResultDepth = "3.1";
                    lastResultType = WispEventSystemResultType.Modal;
                    myScore *= 5;
                    // break;
                }
                // Is the popup my child ?
                else if (WispWindow.CurrentlyOpen.IsMyParent_Recursive(info.Owner))
                {
                    result = info;
                    lastResultDepth = "3.2";
                    lastResultType = WispEventSystemResultType.Modal;
                    myScore *= 5;
                    // break;
                }
            }

            // If it does not require focus
            if (!info.RequireFocus)
            {
                result = info;
                lastResultDepth = "4";
                lastResultType = WispEventSystemResultType.WinnerDoesNotRequireFocus;
                myScore *= 1;
                // break;
            }

            // Check who is the best
            if (myScore > score)
                best = info;
        }

        result = best;
        return result;
    }

    // Bind an event to multiple keys.
    // Returns multiple eventIDs.
    public static List<int> AddEventOnKey(UnityAction ParamAction, WispVisualComponent ParamEventOwner, bool ParamRequireFocus = true, params KeyCode[] ParamKeys)
    {
        if (ParamKeys.Length == 0)
            return null;

        List<int> result = new List<int>();

        for (int i = 0; i < ParamKeys.Length; i++)
        {
            result.Add(AddEventOnKey(ParamKeys[i], ParamAction, ParamEventOwner, ParamRequireFocus));
        }

        return result;
    }

    // Returns eventID
    public static int AddEventOnKey(KeyCode ParamKey, UnityAction ParamAction, WispVisualComponent ParamEventOwner, bool ParamRequireFocus = true)
    {
        if (singleton == null)
        {
            singleton = new GameObject("WispEventSystem (Auto)").AddComponent<WispKeyBoardEventSystem>();
        }

        singleton.currentEventID++;
        WispKeyActionInfo action = new WispKeyActionInfo(ParamKey, ParamAction, ParamEventOwner, singleton.currentEventID);
        action.RequireFocus = ParamRequireFocus;

        singleton.indexedActions.Add(singleton.currentEventID, action);

        if (singleton.actions.ContainsKey(ParamKey))
        {
            singleton.actions[ParamKey].Add(action);
        }
        else
        {
            List<WispKeyActionInfo> list = new List<WispKeyActionInfo>();
            singleton.actions.Add(ParamKey, list);
            list.Add(action);
        }

        #if UNITY_EDITOR
        singleton.UpdateDebugInfoText();
        #endif

        return singleton.currentEventID;
    }

    private void UpdateDebugInfoText()
    {
        StringBuilder sb = new StringBuilder();

        foreach (KeyValuePair<KeyCode, List<WispKeyActionInfo>> kvp in actions)
        {
            sb.AppendLine(kvp.Key.ToString() + " : " + kvp.Value.Count);
        }

        debugInfoText = sb.ToString();
    }

    public static void SetAction(int ParamEventID, UnityAction ParamAction)
    {
        if (singleton.indexedActions.ContainsKey(ParamEventID))
            singleton.indexedActions[ParamEventID].Action = ParamAction;
    }

    public static List<int> ReplaceAction(UnityAction ParamActionOld, UnityAction ParamActionNew)
    {
        List<int> result = new List<int>();

        foreach (KeyValuePair<KeyCode, List<WispKeyActionInfo>> kv in singleton.actions)
        {
            foreach(WispKeyActionInfo info in kv.Value)
            {
                if (info.Action == ParamActionOld)
                {
                    info.Action = ParamActionNew;
                    result.Add(info.EventID);
                }
            }
        }

        return result;
    }
}