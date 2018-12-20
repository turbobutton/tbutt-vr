using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using TButt;
using System.IO;

namespace TButt.Editor
{
    /// <summary>
    /// Helper classes for TButt editor scripts.
    /// </summary>
    public class TBEditorHelper
    {
        public static void CheckoutAndSaveJSONFile(string fileName, object obj, TBDataManager.PathType pathType = TBDataManager.PathType.PersistentDataPath, bool skipVersioning = false, bool isString = false)
        {
            bool addToVersionControl = false;
            if (Provider.isActive)  // Is version control enabled in the project?
            {
                if (Provider.hasCheckoutSupport)    // Does the project's version control settings support auto-checkout?
                {
                    Asset targetAsset = Provider.GetAssetByPath(fileName);
                    if (targetAsset != null)    // Does the thing we want to checkout exist?
                    {
                        if (!Provider.IsOpenForEdit(targetAsset))   // Is it already checked out?
                        {
                            Task checkoutTask = Provider.Checkout(targetAsset, CheckoutMode.Both);
                            checkoutTask.Wait();    // Wait for checkout to finish
                        }
                    }
                    else
                    {
                        addToVersionControl = true;
                    }
                }
            }

            if (!Directory.Exists("Assets/Resources/"))
                Directory.CreateDirectory("Assets/Resources/");

            if(!Directory.Exists("Assets/Resources/" + TBSettings.settingsFolder))
                Directory.CreateDirectory("Assets/Resources/" + TBSettings.settingsFolder);

            if (!isString)
                TBDataManager.SerializeObjectToFile(obj, fileName, pathType);   // Save the file
            else
                TBDataManager.WriteStringToFile((string)obj, fileName, pathType);

            AssetDatabase.Refresh();

            if (skipVersioning)
                return;

            if (addToVersionControl)
            {
                Asset asset = Provider.GetAssetByPath(fileName);
                if (asset == null)
                {
                    TBLogging.LogWarning("A new file was created but could not be automatically added to version control: " + fileName);
                    return;
                }
                if (Provider.AddIsValid(new AssetList() { asset }))
                {
                    Task addTask = Provider.Add(asset, false);
                    addTask.Wait();
                }
                else
                {
                    TBLogging.LogWarning("A new file was created but could not be automatically added to version control: " + fileName);
                }
            }
        }
    }
}