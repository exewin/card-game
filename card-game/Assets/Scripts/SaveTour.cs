using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

// === This is the info container class ===
[Serializable ()]
public class SaveTour : ISerializable 
{

	// === Values ===
	// Edit these during gameplay
	
	public static string tourName;
	public static string[] tourNames;
	public static int[] tourPoints;
	public static int[] tourSkill;
	public static int tourXChar;
	public static int curWeek;
	
	// === /Values ===

	// The default constructor. Included for when we call it during Save() and Load()
	public SaveTour () {}

	// This constructor is called automatically by the parent class, ISerializable
	// We get to custom-implement the serialization process here
	public SaveTour (SerializationInfo info, StreamingContext ctxt)
	{
		// Get the values from info and assign them to the appropriate properties. Make sure to cast each variable.
		// Do this for each var defined in the Values section above

		
		
		tourName = (string)info.GetValue("tourName", typeof(string));
		tourNames = (string[])info.GetValue("tourNames", typeof(string[]));
		tourPoints = (int[])info.GetValue("tourPoints", typeof(int[]));
		tourSkill = (int[])info.GetValue("tourSkill", typeof(int[]));
		tourXChar = (int)info.GetValue("tourXChar", typeof(int));
		curWeek = (int)info.GetValue("curWeek", typeof(int));
	}

	// Required by the ISerializable class to be properly serialized. This is called automatically
	public void GetObjectData (SerializationInfo info, StreamingContext ctxt)
	{
		// Repeat this for each var defined in the Values section
		
		
		info.AddValue("tourName", tourName);
		info.AddValue("tourNames", tourNames);
		info.AddValue("tourPoints", tourPoints);
		info.AddValue("tourSkill", tourSkill);
		info.AddValue("tourXChar", tourXChar);
		info.AddValue("curWeek", curWeek);
	}
}

	// === This is the class that will be accessed from scripts ===
public class LoadTour
{

	public static string currentFilePaths;    // Edit this for different save files

	// Call this to write data
	public static void Save ()  // Overloaded
	{ 
		Save (currentFilePaths);
	}
	public static void Save (string filePath)
	{
		SaveTour data = new SaveTour ();
		Stream stream = File.Open(filePath, FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBindere(); 
		bformatter.Serialize(stream, data);
		stream.Close();
	}

	// Call this to load from a file into "data"
	public static void Load ()  { Load(currentFilePaths);  }   // Overloaded
	public static void Load (string filePath) 
	{
		SaveTour data = new SaveTour ();
		Stream stream = File.Open(filePath, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBindere(); 
		data = (SaveTour)bformatter.Deserialize(stream);
		stream.Close();

	// Now use "data" to access your Values
	}

}

	// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
	// Do not change this
public sealed class VersionDeserializationBindere : SerializationBinder 
{ 
	public override Type BindToType( string assemblyName, string typeName )
	{ 
	if ( !string.IsNullOrEmpty( assemblyName ) && !string.IsNullOrEmpty( typeName ) ) 
	{ 
		Type typeToDeserialize = null; 

		assemblyName = Assembly.GetExecutingAssembly().FullName; 

	// The following line of code returns the type. 
		typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) ); 

	return typeToDeserialize; 
	} 

		return null; 
	} 
}

