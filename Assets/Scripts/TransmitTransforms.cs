/* Copyright (c) 2017 ExT (V.Sigalkin) */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extOSC.Examples
{
    public class TransmitTransforms : MonoBehaviour
    {
        #region Public Vars
    //    public int totalItems;
        public GameObject[] transformObject;
        public string[] Address;

        [Header("OSC Settings")]
        public OSCTransmitter Transmitter;

        #endregion

        #region Unity Methods

        void Start()
        { /*
            var message = new OSCMessage(Address);
            float transformObjectX = transformObject.transform.position.x;
            message.AddValue(OSCValue.Float(transformObjectX));

            Transmitter.Send(message);
            */
       //     transformObject = new GameObject[totalItems];
       //     Address = new string[totalItems];
        }

        protected virtual void Update()
        {

            Debug.Log(transformObject[1].transform.rotation.z);

            //left palm position variables
            float LeftPalmPositionX = transformObject[0].transform.position.x;
            float LeftPalmPositionY = transformObject[0].transform.position.y;
            float LeftPalmPositionZ = transformObject[0].transform.position.z;

            // left palm rotation variables
            float LeftPalmRotationX = transformObject[0].transform.rotation.x;
            float LeftPalmRotationY = transformObject[0].transform.rotation.y;
            float LeftPalmRotationZ = transformObject[0].transform.rotation.z;

            //right palm position variables
            float RightPalmPositionX = transformObject[1].transform.position.x;
            float RightPalmPositionY = transformObject[1].transform.position.y;
            float RightPalmPositionZ = transformObject[1].transform.position.z;

            // left palm rotation variables
            float RightPalmRotationX = transformObject[1].transform.rotation.x;
            float RightPalmRotationY = transformObject[1].transform.rotation.y;
            float RightPalmRotationZ = transformObject[1].transform.rotation.z;

            //left palm position messages
            SendFloatMessages(Address[0], LeftPalmPositionX);
            SendFloatMessages(Address[1], LeftPalmPositionY);
            SendFloatMessages(Address[2], LeftPalmPositionZ);

            //left palm rotation messages
            SendFloatMessages(Address[3], LeftPalmRotationX);
            SendFloatMessages(Address[4], LeftPalmRotationY);
            SendFloatMessages(Address[5], LeftPalmRotationZ);

            //right palm position messages
            SendFloatMessages(Address[6], RightPalmPositionX);
            SendFloatMessages(Address[7], RightPalmPositionY);
            SendFloatMessages(Address[8], RightPalmPositionZ);

            //right palm rotation messages
            SendFloatMessages(Address[9], RightPalmRotationX);
            SendFloatMessages(Address[10], RightPalmRotationY);
            SendFloatMessages(Address[11], RightPalmRotationZ);
        } 

         void SendFloatMessages(string _address, float _parameter)
        {
           var message = new OSCMessage(_address);
            message.AddValue(OSCValue.Float(_parameter));
            Transmitter.Send(message);

        }

        #endregion
    }
}