using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using KModkit;


public class malfunctions : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    public MeshRenderer meshNumberA;
    public MeshRenderer meshNumberB;
    public MeshRenderer functLetter;
    public MeshRenderer inputA;
    public MeshRenderer inputB;
    public MeshRenderer inputComma;
    public MeshRenderer inputResult;

    public KMSelectable[] buttons;
    public KMSelectable buttonQuery;
    public KMSelectable buttonComma;
    public KMSelectable buttonClear;
    public KMSelectable buttonSubmit;

    public KMRuleSeedable RuleSeedable;

    string[] alphabet = new string[26]
    { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
      "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};

    int[] theNumbers = new int[36] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
                        25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35 /* 36, 37, 38, 39, 40, 41,
        //RULESEED NUMBERS BELOW
        42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 */ };

    string[] theFunctions = new string[36]
    {
        "abs(a minus 3) times abs(b minus 3)","Larger modulo smaller","10,000 modulo Larger","7","(Larger divided by smaller) modulo 10","Triple the number of odd numbers",
        "10 minus (abs(digits in a minus digits in b))","(sum of digits in a) times (sum of digits in b)","a times b","a plus b, modulo 12","Highest digit",
        "((a modulo 10) cubed) plus ((b modulo 10) cubed)","Lunar Addition","abs(10,000 minus a) times abs(1,000 minus b)","(Larger modulo smaller) modulo 8",
        "Number of different digits","Smaller minus (Larger modulo Smaller)","Number of different odd digits","((a + b) times 10) plus (abs(a - b) modulo 10)",
        "a plus b","(a times b) divided by (ports + 2)","(a times b) modulo 10","Number of different digits missing","(Larger modulo smaller) times smaller",
        "a + (b squared)","11 minus twice the number of non-two-digit variables","abs(a minus b)","(a times b) modulo 73","Digits in a and b times 1,500","3 plus the number of numbers over 2,500",
        "(a squared) + b","Larger divided by (digits in both a and b)","a + b + concatenated serial number digits + 1","8 minus (Number of numbers below 100)","(a modulo 50) + b","Larger divided by smaller"
	 /*
        "(a times 10) + b", "Dashes found in digits when using morse code", "a concatenated with b, even-position digits removed", "Sum of digits in bomb serial number",
        "(a squared) * (b squared)", "| (a squared) minus ( (b + 1) squared) |", "(a squared) + b", "a + (b squared)", "(a times b) modulo 73", "(a modulo 50) + b",
        "((a modulo 4) + 2) to the power of ((b modulo 4) + 2)", "808 modulo (a + b)", "|a minus b| modulo 1000", "(a + b) times (a minus b)", "(a + b) times Larger", "(a + b) times Smaller",
        "(a + b) * b", "(a + b) * a"
	*/
    };

    string[,] theRules = new string[26, 2]
    {
        { "KBU or M in SN", "KBU and M not in SN" },        { "Battery, indicator, or port count = 2", "Battery, indicator, or port count not equal to 2" },
        { "First character in SN a digit", "First character in SN a letter" },        { "Lit BOB indicator", "No lit BOB indicator" },
        { "Unlit BOB indicator", "No unlit BOB indicator" },        { "First character in SN a letter", "First character in SN a digit" },
        { "Parallel port but no Serial port", "!(Parallel port but no Serial port)" },        { "At least one empty port plate", "No empty port plates" },
        { "No batteries", "At least one battery" },        { "Vowel in SN", "No vowel in SN" },        { "Indicators > 3", "Indicators <= 2" },
        { "Battery count even", "Battery count odd" },
        { "Ports > indicators", "Ports <= indicators" },        { "Lit indicators > Unlit indicators", "Lit indicators <= Unlit indicators" },
        { "Indicators > batteries", "Indicators <= batteries" },        { "Indicator count even", "Indicator count odd" },
        { "ERI or S in SN", "ERI and S not in SN" },        { "Exactly 3 letters in SN", "Not exactly 3 letters in SN" },
        { "Batteries > ports", "Batteries <= ports" },        { "Batteries > 4", "Batteries <= 4" },
        { "Lit and unlit indicator counts equal", "Lit and unlit indicator counts not equal" },        { "JQX or Z in SN", "JQX and Z not in SN" },
        { "At least three ports", "Two or fewer ports" },        { "No indicators", "At least one indicator" },
        { "At least 4 SN digits", "3 or fewer SN digits" },        { "No ports", "At least one port" }
    };

    int[] rsFunctionNum = new int[36];

    int[] rsRuleNum = new int[26];

    int[,] rsRuleOffset = new int[26, 2];

    int[,] ruleNumber = new int[26, 2]
    {
        { 6, -4 },        { 2, -3 },        { 5, -4 },        { 8, -8 },        { 6, -2 },        { 6, -5 },
        { 1, -5 },        { 1, -3 },        { 1, 5 },        { 5, -3 },        { 4, -1 },        { 6, 7 },
        { 3, -7 },        { 3, -5 },        { 6, -1 },        { 2, -3 },        { 1, -3 },        { 3, -2 },
        { 2, 4 },         { 4, 1 },        { 2, -2 },        { 7, 1 },        { 3, -5 },        { 3, -3 },
        { 4, -1 },        { 5, -1 }
    };
    bool pressedAllowed = false;

    // TWITCH PLAYS SUPPORT
    //int tpStages; This one is not needed for this module
    // TWITCH PLAYS SUPPORT

    int numberA = -1;
    int numberB = -1;
    int pickedLetter = -1;
    int pickedFunction = -1;
    int finalFunction = -1;
    int firstLastDigit = -1;

    int letterRuleOn = -1; // 0 is true, 1 is false, because this is based off of original Functions

    string currentInput = "";
    string ruleLetter = "";
    long currentInputAsNumber;
    long moduleSolution;

    int inputNumberA;
    int inputNumberB;
    bool commaIn = false;
    bool justQueried = false;
    bool isSolved = false;
    bool snHasDigit = false;
	
	int quirkNum;
    //0 is result +X, 1 is query number +X every query, 2 is both variables +X
    string quirkName;
	int xValue;
	int roundNum = 0;
	int prevNumA = -1;
	int prevNumB = -1;
    bool doQuirks = false;
    bool surpriseQuery = false;

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        Init();
        pressedAllowed = true;
    }

    void Init()
    {
        /*
         *     int[] rsFunctionNum = new int[42];

    int[] rsRuleNum = new int[26];

    int[,] rsRuleOffset = new int[26, 2];
    */
        for (int i = 0; i < 6; i++)
        {
            if (Bomb.GetSerialNumber().Substring(i, 1) == "1" || Bomb.GetSerialNumber().Substring(i, 1) == "2" ||
        Bomb.GetSerialNumber().Substring(i, 1) == "3" || Bomb.GetSerialNumber().Substring(i, 1) == "4" ||
        Bomb.GetSerialNumber().Substring(i, 1) == "5" || Bomb.GetSerialNumber().Substring(i, 1) == "6" ||
        Bomb.GetSerialNumber().Substring(i, 1) == "7" || Bomb.GetSerialNumber().Substring(i, 1) == "8" ||
        Bomb.GetSerialNumber().Substring(i, 1) == "9" || Bomb.GetSerialNumber().Substring(i, 1) == "0")
            {
                snHasDigit = true;
                i = 6;
            }
        }
        
        for (int i = 0; i < 36; i++)
            {
                rsFunctionNum[i] = i;
            }
        for (int i = 0; i < 26; i++)
        {
            rsRuleNum[i] = i;
            rsRuleOffset[i, 0] = ruleNumber[i, 0];
            rsRuleOffset[i, 1] = ruleNumber[i, 1];
        }
        var rnd = RuleSeedable.GetRNG();
        if (rnd.Seed != 1)
        {
            var letterRuleHit = 0;
            var letterRuleMiss = 0;
            rnd.ShuffleFisherYates(theNumbers);
            Debug.LogFormat("[Functions #{0}] Using rule seed: {1}.", _moduleId, rnd.Seed);
            
            for (int i = 0; i < 36; i++)
            {
                rsFunctionNum[i] = theNumbers[i];
            }
            for (int i = 0; i < 26; i++)
            {
                letterRuleHit = rnd.Next(1, 10);
                letterRuleMiss = rnd.Next(1, 10);
                if (letterRuleMiss == letterRuleHit || rnd.Next(0, 4) != 0)
                {
                    letterRuleMiss = letterRuleMiss * -1;
                }
                rsRuleOffset[i, 0] = letterRuleHit;
                rsRuleOffset[i, 1] = letterRuleMiss;
            }

        }

        delegationZone();
        Module.OnActivate += delegate { inputResult.GetComponentInChildren<TextMesh>().text = ""; };
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			xValue = -1;
		}
		else
		{
			xValue = 1;
		}
		quirkNum = UnityEngine.Random.Range(0, 3);
		//0 = answer +/- 1, 1 = query function +/- 1, 2 = a and b +/- 1
        if (quirkNum == 0)
        {
            if (xValue == -1)
            {
                quirkName = "Query result is one less than its actual value";
            }
            else
            {
                quirkName = "Query result is one more than its actual value";
            }
        }
        else if (quirkNum == 1)
        {
            if (xValue == -1)
            {
                quirkName = "After each query, the query function moves back on the list by one";
            }
            else
            {
                quirkName = "After each query, the query function moves forward on the list by one";
            }
        }
        else
        {
            if (xValue == -1)
            {
                quirkName = "Both variables decrease by one before query result calculation";
            }
            else
            {
                quirkName = "Both variables increase by one before query result calculation";
            }
        }
        pickedLetter = UnityEngine.Random.Range(0, 26);
        ruleLetter = alphabet[pickedLetter];
        pickedFunction = UnityEngine.Random.Range(0, 36);
        //pickedFunction = 17;
        if (!snHasDigit) //If the SN doesn't contain a digit, two Query Functions could potentially have the same answer regardless of what is queried, which is a problem.
        {                //This shouldn't be a problem as I don't believe any mods allow for digitless SN's but who knows in the future.
            while (rsFunctionNum[pickedFunction] == 9 || rsFunctionNum[pickedFunction] == 15)
            {
                pickedFunction = UnityEngine.Random.Range(0, 36);
            }
        }
        //pickedFunction = 26;
        if (UnityEngine.Random.Range(0, 10) < 7)
        {
            numberA = UnityEngine.Random.Range(1, 100);
        }
        else
        {
            numberA = UnityEngine.Random.Range(1, 1000);
        }

        numberB = numberA;
        while (numberB == numberA)
        {
            if (UnityEngine.Random.Range(0, 10) < 7)
            {
                numberB = UnityEngine.Random.Range(1, 100);
            }
            else
            {
                numberB = UnityEngine.Random.Range(1, 1000);
            }
        }
        meshNumberA.GetComponentInChildren<TextMesh>().text = numberA + "";
        meshNumberB.GetComponentInChildren<TextMesh>().text = numberB + "";
        functLetter.GetComponentInChildren<TextMesh>().text = alphabet[pickedLetter];
        doClear();
        letterRuleOn = UnityEngine.Random.Range(0, 2);
        doesRuleApply();
        finalFunction = pickedFunction + rsRuleOffset[(26 + xValue + pickedLetter) % 26, letterRuleOn];
        if (finalFunction > 35)
        {
            finalFunction = finalFunction - 36;
        }
        else if (finalFunction < 0)
        {
            finalFunction = finalFunction + 36;
        }
        Debug.LogFormat("[Functions #{0}] Display is {1} {2} {3}.", _moduleId, numberA, alphabet[pickedLetter], numberB);
        Debug.LogFormat("[Functions #{0}] Selected malfunction: {1}, (X is {2}).", _moduleId, quirkName, xValue);
        Debug.LogFormat("[Functions #{0}] Initial Query Function is #{1}: {2}.", _moduleId, pickedFunction, theFunctions[rsFunctionNum[pickedFunction]]);
        Debug.LogFormat("[Functions #{0}] {1}, meaning rule {2} is {3}, so adjust {4} by {5} (using the '{3}' offset for the row {6} {2}), so the Final Function is number {7}, solution below.", _moduleId,
        theRules[pickedLetter, letterRuleOn], alphabet[pickedLetter], letterRuleOn == 0 ? "true" : "false", pickedFunction, rsRuleOffset[(26 + xValue + pickedLetter) % 26, letterRuleOn],
        xValue == 1 ? "below" : "above", finalFunction);
        moduleSolution = -1;
        //Debug.Log("The number from 0-41 that was picked for the query function was " + pickedFunction);
        //Debug.Log("The number from 0-41 that was picked for the final function was " + finalFunction);
        functionZone(rsFunctionNum[finalFunction], numberA, numberB);
        moduleSolution = currentInputAsNumber;
        currentInputAsNumber = -1;
        currentInput = "";
        doQuirks = true;
        pressedAllowed = true;
        //dispText.text = numberA + " " + alphabet[pickedLetter] + " " + numberB;
    }

    void doNumber(int n)
    {
        currentInput = currentInput + "" + n;
        if (currentInput.Length > 12)
        {
            currentInput = currentInput.Substring(1, 12);
        }
        currentInputAsNumber = Int64.Parse(currentInput);
        inputResult.GetComponentInChildren<TextMesh>().text = currentInput;
    }

    void doClear()
    {
        inputNumberA = inputNumberB = 0;
        currentInput = "";
        currentInputAsNumber = 0;
        inputComma.GetComponentInChildren<TextMesh>().text = "";
        inputA.GetComponentInChildren<TextMesh>().text = "";
        inputB.GetComponentInChildren<TextMesh>().text = "";
        inputResult.GetComponentInChildren<TextMesh>().text = "";
        commaIn = false;
        justQueried = false;
    }

    void doComma()
    {
        if (!commaIn)
        {
            if (currentInput.Length > 4)
            {
                inputNumberA = Int32.Parse(currentInput.Substring(currentInput.Length - 4, 4));
            }
            else
            {
                inputNumberA = Int32.Parse(currentInput);
            }
            commaIn = true;
            inputComma.GetComponentInChildren<TextMesh>().text = ",";
            currentInput = "";
            currentInputAsNumber = 0;
            inputA.GetComponentInChildren<TextMesh>().text = "" + inputNumberA;
        }

    }

    void doQuery()
    {

        if (commaIn && !justQueried)
        {
            var giveStrike = false;
            if (currentInput.Length > 4)
            {
                inputNumberB = Int32.Parse(currentInput.Substring(currentInput.Length - 4, 4));
            }
            else
            {
                inputNumberB = Int32.Parse(currentInput);
            }
            Debug.LogFormat("[Functions #{0}] {1} and {2}.", _moduleId, inputNumberA, inputNumberB);
            if (inputNumberA == 0 || inputNumberB == 0)
            {
                Debug.LogFormat("[Functions #{0}] You queried a zero, that's a strike! Query not made.", _moduleId);
                giveStrike = true;
            }
            else if (inputNumberA == inputNumberB)
            {
                Debug.LogFormat("[Functions #{0}] You queried two of the same number, that's a strike! Query not made.", _moduleId);
                giveStrike = true;
            }
            else if ((inputNumberA == prevNumA || inputNumberA == prevNumB || inputNumberB == prevNumA || inputNumberB == prevNumB) && !surpriseQuery)
            {
                Debug.LogFormat("[Functions #{0}] You queried a number that was queried during the last query, that's a strike! Query not made.", _moduleId);
                giveStrike = true;
            }
            if (giveStrike)
            {
                giveStrike = false;
                GetComponent<KMBombModule>().HandleStrike();
            }
            else
            {
                prevNumB = inputNumberB;
                prevNumA = inputNumberA;
                if (quirkNum == 0) //result +/- 1
                {
                    inputB.GetComponentInChildren<TextMesh>().text = "" + inputNumberB;
                    currentInputAsNumber = inputNumberA + inputNumberB;
                    functionZone(rsFunctionNum[pickedFunction], inputNumberA, inputNumberB);
                    inputResult.GetComponentInChildren<TextMesh>().text = "" + currentInputAsNumber;
                }
                else if (quirkNum == 1) //query function +/- 1
                {
                    inputB.GetComponentInChildren<TextMesh>().text = "" + inputNumberB;
                    currentInputAsNumber = inputNumberA + inputNumberB;
                    functionZone(rsFunctionNum[(36 + (xValue * roundNum) + pickedFunction) % 36], inputNumberA, inputNumberB);
                    inputResult.GetComponentInChildren<TextMesh>().text = "" + currentInputAsNumber;
                    Debug.LogFormat("[Functions #{0}] This module's malfunction {1} one to the position on the list of functions to use, so the next function will be #{2}, {3}.", _moduleId,
                        xValue == 1 ? "adds" : "subtracts", rsFunctionNum[(36 + (xValue * (1 + roundNum)) + pickedFunction) % 36], theFunctions[rsFunctionNum[(36 + (xValue * (1 + roundNum)) + pickedFunction) % 36]]);
                }
                else
                {
                    inputB.GetComponentInChildren<TextMesh>().text = "" + inputNumberB;
                    currentInputAsNumber = inputNumberA + inputNumberB;
                    var varChangeString = "This module's malfunction ";
                    if (xValue == 1)
                    {
                        varChangeString = varChangeString + "adds one to each variable before performing the query calculation. You input " + inputNumberA + " for a and " + inputNumberB + " for b.";
                    }
                    else
                    {
                        varChangeString = varChangeString + "subtracts one from each variable before performing the query calculation. You input " + inputNumberA + " for a and " + inputNumberB + " for b.";
                    }

                    if (inputNumberA == 1 && xValue == -1)
                    {
                        
                        varChangeString = varChangeString + " a is already 1, so it will stay at 1.";
                    }
                    else if (inputNumberA == 9999 && xValue == 1)
                    {
                        
                        varChangeString = varChangeString + " a is already 9999, so it will stay at 9999.";
                    }
                    else
                    {
                        inputNumberA = inputNumberA + xValue;
                        varChangeString = varChangeString + " a will become " + (inputNumberA) + ".";
                    }

                    if (inputNumberB == 1 && xValue == -1)
                    {
                        
                        varChangeString = varChangeString + " b is already 1, so it will stay at 1.";
                    }
                    else if (inputNumberB == 9999 && xValue == 1)
                    {
                        
                        varChangeString = varChangeString + " b is already 9999, so it will stay at 9999.";

                    }
                    else
                    {
                        inputNumberB = inputNumberB + xValue;
                        varChangeString = varChangeString + " b will become " + (inputNumberB) + ".";
                    }

                        Debug.LogFormat("[Functions #{0}] {1}", _moduleId, varChangeString);
                        functionZone(rsFunctionNum[pickedFunction], inputNumberA, inputNumberB);
                        inputResult.GetComponentInChildren<TextMesh>().text = "" + currentInputAsNumber;
                    /*
                    Debug.LogFormat("[Functions #{0}] This module's malfunction {1} one {2} each variable before performing the query calculation.", _moduleId,
                                            xValue == 1 ? "adds" : "subtracts");
                    functionZone(rsFunctionNum[pickedFunction], inputNumberA, inputNumberB);
                    inputResult.GetComponentInChildren<TextMesh>().text = "" + currentInputAsNumber; */
                }

                if (firstLastDigit == -1)
                {
                    firstLastDigit = (int)currentInputAsNumber % 10;
                }
				roundNum++;
                roundNum = roundNum % 36;
                currentInputAsNumber = 0;
                currentInput = "";
                justQueried = true;
                surpriseQuery = false;
            }
        }
    }

    void doSubmit()
    {
        if (pressedAllowed)
        {
			if (currentInput == null || currentInput == "")
			{
				//nothing
			}
            else if (Int64.Parse(currentInput) == moduleSolution)
            {
                Debug.LogFormat("[Functions #{0}] Submitted input of {1} and the expected {2} match, module disarmed!", _moduleId, Int64.Parse(currentInput), moduleSolution);
                var winMessage = new string[19] { "BOOYAH!", "--DISARMED--", "YES! YES!", "NAILED IT!", "WOO!", "CHA-CHING!", "GOT IT!", "GENIUS!", "WELL DONE!", "YOU DID IT!",
					"  WAHOO!  ", "  SCORE    ", "COOL BEANS!", "SOLVE PHRASE", "GOOD ONE", "LED = GREEN", "SOLVES++;", "^^vv< >< >BA", "MATH IS FUN"};
                isSolved = true;
                inputResult.GetComponentInChildren<TextMesh>().text = winMessage[UnityEngine.Random.Range(0, 19)];
				if (UnityEngine.Random.Range(0, 400) == 0)
				{
					inputResult.GetComponentInChildren<TextMesh>().text = "OH NO I'M OFF THE DISPLAY HELP                ";
				}
                if (Bomb.GetSolvableModuleNames().Where(x => "Souvenir".Contains(x)).Count() > 0)
                {
					if (UnityEngine.Random.Range(0, 25) == 0)
					{
						inputResult.GetComponentInChildren<TextMesh>().text = "DON'T FORGET";
					}
                    meshNumberA.GetComponentInChildren<TextMesh>().text = "???";
                    meshNumberB.GetComponentInChildren<TextMesh>().text = "???";
                    functLetter.GetComponentInChildren<TextMesh>().text = "!";
                }
                pressedAllowed = false;
                GetComponent<KMBombModule>().HandlePass();
            }
            else
            {
                Debug.LogFormat("[Functions #{0}] Submitted input of {1} and the expected {2} do not match, that's a strike!", _moduleId, Int64.Parse(currentInput), moduleSolution);
                GetComponent<KMBombModule>().HandleStrike();
            }
        }

    }

    void OnPress()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
    }

    void OnRelease()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);
        if (pressedAllowed)
        {

            return;
        }

    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Perform a query (which will first clear the top and middle screens) with !{0} (query/q) 9876, 5432. You can omit the comma. Submit an answer with !{0} (submit/s/answer/a) 1234567890. Use !{0} surprise/random for a random query (this will never cause a duplication strike)!";
    private readonly bool TwitchShouldCancelCommand = false;
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        var twitchString = command;
        for (int cN = 0; cN < command.Length - 1; cN++)
        {
            if (command.Substring(cN, 1) == ",")
            {
                twitchString = command.Substring(0, cN) + " " + command.Substring(cN + 1, command.Length - (cN + 1));
                cN = command.Length;
            }
        }
        var pieces = twitchString.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string theError;
        theError = "";
        yield return null;
        if (pieces.Count() == 0)
        {
            theError = "sendtochaterror Not enough arguments! You need at least 'query/q' with two numbers separated with a comma, or 'submit/s/answer/a', with one number.";
            yield return theError;
        }
        else if (pieces.Count() == 1 && (pieces[0] == "submit" || pieces[0] == "answer" || pieces[0] == "s" || pieces[0] == "a"))
        {
            theError = "sendtochaterror Not enough arguments! You need a number to submit, e.g. !{0} submit 1234567890.";
            yield return theError;

        }
        else if (pieces.Count() == 1 && (pieces[0] == "surpriseme" || pieces[0] == "mystery" || pieces[0] == "surprise" || pieces[0] == "doyourworst" || pieces[0] == "random"))
        {
            int surpriseA;
            int surpriseB;
            surpriseQuery = true;
            if (UnityEngine.Random.Range(0, 10) < 7)
            {
                surpriseA = UnityEngine.Random.Range(1, 100);
            }
            else
            {
                surpriseA = UnityEngine.Random.Range(1, 1000);
            }

            surpriseB = surpriseA;
            while (surpriseB == surpriseA)
            {
                if (UnityEngine.Random.Range(0, 10) < 7)
                {
                    surpriseB = UnityEngine.Random.Range(1, 100);
                }
                else
                {
                    surpriseB = UnityEngine.Random.Range(1, 1000);
                }
            }
            yield return new WaitForSeconds(.1f);
            yield return null;
            buttonClear.OnInteract();
            for (int l = 0; l < (""+ surpriseA).Length; l++)
            {
                var curDigit = Int16.Parse((""+ surpriseA).Substring(l, 1));
                yield return new WaitForSeconds(.1f);
                yield return null;
                buttons[curDigit].OnInteract();
            }
            yield return new WaitForSeconds(.1f);
            yield return null;
            buttonComma.OnInteract();
            for (int l = 0; l < (""+surpriseB).Length; l++)
            {
                var curDigit = Int16.Parse((""+surpriseB).Substring(l, 1));
                yield return new WaitForSeconds(.1f);
                yield return null;
                buttons[curDigit].OnInteract();
            }
            yield return new WaitForSeconds(.1f);
            yield return null;
            buttonQuery.OnInteract();

        }
        else if ((pieces.Count() == 1 || pieces.Count() == 2) && (pieces[0] == "query" || pieces[0] == "q"))
        {
            theError = "sendtochaterror Not enough arguments! You need two numbers to query, e.g. !{0} query 1234, 5678 or !{0} q 9876, 5432.";
            yield return theError;

        } 
        else if (pieces[0] != "submit" && pieces[0] != "s" && pieces[0] != "answer" && pieces[0] != "a" && pieces[0] != "query" && pieces[0] != "q")
        {
            theError = "sendtochaterror Invalid argument! You need at least 'query/q' with two numbers separated with a comma, or 'submit/s/answer/a', with one number.";
            yield return theError;
        }
        else if (pieces.Count() >= 2 && (pieces[0] == "submit" || pieces[0] == "s" || pieces[0] == "answer" || pieces[0] == "a"))
        {
                for (int k = 0; k < pieces[1].Length; k++)
                {
                    if (pieces[1].Substring(k, 1) != "0" && pieces[1].Substring(k, 1) != "1" && pieces[1].Substring(k, 1) != "2" && pieces[1].Substring(k, 1) != "3" &&
                        pieces[1].Substring(k, 1) != "4" && pieces[1].Substring(k, 1) != "5" && pieces[1].Substring(k, 1) != "6" && pieces[1].Substring(k, 1) != "7" &&
                        pieces[1].Substring(k, 1) != "8" && pieces[1].Substring(k, 1) != "9")
                    {
                        if (pieces[1].Substring(k, 1) == "-")
                        {
                            theError = "sendtochaterror Invalid character! Minus signs and negatives have no place here.";
                        }
                        else
                        {
                            theError = "sendtochaterror Invalid character! " + pieces[1].Substring(k, 1) + " is not a digit.";
                        }
                        yield return theError;
                    }
                }
            if (theError == "")
            {
                yield return new WaitForSeconds(.1f);
                yield return null;
                buttonClear.OnInteract();
                for (int l = 0; l < pieces[1].Length; l++)
                {
                    var curDigit = Int16.Parse(pieces[1].Substring(l, 1));
                    yield return new WaitForSeconds(.1f);
                    yield return null;
                    buttons[curDigit].OnInteract();
                }
                yield return new WaitForSeconds(.1f);
                yield return null;
                buttonSubmit.OnInteract();
            }
        }
        else if (pieces.Count() >= 3 && (pieces[0] == "query" || pieces[0] == "q"))
        {
            if (pieces[1].Substring(pieces[1].Length - 1, 1) == ",")
            {
                pieces[1] = pieces[1].Substring(0, pieces[1].Length - 1);
            }
			var mistakeThing = false;
            for (int j = 1; j < 3; j++)
            {
                for (int k = 0; k < pieces[j].Length; k++)
                {
					//Debug.Log("k is " + k + " and j is " + j);
					//Debug.Log("a is " + pieces[1] + " and b is " + pieces[2]);
                    if (pieces[j].Substring(k, 1) != "0" && pieces[j].Substring(k, 1) != "1" && pieces[j].Substring(k, 1) != "2" && pieces[j].Substring(k, 1) != "3" &&
                        pieces[j].Substring(k, 1) != "4" && pieces[j].Substring(k, 1) != "5" && pieces[j].Substring(k, 1) != "6" && pieces[j].Substring(k, 1) != "7" &&
                        pieces[j].Substring(k, 1) != "8" && pieces[j].Substring(k, 1) != "9" )
                    {
                        if (pieces[j].Substring(k, 1) == "-")
                        {
                            theError = "sendtochaterror Invalid character! Minus signs and negatives have no place here.";
                        }
                        else
                        {
                            theError = "sendtochaterror Invalid character! " + pieces[j].Substring(k,1) + " is not a digit.";
                           
                        }
                        k = pieces[j].Length;
                        yield return theError;
						mistakeThing = true;
                    }
                }
				if (mistakeThing)
				{
                        j = 3;
				}
            }
            if (theError == "")
            {

                yield return new WaitForSeconds(.1f);
                yield return null;
                buttonClear.OnInteract();
                for (int l = 0; l < pieces[1].Length; l++)
                {
                    var curDigit = Int16.Parse(pieces[1].Substring(l, 1));
                    yield return new WaitForSeconds(.1f);
                    yield return null;
                    buttons[curDigit].OnInteract();
                }
                yield return new WaitForSeconds(.1f);
                yield return null;
                buttonComma.OnInteract();
                for (int l = 0; l < pieces[2].Length; l++)
                {
                    var curDigit = Int16.Parse(pieces[2].Substring(l, 1));
                    yield return new WaitForSeconds(.1f);
                    yield return null;
                    buttons[curDigit].OnInteract();
                }
                yield return new WaitForSeconds(.1f);
                yield return null;
                buttonQuery.OnInteract();
            }
        }
     }

    void delegationZone()
    {

        buttons[0].OnInteract += delegate () { OnPress(); doNumber(0); buttons[0].AddInteractionPunch(0.2f); return false; };
        buttons[1].OnInteract += delegate () { OnPress(); doNumber(1); buttons[1].AddInteractionPunch(0.2f); return false; };
        buttons[2].OnInteract += delegate () { OnPress(); doNumber(2); buttons[2].AddInteractionPunch(0.2f); return false; };
        buttons[3].OnInteract += delegate () { OnPress(); doNumber(3); buttons[3].AddInteractionPunch(0.2f); return false; };
        buttons[4].OnInteract += delegate () { OnPress(); doNumber(4); buttons[4].AddInteractionPunch(0.2f); return false; };
        buttons[5].OnInteract += delegate () { OnPress(); doNumber(5); buttons[5].AddInteractionPunch(0.2f); return false; };
        buttons[6].OnInteract += delegate () { OnPress(); doNumber(6); buttons[6].AddInteractionPunch(0.2f); return false; };
        buttons[7].OnInteract += delegate () { OnPress(); doNumber(7); buttons[7].AddInteractionPunch(0.2f); return false; };
        buttons[8].OnInteract += delegate () { OnPress(); doNumber(8); buttons[8].AddInteractionPunch(0.2f); return false; };
        buttons[9].OnInteract += delegate () { OnPress(); doNumber(9); buttons[9].AddInteractionPunch(0.2f); return false; };

        buttonClear.OnInteract += delegate () {
            OnPress(); doClear();
            buttonClear.AddInteractionPunch(0.2f); return false;
        };
        buttonComma.OnInteract += delegate () { OnPress(); doComma(); buttonClear.AddInteractionPunch(0.2f); return false; };
        buttonSubmit.OnInteract += delegate () { OnPress(); doSubmit(); buttonClear.AddInteractionPunch(0.4f); return false; };
        buttonQuery.OnInteract += delegate () { OnPress(); doQuery(); buttonClear.AddInteractionPunch(0.2f); return false; };

        buttons[0].OnInteractEnded += delegate () { OnRelease(); };
        buttons[1].OnInteractEnded += delegate () { OnRelease(); };
        buttons[2].OnInteractEnded += delegate () { OnRelease(); };
        buttons[3].OnInteractEnded += delegate () { OnRelease(); };
        buttons[4].OnInteractEnded += delegate () { OnRelease(); };
        buttons[5].OnInteractEnded += delegate () { OnRelease(); };
        buttons[6].OnInteractEnded += delegate () { OnRelease(); };
        buttons[7].OnInteractEnded += delegate () { OnRelease(); };
        buttons[8].OnInteractEnded += delegate () { OnRelease(); };
        buttons[9].OnInteractEnded += delegate () { OnRelease(); };

        buttonClear.OnInteractEnded += delegate () { OnRelease(); };
        buttonComma.OnInteractEnded += delegate () { OnRelease(); };
        buttonSubmit.OnInteractEnded += delegate () { OnRelease(); };
        buttonQuery.OnInteractEnded += delegate () { OnRelease(); };
        

    }
    /*
    public static class Extensions
    {
        // Fisher-Yates Shuffle
        public static IList<T> shuffle<T>(this IList<T> list, MonoRandom rnd)
        {
            var i = list.Count;
            while (i > 1)
            {
                var index = rnd.Next(i);
                i--;
                var value = list[index];
                list[index] = list[i];
                list[i] = value;
            }

            return list;
        }
    } */
    /*
    void doShuffle()
    {
        var rnd = RuleSeedable.GetRNG();
        if (rnd.Seed == 1)
        {

        }
        else
        {
            
            var numberCount = theNumbers.Length;
            while (numberCount > 1)
            {
                var xyz = rnd.Next(numberCount);
                numberCount--;
                var value = theNumbers[xyz];
                theNumbers[xyz] = theNumbers[numberCount];
                theNumbers[numberCount] = value;
            }
            var theThingy = "";

            for (var i = 0; i < 42; i++)
            {
                //list[i].innerText = theFunctions[theNumbers[i]];
            }
        }
        
    } */

    void doesRuleApply()
    {
        letterRuleOn = 1;
        switch (pickedLetter)
        {
            case 0:
                for (int i = 0; i < 6; i++)
                {
                    if (Bomb.GetSerialNumber().Substring(i, 1) == "K" || Bomb.GetSerialNumber().Substring(i, 1) == "B" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "U" || Bomb.GetSerialNumber().Substring(i, 1) == "M")
                    {
                        letterRuleOn = 0;
                    }
                }
                break;
            case 1:
                if (Bomb.GetBatteryCount() == 2 || Bomb.GetIndicators().Count() == 2 || Bomb.GetPortCount() == 2)
                {
                    letterRuleOn = 0;
                }
                break;
            case 2:
                if (Bomb.GetSerialNumber().Substring(0, 1) == "1" || Bomb.GetSerialNumber().Substring(0, 1) == "2" ||
                    Bomb.GetSerialNumber().Substring(0, 1) == "3" || Bomb.GetSerialNumber().Substring(0, 1) == "4" ||
                    Bomb.GetSerialNumber().Substring(0, 1) == "5" || Bomb.GetSerialNumber().Substring(0, 1) == "6" ||
                    Bomb.GetSerialNumber().Substring(0, 1) == "7" || Bomb.GetSerialNumber().Substring(0, 1) == "8" ||
                    Bomb.GetSerialNumber().Substring(0, 1) == "9" || Bomb.GetSerialNumber().Substring(0, 1) == "0")
                {
                    letterRuleOn = 0;
                }
                break;
            case 3:
                if (Bomb.IsIndicatorOn("BOB"))
                {
                    letterRuleOn = 0;
                }
                break;
            case 4:
                if (Bomb.IsIndicatorOff("BOB"))
                {
                    letterRuleOn = 0;
                }
                break;
            case 5:
                if (!(Bomb.GetSerialNumber().Substring(0, 1) == "1" || Bomb.GetSerialNumber().Substring(0, 1) == "2" ||
                    Bomb.GetSerialNumber().Substring(0, 1) == "3" || Bomb.GetSerialNumber().Substring(0, 1) == "4" ||
                    Bomb.GetSerialNumber().Substring(0, 1) == "5" || Bomb.GetSerialNumber().Substring(0, 1) == "6" ||
                    Bomb.GetSerialNumber().Substring(0, 1) == "7" || Bomb.GetSerialNumber().Substring(0, 1) == "8" ||
                    Bomb.GetSerialNumber().Substring(0, 1) == "9" || Bomb.GetSerialNumber().Substring(0, 1) == "0"))
                {
                    letterRuleOn = 0;
                }
                break;
            case 6:
                if (Bomb.IsPortPresent("Parallel") && !Bomb.IsPortPresent("Serial"))
                {
                    letterRuleOn = 0;
                }
                break;
            case 7:
                foreach (object[] plate in Bomb.GetPortPlates())
                {
                    if (plate.Length == 0)
                    {
                        letterRuleOn = 0;

                    }
                }
                break;
            case 8:
                if (Bomb.GetBatteryCount() == 0)
                {
                    letterRuleOn = 0;
                }
                break;
            case 9:
                for (int i = 0; i < 6; i++)
                {
                    if (Bomb.GetSerialNumber().Substring(i, 1) == "A" || Bomb.GetSerialNumber().Substring(i, 1) == "E" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "I" || Bomb.GetSerialNumber().Substring(i, 1) == "O" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "U")
                    {
                        letterRuleOn = 0;
                    }
                }
                break;
            case 10:
                if (Bomb.GetIndicators().Count() > 3)
                {
                    letterRuleOn = 0;
                }
                break;
            case 11:
                if (Bomb.GetBatteryCount() % 2 == 0)
                {
                    letterRuleOn = 0;
                }
                break;
            case 12:
                if (Bomb.GetPortCount() > Bomb.GetIndicators().Count())
                {
                    letterRuleOn = 0;
                }
                break;
            case 13:
                if (Bomb.GetOnIndicators().Count() > Bomb.GetOffIndicators().Count())
                {
                    letterRuleOn = 0;
                }
                break;
            case 14:
                if (Bomb.GetIndicators().Count() > Bomb.GetBatteryCount())
                {
                    letterRuleOn = 0;
                }
                break;
            case 15:
                if (Bomb.GetIndicators().Count() % 2 == 0)
                {
                    letterRuleOn = 0;
                }
                break;
            case 16:
                for (int i = 0; i < 6; i++)
                {
                    if (Bomb.GetSerialNumber().Substring(i, 1) == "E" || Bomb.GetSerialNumber().Substring(i, 1) == "R" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "I" || Bomb.GetSerialNumber().Substring(i, 1) == "S")
                    {
                        letterRuleOn = 0;
                    }
                }
                break;
            case 17:
                var numLetters = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (!(Bomb.GetSerialNumber().Substring(i, 1) == "1" || Bomb.GetSerialNumber().Substring(i, 1) == "2" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "3" || Bomb.GetSerialNumber().Substring(i, 1) == "4" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "5" || Bomb.GetSerialNumber().Substring(i, 1) == "6" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "7" || Bomb.GetSerialNumber().Substring(i, 1) == "8" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "9" || Bomb.GetSerialNumber().Substring(i, 1) == "0"))
                    {
                        numLetters++;
                    }
                }
                if (numLetters == 3)
                {
                    letterRuleOn = 0;
                }
                break;
            case 18:
                if (Bomb.GetBatteryCount() > Bomb.GetPortCount())
                {
                    letterRuleOn = 0;
                }
                break;
            case 19:
                if (Bomb.GetBatteryCount() > 4)
                {
                    letterRuleOn = 0;
                }
                break;
            case 20:
                if (Bomb.GetOnIndicators().Count() == Bomb.GetOffIndicators().Count())
                {
                    letterRuleOn = 0;
                }
                break;
            case 21:
                for (int i = 0; i < 6; i++)
                {
                    if (Bomb.GetSerialNumber().Substring(i, 1) == "J" || Bomb.GetSerialNumber().Substring(i, 1) == "Q" ||
                        Bomb.GetSerialNumber().Substring(i, 1) == "X" || Bomb.GetSerialNumber().Substring(i, 1) == "Z")
                    {
                        letterRuleOn = 0;
                    }
                }
                break;
            case 22:
                if (Bomb.GetPortCount() > 2)
                {
                    letterRuleOn = 0;
                }
                break;
            case 23:
                if (Bomb.GetIndicators().Count() == 0)
                {
                    letterRuleOn = 0;
                }
                break;
            case 24:
                var numNumbers = 0;
                for (int i = 0; i < 6; i++)
                {
                        if (Bomb.GetSerialNumber().Substring(i, 1) == "1" || Bomb.GetSerialNumber().Substring(i, 1) == "2" ||
                    Bomb.GetSerialNumber().Substring(i, 1) == "3" || Bomb.GetSerialNumber().Substring(i, 1) == "4" ||
                    Bomb.GetSerialNumber().Substring(i, 1) == "5" || Bomb.GetSerialNumber().Substring(i, 1) == "6" ||
                    Bomb.GetSerialNumber().Substring(i, 1) == "7" || Bomb.GetSerialNumber().Substring(i, 1) == "8" ||
                    Bomb.GetSerialNumber().Substring(i, 1) == "9" || Bomb.GetSerialNumber().Substring(i, 1) == "0")
                    {
                        numNumbers++;
                    }
                }
                if (numNumbers > 3)
                {
                    letterRuleOn = 0;
                }
                break;
            case 25:
                if (Bomb.GetPortCount() == 0)
                {
                    letterRuleOn = 0;
                }
                break;
            default:
                break;
        }
    }


    void functionZone(int fNum, int inputY, int inputZ)
    {
        var toPrepend = "";
        if (moduleSolution == -1)
        {
            toPrepend = "Final ";
        }
        else
        {
            toPrepend = "Query ";
        }
        var wackyString = "";
        //fNum = 20;
        switch (fNum)
        {
            case 0: // abs(a minus 3) times abs(b minus 3)
                
                currentInputAsNumber = Math.Abs(inputY - 3) * Math.Abs(inputZ - 3);
                currentInput = toPrepend + "Function: abs(a minus 3) times abs(b minus 3). abs(" + (inputY-3) + ") * abs(" + (inputZ - 3) + ") is " + currentInputAsNumber + ".";

                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3}", _moduleId, inputY, inputZ, currentInput);
                break;
            case 1: // Larger modulo smaller
                int largerOneD;
                int smallerOneD;
                currentInput = "Function: Larger modulo smaller.";
                if (inputY > inputZ)
                {
                    largerOneD = inputY;
                    smallerOneD = inputZ;
                }
                else
                {
                    largerOneD = inputZ;
                    smallerOneD = inputY;
                }
                currentInputAsNumber = largerOneD % smallerOneD;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 2: //10,000 modulo Larger
                currentInput = toPrepend + "Function: 10,000 modulo Larger.";
                if (inputY > inputZ)
                {
                    currentInputAsNumber = 10000 % inputY;

                    Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} 10,000 modulo {4} is {5}.", _moduleId, inputY, inputZ, currentInput, inputY, currentInputAsNumber);
                }
                else
                {
                    currentInputAsNumber = 10000 % inputZ;
                    Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} 10,000 modulo {4} is {5}.", _moduleId, inputY, inputZ, currentInput, inputZ, currentInputAsNumber);
                }
                break;
            case 3: //7
                currentInput = toPrepend + "Function: Just return 7.";
                currentInputAsNumber = 7;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} Okay, here is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 4: //(Larger divided by smaller) modulo 10
                int largerOneE;
                int smallerOneE;
                currentInput = toPrepend + "Function: (Larger divided by smaller) modulo 10.";
                if (inputY > inputZ)
                {
                    largerOneE = inputY;
                    smallerOneE = inputZ;
                }
                else
                {
                    largerOneE = inputZ;
                    smallerOneE = inputY;
                }
                currentInputAsNumber = (int)(largerOneE / smallerOneE) % 10;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}, the integer of which is {5}. This modulo 10 is {6}.", _moduleId, inputY, inputZ, currentInput,
                    (float)largerOneE / (float)smallerOneE, (int)(largerOneE / smallerOneE), currentInputAsNumber);
                break;
            case 5: // Triple the number of odd numbers
                currentInputAsNumber = 0;
                if (inputY % 2 == 1)
                {
                    currentInputAsNumber = 3;
                }
                if (inputZ % 2 == 1)
                {
                    currentInputAsNumber = currentInputAsNumber + 3;
                }
                currentInput = toPrepend + "Function: Triple the number of odd numbers..";
                if (currentInputAsNumber == 3)
                {

                    Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. There is 1 odd number, which gives 3.", _moduleId, inputY, inputZ);
                }
                else
                {

                    Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. There are {3} odd numbers, which gives {4}.", _moduleId, inputY, inputZ, currentInputAsNumber/3, currentInputAsNumber);
                }
                break;
            case 6: //10 minus (abs(digits in a minus digits in b))
                currentInput = toPrepend + "Function: 10 minus abs(digits in a minus digits in b).";
                currentInputAsNumber = 10 - Math.Abs(inputY.ToString().Length - inputZ.ToString().Length);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 7: //(sum of a's digits) * (sum of b's digits)
                currentInput = toPrepend + "Function: Sum of a's digits times sum of b's digits.";
                int sumA = 0;
                int sumB = 0;
                for (int i = 0; i < ("" + inputY).Length; i++)
                {
                    sumA = sumA + Int16.Parse(("" + inputY).Substring(i, 1));
                }
                for (int j = 0; j < ("" + inputZ).Length; j++)
                {
                    sumB = sumB + Int16.Parse(("" + inputZ).Substring(j, 1));
                }
                currentInputAsNumber = sumA * sumB;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} Digit sums are {4} and {5}, and their product is {6}.", _moduleId, inputY, inputZ,
                    currentInput, sumA, sumB, currentInputAsNumber);
                break;
            case 8: //a times b
                currentInputAsNumber = inputY * inputZ;
                currentInput = toPrepend + "Function: a times b.";

                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 9: //(a+b) modulo 12
                currentInput = toPrepend + "Function: (a+b) modulo 12.";
                currentInputAsNumber = (inputY + inputZ) % 12;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} The sum is {4}, that modulo 12 is {5}.", _moduleId, inputY, inputZ, currentInput,
                    (inputZ + inputY), currentInputAsNumber);
                break;
            case 10: //Highest digit
                currentInput = toPrepend + "Function: Highest digit.";
                wackyString = "" + inputY + inputZ;
                var hiDigit = 0;
                for (int i = 0; i < wackyString.Length; i++)
                {
                    if (Int16.Parse(wackyString.Substring(i, 1)) > hiDigit)
                    {
                        hiDigit = Int16.Parse(wackyString.Substring(i, 1));
                    }

                }
                currentInputAsNumber = hiDigit;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 11: //((a modulo 10) cubed) plus ((b modulo 10) cubed)
                currentInput = toPrepend + "Function: ((a modulo 10) cubed) plus ((b modulo 10) cubed).";
                currentInputAsNumber = (int)Math.Pow(inputY % 10, 3) + (int)Math.Pow(inputZ % 10, 3);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} These are {4} and {5}, the sum of which is {6}.", _moduleId, inputY, inputZ, currentInput, (int)Math.Pow(inputY % 10, 3),
                    (int)Math.Pow(inputZ % 10, 3), currentInputAsNumber);
                break;
            case 12: //Lunar Addition
                currentInput = toPrepend + "Function: Lunar Addition.";
                wackyString = "";
                string LAa = "" + inputY;
                string LAb = "" + inputZ;
                while (LAa.Length < 4)
                {
                    LAa = "0" + LAa;
                }
                while (LAb.Length < 4)
                {
                    LAb = "0" + LAb;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (Int16.Parse(LAa.Substring(i, 1)) > Int16.Parse(LAb.Substring(i, 1)))
                    {
                        wackyString = wackyString + LAa.Substring(i, 1);
                    }
                    else
                    {
                        wackyString = wackyString + LAb.Substring(i, 1);
                    }
                }
                currentInputAsNumber = Int16.Parse(wackyString);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 13: // abs(10,000 minus a) times abs(1,000 minus b)

                currentInputAsNumber = Math.Abs(10000 - inputY) * Math.Abs(1000 - inputZ);
                currentInput = toPrepend + "Function: abs(10,000 minus a) times abs(1,000 minus b). abs(" + (10000 - inputY) + ") * abs(" + (1000 - inputZ) + ") is " + currentInputAsNumber + ".";

                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3}", _moduleId, inputY, inputZ, currentInput);
                break;
            case 14: // (Larger modulo smaller) modulo 8
                int largerOneEi;
                int smallerOneEi;
                currentInput = toPrepend + "Function: (Larger modulo smaller) modulo 8.";
                if (inputY > inputZ)
                {
                    largerOneEi = inputY;
                    smallerOneEi = inputZ;
                }
                else
                {
                    largerOneEi = inputZ;
                    smallerOneEi = inputY;
                }
                currentInputAsNumber = (largerOneEi % smallerOneEi) % 8;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} Larger modulo smaller is {4}, that modulo 8 is {5}.", _moduleId, inputY, inputZ, currentInput,
                    largerOneEi % smallerOneEi, currentInputAsNumber);
                break;
            case 15: //Number of different digits
                currentInput = toPrepend + "Function: Number of different digits in a and b.";
                wackyString = "" + inputY + inputZ;
                int digitsIn = 0;
                bool isDigitFound = false;

                for (int j = 0; j < 10; j++)
                {
                    isDigitFound = false;
                    for (int i = 0; i < wackyString.Length; i++)
                    {
                        if (Int16.Parse(wackyString.Substring(i, 1)) == j)
                        {
                            isDigitFound = true;
                            i = wackyString.Length;
                        }

                    }
                    if (isDigitFound)
                    {
                        digitsIn++;
                    }
                }
                currentInputAsNumber = digitsIn;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 16:  //smaller minus (larger modulo smaller)
                int largerOneC;
                int smallerOneC;
                currentInput = toPrepend + "Function: Smaller minus (Larger modulo smaller).";
                if (inputY > inputZ)
                {
                    largerOneC = inputY;
                    smallerOneC = inputZ;
                }
                else
                {
                    largerOneC = inputZ;
                    smallerOneC = inputY;
                }
                currentInputAsNumber = smallerOneC - (largerOneC % smallerOneC);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} That is {4}.", _moduleId, inputY, inputZ,
                currentInput, currentInputAsNumber);
                break;
            case 17: //Number of different odd digits
                currentInput = toPrepend + "Function: Number of different odd digits in a and b.";
                wackyString = "" + inputY + inputZ;
                int digitsInX = 0;
                bool isDigitFoundX = false;
                for (int j = 0; j < 9; j++)
                {
					j++;
                    isDigitFoundX = false;
                    for (int i = 0; i < wackyString.Length; i++)
                    {
                        if (Int16.Parse(wackyString.Substring(i, 1)) == j)
                        {
                            isDigitFoundX = true;
                            i = wackyString.Length;
                        }

                    }
                    if (isDigitFoundX)
                    {
                        digitsInX++;
                    }
                }
                currentInputAsNumber = digitsInX;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 18: //((a + b) times 10) plus (abs(a - b) modulo 10)
                currentInput = toPrepend + "Function: ((a + b) times 10) plus (abs(a - b) modulo 10).";
                currentInputAsNumber = ((inputY + inputZ) * 10) + (Math.Abs(inputY - inputZ) % 10);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 19: //a plus b
                currentInput = toPrepend + "Function: a plus b.";
                currentInputAsNumber = inputY + inputZ;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 20: //(a times b) divided by (ports + 2)
                int largerOneP;
                int smallerOneP;
                currentInput = toPrepend + "Function: (a times b) divided by (ports + 2).";
                if (inputY > inputZ)
                {
                    largerOneP = inputY;
                    smallerOneP = inputZ;
                }
                else
                {
                    largerOneP = inputZ;
                    smallerOneP = inputY;
                }
                currentInputAsNumber = ((smallerOneP * largerOneP) / (2 + Bomb.GetPortCount()));
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);

                break;
            case 21: //a times b, modulo 10

                currentInput = toPrepend + "Function: a times b, modulo 10.";
                currentInputAsNumber = (inputY * inputZ) % 10;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4} modulo 10, or {5}.", _moduleId, inputY, inputZ, currentInput, inputY * inputZ, currentInputAsNumber);
                break;
            case 22: //Number of digits missing
                currentInput = toPrepend + "Function: Number of digits missing.";
                wackyString = "" + inputY + inputZ;
                int digitsOut = 0;
                bool isDigitFoundY = false;

                for (int j = 0; j < 10; j++)
                {
                    isDigitFoundY = false;
                    for (int i = 0; i < wackyString.Length; i++)
                    {
                        if (Int16.Parse(wackyString.Substring(i, 1)) == j)
                        {
                            isDigitFoundY = true;
                        }

                    }
                    if (!isDigitFoundY)
                    {
                        digitsOut++;
                    }
                }
                currentInputAsNumber = digitsOut;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 23: //(Larger modulo smaller) times smaller
                int largerOnePx;
                int smallerOnePx;
                currentInput = toPrepend + "Function: (Larger modulo smaller) times smaller.";
                if (inputY > inputZ)
                {
                    largerOnePx = inputY;
                    smallerOnePx = inputZ;
                }
                else
                {
                    largerOnePx = inputZ;
                    smallerOnePx = inputY;
                }
                currentInputAsNumber = (largerOnePx % smallerOnePx) * smallerOnePx;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4} times {5}, or {6}.", _moduleId, inputY, inputZ, currentInput, largerOnePx % smallerOnePx, smallerOnePx,
                    currentInputAsNumber);

                break;
            case 24: //a + (b squared)
                currentInput = toPrepend + "Function: a + (b squared).";
                currentInputAsNumber = (inputZ * inputZ) + (inputY);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} The final answer is {4}.", _moduleId, inputY, inputZ,
                     currentInput, currentInputAsNumber);
                break;
            case 25: // 11 minus twice the number of non-two-digit variables
                currentInputAsNumber = 11;
                if (inputY >= 10 && inputY <= 99)
                {
                    currentInputAsNumber = currentInputAsNumber - 2;
                }
                if (inputZ >= 10 && inputZ <= 99)
                {
                    currentInputAsNumber = currentInputAsNumber - 2;
                }
                currentInput = toPrepend + "Function: 11 minus twice the number of non-two-digit variables.";

                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is made into the integer {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 26: //Absolute value of a minus b
                currentInput = toPrepend + "Function: Absolute value of (a minus b)";
                currentInputAsNumber = Math.Abs(inputY - inputZ);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 27: //(a times b) module 73
                currentInput = toPrepend + "Function: (a times b) module 73.";
                currentInputAsNumber = (inputY * inputZ) % 73;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} The product is {4}, that modulo 73 is {5}.", _moduleId, inputY, inputZ,
                     currentInput, (inputY * inputZ), currentInputAsNumber);
                break;
            case 28: //Number of digits * 1500
                currentInput = toPrepend + "Function: Number of digits times 1,500.";
                currentInputAsNumber = ("" + inputY + inputZ).Length * 1500;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 29: //3 plus the number of numbers over 2,500
                currentInputAsNumber = 3;
                if (inputY > 2500)
                {
                    currentInputAsNumber++;
                }
                if (inputZ > 2500)
                {
                    currentInputAsNumber++;
                }

                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. " + toPrepend + "3 plus the number of numbers over 2,500, which is {3}.", _moduleId, inputY, inputZ, currentInputAsNumber);
                break;
            case 30: //(a squared) + b
                currentInput = toPrepend + "Function: (a squared) + b.";
                currentInputAsNumber = (inputY * inputY) + (inputZ);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} The final answer is {4}.", _moduleId, inputY, inputZ,
                     currentInput, currentInputAsNumber);
                break;
            case 31: //Larger divided by (digits in both a and b)
                int largerOneEd;
                currentInput = toPrepend + "Function: Larger divided by (digits in both a and b).";
                if (inputY > inputZ)
                {
                    largerOneEd = inputY;
                }
                else
                {
                    largerOneEd = inputZ;
                }
                currentInputAsNumber = (int)(largerOneEd / ("" + inputY + inputZ).Length);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}, the integer of which is {5}.", _moduleId, inputY, inputZ, currentInput,
                    (float)largerOneEd / ("" + inputY + inputZ).Length, currentInputAsNumber);
                break;
            case 32: //a + b + all concatenated serial number digits + 1
                currentInput = toPrepend + "Function: a + b + concatenated serial number digits + 1.";
                currentInputAsNumber = inputY + inputZ + 1;
                wackyString = "";
                foreach (char character in Bomb.GetSerialNumber())
                {
                    if (character == '0' || character == '1' || character == '2' || character == '3' || character == '4' ||
                        character == '5' || character == '6' || character == '7' || character == '8' || character == '9')
                    {
                        wackyString = wackyString + character;
                    }
                }
                currentInputAsNumber = currentInputAsNumber + Int16.Parse(wackyString);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 33: //8 minus (Number of numbers below 100)
                currentInputAsNumber = 8;
                if (inputY < 100)
                {
                    currentInputAsNumber--;
                }
                if (inputZ < 100)
                {
                    currentInputAsNumber--;
                }
                currentInput = toPrepend + "Function: 8 minus (Number of numbers below 100).";
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ,
                    currentInput, currentInputAsNumber);
                break;
            case 34: //(a modulo 50) + b
                currentInput = toPrepend + "Function: (a modulo 50) + b.";
                currentInputAsNumber = (inputY % 50) + inputZ;
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}.", _moduleId, inputY, inputZ, currentInput, currentInputAsNumber);
                break;
            case 35: // (Larger/Smaller)
                int largerOneEz;
                int smallerOneEz;
                currentInput = toPrepend + "Function: Larger divided by smaller.";
                if (inputY > inputZ)
                {
                    largerOneEz = inputY;
                    smallerOneEz = inputZ;
                }
                else
                {
                    largerOneEz = inputZ;
                    smallerOneEz = inputY;
                }
                currentInputAsNumber = (int)(largerOneEz / smallerOneEz);
                Debug.LogFormat("[Functions #{0}] Variables are {1}, {2}. {3} This is {4}, the integer of which is {5}.", _moduleId, inputY, inputZ, currentInput,
                    (float)largerOneEz / (float)smallerOneEz, currentInputAsNumber);
                break;
            default: // Uh oh, something's wrong, solve module and tell them to contact me
                isSolved = true;
                inputA.GetComponentInChildren<TextMesh>().text = "OOPS";
                inputB.GetComponentInChildren<TextMesh>().text = "MSG";
                inputResult.GetComponentInChildren<TextMesh>().text = "@JerryEris";
                pressedAllowed = false;
                GetComponent<KMBombModule>().HandlePass();
                Debug.LogFormat("[Functions #{0}] Something went wrong, please contact JerryEris#6034 on Discord!", _moduleId);
                break;
        }
        /*  
        "|a minus b| modulo 1000", "(a + b) times (a minus b)",, "(a + b) times Larger", "(a + b) * b", "(a + b) * a"
        */
        if (quirkNum == 0 && doQuirks)
        {
            currentInputAsNumber = currentInputAsNumber + xValue;
            if (currentInputAsNumber < 0)
            {
                Debug.LogFormat("[Functions #{0}] This module's malfunction would subtract one to the query result, but it cannot display a negative number, so it will display 0.", _moduleId);
                currentInputAsNumber = 0;
            }
            else if (currentInputAsNumber > 999999999999)
            {
                currentInputAsNumber = Int64.Parse(currentInputAsNumber.ToString().Substring(0, 12));
                Debug.LogFormat("[Functions #{0}] This module's malfunction {1} one to the query result, so it will display {2}.", _moduleId, xValue == 1 ? "adds" : "subtracts", currentInputAsNumber);
            }
            else
            {
                Debug.LogFormat("[Functions #{0}] This module's malfunction {1} one to the query result, so it will display {2}.", _moduleId, xValue == 1 ? "adds" : "subtracts", currentInputAsNumber);
            }
            
        }
    }
}
