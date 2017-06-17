using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO; // Read Write
using LitJson; // Needed for JsonMapper

// List of JSON file with path extensions
// Validation and Exception handling

class FileContainer
{
    public static string Test = "Test/TestTextFile";
    public static string ActTwo_01 = "Act2/Act2_Hotel_01";
    public static string ActTwo_02 = "Act2/Act2_Hotel_02";

}

namespace TextFactory
{
    class TextAssembly
    {

        // So when calling this function, you need to pass in the name
        // of the text file in the resources folder + textfiles folder
        public static DialogueEvent RunTextFactoryForFile(string filePath)
        {
            TextAsset file = Resources.Load("TextFiles/" + filePath) as TextAsset;
            string content = file.ToString();
            DialogueEvent dialogue = JsonMapper.ToObject<DialogueEvent>(content);
            return dialogue;
        }


        // Below doesnt work in builds, since unity doesnt support json extensions at build time
        // Instead going to use Json Schema, but in .txt format

        //// Look up Dictionary - Extensible for any JSON purposes
        //private static Dictionary<string, string> _resourceList = new Dictionary<string, string>
        //{
        //    {"DataTest", "/Resources/LyricData/DataTest.json" },
        //    {"Test", "/Resources/LyricData/DataTest.json" }


        //};

        //// Load the conversation at a given dictionary key
        //// + Validity checks
        //public static DialogueEvent RunJSONFactoryForIndex(string dictionaryIndex)
        //{
        //    string resourcePath = PathForData(dictionaryIndex);

        //    if (IsValidJSON(resourcePath) == true)
        //    {
        //        string jsonString = File.ReadAllText(Application.dataPath + resourcePath);
        //        DialogueEvent narEvent = JsonMapper.ToObject<DialogueEvent>(jsonString);
        //        return narEvent;
        //    }
        //    else
        //    {
        //        throw new Exception("The JSON is not valid, please check the schema and file extension");
        //    }
        //}

        //// Validity check on the dictionary look up
        //private static string PathForData(string dictionaryIndex)
        //{
        //    string resourcePathResult;
        //    if (_resourceList.TryGetValue(dictionaryIndex, out resourcePathResult))
        //    {
        //        return _resourceList[dictionaryIndex];
        //    }
        //    else
        //    {
        //        //return "Failed";
        //        throw new Exception("The look up key you provided is not in the resource list. Please check the JSONFactory namespace");
        //    }
        //}

        //// Validity check for json file extension
        //private static bool IsValidJSON(string path)
        //{
        //    return (Path.GetExtension(path) == ".json") ? true : false;
        //}
    }
}

