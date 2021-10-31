using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Debugging
{
    public static class DebugCommands
    {
        struct Command
        {
            public string keyword;
            readonly string desc;
            readonly string param;
            public readonly string HelpString => Colored("yellow", keyword) + (param != "" ? Colored("orange", $" [{param}]") : "") + " - " + desc;

            readonly Func<string, string> function;
            readonly bool paramRequired;

            public Command(string keyword, string desc, string param = "", bool paramRequired = false, Func<string, string> function = null)
            {
                this.keyword = keyword;
                this.desc = desc;
                this.param = param;
                this.paramRequired = !string.IsNullOrEmpty(param) && paramRequired;
                this.function = function ?? (_ => "");
            }

            public string Execute(string arg)
            {
                if (paramRequired && string.IsNullOrWhiteSpace(arg))
                    throw new Exception($"'{keyword}' requires at least one argument. (Consult the help command.)");
            
                return function(arg);
            }
        }

        static string inputKeyword;

        static string Colored(string color, string text) => $"<color={color}>{text}</color>";

        public static string Process(string input)
        {
            var sepIndex = input.IndexOf(' ');
            inputKeyword = sepIndex == -1 ? input : input.Substring(0, sepIndex);
            var combinedArgs = sepIndex == -1 ? "" : input.Substring(sepIndex + 1);

            foreach (var command in Commands)
                if (inputKeyword == command.keyword) return command.Execute(combinedArgs);

            throw new Exception($"The command '{inputKeyword}' does not exist.");
        }

        static readonly Command[] Commands =
        {
            new Command("allthings", "Lists all objects in the scene.", function:_ =>
            {
                var list = "";

                foreach (var go in Object.FindObjectsOfType<GameObject>())
                    list += ", " + go.name;

                return Colored("yellow", "Found: ") + list.Substring(2);
            }),

            new Command("childrenof", "Lists an object's children.", "object name", true, objName =>
            {
                if (!GameObject.Find(objName)) throw new Exception($"'{objName}' not found.");

                var hierarchy = GameObject.Find(objName).transform.GetComponentsInChildren<Transform>();

                if (hierarchy.Length == 1) return Colored("yellow", $"'{objName}' has no children.");

                var list = "";

                for (var i = 1; i < hierarchy.Length; i++)
                    list += ", " + hierarchy[i].name;

                return Colored("yellow", $"Children of '{objName}': ") + list.Substring(2);
            }),

            new Command("clear", "Clears the console output."),

            new Command("destroy", "Destroys the first object with a given name.", "object name", true, objName =>
            {
                GameObject found;
                if (!(found = GameObject.Find(objName))) throw new Exception($"'{objName}' not found.");

                if (found.GetComponentInChildren<DebugConsole>()) return Colored("yellow", "You can't destroy the console!");

                Object.Destroy(found);
                return $"Destroyed '{objName}'.";
            }),

            new Command("help", "Lists all commands.", function:_ =>
            {
                var fullHelpString = "";

                foreach (var command in Commands)
                    fullHelpString += "\n" + command.HelpString;

                // Ditch the first newline.
                return fullHelpString.Substring(1);
            }),

            new Command("reload", "Reloads the scene.", function:_ =>
            {
                var activeScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(activeScene.buildIndex);
                return $"Successfully reloaded scene '{activeScene.name}'.";
            }),

            new Command("topthings", "Lists all top-level objects in the scene.", function:_ =>
            {
                var list = "";

                foreach (var go in Object.FindObjectsOfType<GameObject>())
                {
                    if (go.transform.parent == null)
                        list += ", " + go.name;
                }

                return Colored("yellow", "Top-Level:") + list.Substring(1);
            }),
        };
    }
}
