using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class SimpleMenu : BaseState
    {

        protected Menus.SimpleMenuGUI menuComponent;
        public override void Initialize()
        {
            menuComponent = SpawnUI<Menus.SimpleMenuGUI>(StringConstants.PrefabsSimpleMenu);
            if (string.IsNullOrEmpty(CommonData.nameMyPet))
            {
                menuComponent.PlayButton.gameObject.SetActive(false);
            }
            else
            {
                menuComponent.PlayButton.gameObject.SetActive(true); 
            }
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
            Menus.SimpleMenuGUI buttonComponent = source.GetComponent<Menus.SimpleMenuGUI>();
            if (source == menuComponent.ChooseButton.gameObject)
            {
                Debug.Log(source.name);
                manager.SwapState(new SelectPetMenu());
            }
            else if (source == menuComponent.PlayButton.gameObject)
            {
                Debug.Log(source.name);
                manager.PushState(new AR());
            }
            else if (source == menuComponent.CancelButton.gameObject)
            {
                Debug.Log(source.name);
                manager.SwapState(new SelectModeState());
            }


        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();

        }
    }

}
 
