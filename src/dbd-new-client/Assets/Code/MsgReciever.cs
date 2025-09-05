using System;
using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;

public class MsgReciever
{
    private List<Message> _msgList = new List<Message>(); //  Messsage queue implementation
    public event Action<string, Vector3, int> SvUserJoined;
    public event Action<string> SvUserLeft;
    public event Action<string,Vector3> SvMove;
    public event Action<string,float> SvTurnY;
    public event Action<string,string> SvKill;
    public event Action SvRestartGame;
    public event Action<string> SvRevive;

    public void Recieve(Message msg)
    {
        _msgList.Add(msg);
    }

    public void Tick()
    {
        foreach (Message m in _msgList) 
		{
			Debug.Log(m.Type);
			switch (m.Type) {
				case "SvUserJoined":
					SvUserJoined?.Invoke(m.GetString(0), new Vector3(m.GetFloat(1),m.GetFloat(2),m.GetFloat(3)),m.GetInteger(4));
					break;
				case "SvUserLeft":
					SvUserLeft?.Invoke(m.GetString(0));
					break;
				case "SvMove":
					SvMove?.Invoke(m.GetString(0),new Vector3(m.GetFloat(1), m.GetFloat(2),m.GetFloat(3)));
					break;				
				case "SvTurnY":
					SvTurnY?.Invoke(m.GetString(0),m.GetFloat(1));
					break;
				case "SvKill":
					SvKill?.Invoke(m.GetString(0), m.GetString(1));
					break;
				case "SvRestartGame":
					SvRestartGame?.Invoke();
					break;
				case "SvRevive":
					SvRevive?.Invoke(m.GetString(0));
					break;
			}
		}

        _msgList.Clear();
    }
}
