using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class SelectPetMenu : BaseState
    {

        protected Menus.SelectPetMenuGUI menuComponent;
        public override void Initialize()
        {
            menuComponent = SpawnUI<Menus.SelectPetMenuGUI>(StringConstants.PrefabsSelectPetStateMenu);
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
            Menus.SelectPetMenuGUI buttonComponent = source.GetComponent<Menus.SelectPetMenuGUI>();
            if (source == menuComponent.Pet_1_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.nameMyPet = "cat_1";
                manager.PushState(new AR());
            }
            else if (source == menuComponent.Pet_2_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.nameMyPet = "cat_2";
                manager.PushState(new AR());
            }
            else if (source == menuComponent.Pet_3_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.nameMyPet = "puppy_1";
                manager.PushState(new AR());
            }
            else if (source == menuComponent.Pet_4_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.nameMyPet = "puppy_2";
                manager.PushState(new AR());
            }
            else if (source == menuComponent.Cancel_Button.gameObject)
            {
                Debug.Log(source.name);
                 manager.SwapState(new SimpleMenu());
            }

        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();

        }
    }


}
 