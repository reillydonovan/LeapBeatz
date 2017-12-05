/* Copyright (c) 2017 ExT (V.Sigalkin) */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extOSC.Examples
{
    public class TransmitTransforms : MonoBehaviour
    {
        #region Public Vars
        public GameObject transformObject;
        public string Address = "/example/1";

        [Header("OSC Settings")]
        public OSCTransmitter Transmitter;

        #endregion

        #region Unity Methods

        protected virtual void Start()
        { /*
            var message = new OSCMessage(Address);
            float transformObjectX = transformObject.transform.position.x;
            message.AddValue(OSCValue.Float(transformObjectX));

            Transmitter.Send(message);
            */
        }

        protected virtual void Update()
        {
            var message = new OSCMessage(Address);
            float transformObjectX = transformObject.transform.position.x * 10;
            message.AddValue(OSCValue.Float(transformObjectX));

            Transmitter.Send(message);
        } 

        #endregion
    }
}