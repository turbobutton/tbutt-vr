using UnityEngine;
using System.IO;

namespace TButt
{
    /// <summary>
    /// Handles serialization and deserialization of JSON files.
    /// </summary>
    public static class TBDataManager
    {
        public static string persistentDataPath = Application.persistentDataPath + "/";

        /// <summary>
        /// Serializes an object to JSON and writes it to a file.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        /// <param name="pathType"></param>
        public static void SerializeObjectToFile(System.Object obj, string fileName, PathType pathType = PathType.PersistentDataPath)
        {
            if (obj == null)
            {
                Debug.LogError("Attempted to serialize an object, but the object was null.");
                return;
            }

            string serializedString = SerializeString(obj);

            string path = GetPathForType(pathType);

            if ((pathType == PathType.PersistentDataPath) && Application.isPlaying)
                TBLogging.LogWarning("Sserializing files from the persistent data path at runtime will not work on all platforms! Use the Async subclass methods instead.");
                
            SaveStringToStorageDefault(serializedString, path + fileName);
        }

        /// <summary>
        /// Deserializes a given JSON file into the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string fileName, PathType pathType = PathType.PersistentDataPath)
        {
            // Need to handle resource folder files a bit differently.
            if (pathType == PathType.ResourcesFolder)
            {
                TextAsset raw = Resources.Load(fileName) as TextAsset;
                string rawString = null;
                if (raw != null)
                    rawString = raw.ToString();

                if (string.IsNullOrEmpty(rawString))
                    return GetNullOrEmptyOfType<T>();
                else
                {
                    return DeserializeString<T>(rawString, fileName);
                }
            }

            string path = GetPathForType(pathType);
            string serializedString = "";

            if ((pathType == PathType.PersistentDataPath) && Application.isPlaying)
                TBLogging.LogWarning("Deserializing files from the persistent data path at runtime will not work on all platforms! Use the Async subclass methods instead.");

            serializedString = LoadStringFromStorageDefault(path + fileName);

            return DeserializeString<T>(serializedString, fileName);
        }

        public static void SaveStringToStorageDefault(string serializedString, string pathAndFileName)
        {
            using (FileStream fs = File.Open(pathAndFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.SetLength(0);

                    sw.Write(serializedString);

                    sw.Close();
                    fs.Close();
                }
            }
        }

        public static string LoadStringFromStorageDefault(string pathAndFileName)
        {
            string serializedString;
            using (FileStream stream = new FileStream(pathAndFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader file = new StreamReader(stream))
                {
                    serializedString = file.ReadToEnd();

                    stream.Close();
                    file.Close();
                }
            }
            return serializedString;
        }

        /// <summary>
        /// Deserializes a string into the given object type. Does NOT handle file reading. Use DeserializeFromFile or Async.Load
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedString"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeserializeString<T>(string serializedString, string fileName)
        {
            if (!string.IsNullOrEmpty(serializedString))
            {
                T convertedObject;

                // If the data isn't null or empty, return it as the requested object type.
                try
                {
                    convertedObject = (T)JsonUtility.FromJson(serializedString, typeof(T));
                }
                catch
                {
                    Debug.LogError("The serialized string did not match the expected format, and will be replaced.");
                    Debug.LogError("The serialized string was: " + serializedString);
                    return GetNullOrEmptyOfType<T>();
                }
                return convertedObject;
            }
            else
            {
                // If the serilaized string is null, then either there was an error or the file didn't exist. Data will be created if possible.
                Debug.LogWarning("TBData attempted to deserialize file " + fileName + " but the deserialized version of the data was null. This expected if the file didn't exist.");
                return GetNullOrEmptyOfType<T>();
            }
        }

        public static string SerializeString(System.Object obj)
        {
            string serializedString;
            if (obj.GetType() == typeof(string))
                serializedString = (string)obj;
            else
            {
                serializedString = JsonUtility.ToJson(obj);
            }

            return serializedString;
        }

        /// <summary>
        /// If T is a nullable type, returns null. Otherwise initializes and return an empty object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetNullOrEmptyOfType<T>()
        {
            if (System.Nullable.GetUnderlyingType(typeof(T)) == null)
                return (T)System.Activator.CreateInstance(typeof(T));
            else
            {
                return (T)JsonUtility.FromJson(string.Empty, typeof(T));
            }
        }

        public static string GetPathForType(PathType pathtype)
        {
            switch (pathtype)
            {
                case PathType.PersistentDataPath:
                    return persistentDataPath;
                case PathType.ProjectFolder:
                    return "";
                case PathType.ResourcesFolder:
                    return "";
            }
            return "";
        }

        public enum PathType
        {
            PersistentDataPath,
            ProjectFolder,
            ResourcesFolder
        }

        public static bool HasDataFile(string fileName, PathType pathType = PathType.PersistentDataPath)
        {
            if (Application.isPlaying)
                TBLogging.LogWarning("TBDataManager.HasDataFile is not safe to call on all platforms! Use the Async subclass.");

            return File.Exists(GetPathForType(pathType) + fileName);
        }

        public static void DeleteDataFile(string fileName, PathType pathType)
        {
            if (File.Exists(GetPathForType(pathType) + fileName))
                File.Delete(GetPathForType(pathType) + fileName);
        }

        public static string ReadStringFromFile(string fileName, PathType pathType)
        {
            string path = GetPathForType(pathType);

            using (FileStream stream = new FileStream(path + fileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader file = new StreamReader(stream))
                {
                    string serializedString = file.ReadToEnd();

                    stream.Close();
                    file.Close();

                    if (!string.IsNullOrEmpty(serializedString))
                        return serializedString;
                    else
                    {
                        Debug.LogWarning("TBData attempted to deserialize file " + fileName + " but the deserialized version of the data was null. This expected if the file didn't exist.");
                        return "";
                    }
                }
            }
        }

        public static void WriteStringToFile(string text, string fileName, PathType pathType = PathType.PersistentDataPath)
        {
            if (text == null)
            {
                Debug.LogError("Attempted to save a string, but the string was null.");
                return;
            }

            string path = GetPathForType(pathType);

            using (FileStream fs = File.Open(path + fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.SetLength(0);
                    sw.Write(text);
                    sw.Close();
                    fs.Close();
                }
            }
        }

#region JSON WRAPPER
        // Wrapper for successfully serializing arrays in Unity's native JSON utility.
        public static T[] FromJsonWrapper<T>(Wrapper<T> wrapper)
        {
            return wrapper.Items;
        }

        public static Wrapper<T> ToJsonWrapper<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return wrapper;
        }

        [System.Serializable]
        public class Wrapper<T>
        {
            public T[] Items;
        }
#endregion

    }
}
