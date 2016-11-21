using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace UnityEditor.ProjectWindowCallback
{
	internal class CreateScriptAssetAction : EndNameEditAction
	{
		public override void Action( int instanceId, string path, string source )
		{
			string className = Path.GetFileNameWithoutExtension( path );
			source = source.Replace( "#CLASSNAME#", className );

			File.WriteAllText( path, source );

			AssetDatabase.ImportAsset( path );
			Object o = AssetDatabase.LoadAssetAtPath( path, typeof( Object ) );
			ProjectWindowUtil.ShowCreatedAsset( o );
		}
	}
}

namespace BitStrap
{
	public class ScriptCreator
	{
		private const string scriptSource =
@"using UnityEngine;

public class #CLASSNAME# : MonoBehaviour
{
}
";

		private const string editorScriptSource =
@"using UnityEditor;
using UnityEngine;

public class #CLASSNAME# : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}
";

		private static Texture2D ScriptIcon
		{
			get { return EditorGUIUtility.IconContent( "cs Script Icon" ).image as Texture2D; }
		}

		[MenuItem( "Assets/Create/C# Script", false, 75 )]
		public static void CreateCSharpScript()
		{
			string path = GetNewScriptPath( "NewMonobehaviour" );
			CreateScript( path, scriptSource );
		}

		[MenuItem( "Assets/Create/C# Editor Script", false, 75 )]
		public static void CreateCSharpEditorScript()
		{
			string path = GetNewScriptPath( "NewEditor" );
			CreateScript( path, editorScriptSource );
		}

		private static string GetNewScriptPath( string scriptName )
		{
			string path = "Assets";
			if( Selection.activeObject != null )
			{
				path = AssetDatabase.GetAssetPath( Selection.activeObject );
				if( !AssetDatabase.IsValidFolder( path ) )
					path = Path.GetDirectoryName( path );
			}

			return Path.Combine( path, string.Concat( scriptName, ".cs" ) );
		}

		private static void CreateScript( string path, string source )
		{
			var createScriptAssetAction = ScriptableObject.CreateInstance<CreateScriptAssetAction>();
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists( 0, createScriptAssetAction, path, ScriptIcon, source );
		}
	}
}
