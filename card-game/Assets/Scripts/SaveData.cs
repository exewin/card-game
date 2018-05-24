using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

// === This is the info container class ===
[Serializable ()]
public class SaveData : ISerializable 
{

	// === Values ===
	// Edit these during gameplay
	public static string duelName;
	public static int[] duelCaster;
	public static int winner;
	public static int dc;
	public static int[] duelDeckCard;
	public static int[] duelPos;
	public static int[] duelDeck0;
	public static int[] duelDeck1;
	public static string duelVersion;
	// === /Values ===

	// The default constructor. Included for when we call it during Save() and Load()
	public SaveData () {}

	// This constructor is called automatically by the parent class, ISerializable
	// We get to custom-implement the serialization process here
	public SaveData (SerializationInfo info, StreamingContext ctxt)
	{
		// Get the values from info and assign them to the appropriate properties. Make sure to cast each variable.
		// Do this for each var defined in the Values section above
		duelName = (string)info.GetValue("duelName", typeof(string));
		duelVersion = (string)info.GetValue("duelVersion", typeof(string));
		duelCaster = (int[])info.GetValue("duelCaster", typeof(int[]));
		duelDeckCard = (int[])info.GetValue("duelDeckCard", typeof(int[]));
		duelPos = (int[])info.GetValue("duelPos", typeof(int[]));
		winner = (int)info.GetValue("winner", typeof(int));
		dc = (int)info.GetValue("dc", typeof(int));
		duelDeck0 = (int[])info.GetValue("duelDeck0", typeof(int[]));
		duelDeck1 = (int[])info.GetValue("duelDeck1", typeof(int[]));
	}

	// Required by the ISerializable class to be properly serialized. This is called automatically
	public void GetObjectData (SerializationInfo info, StreamingContext ctxt)
	{
		// Repeat this for each var defined in the Values section
		info.AddValue("duelName", duelName);
		info.AddValue("duelVersion", duelVersion);
		info.AddValue("winner", winner);
		info.AddValue("dc", dc);
		info.AddValue("duelCaster", duelCaster);
		info.AddValue("duelDeckCard", duelDeckCard);
		info.AddValue("duelDeck0", duelDeck0);
		info.AddValue("duelDeck1", duelDeck1);
		info.AddValue("duelPos", duelPos);
	}
}

	// === This is the class that will be accessed from scripts ===
public class SaveLoad 
{

	public static string currentFilePath;    // Edit this for different save files

	// Call this to write data
	public static void Save ()  // Overloaded
	{
		Save (currentFilePath);
	}
	public static void Save (string filePath)
	{
		SaveData data = new SaveData ();
		Stream stream = File.Open(filePath, FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder(); 
		bformatter.Serialize(stream, data);
		stream.Close();
	}

	// Call this to load from a file into "data"
	public static void Load ()  { Load(currentFilePath);  }   // Overloaded
	public static void Load (string filePath) 
	{
		SaveData data = new SaveData ();
		Stream stream = File.Open(filePath, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder(); 
		data = (SaveData)bformatter.Deserialize(stream);
		stream.Close();

	// Now use "data" to access your Values
	}

}

	// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
	// Do not change this
public sealed class VersionDeserializationBinder : SerializationBinder 
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

