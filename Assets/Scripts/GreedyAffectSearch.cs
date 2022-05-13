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

        // perform every possible action and modifier combination to find the one with the highest value of the goal emotion
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

        // sort states into ascending order by the goalEmotionValue
        futureStatesForEval.Sort((x, y) => x.Item1.CompareTo(y.Item1));

        // choose the state with the highest goalEmotionValue and return the action and modifier that was performed to get there
        Tuple<double, string, string> bestActionModifier = futureStatesForEval[futureStatesForEval.Count - 1];
        Tuple<string, string> finalActionModifier = new Tuple<string, string>(bestActionModifier.Item2, bestActionModifier.Item3);
        return finalActionModifier;
    }
}
