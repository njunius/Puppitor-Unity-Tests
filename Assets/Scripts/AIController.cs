using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Puppitor;

public class AIController : MonoBehaviour
{

    public string outgoingAction;
    public string outgoingModifier;

    public double curveLocDegrees;
    public double curveDelta;

    public string emotionalGoal;
    public string characterDefaultEmotion;
    public string prevailingExpressedAffect;
    public string prevailingInternalizedAffect;
    public string prevailingReceivedAffect;

    public Affecter otherAffecter;

    public GameObject otherCharacter;

    public Dictionary<string, double> otherAffectVector;

    public List<string> actionList;
    public List<string> modifierList;

    public string affecterOutgoingFilePath;
    public string affecterIncomingFilePath;
    string jsonString;

    public Affecter affecterOutGoing;
    public Dictionary<string, double> affectVectorOutgoing;

    public Affecter affecterIncoming;
    //public Dictionary<string, double> affectVectorIncoming;

    public ActionKeyMap<KeyCode> test;

    // Start is called before the first frame update
    void Start()
    {
        curveLocDegrees = 0;

        otherAffecter = otherCharacter.GetComponent<PlayerController>().affecterTest;
        otherAffectVector = otherCharacter.GetComponent<PlayerController>().affectVector;

        //emotionalGoal = ;

        // character affect state setup
        //fileName = @"Assets\Scripts\affect_rules\test_rules.json";
        jsonString = File.ReadAllText(affecterOutgoingFilePath);
        Debug.Log(jsonString);

        affecterOutGoing = new Affecter(jsonString);

        Debug.Log("Outgoing Affect Entries");
        foreach (KeyValuePair<string, AffectEntry> affect in affecterOutGoing.affectRules)
        {
            Debug.Log(affect.Value);
        }

        affectVectorOutgoing = Affecter.MakeAffectVector(affecterOutGoing.affectRules.Keys.ToList<string>(), affecterOutGoing.affectRules);

        Debug.Log("Outgoing Affect Vector");

        foreach (KeyValuePair<string, double> affect in affectVectorOutgoing)
        {
            Debug.Log(affect.Key + ": " + affect.Value);
        }

        jsonString = File.ReadAllText(affecterIncomingFilePath);
        Debug.Log(jsonString);
        affecterIncoming = new Affecter(jsonString);

        /*Debug.Log("Incoming Affect Entries");
        foreach (KeyValuePair<string, AffectEntry> affect in affecterIncoming.affectRules)
        {
            Debug.Log(affect.Value);
        }

        affectVectorIncoming = Affecter.MakeAffectVector(affecterIncoming.affectRules.Keys.ToList<string>(), affecterIncoming.affectRules);

        Debug.Log("Incoming Affect Vector");

        foreach (KeyValuePair<string, double> affect in affectVectorIncoming)
        {
            Debug.Log(affect.Key + ": " + affect.Value);
        }*/

        Dictionary<string, List<KeyCode>> actionDict = new Dictionary<string, List<KeyCode>>();

        foreach (string action in actionList)
        {
            actionDict.Add(action, new List<KeyCode>());
        }

        Dictionary<string, List<KeyCode>> modifierDict = new Dictionary<string, List<KeyCode>>();

        foreach (string modifier in modifierList)
        {
            modifierDict.Add(modifier, new List<KeyCode>());
        }

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
        
    }

    void FixedUpdate()
    {

        prevailingReceivedAffect = otherAffecter.GetPrevailingAffect(otherAffectVector);

        //if (affecterOutGoing.)
        //{

        //}
        
        curveLocDegrees += curveDelta;

        if (curveLocDegrees > 360)
        {
            curveLocDegrees = 0;
        }

        if (0.5 * Math.Sin(curveLocDegrees * Math.PI / 180) + 0.5 < 0.3)
        {
            emotionalGoal = characterDefaultEmotion;
        }
        else
        {
            emotionalGoal = otherAffecter.GetPrevailingAffect(otherAffectVector);
        }

        Tuple<string, string> actionAndModifier = GreedyAffectSearch.Think(test, affecterOutGoing, affectVectorOutgoing, emotionalGoal);
        outgoingAction = actionAndModifier.Item1;
        outgoingModifier = actionAndModifier.Item2;

        test.UpdateActualStates(outgoingAction, "actions", true);
        test.UpdateActualStates(outgoingModifier, "modifiers", true);

        affecterOutGoing.UpdateAffect(affectVectorOutgoing, test.currentStates["actions"], test.currentStates["modifiers"]);
        prevailingExpressedAffect = affecterOutGoing.GetPrevailingAffect(affectVectorOutgoing);


        //Debug.Log(test);

        //foreach (KeyValuePair<string, double> affect in affectVectorOutgoing)
        //{
        //    Debug.Log(affect.Key + ": " + affect.Value);
        //}
    }
}
