using System;
using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;

public class MsgReciever
{
    private List<Message> _msgList = new List<Message>(); //  Messsage queue implementation
    public event Action<string> SvUserJoined;
    public event Action<string> SvUserLeft;
    public event Action<string,Vector3> SvMove;

    public void Recieve(Message msg)
    {
        _msgList.Add(msg);
    }

    public void Tick()
    {
        foreach (Message m in _msgList) 
		{
			switch (m.Type) {
				case "SvUserJoined":
					SvUserJoined?.Invoke(m.GetString(0));
					break;
				case "SvUserLeft":
					SvUserLeft?.Invoke(m.GetString(0));
					break;
				case "SvMove":
					SvMove?.Invoke(m.GetString(0),new Vector3(m.GetFloat(1), m.GetFloat(2),m.GetFloat(3)));
					break;
			}
		}

        _msgList.Clear();
    }
}
