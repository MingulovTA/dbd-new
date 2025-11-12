using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Text _text;

    private Vector3 _last;
    private string str;
    private int _tmr;
    void Update()
    {
        Vector3 cur = _player.transform.position;
        cur.y = 0;
        int speed = Mathf.RoundToInt(Vector3.Distance(cur, _last)*100/Time.deltaTime);
        

        _last = cur;
        _tmr++;
        if (_tmr > 20)
        {
            _tmr = 0;
            _text.text = speed.ToString();
        }

    }
}
