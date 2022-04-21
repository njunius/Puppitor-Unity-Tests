using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Puppitor;

public static class GreedyAffectSearch
{
    public static Tuple<string, string> Think(ActionKeyMap<KeyCode> actionsToTry, Affecter characterAffecter, Dictionary<string, double> currAffectVector, string goalEmotion)
    {
        List<Tuple<double, string, string>> futureStatesForEval = new List<Tuple<double, string, string>>();
        Dictionary<string, double> copiedAffectVector;
        double goalEmotionValue = 0;
        Tuple<double, string, string> futureStateEntry;

        foreach(string action in actionsToTry.actualActionStates["actions"].Keys)
        {
            foreach (string modifier in actionsToTry.actualActionStates["modifiers"].Keys)
            {
                copiedAffectVector = new Dictionary<string, double>(currAffectVector);
                characterAffecter.UpdateAffect(copiedAffectVector, action, modifier);
                goalEmotionValue = copiedAffectVector[goalEmotion];
                futureStateEntry = new Tuple<double, string, string>(goalEmotionValue, action, modifier);
                futureStatesForEval.Add(futureStateEntry);
            }
        }

        futureStatesForEval.Sort((x, y) => x.Item1.CompareTo(y.Item1));

        string print = "";

        /*foreach (Tuple<double, string, string> entry in futureStatesForEval)
        {
            print += $"entry: {entry.Item1}, {entry.Item2}, {entry.Item3}\n";
        }
        Debug.Log(print);*/

        Tuple<double, string, string> bestActionModifier = futureStatesForEval[futureStatesForEval.Count - 1];
        Tuple<string, string> finalActionModifier = new Tuple<string, string>(bestActionModifier.Item2, bestActionModifier.Item3);
        return finalActionModifier;
    }
}
