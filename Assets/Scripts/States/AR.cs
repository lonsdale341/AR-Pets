using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class AR : BaseState
    {

        protected Menus.ARGUI menuComponent;
        public override void Initialize()
        {
            menuComponent = SpawnUI<Menus.ARGUI>(StringConstants.PrefabsARMenu);
            CommonData.poseController.msgTapTheCircle = menuComponent.MsgTapTheCircleButton.gameObject;
            CommonData.poseController.msgTryAgain = menuComponent.MsgTapTryAgainButton.gameObject;
            CommonData.poseController.msgGetCloser = menuComponent.MsgGetClosedButton.gameObject;
            CommonData.poseController.InitActive();
        }
        public override void Suspend()
        {

            HideUI();
        }

        public override StateExitValue Cleanup()
        {

            DestroyUI();
            CommonData.poseController.mTrackingMode=PoseController.TrackingMode.IDLE;
            return null;
        }
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            Menus.ARGUI buttonComponent = source.GetComponent<Menus.ARGUI>();
            if (source == menuComponent.ResetButton.gameObject)
            {
                Debug.Log(source.name);
                CommonData.poseController.ResetState();
            }
            else if (source == menuComponent.CancelButton.gameObject)
            {
                Debug.Log(source.name);
                manager.PopState();
            }
            


        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();

        }
    }


}
