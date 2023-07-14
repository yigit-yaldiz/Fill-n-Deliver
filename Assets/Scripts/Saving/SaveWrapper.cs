using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;

namespace Saving
{
    [RequireComponent(typeof(SavingSystem))]
    public class SaveWrapper : MonoBehaviour
    {
        [SerializeField] private string saveFile= "mbghtrzxgtidltqwv";
        private SavingSystem savingSystem;
        //private static bool _isItFirstTime = true;

        public static SaveWrapper Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            savingSystem = GetComponent<SavingSystem>();
            //if (_isItFirstTime)
            {
                savingSystem.Load(saveFile);
                //_isItFirstTime = false;
            }
            //else
            //{
                //savingSystem.Save(saveFile);
            //}
        }

        public void Save()
        {
            savingSystem.Save(saveFile);
        }

        public void SaveEmpty()
        {
            savingSystem.DeleteThisLevelsSaveFiles(saveFile);
        }
    }
}
