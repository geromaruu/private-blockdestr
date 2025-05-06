using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class BinaryDataStream
{

    public static void Save<T>(T serializedObject, string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(path);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + fileName + ".sav", FileMode.Create);

        try
        {
            formatter.Serialize(fileStream, serializedObject);
        }

        catch(SerializationException e)
        {
            Debug.Log("Save fialed. Error:" + e.Message);
        }
        finally
        {
            fileStream.Close(); 
        }
    }
   public static bool Exist(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        string fullFileName = fileName + ".sav";
        return File.Exists(path + fullFileName);
    }

    public static T Read<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + fileName + ".sav", FileMode.Open);
        T returnType = default(T);
        try
        {
            returnType = (T)formatter.Deserialize(fileStream);
        }

        catch (SerializationException e)
        {
            Debug.Log("Read fialed. Error:" + e.Message);
        }
        finally
        {
            fileStream.Close(); 
        }
        return returnType;  
    }
}
