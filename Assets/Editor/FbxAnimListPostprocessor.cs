
// FbxAnimListPostprocessor.cs : Use an external text file to import a list of 
// splitted animations for FBX 3D models.
//
// Put this script in your "Assets/Editor" directory. When Importing or 
// Reimporting a FBX file, the script will search a text file with the 
// same name and the ".txt" extension.
// File format: one line per animation clip "firstFrame-lastFrame loopFlag animationName"
// The keyworks "loop" or "noloop" are optional.
// Example:
// 0-50 loop Move forward
// 100-190 die

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class FbxAnimListPostprocessor : AssetPostprocessor
{
    public void OnPreprocessModel()
    {
        if (Path.GetExtension(assetPath).ToLower() == ".fbx"
            && !assetPath.Contains("@"))
        {
            try
            {
                string fileAnim;
                if (DragAndDrop.paths.Length <= 0)
                {
                    return;
                }
                fileAnim = DragAndDrop.paths[0];
                string ClipText = Path.ChangeExtension(fileAnim, ".txt");
                StreamReader file = new StreamReader(ClipText);
                string sAnimList = file.ReadToEnd();
                file.Close();
                //
                if (EditorUtility.DisplayDialog("FBX Animation Import from file",
                    fileAnim, "Import", "Cancel"))
                {
                    System.Collections.ArrayList List = new ArrayList();
                    ParseAnimFile(sAnimList, ref List);

                    ModelImporter modelImporter = assetImporter as ModelImporter;
                    //modelImporter.clipAnimations. = true;
                    modelImporter.clipAnimations = (ModelImporterClipAnimation[])
                        List.ToArray(typeof(ModelImporterClipAnimation));

                    EditorUtility.DisplayDialog("Imported animations",
                        "Number of imported clips: "
                        + modelImporter.clipAnimations.GetLength(0).ToString(), "OK");
                }
            }
            catch { }
            // (Exception e) { EditorUtility.DisplayDialog("Imported animations", e.Message, "OK"); }
        }
    }

    void ParseAnimFile(string sAnimList, ref System.Collections.ArrayList List)
    {
        Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(loop|noloop| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        Match match = regexString.Match(sAnimList, 0);
        while (match.Success)
        {
            ModelImporterClipAnimation clip = new ModelImporterClipAnimation();

            if (match.Groups["firstFrame"].Success)
            {
                clip.firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);
            }
            if (match.Groups["lastFrame"].Success)
            {
                clip.lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);
            }
            if (match.Groups["loop"].Success)
            {
                clip.loop = match.Groups["loop"].Value == "loop";
            }
            if (match.Groups["name"].Success)
            {
                clip.name = match.Groups["name"].Value;
            }

            List.Add(clip);

            match = regexString.Match(sAnimList, match.Index + match.Length);
        }
    }
}