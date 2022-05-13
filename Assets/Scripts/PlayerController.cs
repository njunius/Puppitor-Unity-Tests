using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Puppitor;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    string fileName;
    string jsonString;

    public Affecter affecterTest;
    public Dictionary<string, double> affectVector;

    public ActionKeyMap<KeyCode> test;

    // Start is called before the first frame update
    void Awake()
    {
        // character affect state setup
        fileName = @"Assets\Scripts\affect_rules\test_passions_rules.json";
        jsonString = File.ReadAllText(fileName);
        Debug.Log(jsonString);

        affecterTest = new Affecter(jsonString);

        Debug.Log("Affect Entries");
        foreach(KeyValuePair<string, AffectEntry> affect in affecterTest.affectRules)
        {
            Debug.Log(affect.Value);
        }

        affectVector = Affecter.MakeAffectVector(affecterTest.affectRules.Keys.ToList<string>(), affecterTest.affectRules);

        Debug.Log("Affect Vector");

        foreach(KeyValuePair<string, double> affect in affectVector)
        {
            Debug.Log(affect.Key + ": " + affect.Value);
        }

        // character gesture setup
        Dictionary<string, List<KeyCode>> modifierDict = new Dictionary<string, List<KeyCode>>(){
            {"tempo_up", new List<KeyCode>{KeyCode.C}},
            {"tempo_down", new List<KeyCode>{KeyCode.Z}}
        };

        Dictionary<string, List<KeyCode>> actionDict = new Dictionary<string, List<KeyCode>>(){
            {"projected_energy", new List<KeyCode>{KeyCode.B}},
            {"open_flow", new List<KeyCode>{KeyCode.N}},
            {"closed_flow", new List<KeyCode>{KeyCode.M}}
        };

        Dictionary<string, Dictionary<string, List<KeyCode>>> keyMap = new Dictionary<string, Dictionary<string, List<KeyCode>>>(){
            {"actions", actionDict},
            {"modifiers", modifierDict}

        };

        test = new ActionKeyMap<KeyCode>(keyMap);

        Debug.Log("Action Key Map");
        Debug.Log(test);
    }

    // Update is called once per frame
    void Update()
    {
        // action listeners
        if (Input.GetKeyDown(test.classKeyMap["actions"]["open_flow"][0]))
        {
            test.UpdatePossibleStates("open_flow", true);
        }
        if (Input.GetKeyDown(test.classKeyMap["actions"]["closed_flow"][0]))
        {
            test.UpdatePossibleStates("closed_flow", true);
        }
        if (Input.GetKeyDown(test.classKeyMap["actions"]["projected_energy"][0]))
        {
            test.UpdatePossibleStates("projected_energy", true);
        }

        if (Input.GetKeyUp(test.classKeyMap["actions"]["open_flow"][0]))
        {
            test.UpdatePossibleStates("open_flow", false);
        }
        if (Input.GetKeyUp(test.classKeyMap["actions"]["closed_flow"][0]))
        {
            test.UpdatePossibleStates("closed_flow", false);
        }
        if (Input.GetKeyUp(test.classKeyMap["actions"]["projected_energy"][0]))
        {
            test.UpdatePossibleStates("projected_energy", false);
        }

        // modifier listeners
        if (Input.GetKeyDown(test.classKeyMap["modifiers"]["tempo_up"][0]))
        {
            test.UpdatePossibleStates("tempo_up", true);
        }
        if (Input.GetKeyDown(test.classKeyMap["modifiers"]["tempo_down"][0]))
        {
            test.UpdatePossibleStates("tempo_down", true);
        }

        if (Input.GetKeyUp(test.classKeyMap["modifiers"]["tempo_up"][0]))
        {
            test.UpdatePossibleStates("tempo_up", false);
        }
        if (Input.GetKeyUp(test.classKeyMap["modifiers"]["tempo_down"][0]))
        {
            test.UpdatePossibleStates("tempo_down", false);
        }
    }

    void FixedUpdate()
    {
        // check each action and do the default otherwise
        if(test.possibleActionStates["open_flow"])
        {
            test.UpdateActualStates("open_flow", "actions", true);
        }
        else if(test.possibleActionStates["closed_flow"])
        {
            test.UpdateActualStates("closed_flow", "actions", true);
        }
        else if(test.possibleActionStates["projected_energy"])
        {
            test.UpdateActualStates("projected_energy", "actions", true);
        }
        else
        {
            test.UpdateActualStates("resting", "actions", true);
        }

        // check each modifier and do the default otherwise
        if (test.possibleActionStates["tempo_up"])
        {
            test.UpdateActualStates("tempo_up", "modifiers", true);
        }
        else if (test.possibleActionStates["tempo_down"])
        {
            test.UpdateActualStates("tempo_down", "modifiers", true);
        }
        else
        {
            test.UpdateActualStates("neutral", "modifiers", true);
        }

        affecterTest.UpdateAffect(affectVector, test.currentStates["actions"], test.currentStates["modifiers"]);

        //Debug.Log(test);

        foreach (KeyValuePair<string, double> affect in affectVector)
        {
            Debug.Log(affect.Key + ": " + affect.Value);
        }
    }
}
