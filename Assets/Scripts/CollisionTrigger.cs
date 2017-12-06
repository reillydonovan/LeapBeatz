using UnityEngine;

using extOSC;

[RequireComponent(typeof(Collider))]
public class CollisionTrigger : MonoBehaviour
{
    #region Public Vars

    public OSCTransmitter Transmitter
    {
        get { return transmitter; }
        set { transmitter = value; }
    }

    public string TransmitterAddress
    {
        get { return transmitterAddress; }
        set { transmitterAddress = value; }
    }

    #endregion

    #region Protected Vars

    [SerializeField]
    protected OSCTransmitter transmitter;

    [SerializeField]
    protected string transmitterAddress = "/address";

    #endregion

    #region Unity Methods

    protected void OnTriggerEnter(Collider collider)
    {
        var message = new OSCMessage(transmitterAddress);
        message.AddValue(OSCValue.String("Hello, world!"));

        transmitter.Send(message);
    }

    #endregion
}