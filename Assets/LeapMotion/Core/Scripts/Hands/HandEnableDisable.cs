/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Leap;

namespace Leap.Unity{
  public class HandEnableDisable : HandTransitionBehavior {
        public GameObject[] SkinnerObject;
    protected override void Awake() {
      base.Awake();
      gameObject.SetActive(false);
         //   Debug.Log("Awake: Hand is false");
            for (int i = 0; i < SkinnerObject.Length; i++)
            {
                SkinnerObject[i].SetActive(false);
            }
          //  DisableChildren();
    }

  	protected override void HandReset() {
      gameObject.SetActive(true);
     //       Debug.Log("Reset, Hand is true");
            for (int i = 0; i < SkinnerObject.Length; i++)
            {
                SkinnerObject[i].SetActive(true);

            }
          //  EnableChildren();
        }

    protected override void HandFinish() {
       //    gameObject.transform.position = new Vector3(0, 0, 0);
            gameObject.SetActive(false);
         //   DisableChildren();
        //    Debug.Log("Finished, hand is false");
            for (int i = 0; i < SkinnerObject.Length; i++)
            {
                SkinnerObject[i].SetActive(false);
            }
        }

        public void DisableChildren()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        public void EnableChildren()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

    }
}
