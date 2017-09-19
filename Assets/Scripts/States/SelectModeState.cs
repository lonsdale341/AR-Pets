using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class SelectModeState : BaseState
    {
        protected Menus.SelectModeStateGUI menuComponent;
        public override void Initialize()
        {
            menuComponent = SpawnUI<Menus.SelectModeStateGUI>(StringConstants.PrefabsSelectModeStateMenu);
        }
        public override void Suspend()
        {

            HideUI();
        }

        public override StateExitValue Cleanup()
        {

            DestroyUI();
            return null;
        }
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            Menus.SelectModeStateGUI buttonComponent = source.GetComponent<Menus.SelectModeStateGUI>();
            if (source == menuComponent.SimpleButton.gameObject)
            {
                Debug.Log(source.name);
                CommonData.modeState = "simple";
                manager.SwapState(new SimpleMenu());
            }
            else if (source == menuComponent.FacebookButton.gameObject)
            {
                Debug.Log(source.name);
                CommonData.modeState = "facebook";
                if (string.IsNullOrEmpty(CommonData.nameMyPet))
                {
                    manager.SwapState(new WarningChosePet()); 
                }
                else
                {
                    manager.PushState(new AR());
                }
                // manager.SwapState(new MainMenu());
            }
            


        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();

        }
    }


}
